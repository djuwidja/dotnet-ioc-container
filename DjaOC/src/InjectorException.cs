using System;
using System.Collections.Generic;
using System.Text;

namespace DjaOC
{
    [Serializable]
    public class InvalidIoCTypeException : Exception
    {
        public InvalidIoCTypeException() { }
        public InvalidIoCTypeException(string message) : base(message) { }
        public InvalidIoCTypeException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class IoCConstructorException : Exception
    {
        public IoCConstructorException() { }
        public IoCConstructorException(string message) : base(message) { }
        public IoCConstructorException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class IoCDefinitionNotFoundException : Exception
    {
        public IoCDefinitionNotFoundException() { }
        public IoCDefinitionNotFoundException(string message) : base(message) { }
        public IoCDefinitionNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
    [Serializable]
    public class AttributeNotFoundException : Exception
    {
        public AttributeNotFoundException() { }
        public AttributeNotFoundException(string message) : base(message) { }
        public AttributeNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
