using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DjaOC;
using System.Reflection;

namespace DjaOC.Tests
{
    public class StaticContainerTest
    {
        private const string DEFAULT_ID = "default";
        private const string CUSTOM_ID = "custom";
        private const string PROTOTYPE_ID = "prototype";
        private const string SUBCLASS_ID = "subclass";
        private TestEmptyClass DEFAULT_EMPTY_CLASS = new TestEmptyClass();
        private TestEmptyClass CUSTOM_EMPTY_CLASS = new TestEmptyClass();
        private TestEmptyClass PROTOTYPE_EMPTY_CLASS = new TestEmptyClass();
        private TestEmptyChildClass EMPTY_CHILD_CLASS = new TestEmptyChildClass();

        [Test, Order(1)]
        public void CanInstantiate()
        {
            StaticContainer.Instantiate();
            Assert.IsTrue(StaticContainer.IsInstantiated);
        }

        [Test, Order(2)]
        public void CanBind()
        {
            TestStruct testStruct;
            testStruct.idx = 668;
            testStruct.value = 4672;

            StaticContainer.Bind<TestEmptyClass>(DEFAULT_EMPTY_CLASS);
            StaticContainer.Bind<TestEmptyClass>(CUSTOM_EMPTY_CLASS, CUSTOM_ID);
            StaticContainer.Bind<TestEmptyClass>(PROTOTYPE_EMPTY_CLASS, InstantiationType.PROTOTYPE, PROTOTYPE_ID);
            StaticContainer.Bind<TestStruct>(testStruct, CUSTOM_ID);
            StaticContainer.Bind<TestEmptyClass>(EMPTY_CHILD_CLASS, SUBCLASS_ID);
        }

        [Test, Order(3)]
        public void CanGetIsManagedType()
        {
            Assert.IsTrue(StaticContainer.IsManagedType<TestEmptyClass>());
            Assert.IsTrue(StaticContainer.IsManagedType<TestStruct>());
            Assert.IsFalse(StaticContainer.IsManagedType<int>());
        }

        [Test, Order(4)]
        public void CanGetContainsCustomId()
        {
            Assert.IsTrue(StaticContainer.ContainsCustomId<TestEmptyClass>(DEFAULT_ID));
            Assert.IsTrue(StaticContainer.ContainsCustomId<TestEmptyClass>(CUSTOM_ID));
            Assert.IsTrue(StaticContainer.ContainsCustomId<TestEmptyClass>(PROTOTYPE_ID));
            Assert.IsFalse(StaticContainer.ContainsCustomId<TestStruct>(DEFAULT_ID));
            Assert.IsTrue(StaticContainer.ContainsCustomId<TestStruct>(CUSTOM_ID));
        }

        [Test, Order(5)]
        public void CanGet()
        {
            TestEmptyClass defaultSingleton = StaticContainer.Get<TestEmptyClass>();
            Assert.AreSame(DEFAULT_EMPTY_CLASS, defaultSingleton);

            TestEmptyClass customSingleton = StaticContainer.Get<TestEmptyClass>(CUSTOM_ID);
            Assert.AreSame(CUSTOM_EMPTY_CLASS, customSingleton);

            TestEmptyClass prototype = StaticContainer.Get<TestEmptyClass>(PROTOTYPE_ID);
            Assert.AreNotSame(PROTOTYPE_EMPTY_CLASS, prototype);
            Assert.AreEqual(typeof(TestEmptyClass), prototype.GetType());

            TestEmptyChildClass childClass = (TestEmptyChildClass) StaticContainer.Get<TestEmptyClass>(SUBCLASS_ID);
            Assert.AreSame(EMPTY_CHILD_CLASS, childClass);
        }

        [Test, Order(6)]
        public void CanNewInstance()
        {
            TestClassWithConstructor objWithDefaultSingleton = StaticContainer.NewInstance<TestClassWithConstructor>();
            Assert.IsNotNull(objWithDefaultSingleton);
            Assert.AreSame(DEFAULT_EMPTY_CLASS, objWithDefaultSingleton.TestEmptyCls);

            TestClassWithCustomConstructor objWithCustomSingleton = StaticContainer.NewInstance<TestClassWithCustomConstructor>();
            Assert.IsNotNull(objWithCustomSingleton);
            Assert.AreSame(CUSTOM_EMPTY_CLASS, objWithCustomSingleton.TestEmptyCls);
        }

        [Test, Order(7)]
        public void CanDispose()
        {
            StaticContainer.Dispose();
            Assert.IsFalse(StaticContainer.IsInstantiated);
        }

        [Test]
        public void CountWrapperMethods()
        {
            Type djaOCType = typeof(StaticContainer);
            MethodInfo[] djaOCMethodArr = djaOCType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            Type injectorType = typeof(Injector);
            MethodInfo[] injectorMethodArr = injectorType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            //DjaOC must wrap all public non-static methods from Injector. So it has 3 more methods [Instantiate(), IsInstantiated Getter, Dispose()] than Injector.
            Assert.AreEqual(djaOCMethodArr.Length - 3, injectorMethodArr.Length);
        }
    }
}
