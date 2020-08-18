using System;
using System.Collections.Generic;
using System.Text;
using DjaOC.Attributes;

namespace DjaOC.Tests
{
    struct TestStruct
    {
        public int idx;
        public int value;
    }

    [Singleton]
    class TestEmptyClass
    {
        [Inject]
        public TestEmptyClass()
        {

        }
    }

    [Singleton]
    [ID("child")]
    class TestEmptyChildClass : TestEmptyClass
    {
        [Inject]
        public TestEmptyChildClass()
        {

        }
    }

    [Prototype]
    class TestClassWithConstructor
    {
        public TestEmptyClass TestEmptyCls { get; }

        private TestClassWithConstructor()
        {
            this.TestEmptyCls = null;
        }

        [Inject]
        public TestClassWithConstructor(TestEmptyClass cls)
        {
            this.TestEmptyCls = cls;
        }
    }

    class TestClassWithCustomConstructor
    {
        public TestEmptyClass TestEmptyCls { get; }
        private TestClassWithCustomConstructor()
        {
            this.TestEmptyCls = null;
        }

        [Inject]
        public TestClassWithCustomConstructor([ID("custom")] TestEmptyClass cls)
        {
            this.TestEmptyCls = cls;
        }

    }
   
    [Singleton]
    class TestClassWith2InjectConstructors
    {
        [Inject]
        public TestClassWith2InjectConstructors()
        {

        }

        [Inject]
        public TestClassWith2InjectConstructors(int idx)
        {

        }
    }

    [Prototype]
    class TestClassWithNoInjectConstructor
    {
        public TestClassWithNoInjectConstructor()
        {

        }
    }
    
    [Singleton]
    class TestClassWithConstructorInjection
    {
        private int _intValue;
        [Inject] [ID("custom")] private float _floatValue;
        [Inject] public TestStruct TestStruct { get; set; }

        private long _longValue;
        private short _shortValue;

        public int IntValue { get { return _intValue; } }
        public long LongValue { get { return _longValue; } }
        public short ShortValue { get { return _shortValue;  } }
        public float FloatValue { get { return _floatValue; } }
        
        public TestEmptyClass EmptyClass { get; }

        [Inject]
        public TestClassWithConstructorInjection(TestEmptyClass emptyClass,
                                                 [ID("custom")] int intValue)
        {
            _intValue = intValue;
            EmptyClass = emptyClass;
        }

        [Inject]
        public void TestPublicInjection(long longValue)
        {
            _longValue = longValue;
        }

        [Inject]
        private void TestPrivateMethodInjection([ID("custom")] short shortValue)
        {
            _shortValue = shortValue;
        }
    }

    [Prototype]
    class TestClassWithConstructorInjectionFail
    {
        public float TestFloat { get; }
        public TestEmptyClass TestEmptyCls { get; }

        public TestClassWithConstructorInjectionFail(float testFloat, TestEmptyClass emptyCls)
        {
            this.TestFloat = testFloat;
            this.TestEmptyCls = emptyCls;
        }
    }

    class TestClassWithoutClassAttribute
    {
        public TestClassWithoutClassAttribute()
        {

        }
    }

    [Singleton]
    [Prototype]
    class TestClassWith2ClassAttributes
    { 
        public TestClassWith2ClassAttributes()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    /// <summary>
    /// Attribute to tag a class as Instances. Injector will instantiate an object from the class every time it is being injected.
    /// </summary>
    public sealed class Unsupported : IoCComponent
    {

    }

    [Unsupported]
    class TestClassWithUnsupportedAttribute
    {
        public TestClassWithUnsupportedAttribute()
        {

        }
    }
}
