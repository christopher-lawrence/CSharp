using System;
using System.Collections.Generic;
using System.IO;

namespace hack_asm
{
    public class Parser
    {
        private readonly string[] _file;
        private int _lineNumber;

        public List<CommandLine> CommandLines;

        public Parser(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            _file = File.ReadAllLines(filePath);
            _lineNumber = 0;
            CommandLines = new List<CommandLine>();

            SkipInvalidLines();

            if (HasMoreCommands())
            {
                Advance();
            }
        }

        public bool HasMoreCommands()
        {
            return _file.Length > 0 && _lineNumber <= _file.Length-1;
        }

        public void Advance()
        {
            if (!HasMoreCommands()) return;

            CommandLines.Add(new CommandLine(_file[_lineNumber]));
            
            _lineNumber++;
        }

        private void SkipInvalidLines()
        {
            while (HasMoreCommands() &&
                   (_file[_lineNumber].StartsWith(@"//") || string.IsNullOrWhiteSpace(_file[_lineNumber])))
            {
                _lineNumber++;
            }
        }

        
    }

    public class CommandLine
    {
        public string CommandString { get; set; }
        public Command Type { get; set; }
        public string Symbol { get; set; }
        public CCommand CCommand { get; set; }

        public CommandLine(string line)
        {
            CommandString = StripLine(line);
            Type = GetType(CommandString);
            Symbol = GetSymbol(CommandString);
            CCommand = new CCommand
                {
                    Destination = GetDestination(CommandString),
                    Computation = GetComputation(CommandString),
                    Jump = GetJump(CommandString)
                };
        }

        private static string GetSymbol(string line)
        {
            if (GetType(line) == Command.CCommand) return null;

            return line.StartsWith(@"@")
                       ? line.Substring(1)
                       : line.Substring(1, line.IndexOf(@")", StringComparison.Ordinal)-1);
        }

        private static string GetDestination(string line)
        {
            return !line.Contains(@"=") ? null : line.Substring(0, line.IndexOf(@"=", StringComparison.Ordinal));
        }

        private static Command GetType(string line)
        {
            var type = Command.CCommand;
            if (line.StartsWith(@"@"))
            {
                type = Command.ACommand;
            }
            else if (line.StartsWith(@"("))
            {
                type = Command.LCommand;
            }

            return type;
        }

        private static string GetComputation(string line)
        {
            if (GetType(line) != Command.CCommand) return null;

            if (line.Contains(@"="))
            {
                return line.Substring(line.IndexOf(@"=", StringComparison.Ordinal)+1);
            }
            
            return line.Contains(@";") ? line.Substring(0, line.IndexOf(@";", StringComparison.Ordinal)) : null;
        }

        private static string GetJump(string line)
        {
            if (GetType(line) != Command.CCommand || !line.Contains(@";")) return null;

            return line.Substring(line.IndexOf(@";", StringComparison.Ordinal)+1);
        }

        private static string StripLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return null;

            line = line.TrimStart();
            return !line.Contains(@"//") ? line : line.Substring(0, line.IndexOf(" ", StringComparison.Ordinal));
        }
    }

    //TODO
    public class CCommand
    {
        public string Destination { get; set; }
        public string Computation { get; set; }
        public string Jump { get; set; }

        // Destination
        private const byte M = 0x1;
        private const byte D = 0x2;
        private const byte A = 0x4;

        // Computation
        // -- A=0
        private static readonly byte CZero      = Convert.ToByte("0101010", 2);
        private static readonly byte COne       = Convert.ToByte("0111111", 2);
        private static readonly byte CNegOne    = Convert.ToByte("0111010", 2);
        private static readonly byte CD         = Convert.ToByte("0001100", 2);
        private static readonly byte CA         = Convert.ToByte("0110000", 2);
        private static readonly byte CNotD      = Convert.ToByte("0001101", 2);
        private static readonly byte CNotA      = Convert.ToByte("0110001", 2);
        private static readonly byte CNegD      = Convert.ToByte("0001111", 2);
        private static readonly byte CNegA      = Convert.ToByte("0110011", 2);
        private static readonly byte CDPlus1    = Convert.ToByte("0011111", 2);
        private static readonly byte CAPlus1    = Convert.ToByte("0110111", 2);
        private static readonly byte CDMinus1   = Convert.ToByte("0001110", 2);
        private static readonly byte CAMinus1   = Convert.ToByte("0110010", 2);
        private static readonly byte CDPlusA    = Convert.ToByte("0000010", 2);
        private static readonly byte CDMinusA   = Convert.ToByte("0010011", 2);
        private static readonly byte CAMinusD   = Convert.ToByte("0000111", 2);
        private static readonly byte CDAndA     = Convert.ToByte("0000000", 2);
        private static readonly byte CDOrA      = Convert.ToByte("0010101", 2);
        // -- A=1
        private static readonly byte CM         = Convert.ToByte("1110000", 2);
        private static readonly byte CNotM      = Convert.ToByte("1110001", 2);
        private static readonly byte CNegM      = Convert.ToByte("1110011", 2);
        private static readonly byte CMPlus1    = Convert.ToByte("1110111", 2);
        private static readonly byte CMMinus1   = Convert.ToByte("1110010", 2);
        private static readonly byte CDPlusM    = Convert.ToByte("1000010", 2);
        private static readonly byte CDMinusM   = Convert.ToByte("1010011", 2);
        private static readonly byte CMMinusD   = Convert.ToByte("1000111", 2);
        private static readonly byte CDAndM     = Convert.ToByte("1000000", 2);
        private static readonly byte CDOrM      = Convert.ToByte("1010101", 2);

        public byte ConvertComputation()
        {
            byte retVal = 0;

            throw new NotImplementedException();
        }

        public byte ConvertDestination()
        {
            byte retVal = 0;
            if (string.IsNullOrEmpty(Destination))
                    return retVal;

            if (Destination.Contains("M"))
            {
                retVal = (byte) (retVal & M);
            }

            if (Destination.Contains("D"))
            {
                retVal = (byte) (retVal & D);
            }

            if (Destination.Contains("A"))
            {
                retVal = (byte) (retVal & A);
            }

            return retVal;
        }

        public byte ConvertJump()
        {
            throw new NotImplementedException();
        }
    }

    public enum Command
    {
        ACommand,
        CCommand,
        LCommand
    }
}
