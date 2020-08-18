using System;
using System.Collections.Generic;
using System.Text;
using DjaOC.Attributes;
using NUnit.Framework;
using System.Reflection;

namespace DjaOC.Attributes.Tests
{
    public class Attributes
    {
        [Test]
        public void CanFindComponent()
        {
            List<IoCComponent> compList = new List<IoCComponent>(typeof(TestClass).GetCustomAttributes<IoCComponent>());
            Assert.NotNull(compList);
            Assert.AreEqual(2, compList.Count);
        }

        [Test]
        public void CanFindInjectConstructor()
        {
            int numInjectConstructor = 0;
            ConstructorInfo[] cInfoArr = typeof(TestClass).GetConstructors();
            foreach (ConstructorInfo cInfo in cInfoArr)
            {
                Inject cAttr = cInfo.GetCustomAttribute<Inject>();
                if (cAttr != null)
                {
                    numInjectConstructor++;
                }
            }

            Assert.AreEqual(1, numInjectConstructor);
        }

        [Test]
        public void CanFindInjectorConstructorParam()
        {
            int numInjectAttribute = 0;

            ConstructorInfo[] cInfoArr = typeof(TestClass).GetConstructors();
            foreach (ConstructorInfo cInfo in cInfoArr)
            {
                IEnumerator<Inject> cAttributeIter = cInfo.GetCustomAttributes<Inject>().GetEnumerator();
                while (cAttributeIter.MoveNext())
                {
                    ParameterInfo[] pInfoArr = cInfo.GetParameters();
                    foreach (ParameterInfo pInfo in pInfoArr)
                    {
                        IEnumerator<ID> pAttributeIter = pInfo.GetCustomAttributes<ID>().GetEnumerator();
                        while (pAttributeIter.MoveNext())
                        {
                            numInjectAttribute++;
                        }      
                    }  
                }
            }

            Assert.AreEqual(2, numInjectAttribute);
        }

        [Test]
        public void CanFindInjectField()
        {
            int numFieldInjectAttribute = 0;

            FieldInfo[] fInfoArr = typeof(TestClass).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo fInfo in fInfoArr)
            {
                IEnumerator<Inject> fInfoIter = fInfo.GetCustomAttributes<Inject>().GetEnumerator();
                while (fInfoIter.MoveNext())
                {
                    numFieldInjectAttribute++;
                }
            }

            Assert.AreEqual(1, numFieldInjectAttribute);
        }

        [Test]
        public void CanFindInjectMethod()
        {
            int numMethodInjection = 0;
            int numParamInjection = 0;

            MethodInfo[] mInfoArr = typeof(TestClass).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MethodInfo mInfo in mInfoArr)
            {
                Inject mInfoAttribute = mInfo.GetCustomAttribute<Inject>();
                if (mInfoAttribute != null)
                {
                    numMethodInjection++;

                    ParameterInfo[] pInfoArr = mInfo.GetParameters();
                    foreach (ParameterInfo pInfo in pInfoArr)
                    {
                        IEnumerator<ID> customAttrIter = pInfo.GetCustomAttributes<ID>().GetEnumerator();
                        while (customAttrIter.MoveNext())
                        {
                            numParamInjection++;
                        }
                    }
                }
            }

            Assert.AreEqual(2, numMethodInjection);
            Assert.AreEqual(3, numParamInjection);
        }
    }

    [Singleton]
    [Prototype]
    class TestClass
    {
        [Inject]
        [ID("custom")]
        private int _intValue;
        private float _floatValue;

        public int IntValue { get { return _intValue;  } }

        public float FloatValue { get { return _floatValue; } }
        public TestClass()
        {

        }

        [Inject]
        public TestClass(
            int idx, 
            [ID("custom")] string str,
            [ID("custom")] float flt)
        {

        }

        [Inject]
        public void SetValues(
            [ID] float fvalue,
            [ID] int ivalue)
        {
            this._floatValue = fvalue;
            this._intValue = ivalue;
        }

        [Inject]
        private void SetInt(
            [ID] int value)
        {
            this._intValue = value;
        }
    }

}
