
using System;

namespace LibraryTest
{
    public class NotEmptyClass
    {
        public void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }

        public int GetSum(int a, int b)
        {
            return a + b;
        }
    }
    
    public class NotEmptyClass1
    {
        public void PrintHello()
        {
            Console.WriteLine("Hello!");
        }

        public int GetMult(int a, int b)
        {
            return a * b;
        }
    }
}