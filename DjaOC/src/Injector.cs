using System;
using System.Collections.Generic;
using System.Reflection;
using DjaOC.Attributes;

namespace DjaOC
{
    /// <summary>
    /// Implements the Injector in Dependency Injection.
    /// This class is NOT thread-safe.
    /// </summary>
    public sealed class Injector
    {
        internal const string DEFAULT = "default";

        private IoCObjectContainer _container;
        public Injector()
        {
            _container = new IoCObjectContainer();
        }
        /// <summary>
        /// Bind an object to its type and the default id. 
        /// The object type must have the [Singleton] or [Prototype] attribute.
        /// </summary>
        /// <param name="obj">Target object.</param>
        public void Bind<T>(T obj)
        {
            Bind<T>(obj, DEFAULT);
        }
        /// <summary>
        /// Bind an object to its type and the supplied id. 
        /// Object must have the supplied type. 
        /// The object type must have the [Singleton] or [Prototype] attribute.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <param name="id">Custom id of the object.</param>
        public void Bind<T>(T obj, string id)
        {
            _container.Bind<T>(obj, id);
        }
        /// <summary>
        /// Bind an object to its type and the default id. 
        /// Object must have the supplied type. 
        /// This object will ingore the [Singleton] or [Prototype] attribute.
        /// </summary>
        /// <param name="instType">Singleton or Prototype.</param>
        /// <param name="obj">Target obj.</param>
        public void Bind<T>(T obj, InstantiationType instType)
        {
            _container.Bind<T>(obj, instType, DEFAULT);
        }
        /// <summary>
        /// Bind an object to its type and the supplied id. 
        /// Object must have the supplied type. 
        /// This object will ingore the [Singleton] or [Prototype] attribute.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <param name="instType">Singleton or Prototype.</param>
        /// <param name="id">Custom id of the object.</param>
        public void Bind<T>(T obj, InstantiationType instType, string id)
        {
            _container.Bind<T>(obj, instType, id);
        }
        /// <summary>
        /// Returns true if the supplied type is binded to an object in this injector.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>True if the type is managed by this injector.</returns>
        public bool IsManagedType<T>()
        {
            Type type = typeof(T);
            return _container.IsManagedType(type);
        }
        /// <summary>
        /// Returns true if the supplied type has an object that was binded to the supplied custom id.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="id">The supplied custom id.</param>
        /// <returns></returns>
        public bool ContainsCustomId<T>(string id)
        {
            Type type = typeof(T);
            return _container.ContainsId(type, id);
        }
        /// <summary>
        /// Get the object with default key that was binded to the type. If the type is declared as a [Singleton], the same instance will be returned. 
        /// If the type is declared as a [Prototype], a new cloned instance will be returned instead.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="id">Custom id of the object.</param>
        /// <returns>The binded object instance if it is declared as [Singleton], or a cloned instance of the binded object if it is declared as [Prototype].</returns>
        public T Get<T>()
        {
            return Get<T>(DEFAULT);
        }
        /// <summary>
        /// Get the object with supplied id as key that was binded to the type. If the type is declared as a [Singleton], the same instance will be returned. 
        /// If the type is declared as a [Prototype], a new cloned instance will be returned instead.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="id">Custom id of the object.</param>
        /// <returns>The binded object instance if it is declared as [Singleton], or a cloned instance of the binded object if it is declared as [Prototype].</returns>
        public T Get<T>(string id)
        {
            Type type = typeof(T);
            return (T) _container.Get(type, id);
        }
        /// <summary>
        /// Creates a new instance from the type. The definition of the type must have a constructor with attribute [Inject].
        /// The newly created instance will perform dependency injection with constructors, methods, fields and properties that have the tag
        /// [Inject].
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>A newly created instance of the supplied type.</returns>
        public T NewInstance<T>()
        {
            Type type = typeof(T);
            //Constructor Injection
            ConstructorInfo cInfo = GetConstructor(type);
            ParameterInfo[] cInfoParams = cInfo.GetParameters();
            object[] constructorParamList = ComputeParamInjection(cInfoParams);
            object result = cInfo.Invoke(constructorParamList);

            //Method Injection
            MethodInfo[] mInfoArr = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MethodInfo mInfo in mInfoArr)
            {
                Inject customInjectMethod = mInfo.GetCustomAttribute<Inject>();
                if (customInjectMethod != null)
                {
                    ParameterInfo[] mInfoParams = mInfo.GetParameters();
                    object[] methodParamList = ComputeParamInjection(mInfoParams);
                    mInfo.Invoke(result, methodParamList);
                }
            }

            //Field Injection
            FieldInfo[] fInfoArr = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo fInfo in fInfoArr)
            {
                string resId;
                if (FindIDProperty(fInfo, fInfo.FieldType, out resId))
                {
                    fInfo.SetValue(result, _container.Get(fInfo.FieldType, resId));
                }
            }

            //Property Injection
            PropertyInfo[] pInfoArr = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (PropertyInfo pInfo in pInfoArr)
            {
                string resId;
                if (FindIDProperty(pInfo, pInfo.PropertyType, out resId))
                {
                    pInfo.SetValue(result, _container.Get(pInfo.PropertyType, resId));
                }
            }

