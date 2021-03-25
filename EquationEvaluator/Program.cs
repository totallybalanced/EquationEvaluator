using System;

namespace EquationEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {   
            Evaluator eval = new();

            while (true)
            {
                Console.Write("Enter equation: ");
                string x = Console.ReadLine();
                Console.WriteLine(eval.solve(x));
            }
            //Console.ReadKey();
        }
    }
}
