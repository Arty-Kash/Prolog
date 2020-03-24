//using System.Text;

//#define SystemIO
#define PCLStorage

// #define iOS

using System.Collections.Generic;
using System.Linq;

#if SystemIO
    using System.IO;
#elif PCLStorage
    using System.Threading.Tasks;    
    using PCLStorage;
#endif

using Xamarin.Forms;


namespace Prolog
{
    public partial class MainPage : ContentPage
    {
        // Create Prolog Engine
        PrologEngine prolog
            = new PrologEngine(persistentCommandHistory: false);

        bool RunProlog = false;
        bool RunUserSetClause = false;
        string UserSetClauses = "";

        
        /*
        class Clause
        {
            public string Predicate { get; set; }
            private bool fact;
            public bool Fact
            {
                get { return !this.rule; }
                set
                {
                    if (this.fact != value) this.fact = value;
                }
            }

            private bool rule;
            public bool Rule
            {
                get { return !this.fact; }
                set
                {
                    if (this.rule != value) this.rule = value;
                }
            }
        }
        */
        //List<Clause> Program = new List<Clause>();
        

        class Solution
        {
            public string Name { get; set; }
            public string Type { get; set; }
            //public ITermNode Value { get; set; }
            public string Value { get; set; }
            public string Note { get; set; }
        }


        // Prolog Interpreter:
        //    Identify Input Command and Execute it
#if SystemIO
        void PrologInterpreter(string command)
#elif PCLStorage
        async void PrologInterpreter(string command)
#endif
        {
            string result = "";
            string com = "";
            List<Solution> solutions = new List<Solution>();

            // Get the first character of the command
// #if iOS
            if (command != null)
// #else // else if UWP
//             if(command != "")
// #endif
                com = command.Substring(0, 1);

            // Identify the command from the three characters            
            switch (com)
            {
                case "":
                case null:
                    AddPrompt(GetPromptText());
                    return;

                case "l":   // List Segments of the current folder
                    if (command == "ls.")
                    {
                        result = ls();
                        break;
                    }
                    else goto default;

                case "[":   // Load Program
#if SystemIO
                    result = SetProgram(command);
#elif PCLStorage
                    result = await SetProgram(command);
#endif
                    break;


                case "h":   // Exit Prolog Interpreter
                    if (command == "halt.")
                    {
                        RunProlog = false;
                        prolog.Reset();                        
                        result = "\n";
                        //System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                        break;
                    }
                    else goto default;
                    //{
                    //    result = "Command Invalid";
                    //}
                    //break;

                case "r":   // Reset Prolog Engine
                    if (command == "reset.")
                    {
                        prolog.Reset();
                        result = "\n";
                        break;
                    }
                    else goto default;
                    //{
                    //    result = "Command Invalid";
                    //}
                    //break;


                default:    // Query Input
                    solutions = GetSolutions(prolog, command);
                    result = SolutionsString(solutions);
                    break;
            }

            // Display the Command Execute Result following the Command Line
            Console.Children.Add(new Label() { Text = result });

            // Store the result
            CommandLines.Last().Results = result;

            // Add a new Command Line
            AddPrompt(GetPromptText());
        }


        // List Segments of the current folder
        string ls()
        {
            string result = "";

#if SystemIO
            string[] files =
                Directory.GetFiles(WorkingDir, "*.*", SearchOption.TopDirectoryOnly);
            int WorkingDirTextLength = WorkingDir.Length;

            foreach (string file in files)
                result += (file.Substring(WorkingDirTextLength) + "\n");
#elif PCLStorage
            Folder folder = CurrentFolder;

            foreach (Folder subfolder in folder.SubFolders)
                result += string.Format("<DIR>  {0}\n", subfolder.Name);

            foreach (File file in folder.Files)
                result += file.Name + "\n";
#endif

            return result;
        }



