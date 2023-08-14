using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Xml.Linq;

namespace My_most_complex_interpreted_language_yet
{
    internal class Program
    {
        public static bool inFunctionDefinition = false;
        public static Dictionary<string, string> functions = new Dictionary<string, string>();
        public static Dictionary<string, object> variables = new Dictionary<string, object>();
        //public static Function<string>[] preDefinedFunctionsTypeString =
        //{
        //    new Function<string>("Write", new Action<string>((string input) => Console.Write(input)), variables)
        //};
        public static Function[] preDefinedFunctions =
        {
            new Function("Reset", new Action(() => runCode(path))),
            new Function("HelloWorld", new Action(() => Console.WriteLine("Hello world!"))),
            new Function("PrintVars", new Action(() => PrintVars()))
        };

        public static void PrintVars()
        {
            foreach (string key in variables.Keys)
            {
                Console.Write(key + " = ");
                Console.WriteLine(variables[key]);
            }
        }

        public static string path = string.Empty;

        static void Main(string[] args)
        {
            Console.Title = "M# interpreter";
            if (args.Length < 1)
            {
                //writeColours(new ConsoleLine("WARN: ", ConsoleColor.Yellow));
                //Console.WriteLine("You do not have a file loaded!");
                writeNotice(new ConsoleLine("WARN: ", ConsoleColor.Yellow), "You do not have a file loaded!");
                Console.Write("File path: ");
                string input = Console.ReadLine();
                path = input;
            } else
            {
                path = args[0];
            }
            try
            {
                runCode(path);
            } catch (Exception ex)
            {
                writeNotice(new ConsoleLine("ERROR: ", ConsoleColor.Red), ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static void runCode(string filePath)
        {
            string code = File.ReadAllText(filePath);
            string[] lines = code.Split("\n");
            int index = 0;
            foreach (string l in lines)
            {
                // YOU'RE SUPPOSED TO ACTUALLY RUN THE FUNCTION!!!! DON'T RUN IT WHEN IT'S DEFINED!
                if (functions.ContainsKey(l))
                {
                    foreach (string s in functions[l].Split("\n"))
                    {
                        runLine(s, index);
                    }
                    break;
                } else
                {
                    runLine(l, index);
                }
                index++;
            }
        }

        public static void runLine(string line, int index)
        {
            string[] lines = line.Split("\n");
            if (!line.StartsWith("//") && !inFunctionDefinition)
            {
                // Important
                if (line.StartsWith("end"))
                {
                    inFunctionDefinition = false;
                }
                #region keywords
                if (line.StartsWith("function "))
                {
                    inFunctionDefinition = true;
                    string name = line.Split(" ")[1];
                    if (!functions.ContainsKey(name))
                    {
                        functions.Add(name, lines.getLines(index + 1, "end"));
                    }
                    //writeColours(new ConsoleLine("INFO: ", ConsoleColor.Green));
                    writeNotice(new ConsoleLine("INFO: ", ConsoleColor.Green), "Added function: " + name);
                    //writeNotice(new ConsoleLine("INFO: ", ConsoleColor.Green), "^ With code: " + lines.getLines(index + 1, "end"));
                }
                if (line.StartsWith("var "))
                {
                    string[] varDec = line.Split(" = ");
                    varDec[0] = varDec[0].Replace("var ", "");
                    variables.Add(varDec[0], varDec[1]);
                    writeNotice(new ConsoleLine("INFO: ", ConsoleColor.Green), "Added variable: " + varDec[0] + " with value: " + varDec[1]);
                }
                #endregion
                foreach(Function f in preDefinedFunctions)
                {
                    if (line.StartsWith(f.name))
                    {
                        f.action();
                        writeNotice(new ConsoleLine("INFO: ", ConsoleColor.Green), "Ran pre-defined function: " + f.name);
                    }
                }
            }
        }

        public static void writeNotice(ConsoleLine prefix, string output)
        {
            writeColours(prefix);
            Console.WriteLine(output);
        }

        public static void writeColours(ConsoleLine line)
        {
            Console.ForegroundColor = line.colour;
            Console.Write(line.msg);
            Console.ResetColor();
        }
    }

    public class ConsoleLine
    {
        public string msg;
        public ConsoleColor colour;

        /// <summary>
        /// Creates a <see cref="ConsoleLine"/> which may be used in some methods in the <see cref="Program"/> <see langword="class"/>
        /// <para></para>
        /// <example>
        /// Use like this:
        /// <code>
        /// writeNotice(<see langword="new"/> <see cref="ConsoleLine"/>("PREFIX: ", <see cref="ConsoleColor"/>.Green), "Info");
        /// // or
        /// writeColours(<see langword="new"/> <see cref="ConsoleLine"/>("PREFIX: ", <see cref="ConsoleColor"/>.Green));
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="txt">The text</param>
        /// <param name="color">The colour</param>
        public ConsoleLine(string txt, ConsoleColor color)
        {
            msg = txt;
            colour = color;
        }
    }

    public static class extensionMethods
    {
        /// <summary>
        /// Returns a <see cref="string"/> containing all lines from <paramref name="from"/> until <paramref name="until"/>
        /// </summary>
        /// <param name="myStrArray"></param>
        /// <param name="from">The index to start on</param>
        /// <param name="until">The string to stop at</param>
        /// <returns></returns>
        public static string getLines(this string[] myStrArray, int from, string until)
        {
            string myStr = string.Empty;

            for (int i = from; i < myStrArray.Length; i++)
            {
                if (myStrArray[i] == until)
                {
                    break;
                }
                myStr += myStrArray[i] + "\n";
            }
            return myStr;
        }
    }
}