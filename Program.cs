using System;
using System.Threading.Tasks;

namespace Quotes_Generator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var configReader = new ConfigReader();
                var generationConfig = configReader.ReadGenerationConfig();
                Console.WriteLine("Generation configuration successfully loaded!");

                var valuesGenerator = new ValuesGenerator(generationConfig);
                Task.Factory.StartNew(valuesGenerator.StartGeneration, TaskCreationOptions.LongRunning);
                Console.WriteLine("Data generator successfully started!");

                Console.WriteLine();
                Console.WriteLine("Press 'Enter' for show send packets count.");
                Console.WriteLine("Press 'CTRL+C' for close application.");
                Console.WriteLine();

                while (true)
                {
                    while (Console.ReadKey().Key != ConsoleKey.Enter) { }

                    Console.WriteLine($"Send Packets: {valuesGenerator.SendPackets}");
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key for exit...");
            Console.ReadKey();
        }
    }
}