        // Load Prolog program
#if SystemIO
        string SetProgram(string command)
#elif PCLStorage
        async Task<string> SetProgram(string command)
#endif
        {
            string result = "";

            int length = command.Length;
            string file = command.Substring(1, length - 3);

            if (command.Substring(length - 2) != "].")
            {
                result = "Input Invalid";
            }
            else if (file == "user")    // [user].
            {
                RunUserSetClause = true;
                UserSetClauses = "";
                result = "User Set Clause Mode";
            }
            else
            {
#if SystemIO
    #if iOS
                string code = File.ReadAllText(WorkingDir + file);
    #else
                string code = File.ReadAllText(WorkingDir + '\\' + file);
    #endif                
#elif PCLStorage
                IFile iFile = await IGetFile(file, CurrentFolder);
                string code = await IReadFile(iFile);
#endif
                result = code;

                // Set all clauses in the code
                if (SetClause(prolog, code))
                    StatusLabel.Text = "Set Clauses";

            }

            return result;
        }


        // Set Clause to the prolog engine
        bool SetClause(PrologEngine plEngine, string code, bool reset = true,
                        string codeTitle = null)
        {
            bool error = false;

            if (reset) plEngine.Reset();

            // If no syntax error, consult the code to the prolog engine
            if (SyntaxCheck(code))
                plEngine.ConsultFromString(code, codeTitle);

            else error = true;

            return !error;
        }


        // Prolog Syntax Checker
        bool SyntaxCheck(string code)
        {
            bool error = false;

            return !error;
        }



        // Get all solutions of the input query
        List<Solution> GetSolutions(PrologEngine plEngine, string query)
        {
            List<Solution> solutions = new List<Solution>();
            Solution solution = new Solution();

            // If string is in the query, convert to list
            query = StringToList(query);
            Label2.Text = query;

            // Set Query
            if (SyntaxCheck(query))
            {
                plEngine.Query = query;
            }
            else
            {
                solution.Note = "ERROR";
                solutions.Add(solution);
                return solutions;
            }


            // Get All Solutions
            foreach (PrologEngine.ISolution s in plEngine.SolutionIterator)
            {
                // The No of Solutions is ZERO, the solution is True or False
                if (s.VarValuesIterator.Count() == 0)
                {
                    solution.Note = "ZERO";
                    solution.Name = s.Solved.ToString();
                    solutions.Add(solution);
                    break;
                }

                foreach (PrologEngine.IVarValue v in s.VarValuesIterator)
                {
                    solutions.Add(new Solution()
                    {
                        Name = v.Name,
                        Type = v.DataType,
                        Value = v.Value.ToString(),
                        Note = ""
                    });
                }
                //if (s.IsLast) break;
            }
            return solutions;
        }


        // Convert string "aXb(Y)c:d-e" to list [a,'X', b,'(', 'Y', ')', c,':', d, '-', e] 
        string StringToList(string str)
        {
            char[] chars = str.ToCharArray();
            bool DoubleQuote = false;
            string s = "";

            foreach (char c in chars)
            {
                if (c == '\"')  // find Double Quotation
                {
                    if (!DoubleQuote)
                    {
                        s += "[";
                        DoubleQuote = true;
                    }
                    else
                    {
                        s = s.Substring(0, s.Length - 1);
                        s += "]";
                        DoubleQuote = false;
                    }
                }
                else if (DoubleQuote)
                {
                    switch (c)
                    {
                        case '(':
                        case ')':
                        case '[':
                        case ']':
                        case ':':
                        case '-':
                        case ',':
                        case '_':
                        case '.':
                        case ' ':
                        case '%':
                            s += "\'" + c + "\'";
                            break;

                        default:
                            if (char.IsUpper(c))
                                s += ("\'" + c + "\'");
                            else s += c.ToString();
                            break;
                    }

                    s += ",";
                }
                else  // if(!DoubleQuote)
                {
                    s += c.ToString();
                }

            }

            return s;
        }


        /*
        // Convert result list [a,'X', b,'(', 'Y', ')', c,':', d, '-', e] to string "aXb(Y)c:d-e"
        string ListToString(string list)
        {
            char[] chars = list.ToCharArray();
            string s = "";

            foreach (char c in chars)
            {
                if (c == '[' || c == ']' || c == '\'' || c == ',') continue;

                s += c;
            }

            return s;
        }
        */


