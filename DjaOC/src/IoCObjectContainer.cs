using System;
using System.Collections.Generic;
using System.Text;
using DjaOC.Attributes;
using System.Reflection;

namespace DjaOC
{
    public sealed class IoCObjectContainer
    {
        private Dictionary<Type, Dictionary<string, IoCObject>> _objMap = new Dictionary<Type, Dictionary<string, IoCObject>>();
        /// <summary>
        /// Bind an object to the object's type with supplied id as key. Object must have [Singleton] or [Prototype] declared as class attribute.
        /// </summary>
        /// <param name="type">Type of object to bind to.</param>
        /// <param name="obj">Target object.</param>
        /// <param name="id">Custom id of the object.</param>
        public void Bind<T>(T obj, string id)
        {
            InstantiationType instType = GetInstantiationType(obj.GetType());
            Bind<T>(obj, instType, id);
        }
        /// <summary>
        /// Bind an object to the object's type with specific initialization type and with supplied id as Key. This operation will ignore
        /// the [Singleton] or [Prototype] attributes that are declared in class attribute.
        /// </summary>
        /// <param name="type">Type of object to bind to.</param>
        /// <param name="obj">Target object.</param>
        /// <param name="instType">The initialization type.</param>
        /// <param name="id">Custom id of the object.</param>
        public void Bind<T>(T obj, InstantiationType instType, string id)
        {
            Type type = typeof(T);
            Dictionary<string, IoCObject> objectMap;
            if (!_objMap.TryGetValue(type, out objectMap))
            {
                objectMap = new Dictionary<string, IoCObject>();
            }

            objectMap[id] = new IoCObject(instType, obj);
            _objMap[type] = objectMap;
        }
        /// <summary>
        /// Returns true if the supplied type is binded to an object in this injector.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returnsTrue if the type is managed by this IoCObjectContainer></returns>
        public bool IsManagedType(Type type)
        {
            return _objMap.ContainsKey(type);
        }
        /// <summary>
        /// Get the object with supplied id as key that was binded to the type. If the type is declared as a [Singleton], the same instance will be returned. 
        /// If the type is declared as a [Prototype], a new cloned instance will be returned instead.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <param name="id">Custom id of the object.</param>
        /// <returns>The binded object instance if it is declared as [Singleton], or a cloned instance of the binded object if it is declared as [Prototype].</returns>
        public object Get(Type type, string id)
        {
            if (!IsManagedType(type))
            {
                throw new IoCDefinitionNotFoundException(string.Format("{0} is not managed by this injector.", type.FullName));
            }

            if (_objMap.ContainsKey(type))
            {
                Dictionary<string, IoCObject> objMap = _objMap[type];
                if (objMap.ContainsKey(id))
                {
                    return GetObj(objMap[id]);
                }
                else
                {
                    throw new IoCDefinitionNotFoundException(string.Format("{0} does not have a resource with key {1}", type.FullName, id));
                }
            }
            else
            {
                throw new IoCDefinitionNotFoundException(string.Format("{0} is not managed by this injector.", type.FullName));
            }
        }
        /// <summary>
        /// Check if this injector has an id binded to the type.
        /// </summary>
        /// <param name="type">Supplied type.</param>
        /// <param name="id">Supplied id.</param>
        /// <returns>True if the key is binded to the type.</returns>
        public bool ContainsId(Type type, string id)
        {
            if (IsManagedType(type))
            {
                Dictionary<string, IoCObject> objMap = _objMap[type];
                return objMap.ContainsKey(id);
            }

            return false;
        }
        /// <summary>
        /// Retrieve the instantiation type from type attribute. If no attribute is found, then it is a singleton type.
        /// </summary>
        /// <param name="type">The supplied type.</param>
        /// <returns>Instantiation Type.</returns>
        private InstantiationType GetInstantiationType(Type type)
        {
            IoCComponent[] compArr = (IoCComponent[])type.GetCustomAttributes(typeof(IoCComponent), false);
            if (compArr.Length == 0)
            {
                return InstantiationType.SINGLETON;
            }
            else if (compArr.Length > 1)
            {
                throw new InvalidIoCTypeException(string.Format("More than 1 attribute is found in type {0}.", type.FullName));
            }

            IoCComponent comp = compArr[0];
            if (comp.GetType() == typeof(Singleton))
            {
                return InstantiationType.SINGLETON;
            }
            else if (comp.GetType() == typeof(Prototype))
            {
                return InstantiationType.PROTOTYPE;
            }
            else
            {
                throw new InvalidIoCTypeException(string.Format("Unknown attribute type {0}.", comp.GetType().FullName));
            }
        }
        /// <summary>
        /// Get the object with IoCObject. Returns the supplied object if instantiation type is singleton. 
        /// Returns a cloned object of the supplied object if instantiation type is prototype.
        /// </summary>
        /// <returns></returns>
        public object GetObj(IoCObject iocObj)
        {
            if (iocObj.InstantiationType == InstantiationType.SINGLETON)
            {
                return iocObj.Object; ;
            }
            else if (iocObj.InstantiationType == InstantiationType.PROTOTYPE)
            {
                return CloneObj(iocObj.Object);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Clone an object using reflection.
        /// </summary>
        /// <param name="obj">The supplied obj.</param>
        /// <returns>The cloned obj.</returns>
        private object CloneObj(object obj)
        {
            Type objType = obj.GetType();

            FieldInfo[] fInfoArr = objType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo mInfo = objType.GetMethod("MemberwiseClone", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            object newObj = mInfo.Invoke(obj, new object[] { });
            foreach (FieldInfo fInfo in fInfoArr)
            {
                object fieldObj = fInfo.GetValue(obj);
                fInfo.SetValue(newObj, fieldObj);
            }

            return newObj;
        }
    }
}
