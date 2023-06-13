using System.Globalization;
using System.Text;

namespace UsbDumpParser;

internal class Program
{
    static void Main(string[] args)
    {
        var baseFolder = "../../../../../dumps";
        var files = new List<(string In, string Out)>()
        {
            (Path.Combine(baseFolder, "plug-in.txt"), Path.Combine(baseFolder, "plug-in.out.txt")),
            (Path.Combine(baseFolder, "to-mon.txt"), Path.Combine(baseFolder, "to-mon.out.txt")),
        };

        foreach (var filePair in files)
        {
            ProcessDump(filePair.In, filePair.Out);
        }
    }

    static void ProcessDump(string inFileName, string outFileName)
    {
        var fileLines = File.ReadAllLines(inFileName);

        var builder = new StringBuilder();
        foreach (var line in fileLines)
        {
            ProcessLine(line, builder);
        }

        File.WriteAllText(outFileName, builder.ToString());
    }

    static void ProcessLine(string line, StringBuilder builder)
    {
        var parts = line.Split("ERROR");
        var logEntry = parts[1];
        if (logEntry.TrimStart().StartsWith("W"))
        {
            ProcessWriteEvent(logEntry, builder);
        }
        else
        {
            ProcessReadEvent(logEntry, builder);
        }

        builder.AppendLine("|------------------------------------------|");
    }

    static void ProcessWriteEvent(string line, StringBuilder builder)
    {
        var eventParams = line.Split(';');
        string bitCount = eventParams[0].Trim() switch
        {
            "W08" => "08",
            "W16" => "16",
            "W32" => "32",
            "W0N" => " N",
            _=>"_"
        };

        ushort? address = null;
        uint? writeValue = null;

        foreach (var param in eventParams.Skip(1))
        {
            var pair = param.Split(':');
            switch (pair[0])
            {
                case "A":
                    address = UInt16.Parse(pair[1], NumberStyles.HexNumber);
                    break;
                case "V":
                    writeValue = UInt32.Parse(pair[1], NumberStyles.HexNumber);
                    break;
            }
        }

        builder.AppendLine($"Write{bitCount}\tAddr:0x{address:X4}   Value:0x{writeValue}");
    }

    static void ProcessReadEvent(string line, StringBuilder builder)
    {
        var eventParams = line.Split(';');
        string bitCount = eventParams[0].Trim() switch
        {
            "R08" => "08",
            "R16" => "16",
            "R32" => "32",
            "R0N" => " N",
            _ => "_"
        };

        ushort? address = null;
        uint? writeValue = null;

        foreach (var param in eventParams.Skip(1))
        {
            var pair = param.Split(':');
            switch (pair[0])
            {
                case "A":
                    address = UInt16.Parse(pair[1], NumberStyles.HexNumber);
                    break;
                case "R":
                    writeValue = UInt32.Parse(pair[1], NumberStyles.HexNumber);
                    break;
            }
        }

        builder.AppendLine($"Read{bitCount}\tAddr:0x{address:X4}   Value:0x{writeValue}");
    }
}