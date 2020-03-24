//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Text;


//#define SystemIO
#define PCLStorage
//#define iOS

using System;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Prolog
{
	public partial class MainPage : ContentPage
	{

        class CommandLine        
        {
            // Prompt
            public string Prompt { get; set; }            
            private Label promptlabel()
            {
                return new Label() { Text = Prompt,
                                     TextColor = Color.Blue }; 
            }            
            public Label PromptLabel { get { return this.promptlabel(); } }


            // Command            
            //public string Command { get { return CommandEntry.Text; } }
            public string Command { get; set; }
            private Label commandlabel = new Label() { Text = " " };
            public Label CommandLabel { get { return this.commandlabel; } }


            // Command Input
            private Entry commandentry = new Entry()
            {
                BackgroundColor = Color.Transparent,                
                TextColor = Color.Transparent,
                //BackgroundColor = Color.White,
                //TextColor = Color.Black,
                HeightRequest = 0,      // Not to display this entry                
                Keyboard = Keyboard.Plain
            };
            public Entry CommandEntry { get { return this.commandentry; } }

            public int Length { get; set; }
            

            // StackLayout including all parts of Command Line
            private StackLayout promptcommand()
            {
                StackLayout s1 = new StackLayout()
                {
                    BackgroundColor = Color.Transparent,
                    Orientation = StackOrientation.Horizontal,
                    Children = { PromptLabel, CommandLabel }
                };
                StackLayout s2 = new StackLayout()
                {
                    Spacing = 0,
                    Children = { s1, CommandEntry }
                };

                return s2;
            }
            public StackLayout PromptCommand { get { return promptcommand(); } }


            // Blink Cursor and its Switch            
            public string BlinkString = "|";
            public bool BlinkOff { get; set; }


            // Solution Results
            public string Results { get; set; }
        }
        List<CommandLine> CommandLines = new List<CommandLine>();


        //int ConsoleSizeChanged = 0;
        int TextInputed = 0;
        int TextInputCompleted = 0;

        double ConsoleWidth, ConsoleHeight;
        double ScrollHeight; //ScrollWidth



        //Get "Console" size properly and Scroll "Console" to the end        
        void GetConsoleSize(object sender, EventArgs args)
        {
            ConsoleWidth = Console.Width;
            ConsoleHeight = Console.Height;
            ScrollHeight = ConsoleBack.Height;
            //Label1.Text = string.Format("W:{0},  ScH:{1},  CnH:{2}, CoH: {3},  SizeChange:{4}",
            //                            ConsoleWidth, ScrollHeight, ConsoleHeight,
            //                            ConsoleBack.Content.Height, ConsoleSizeChanged++);

            // Scroll "Console" to the Botom End
#if SystemIO
            ConsoleBack.ScrollToAsync(0, Console.Height-ConsoleBack.Height, false);
#endif
#if PCLStorage
            // Make ScrollToAsync into Timer due to timing probelm(?)
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                ConsoleBack.ScrollToAsync(0, Console.Height - ConsoleBack.Height, false);
                return false;   // Timer Cycle is only one time
            });
#endif            
        }



        // Add Prompt and Command Input Line
        void AddPrompt(string prompt)
        {
            CommandLine commandLine = new CommandLine();            

            commandLine.Prompt = prompt;
            Label comLabel = commandLine.CommandLabel;
            Entry comEntry = commandLine.CommandEntry;

            Console.Children.Add(commandLine.PromptCommand);
            CommandLines.Add(commandLine);
            

            // Add TextChanged & Completed Event on entry
            comEntry.TextChanged += InputChanged;
            comEntry.Completed += InputCompleted;

            comEntry.Focus();


            // Blink Cursor in Command Line
            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {

                // If Text Inputed in Command Line, finish this Timer
                bool NextTimerStart = !commandLine.BlinkOff;
                if (!NextTimerStart)
                {

                    // Not to let label be "|"
                    if (comEntry.Text == null) comLabel.Text = "";
                    //#endif

                    // Delete BlinkString
                    comLabel.Text = commandLine.Command;

                    return NextTimerStart;
                }

                if (commandLine.BlinkString == "|") commandLine.BlinkString = "";
                else commandLine.BlinkString = "|";

                //comLabel.Text = comEntry.Text + commandLine.BlinkString;
                comLabel.Text = commandLine.Command + commandLine.BlinkString;

                return NextTimerStart;

            });

        }


        // Copy Input Text in CommandEntry to CommandLabel
        void InputChanged(object sender, EventArgs args)
        {
            Entry entry = (Entry)sender;
            string command = entry.Text;

            int n = command.Length;
            //Label1.Text = "n = " + n.ToString() + " com = " + CommandLines.Last().Length;


            if (n < CommandLines.Last().Length) // for DEL key input
            {
                CommandLines.Last().Command
                            = CommandLines.Last().Command.Substring(0, n);
            }
            else if (n > 0)
            {
                string LastString = entry.Text.Substring(n - 1);
                //Label3.Text = LastString;
                //if (LastString == 0x0027) Label3.Text += " Yes";




                char[] c1 = LastString.ToCharArray();
                int c2 = (int)c1[0];
                Label3.Text = c2.ToString();

                /*
                int code = Convert.ToInt32(0x0027);
                char c = Convert.ToChar(code);
                string singlequote = c.ToString();
                Label2.Text = "A" + singlequote + "A";
                if (LastString == singlequote) LastString = ":";
                */

#if iOS
                
                switch (LastString) // Change Key from US Keyboard to JIS
                {
                    case "@":
                        LastString = "\"";
                        break;

                    case "{":
                        LastString = "|";
                        break;

                    case "}":
                        LastString = "{";
                        break;

                    case "|":
                        LastString = "}";
                        break;

                    case "&":
                        LastString = "\'";
                        break;

                    case "*":
                        LastString = "(";
                        break;

                    case "(":
                        LastString = ")";
                        break;

                    case "_":
                        LastString = "=";
                        break;

                    case "=":
                        LastString = "_";
                        break;
                    
                    case "]":                    
                        LastString = "[";
                        break;

                    case "\\":
                        LastString = "]";
                        break;

                    case ")":
                        LastString = "\\";
                        break;

                    case ":":
                        LastString = "+";
                        break;
                    

                    default:

                        // For " ' "
                        string s8216 = ((char)8216).ToString();
                        string s8217 = ((char)8217).ToString();
                        if (LastString == s8216 || LastString == s8217) LastString = ":";

                        // For " * "
                        string s8220 = ((char)8220).ToString();
                        string s8221 = ((char)8221).ToString();
                        if (LastString == s8220 || LastString == s8221) LastString = "*";

                        break;
                }
                
#endif
                CommandLines.Last().Command += LastString;
            }

            // Copy the input command to the CommandLabel to show on the command line
            CommandLines.Last().CommandLabel.Text = CommandLines.Last().Command;

            // Store the command length
            CommandLines.Last().Length = n;

            TextInputed++;
            //Label2.Text = string.Format("{0} TextChanged,  {1} InputCompleted",
            //                                TextInputed, TextInputCompleted);

        }


        // Called, when CR key is entered
        void InputCompleted(object sender, EventArgs args)
        {            
            Entry entry = (Entry)sender;        
            
            CommandLines.Last().BlinkOff = true; // erase the cursor blink

            // Change the prompt text according to the running mode
            if(!RunProlog)
                CommandExecute(CommandLines.Last().Command);
            else if(RunUserSetClause)
                UserSetClause(CommandLines.Last().Command);
            else
                PrologInterpreter(CommandLines.Last().Command);
            
            entry.IsEnabled = false;    // Not to focus the previous entry

            TextInputCompleted++;
            //Label2==.Text = string.Format("{0} TextChanged,  {1} InputCompleted",
            //                                TextInputed, TextInputCompleted);            
        }

        // Focus Command Line, if Tap Console
        void OnTapConsole(object sender, EventArgs args)
        {
            CommandLines.Last().CommandEntry.Focus();
        }

        // Raised, when the console was scrolled to the bottom end
        void ConsoleBackScrolled(object sender, EventArgs args)
        {
            //Label3.Text = string.Format("Scroll Y = {0}, Scrolled Y = {1}",
            //                            Console.Height - ConsoleBack.Height,
            //                            ConsoleBack.ScrollY);
        }

        /*
        void InputTapped(object sender, EventArgs args)
        {
            CommandLines.Last().CommandEntry.Focus();
            //CurrentEntry.Focus();
            StatusLabel.Text = "Tapped";
        }
        */

    }
}


/*
        public class PromptLine : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string input;

            public string EntryInput
            {
                get { return this.input; }
                set
                {
                    if (this.input != value)
                    {
                        this.input = value;
                        OnPropertyChanged("EntryInput");
                        OnPropertyChanged("LabelInput");
                    }
                }
            }

            protected virtual void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public string LabelInput { get { return EntryInput; } }
        }
        */

/*
        void ConsoleChildAdded(object sender, EventArgs args)
        {
            StackLayout s = (StackLayout)sender;
            ConsoleBack.ScrollToAsync(0, Console.Height - ConsoleBack.Height, false);

            //Console.HeightRequest = Console.Height + 100.0;
            //.Text = s.Height.ToString();
            //Label4.Text += "Changed ";
            //Label4.Text = string.Format("ConsoleBack.Content Size 4 = ( {0}, {1} )",
            //                            s.Width, ConsoleBack.Content.HeightRequest);
        }
        */

