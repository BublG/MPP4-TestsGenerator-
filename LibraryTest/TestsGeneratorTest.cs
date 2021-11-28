using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TestsGeneratorLibrary;

namespace LibraryTest
{
    public class Tests
    {
        private static readonly TestsGenerator TestsGenerator = new TestsGenerator();

        private static string _emptyClassesPath = "D:\\учеба\\СПП\\TestsGenerator\\LibraryTest\\EmptyClass.cs";
        private static string _notEmptyClassesPath = "D:\\учеба\\СПП\\TestsGenerator\\LibraryTest\\NotEmptyClass.cs";
        
        [Test]
        public void ClassesWithoutMethodsTest()
        {
            using StreamReader sourceReader = File.OpenText(_emptyClassesPath);
            List<TestClass> result = TestsGenerator.GenerateTestClasses(sourceReader.ReadToEnd());
            Assert.IsEmpty(result);
        }

        [Test]
        public void ClassesWithMethodsTest()
        {
            using StreamReader sourceReader = File.OpenText(_notEmptyClassesPath);
            List<TestClass> generatedClasses = TestsGenerator.GenerateTestClasses(sourceReader.ReadToEnd());
            Assert.AreEqual(2, generatedClasses.Count);

            string generatedCode = "";
            generatedClasses.ForEach(c => generatedCode += c.SourceCode);
            int testsQuantity = 0;
            for (int i = 0; i < generatedCode.Length - 5; i++)
            {
                if (generatedCode.Substring(i, 6).Equals("[Test]"))
                {
                    testsQuantity++;
                }
            }
            Assert.AreEqual(4, testsQuantity);
        }
    }
}