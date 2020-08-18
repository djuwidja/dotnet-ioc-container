using System;
using System.Collections.Generic;
using System.Text;

namespace DjaOC.Attributes
{
    /// <summary>
    /// Attribute to be used in a method or constructor parameters to specify the custom id of the object to be injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ID : Attribute
    {
        public string Id { get; }

        public ID(string id = DjaOC.Injector.DEFAULT)
        {
            this.Id = id;
        }
    }
    /// <summary>
    /// Attribute to tag an Injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class Inject : Attribute
    {
        public Inject()
        {

        }
    }
    /// <summary>
    /// Attribute to tag a class as an IoC component. Being an IoC component allows class object to be injected with Injector.
    /// </summary>
    public class IoCComponent : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    /// <summary>
    /// Attribute to tag a class as a Singleton. Injector will instantiate an object from the class once and then reuse the object during injection.
    /// </summary>
    public sealed class Singleton : IoCComponent
    {

    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    /// <summary>
    /// Attribute to tag a class as Instances. Injector will instantiate an object from the class every time it is being injected.
    /// </summary>
    public sealed class Prototype : IoCComponent
    {

    }
    
}
