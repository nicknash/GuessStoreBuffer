using System;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace GuessStoreBuffer
{
    class Program
    {
        private static readonly string AsmPrologue = 
@"__asm__
  __volatile__
 (
";

private static readonly string AsmEpilogue = 
@"
	: // No output 
	: ""m""(*p) 
	: ""memory""
	);
";

        private static string GetMov(int index)
        {
            var s = $"movq ${index}, {4*index}%0"; 
            return $"\"{s}\\n\\t\"{Environment.NewLine}";
        }

        private static string GenerateStoreSequence(int numStores, int numNops)
        {
            var sb = new StringBuilder();
            sb.Append(AsmPrologue);
            for(int i = 0; i < numStores; ++i)
            {
                sb.Append(GetMov(i));
            }
            for(int i = 0; i < numNops; ++i)
            {
                sb.Append($"\"nop\\n\\t\"{Environment.NewLine}");
            }
            sb.Append(AsmEpilogue);
            return sb.ToString();
        }

        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine($"Expected arguments <num stores> [num nops]");
                return;
            }
            int numStores = Int32.Parse(args[0]);
            int numNops = 500;
            if(args.Length > 1)
            {
                numNops = Int32.Parse(args[1]);
            }
            var template = File.OpenText("GuessSB_Template.cpp");
            var s = template.ReadToEnd();
            template.Close();
            var output = new StreamWriter("GuessSB.cpp");
            var startToken = "// REPLACE-TOKEN";
            var startIdx = s.IndexOf(startToken);
            var prefix = s.Substring(0, startIdx);
            var suffix = s.Substring(startIdx + startToken.Length);
            var asm = GenerateStoreSequence(numStores, numNops);
            var result = $"{prefix}{asm}{suffix}";
            output.Write(result);
            output.Close();
        }
    }
}
