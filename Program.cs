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

        private static string GetMov (int index)
        {
            var s = $"movq ${index}, {4*index}%0"; 
            return $"\"{s}\\n\\t\"{Environment.NewLine}";
        }

        private static string GenerateStoreSequenceGCC(int numStores, int numNops)
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

        private static string GenerateStoreSequenceMSVC(int numStores, int numNops)
        {
            var sb = new StringBuilder();
            sb.Append("__asm {" + Environment.NewLine);
            sb.Append($"mov ebx, 123{Environment.NewLine}");
            sb.Append($"mov eax, p{Environment.NewLine}");
            for(int i = 0; i < numStores; ++i)
            {
                sb.Append($"mov [eax+{i * 4}], ebx{Environment.NewLine}");
            }
            for(int i = 0; i < numNops; ++i)
            {
                sb.Append($"nop{Environment.NewLine}");
            }
            sb.Append("}" + Environment.NewLine);
            return sb.ToString();
        }

        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine($"Expected arguments <num stores> <msvc/gcc/iacagcc> [num nops]");
                return;
            }
            int numStores = Int32.Parse(args[0]);
            int numNops = 500;
            var mode = args[1].ToUpper();
            if(args.Length > 2)
            {
                numNops = Int32.Parse(args[2]);
            }
            var templateName = $"GuessSB_{mode}_Template.cpp";
            var template = File.OpenText(templateName);
            var s = template.ReadToEnd();
            template.Close();
            var output = new StreamWriter("GuessSB.cpp");
            var startToken = "// REPLACE-TOKEN";
            var startIdx = s.IndexOf(startToken);
            var prefix = s.Substring(0, startIdx);
            var suffix = s.Substring(startIdx + startToken.Length);
            var asm = mode == "msvc" ? GenerateStoreSequenceMSVC(numStores, numNops) : GenerateStoreSequenceGCC(numStores, numNops);
            var result = $"{prefix}{asm}{suffix}";
            output.Write(result);
            output.Close();
        }
    }
}
