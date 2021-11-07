using System;
using System.IO;
using System.Reflection;
using SliccDB.Cypher;
using SliccDB.Serialization;

namespace CypherSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CypherSharp testing tool. Only for testing purposes. \n For implementation see [SliccDB.Cypher] \n MIT License @ Piotr Mikstacki");

            while (true)
            {
                var cypherString = Console.ReadLine();
                Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\temp\\");
                File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\temp\\cypherstring.txt",cypherString);
                CypherInterpreter cypherInterpreter = new CypherInterpreter(new DatabaseConnection(@"C:\Users\mixerek\Desktop\test.db"));
                cypherInterpreter.Interpret(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\temp\\cypherstring.txt");
            }

        }
    }
}
