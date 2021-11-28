using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using TestsGeneratorLibrary;

namespace ConsoleApp
{
    internal class Program
    {
        private const string DestPath = "D:\\учеба\\СПП\\GeneratedTests\\";
        private const int MaxReadingDegree = 1;
        private const int MaxGeneratingDegree = 2;
        private const int MaxWritingDegree = 3;

        private static readonly string[] Files =
        {
            "D:\\учеба\\СПП\\Faker\\Library\\Faker\\Impl\\Faker.cs",
            "D:\\учеба\\СПП\\Faker\\Library\\Generator\\Impl\\BoolGenerator.cs",
            "D:\\учеба\\СПП\\Tracer\\Library\\Tracer\\Impl\\DefaultTracer.cs"
        };

        private static void Main(string[] args)
        {
            var testsGenerator = new TestsGenerator();
            
            var readFile = new TransformBlock<string, string>(async path =>
            {
                Console.WriteLine("Loading to memory '{0}'...", path);
                using var sourceReader = File.OpenText(path);
                return await sourceReader.ReadToEndAsync();
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = MaxReadingDegree,
                EnsureOrdered = false
            });

            var generateTestClasses = new TransformManyBlock<string, TestClass>(text =>
            {
                Console.WriteLine("Generating test classes...");
                return testsGenerator.GenerateTestClasses(text);
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = MaxGeneratingDegree,
                EnsureOrdered = false
            });

            var writeTestClassToFile = new ActionBlock<TestClass>(async testClass =>
            {
                await using var destinationWriter = File.CreateText(DestPath + testClass.FileName);
                Console.WriteLine("Saving '{0}' on disk...", testClass.FileName);
                await destinationWriter.WriteAsync(testClass.SourceCode);
                Console.WriteLine("Saved '{0}'!", testClass.FileName);
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = MaxWritingDegree,
                EnsureOrdered = false
            });

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};

            readFile.LinkTo(generateTestClasses, linkOptions);
            generateTestClasses.LinkTo(writeTestClassToFile, linkOptions);

            foreach (var path in Files)
            {
                readFile.Post(path);
            }

            readFile.Complete();

            writeTestClassToFile.Completion.Wait();
        }
    }
}