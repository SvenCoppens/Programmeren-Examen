using System;
using System.Collections.Generic;
namespace Programmeren_Examen_Tool_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Serialization_extraction SE = new Serialization_extraction();
            Belgie belg = SE.Serialize();
            Console.WriteLine("Hello World!2");
        }
    }
}
