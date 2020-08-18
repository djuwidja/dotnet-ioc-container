using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DjaOC;

namespace DjaOC.Tests
{
    public class InjectorTest
    {
         [Test]
        public void CanNewInstance()
        {
            const string customId = "custom";
            const float defaultTestFloat = 0.6f;
            const float customTestFloat = 1.14f;

            TestStruct testStruct;
            testStruct.idx = 8;
            testStruct.value = 99;

            const int defaultTestInt = 6;
            const int customTestInt = 88;

            const long defaultTestLong = 8864;
            const long customTestLong = 7143;

            const short defaultTestShort = 3;
            const short customTestShort = 16;

            Injector injector = new Injector();
            // Dependency cannot be found
            Assert.Throws<IoCConstructorException>(() =>  injector.NewInstance<TestClassWithConstructor>());

            injector.Bind<float>(defaultTestFloat);
            injector.Bind<float>(customTestFloat, customId);
            injector.Bind<TestStruct>(testStruct);
            injector.Bind<int>(defaultTestInt);
            injector.Bind<int>(customTestInt, customId);
            injector.Bind<TestEmptyClass>(injector.NewInstance<TestEmptyClass>());
            injector.Bind<long>(defaultTestLong);
            injector.Bind<long>(customTestLong, customId);
            injector.Bind<short>(defaultTestShort);
            injector.Bind<short>(customTestShort, customId);

            // Dependecy can be found.
            // Prototype
            Assert.DoesNotThrow(() => injector.NewInstance<TestClassWithConstructor>());


            //Singleton
            TestClassWithConstructorInjection obj = injector.NewInstance<TestClassWithConstructorInjection>();
            Assert.NotNull(obj);
            Assert.AreEqual(injector.Get<int>(customId), obj.IntValue);
            Assert.AreEqual(injector.Get<float>(customId), obj.FloatValue);
            Assert.AreEqual(injector.Get<TestStruct>(), obj.TestStruct);
            Assert.AreEqual(injector.Get<long>(), obj.LongValue);
            Assert.AreEqual(injector.Get<short>(customId), obj.ShortValue);
        }

        [Test]
        public void CanNewInstanceFail()
        {
            Injector injector = new Injector();
            Assert.DoesNotThrow(() => injector.Bind(new TestEmptyClass()));

            Assert.Throws<IoCConstructorException>(() => injector.NewInstance<TestClassWithConstructorInjectionFail>());
        }

        [Test]
        public void CanBindAndGet()
        {
            const string customSingletonId = "customSingleton";
            const string customPrototypeId = "customPrototype";

            TestEmptyClass defaultSingletonEmptyObj = new TestEmptyClass();
            TestEmptyClass customSingletonEmptyObj = new TestEmptyClass();
            TestEmptyClass customPrototypeEmptyObj = new TestEmptyClass();

            Injector injector = new Injector();
            injector.Bind<TestEmptyClass>(defaultSingletonEmptyObj);
            injector.Bind<TestEmptyClass>(customSingletonEmptyObj, customSingletonId);
            injector.Bind<TestEmptyClass>(customPrototypeEmptyObj, InstantiationType.PROTOTYPE, customPrototypeId);

            // Default singleton
            TestEmptyClass resultDefaultSingletonObj = injector.Get<TestEmptyClass>();
            Assert.AreSame(defaultSingletonEmptyObj, resultDefaultSingletonObj);

            // custom singleton
            TestEmptyClass resultCustomSingletonObj = injector.Get<TestEmptyClass>(customSingletonId);
            Assert.AreSame(customSingletonEmptyObj, resultCustomSingletonObj);
            Assert.AreNotSame(defaultSingletonEmptyObj, resultCustomSingletonObj);

            // custom prototype
            TestEmptyClass resultCustomPrototypeObj = injector.Get<TestEmptyClass>(customPrototypeId);
            Assert.AreNotSame(customPrototypeEmptyObj, resultCustomPrototypeObj);
            Assert.AreEqual(typeof(TestEmptyClass), resultCustomPrototypeObj.GetType());
        }

        [Test]
        public void CanGetIsManagedType()
        {
            Injector injector = new Injector();
            injector.Bind<TestEmptyClass>(new TestEmptyClass());
            injector.Bind<int>(568);

            Assert.IsTrue(injector.IsManagedType<TestEmptyClass>());
            Assert.IsTrue(injector.IsManagedType<int>());
            Assert.IsFalse(injector.IsManagedType<float>());
            Assert.IsFalse(injector.IsManagedType<TestClassWithConstructor>());
        }

        [Test]
        public void CanGetContainsCustomId()
        {
            string defaultId = "default";
            string customId1 = "custom1";
            string customId2 = "custom2";

            Injector injector = new Injector();
            injector.Bind<TestEmptyClass>(new TestEmptyClass(), customId1);
            injector.Bind<TestEmptyClass>(new TestEmptyClass(), customId2);
            injector.Bind<int>(662, customId1);
            injector.Bind<int>(477);

            Assert.IsTrue(injector.ContainsCustomId<TestEmptyClass>(customId1));
            Assert.IsTrue(injector.ContainsCustomId<TestEmptyClass>(customId2));
            Assert.IsFalse(injector.ContainsCustomId<TestEmptyClass>(defaultId));
            Assert.IsTrue(injector.ContainsCustomId<int>(customId1));
            Assert.IsTrue(injector.ContainsCustomId<int>(defaultId));
            Assert.IsFalse(injector.ContainsCustomId<int>(customId2));
        }

        [Test]
        public void CanBindNewInstance()
        {
            const string defaultId = "default";
            const string childId = "child";

            Injector injector = new Injector();
            injector.BindNewInstance<TestEmptyClass>();
            injector.BindNewInstance<TestEmptyClass, TestEmptyChildClass>();

            Assert.IsTrue(injector.ContainsCustomId<TestEmptyClass>(defaultId));
            Assert.IsTrue(injector.ContainsCustomId<TestEmptyClass>(childId));

            TestEmptyClass childClass = injector.Get<TestEmptyClass>(childId);
            Assert.AreEqual(typeof(TestEmptyChildClass), childClass.GetType());
        }
    }
}
