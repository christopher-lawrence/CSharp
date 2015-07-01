using System;

namespace hack_asm
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = @"C:\Users\Chris\Dropbox\src\nand2tetris\projects\06\max\Max.asm";

            var parse = new Parser(file);
            var commandLine = 0;

            while (parse.HasMoreCommands())
            {
                PrintCommandLine(parse.CommandLines[commandLine]);
                parse.Advance();
                commandLine++;
            }

            PrintCommandLine(parse.CommandLines[commandLine]);
        }

        static void PrintCommandLine(CommandLine commandLine)
        {
            Console.WriteLine("{0}", commandLine.CommandString);
            Console.WriteLine("\t\t{0}", commandLine.Type);
            if (commandLine.Type == Command.CCommand)
            {
                Console.WriteLine("\t\t{0} - {1} - {2}", commandLine.CCommand.Destination,
                                  commandLine.CCommand.Computation, commandLine.CCommand.Jump);
            }
            else
            {
                Console.WriteLine("\t\t{0}", commandLine.Symbol);
            }
        }
    }
}
