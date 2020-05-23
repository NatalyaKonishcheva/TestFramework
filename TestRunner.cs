using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestFramework
{
    public static class TestRunner
    {
        public static Dictionary<MethodInfo, bool> Run(Assembly assembly)
        {
            Dictionary<MethodInfo, bool> methodMarkPairs = new Dictionary<MethodInfo, bool>();
            bool markOfTestMethod;

            foreach (var TestMethod in SearchTestMethodsInAssembly(assembly))
            {
                markOfTestMethod = true;
                var ObjectOfTestClass = Activator.CreateInstance(TestMethod.DeclaringType);
                try
                {
                    TestMethod.Invoke(ObjectOfTestClass, new object[] { });
                }
                catch (TargetInvocationException)
                {
                    markOfTestMethod = false;
                }
                methodMarkPairs.Add(TestMethod, markOfTestMethod);
            }
            return methodMarkPairs;
        }


        private static List<MethodInfo> SearchTestMethodsInAssembly(Assembly assembly)
        {
            List<MethodInfo> TestMethodsInAssembly = new List<MethodInfo>();

            foreach (var сlassOfAssembly in assembly.GetTypes())
            {
                var attributesOfClass = сlassOfAssembly.GetCustomAttributes(typeof(NTestClassAttribute));
                if (attributesOfClass.Count() != 0)
                {
                    foreach (var methodOfTestClass in сlassOfAssembly.GetMethods())
                    {
                        var attibutesOfMethod = methodOfTestClass.GetCustomAttributes(typeof(NTestMethodAttribute));
                        if (attibutesOfMethod.Count() != 0)
                        {
                            TestMethodsInAssembly.Add(methodOfTestClass);
                        }
                    }
                }
            }

            return TestMethodsInAssembly;
        }
    }



    public static class NAssert
    {
        public static void AreEqual(object expected, object actual)
        {
            if (expected == actual) return;
            if ((expected == null && actual != null)
                || (expected != null && actual == null)
                || !expected.Equals(actual)) throw new TestException();

        }
        public static void IsTrue(bool condition)
        {
            if (!condition) throw new TestException();
        }
    }


    public class TestException : ApplicationException
    {

    }


    [AttributeUsage(AttributeTargets.Class)]
    public class NTestClassAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class NTestMethodAttribute : Attribute
    {

    }
}
