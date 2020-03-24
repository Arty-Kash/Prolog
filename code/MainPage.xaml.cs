//using System.Text;
//using System.Threading.Tasks;
//using System.ComponentModel;
//using System.Collections;
//using System.Globalization;
//using System.Collections.Generic;
//using Prolog;

//#define SystemIO
#define PCLStorage

using System;
using System.Linq;
using Xamarin.Forms;
#if SystemIO
    using System.IO;
#endif


namespace Prolog
{
    public partial class MainPage : ContentPage
    {

        
        // Current Directory when using System I/O
        string CurrentDir = " ";


        public MainPage()
        {
            InitializeComponent();            

            // Add the 1st Prompt on the Console
#if SystemIO
            AddPrompt(GetPromptText());
#elif PCLStorage
            CreateRootFolder();
            AddPrompt(CurrentFolder.Path);
            //Label1.Text = LocalStorage.
#endif

            // Just after launching the App, Entry.Focus() doesn't work 
            // due to timing probelm(?), so maake it into Timer 
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                CommandLines.Last().CommandEntry.Focus();
                return false;   // Timer Cycle is only one time
            });

        }



        // Get the suitable Prompt Text according to the running mode
        string GetPromptText()
        {
            if(!RunProlog)
                return ">" + CurrentDir;
            else if (RunUserSetClause)
                return " |: ";
            else
                // prolog.Prompt: space + CR + No + "?-", 
                // so remove space and CR from it
                return prolog.Prompt.Substring(2);            
        }












    }
}


/*
            string query = "fly(Y).";
            label.Text =
                string.Format("{0} {1}\n", prolog.Prompt, query);

            if (SetClause(prolog, code))
                StatusLabel.Text = "Set Clauses";

            List<Solution> solutions = GetSolutions(prolog, query);

            label.Text += string.Format("Solutions count = {0}\n",
                prolog.SolutionIterator.Count());

            label.Text += SolutionsString(solutions);

            CommandLines.Last().CommandEntry.Text = query;
            CommandLines.Last().CommandEntry.Focus();
            */


/*
label = new Label();
code = File.ReadAllText("test.prolog");
query = "fly(Y).";
label.Text =
    string.Format("{0} {1}\n", prolog.Prompt, query);
if (SetClause(prolog, code))
    StatusLabel.Text = "Set Clauses";

solutions = GetSolutions(prolog, query);

label.Text += string.Format("Solutions count = {0}\n",
    prolog.SolutionIterator.Count());

label.Text += SolutionsString(solutions);

Console.Children.Add(label);
*/


/*
            Label label = new Label();
            string code =
                "human(socrates). " +
                "human(platon). " +
                "mortal(X) :- human(X).";                
            string query = "mortal(Y).";

            label.Text = 
                string.Format("{0} {1}\n", prolog.Prompt, query);

            if (SetClause(prolog, code, false))
                StatusLabel.Text = "Set Clauses";
            
            List<Solution> solutions = GetSolutions(prolog, query);
            
            label.Text += SolutionsString(solutions);
            
            Console.Children.Add(label);




            label = new Label();
            code =
                "fly(X) :- airplane(X). " +
                "airplane(jet_plane). airplane(helicopter). airplane(bbb). airplane(ccc).";
            query = "fly(Y).";
            string codeTitle = "code1";

            label.Text =
                string.Format("{0} {1}\n", prolog.Prompt, query);

            if (SetClause(prolog, code, false, codeTitle))
                StatusLabel.Text = "Set Clauses";

            solutions = GetSolutions(prolog, query);

            label.Text += string.Format("Solutions count = {0}\n",
                prolog.SolutionIterator.Count());
            
            label.Text += SolutionsString(solutions);

            Console.Children.Add(label);



            label = new Label();
            code =
                "human(socrates). " +
                "human(platon). " +
                "human(niche). " +
                "mortal(X) :- human(X).";
            query = "human(niche).";
            label.Text =
                string.Format("{0} {1}\n", prolog.Prompt, query);
            if (SetClause(prolog, code))
                StatusLabel.Text = "Set Clauses";
            solutions = GetSolutions(prolog, query);
            label.Text += SolutionsString(solutions);
            Console.Children.Add(label);

            label = new Label();
            query = "human(monkey).";
            label.Text =
                string.Format("{0} {1}\n", prolog.Prompt, query);
            solutions = GetSolutions(prolog, query);
            label.Text += SolutionsString(solutions);
            Console.Children.Add(label);




            label = new Label();
            code = File.ReadAllText("test.prolog");
            query = "fly(Y).";
            label.Text =
                string.Format("{0} {1}\n", prolog.Prompt, query);
            if (SetClause(prolog, code))
                StatusLabel.Text = "Set Clauses";

            solutions = GetSolutions(prolog, query);

            label.Text += string.Format("Solutions count = {0}\n",
                prolog.SolutionIterator.Count());

            label.Text += SolutionsString(solutions);

            Console.Children.Add(label);
            */