            return (T) result;
        }
        /// <summary>
        /// Bind a newly created instance of InstanceType to BindType.
        /// InstanceType must be child of Bindtype.
        /// The definition of the type must have a constructor with attribute [Inject].
        /// The class definition must have either [Singleton] or [Prototype] declared.
        /// If the type is declared as a [Singleton], the same instance will be returned. 
        /// If the type is declared as a [Prototype], a new cloned instance will be returned instead.
        /// If [ID] is declared, the object is binded with the value of ID, otherwise it will be binded to default id.
        /// </summary>
        /// <typeparam name="BindType"></typeparam>
        /// <typeparam name="InstanceType"></typeparam>
        public void BindNewInstance<BindType, InstanceType>() where InstanceType : BindType
        {
            InstanceType newInstance = NewInstance<InstanceType>();
            string id = DEFAULT;
            ID idAttribute = newInstance.GetType().GetCustomAttribute<ID>();
            if (idAttribute != null)
            {
                id = idAttribute.Id;
            }
            Bind<BindType>(newInstance, id);
        }
        /// <summary>
        /// Bind a newly created instance of InstanceType to InstanceType.
        /// The definition of the type must have a constructor with attribute [Inject].
        /// The class definition must have either [Singleton] or [Prototype] declared.
        /// If the type is declared as a [Singleton], the same instance will be returned. 
        /// If the type is declared as a [Prototype], a new cloned instance will be returned instead.
        /// If [ID] is declared, the object is binded with the value of ID, otherwise it will be binded to default id.
        /// </summary>
        /// <typeparam name="InstanceType"></typeparam>
        public void BindNewInstance<InstanceType>()
        {
            BindNewInstance<InstanceType, InstanceType>();
        }
        /// <summary>
        /// Find the Id from [ID] in a field or property.
        /// </summary>
        /// <param name="info">The FieldInfo or PropertyInfo.</param>
        /// <param name="infoType">The type of FieldInfo or PropertyInfo.</param>
        /// <param name="resId">The returned id, if any.</param>
        /// <returns>True if an id is found in [InjectProperty], false otherwise.</returns>
        private bool FindIDProperty(MemberInfo info, Type infoType, out string resId)
        {
            Inject customInject = info.GetCustomAttribute<Inject>();
            if (customInject != null)
            {
                ID customInjectId = info.GetCustomAttribute<ID>();
                string injectId = customInjectId != null ? customInjectId.Id : DEFAULT;
                if (VerifyManagedType(infoType, injectId))
                {
                    resId = injectId;
                    return true;
                }
                else
                {
                    throw new IoCDefinitionNotFoundException(string.Format("Object with type {0} and {1} cannot be found.", infoType, customInjectId.Id));
                }
            }

            resId = "";
            return false;
        }
        /// <summary>
        /// Gather a list of objects from this injector that fit the supplied list of parameter info.
        /// </summary>
        /// <param name="paramArr">A list of parameters to be processed.</param>
        /// <returns>A list of objects that correspond to the supplied list of parameters.</returns>
        private object[] ComputeParamInjection(ParameterInfo[] paramArr)
        {
            object[] paramObjList = new object[paramArr.Length];
            for (int i = 0; i < paramArr.Length; i++)
            {
                ParameterInfo pInfo = paramArr[i];
                ID injectAttr = pInfo.GetCustomAttribute<ID>();
                string resId = DEFAULT;
                if (injectAttr != null)
                {
                    resId = injectAttr.Id;
                }

                if (VerifyManagedType(pInfo.ParameterType, resId))
                {
                    paramObjList[i] = _container.Get(pInfo.ParameterType, resId);
                }
                else
                {
                    throw new IoCDefinitionNotFoundException(string.Format("Object with type {0} and {1} cannot be found.", pInfo.ParameterType, resId));
                }
            }
            return paramObjList;
        }
        /// <summary>
        /// Verify if the supplied type and id are managed in this injector.
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <param name="id">Custom id of the object.</param>
        /// <returns>If success.</returns>
        private bool VerifyManagedType(Type type, string id)
        {
            return _container.ContainsId(type, id);
        }
        /// <summary>
        /// Get the single unique public nonstatic constructor from the supplied type that was tagged with [InjectConstructor].
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <returns>The constructor info if succeed. Exceptions are thrown otherwise.</returns>
        private ConstructorInfo GetConstructor(Type type)
        {
            ConstructorInfo[] cInfoArr = FindIoCConstructors(type);
            if (cInfoArr.Length == 0)
            {
                throw new IoCConstructorException(string.Format("No IoC compatible constructor can be found within {0}.", type.FullName));
            }
            else if (cInfoArr.Length > 1)
            {
                throw new IoCConstructorException(string.Format("Only 1 InjectConstructor is allowed within {0}.", type.FullName));
            }

            return cInfoArr[0];
        }
        /// <summary>
        /// Get a list of public nonstatic constructors that was tagged with [InjectConstructor]
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <returns>Array of constructor info</returns>
        private ConstructorInfo[] FindIoCConstructors(Type type)
        {
            List<ConstructorInfo> iocConstructorList = new List<ConstructorInfo>();

            ConstructorInfo[] constructors = type.GetConstructors();
            foreach (ConstructorInfo cInfo in constructors)
            {
                if (cInfo.IsPublic && !cInfo.IsStatic)
                {
                    bool isValidIoCConstructor = true;
                    Inject cInject = cInfo.GetCustomAttribute<Inject>();
                    if (cInject == null)
                    {
                        isValidIoCConstructor = false;
                    }
                    else
                    {
                        ParameterInfo[] parameters = cInfo.GetParameters();

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            ParameterInfo pInfo = parameters[i];
                            Type paramType = pInfo.ParameterType;
                            if (!_container.IsManagedType(paramType))
                            {
                                isValidIoCConstructor = false;
                                break;
                            }
                        }
                    }

                    if (isValidIoCConstructor)
                    {
                        iocConstructorList.Add(cInfo);
                    }
                }
            }

            return iocConstructorList.ToArray();
        }
    }
}
