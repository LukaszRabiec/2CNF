namespace TwoCnf.Client
{
    using System;
    using System.IO;

    class Program
    {
        private static readonly string _catalogPath = AppDomain.CurrentDomain.BaseDirectory + @"Data\";
        private static readonly string _expressionPath = _catalogPath + @"Example1.txt";

        static void Main(string[] args)
        {
            var expression = ReadFromFile(_expressionPath);
            bool[] solution = new bool[0];

            Console.WriteLine($"Readed expression: \n{expression}");

            try
            {
                var parser = new LogexParser();
                var parsedDisjunctions = parser.Parse(expression);

                var twoCnfSolver = new TwoCnfSolver();
                solution = twoCnfSolver.FindSolution(parsedDisjunctions);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            PrintSolution(solution);

            Console.ReadKey();
        }

        private static string ReadFromFile(string filePath)
        {
            string expression;

            using (var sr = new StreamReader(filePath))
            {
                expression = sr.ReadToEnd();
            }

            return expression;
        }

        private static void PrintSolution(bool[] solution)
        {
            if (solution.Length == 0)
            {
                return;
            }

            Console.WriteLine("\nSolution:");
            for (int index = 0; index < solution.Length; index++)
            {
                Console.WriteLine($"x{index} = {solution[index]} \t !x{index} = {!solution[index]}");
            }
        }
    }
}
