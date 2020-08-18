using System;
using System.Collections.Generic;
using System.Text;

namespace DjaOC
{
    /// <summary>
    /// Static wrapper class to provide static access to DjaOC's IoC Container. Alternatively, Multiple Injectors can be instantiated by user's discretion without
    /// using this class.
    /// </summary>
    public static class StaticContainer
    {
        private static Injector _injector;
        /// <summary>
        /// If DjaOC had been instantiated.
        /// </summary>
        public static bool IsInstantiated
        {
            get
            {
                return _injector != null;
            }
        }
        /// <summary>
        /// Instantiate DjaOC.
        /// </summary>
        public static void Instantiate()
        {
            _injector = new Injector();
        }
        /// <summary>
        /// Dispose DjaOC.
        /// </summary>
        public static void Dispose()
        {
            _injector = null;
        }
        /// <summary>
        /// Bind an object to its type and the default id. 
        /// The object type must have the [Singleton] or [Prototype] attribute.
        /// </summary>
        /// <param name="obj">Target object.</param>
        public static void Bind<T>(T obj)
        {
            _injector.Bind<T>(obj);
        }
        /// <summary>
        /// Bind an object to its type and the supplied id. 
        /// Object must have the supplied type. 
        /// The object type must have the [Singleton] or [Prototype] attribute.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <param name="id">Custom id of the object.</param>
        public static void Bind<T>(T obj, string id)
        {
            _injector.Bind<T>(obj, id);
        }
        /// <summary>
        /// Bind an object to its type and the default id. 
        /// Object must have the supplied type. 
        /// This object will ingore the [Singleton] or [Prototype] attribute.
        /// </summary>
        /// <param name="instType">Singleton or Prototype.</param>
        /// <param name="obj">Target obj.</param>
        public static void Bind<T>(T obj, InstantiationType instType)
        {
            _injector.Bind<T>(obj, instType);
        }
        /// <summary>
        /// Bind an object to its type and the supplied id. 
        /// Object must have the supplied type. 
        /// This object will ingore the [Singleton] or [Prototype] attribute.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <param name="instType">Singleton or Prototype.</param>
        /// <param name="id">Custom id of the object.</param>
        public static void Bind<T>(T obj, InstantiationType instType, string id)
        {
            _injector.Bind<T>(obj, instType, id);
        }
        /// <summary>
        /// Returns true if the supplied type is binded to an object in this injector.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>True if the type is managed by this injector.</returns>
        public static bool IsManagedType<T>()
        {
            return _injector.IsManagedType<T>();
        }
        /// <summary>
        /// Returns true if the supplied type has an object that was binded to the supplied custom id.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="id">The supplied custom id.</param>
        /// <returns></returns>
        public static bool ContainsCustomId<T>(string id)
        {
            return _injector.ContainsCustomId<T>(id);
        }
        /// <summary>
        /// Get the object with default key that was binded to the type. If the type is declared as a [Singleton], the same instance will be returned. 
        /// If the type is declared as a [Prototype], a new cloned instance will be returned instead.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="id">Custom id of the object.</param>
        /// <returns>The binded object instance if it is declared as [Singleton], or a cloned instance of the binded object if it is declared as [Prototype].</returns>
        public static T Get<T>()
        {
            return _injector.Get<T>();
        }
        /// <summary>
        /// Get the object with supplied id as key that was binded to the type. If the type is declared as a [Singleton], the same instance will be returned. 
        /// If the type is declared as a [Prototype], a new cloned instance will be returned instead.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="id">Custom id of the object.</param>
        /// <returns>The binded object instance if it is declared as [Singleton], or a cloned instance of the binded object if it is declared as [Prototype].</returns>
        public static T Get<T>(string id)
        {
            return _injector.Get<T>(id);
        }
        /// <summary>
        /// Creates a new instance from the type. The definition of the type must have a constructor with attribute [InjectConstructor].
        /// The newly created instance will perform dependency injection with constructors, methods, fields and properties that have the tag
        /// [Inject].
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>A newly created instance of the supplied type.</returns>
        public static T NewInstance<T>()
        {
            return _injector.NewInstance<T>();
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
        public static void BindNewInstance<BindType, InstanceType>() where InstanceType : BindType
        {
            _injector.BindNewInstance<BindType, InstanceType>();
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
        public static void BindNewInstance<InstanceType>()
        {
            _injector.BindNewInstance<InstanceType>();
        }
    }
}