        // Make the solutions strings from the Solution List 
        string SolutionsString(List<Solution> solutions)
        {
            string solution = "";

            foreach (Solution s in solutions)
            {
                if (s.Note == "ERROR")  // Error
                {
                    solution = s.Note + "\n";
                    break;
                }
                if (s.Note == "ZERO")   // Solution is True or False
                {
                    solution = s.Name + "\n";
                    break;
                }

                solution += string.Format("{0}({1}) = {2}\n",
                                                s.Name, s.Type, s.Value);
            }
            return solution;
        }



        // Identify User Input when [user] runs
        void UserSetClause(string clause)
        {
            string result = "";
            switch(clause)
            {
                case "exit.":
                    if (SetClause(prolog, UserSetClauses, false))
                        StatusLabel.Text = "Set Clauses";
                    RunUserSetClause = false;
                    result = "Set Clauses Sccessfully.\n";

                    // Display the Command Execute Result following the Command Line
                    Console.Children.Add(new Label() { Text = result });

                    break;

                default:
                    if(SyntaxCheck(clause))
                        UserSetClauses += (clause + "\n");
                    break;               
            }

            // Add a new Command Line
            AddPrompt(GetPromptText());
        }


    }
}




/*
label = new Label();
label.Text = prolog.Prompt;            

prolog.ConsultFromString("fly(X) :- airplane(X).");
prolog.ConsultFromString("airplane(jet_plane). airplane(helicopter).", "code1");
prolog.ConsultFromString("airplane(jet_plane). airplane(helicopter). airplane(bbb). airplane(ccc).", "code1");
//prolog.ConsultFromString("fly(X) :- airplane(X).", "title");
prolog.Query = "fly(Y).";
label.Text += " " + prolog.Query + "\n";

foreach (PrologEngine.ISolution s in prolog.SolutionIterator)
{
    //label.Text += s.Solved.ToString() + "\n";
    foreach (PrologEngine.IVarValue v in s.VarValuesIterator)
    {
        label.Text += string.Format("{0}({1}) = {2}\n",
                                    v.Name, v.DataType, v.Value.ToString());

    }
    //if (s.IsLast) break;
}            
*/

//var solution = prolog.GetFirstSolution(query: "fly(Y).");
//Label3.Text = solution.Solved.ToString();


/*
var solution = prolog.GetAllSolutions("test.prolog", query: "fly(Y).");
//var solution = prolog.GetAllSolutions("test.prolog", query: "square(2,Y).");            
Label1.Text = solution.Count.ToString();
Label2.Text = solution.Query.ToString();

foreach (var v in solution.NextSolution)
{
    Label3.Text += v.ToString();
}
*/



//foreach(PrologEngine.ISolution s in solution.VarValuesIterator)
//{
//    Label2.Text = s.ToString();
//}


/*
//Encoding enc = Encoding.GetEncoding("shift_jis");
//string file_path = Path.Combine(@"test.prolog");
//StreamWriter writer = new StreamWriter(@"C:\Users\0000010745011\Desktop\test.txt",
//    false, enc);            
Stream stream = File.OpenWrite("test.prolog");
StreamWriter writer = new StreamWriter(stream);
writer.WriteLine("test");
*/

/*
var solution = prolog.GetFirstSolution(query: "fly(Y).");
IEnumerator solutions = solution.VarValuesIterator.GetEnumerator();
while (solutions.MoveNext())
{
PrologEngine.IVarValue v = (PrologEngine.IVarValue)solutions.Current;
Label3.Text += v.ToString();
}
*/


/*
foreach (PrologEngine.IVarValue v in solution.VarValuesIterator)
{
Label2.Text += string.Format("{0} = {1} \n",
                            v.Name, v.Value);

}
Label2.Text += "End";


prolog.ConsultFromString("square(X,Y) :- Y is X * X.");
solution = prolog.GetFirstSolution(query: "square(2,Y).");

foreach (PrologEngine.IVarValue v in solution.VarValuesIterator)
{
Label3.Text = string.Format("{0}",
                            v.Value);
//v.Name,
//v.Value);
}
*/

