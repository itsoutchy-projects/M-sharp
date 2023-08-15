using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Xml.Linq;
using System.Threading;

namespace My_most_complex_interpreted_language_yet
{
    internal class Program
    {
        public static bool inFunctionDefinition = false; // We use this to make sure we don't run any functions as they're being defined, but rather when called
        public static Dictionary<string, string> functions = new Dictionary<string, string>(); // All the custom functions which were defined; We store this in a dictionary so they can easilly be run
        public static Dictionary<string, object> variables = new Dictionary<string, object>(); // All the variables defined in the code
        //public static Function<string>[] preDefinedFunctionsTypeString =
        //{
        //    new Function<string>("Write", new Action<string>((string input) => Console.Write(input)), variables)
        //    // This would be used for functions with parameters, however I faced quite a few difficulties, they were planned to be seperated by ",", and before the first parameter we put "<="
        //    // Meaning a full function call would go like this:
        //    //    name <= "param1"
        //};
        /// <summary>
        /// All the pre-defined functions go in here
        /// </summary>
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

        /// <summary>
        /// Runs a full M# script
        /// </summary>
        /// <param name="filePath">The path to the script</param>
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

        /// <summary>
        /// Runs a single line of code
        /// </summary>
        /// <param name="Theline">The line to run</param>
        /// <param name="index">The current index of iteration through code</param>
        public static void runLine(string Theline, int index)
        {
            string line = Theline.Trim(); // This allows for indentation, meaning your code can be organised and more readable
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

        /// <summary>
        /// Converts <paramref name="s"/> to a <see cref="ConsoleLine"/>
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator ConsoleLine(string s)
        {
            return new ConsoleLine(s, ConsoleColor.White);
        }
    }

    /// <summary>
    /// We put all extension methods here, to be used in the <see cref="Program"/> class
    /// </summary>
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