using System;

namespace MusicNamer
{
    class Program
    {
        static void Main(string[] args)
        {
            var namer = new MusicNamer();

            namer.Process(args[0]);

            Console.ReadLine();
        }
    }
}