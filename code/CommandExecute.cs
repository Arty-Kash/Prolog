//using System.Collections.Generic;
//using System.Text;

//#define SystemIO
#define PCLStorage

using System;
using System.Linq;
using Xamarin.Forms;

#if SystemIO
    using System.IO;
#elif PCLStorage
    using PCLStorage;
    using System.Threading.Tasks;
#endif


namespace Prolog
{
    public partial class MainPage : ContentPage
    {

#if SystemIO
        // Working Directory when using System I/O
        string WorkingDir = "/Users/atsu/Projects/Xamarin/App/Prolog/Prolog/pl/";        
        //string WorkingDir = @"C:\Users\0000010745011\AppData\Local\Packages\135baee3-8b33-4698-955b-a2060bcefdfe_zkjxvjmeh878e\LocalState\RootFolder";

        // Identify Input Command and Execute it
        void CommandExecute(string command)
        {
            string result = ""; // String of the command execute result

            // Get the three characters of the command to identify
            string com = command;
            if (command != null)
                if (command.Length > 3)
                    com = command.Substring(0, 3);

            // Identify the command from the extracted three characters            
            switch (com)
            {
                case "":    // No command, only CR
                case null:
                    AddPrompt(GetPromptText());
                    return;

                case "pro": // Run the Prolog Interpreter
                    if (command == "prolog")
                    {
                        RunProlog = true;
                        prolog.Reset();
                        result = "\n";
                    }
                    else
                        result = "Command Invalid";

                    break;

                case "dir": // Show the files and the directories
                    result = dir();
                    break;

                case "pwd": // Print the current working directory
                    result = ">" + CurrentDir + "\n";
                    break;


                case "ed ": // Run the file editor
                    result = ed(command);
                    break;

                default:    // Text other than the commands is entered, return the text
                    result = command + "\n";
                    break;
            }

            // Display the Command Execute Result following the Command Line
            Console.Children.Add(new Label() { Text = result });

            // Store the result
            CommandLines.Last().Results = result;

            // Add a new Command Line
            AddPrompt(GetPromptText());

        }


        // Show the files and directories in the current directory
        string dir()
        {
            string result = "";

            //string[] files = 
            //    Directory.GetFiles(WorkingDir, "*", SearchOption.TopDirectoryOnly);

            //foreach (string file in files)
            //    result += (file.Substring(WorkingDir.Length) + "\n");




            try
            {
                string[] files =
                    Directory.GetFiles(WorkingDir, "*", SearchOption.TopDirectoryOnly);
                int WorkingDirTextLength = WorkingDir.Length;

                foreach (string file in files)
                    result += (file.Substring(WorkingDirTextLength) + "\n");
            }
            catch (UnauthorizedAccessException UAEx)
            {
                result += UAEx.Message;
            }


            return result;
        }


        string OriginalFile = "";
        // Pop up the file editor
        string ed(string command)
        {
            string result = " ";
            string name = "";

            if (command.Substring(3) == "")
                return "No File Name";
            else name = command.Substring(3);

            // Check the existence of the File
            bool IsExist = File.Exists(WorkingDir + name);


            if (IsExist)    // Get all text of the file
            {
                FileEditor.Text = File.ReadAllText(WorkingDir + name);
                OriginalFile = FileEditor.Text;
            }
            else            // If no file, create a new file
            {
                //File.Create(name).Close();
                File.Create(name);
                FileEditor.Text = "";
            }

            FileEditorTitle.Text = "File Name: " + name;
            FileEditorGrid.IsVisible = true;    // Make the file editor visible

            // Editor.Focus() doesn't work due to timing probelm(?), 
            // so maake it into Timer 
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                FileEditor.Focus();
                return false;   // Timer Cycle is only one time
            });

            return result;
        }



        // Buttons of FileEditor Menu: 
        //     save File & exit, save File, exit without saving file
        void SaveExitFile(object sender, EventArgs e)
        {
            string name = (FileEditorTitle.Text).Substring(11);
            File.WriteAllText(WorkingDir + name, FileEditor.Text);
            FileEditorGrid.IsVisible = false;
            CommandLines.Last().CommandEntry.Focus();
        }
        void SaveFile(object sender, EventArgs e)
        {
            string name = (FileEditorTitle.Text).Substring(11);
            File.WriteAllText(WorkingDir + name, FileEditor.Text);
        }
        async void CancelEditor(object sender, EventArgs e)
        {
            if (FileEditor.Text != OriginalFile)
                if (await DisplayAlert("Don't Save?", "", "Cancel", "OK"))
                    return;

            FileEditorGrid.IsVisible = false;
            CommandLines.Last().CommandEntry.Focus();

            /*
            if (FileEditor.Text == OriginalFile)
            {
                FileEditorGrid.IsVisible = false;
                CommandLines.Last().CommandEntry.Focus();
            }
            else if(!await DisplayAlert("Don't Save?", "", "Cancel", "OK"))
            {
                FileEditorGrid.IsVisible = false;
                CommandLines.Last().CommandEntry.Focus();
            }
            */
        }


        // Buttons for Debug
        void Button01(object sender, EventArgs args)
        {
            prolog.Reset();

            CommandLines.Last().Command = "[test.txt].";
            CommandLines.Last().CommandLabel.Text = CommandLines.Last().Command;
            CommandLines.Last().CommandEntry.Focus();

            //Label label = new Label();

            //string code = File.ReadAllText("/Users/atsu/Projects/Xamarin/App/Prolog/Prolog/SyntaxChecker.pl");
            //string code = File.ReadAllText(WorkingDir + "test.txt");

            //label.Text = code;
            //Console.Children.Add(label);

            //if (SetClause(prolog, code))
            //    StatusLabel.Text = "Set Clauses";


            //List<Solution> solutions = GetSolutions(prolog, query);
            //label.Text += SolutionsString(solutions);

            //var solution = prolog.GetAllSolutions("test.prolog", query);
            //foreach(var s in solution.NextSolution)
            //    label.Text += s.ToString();

            //Console.Children.Add(label);

            //prolog.Reset();
            //AddPrompt(GetPromptText(prolog.Prompt));

        }

        void Button02(object sender, EventArgs args)
        {
            //prolog.Reset();
            //Label label = new Label();
            //string code = "square(X,Y) :- Y is X * X. ";
            string query = "split(\"ab.cd.\", X).";

            //string codeTitle = "conde2";

            //label.Text =
            //    string.Format("{0} {1}\n", prolog.Prompt, query);

            //if (SetClause(prolog, code, false, codeTitle))
            //    StatusLabel.Text = "Set Clauses";

            CommandLines.Last().Command = query;
            CommandLines.Last().CommandLabel.Text = CommandLines.Last().Command;
            CommandLines.Last().CommandEntry.Focus();

            //List<Solution> solutions = GetSolutions(prolog, query);
            //label.Text += SolutionsString(solutions);
            //Console.Children.Add(label);

            //prolog.Reset();
            //AddPrompt(GetPromptText(prolog.Prompt));

            /*
            Console.HeightRequest = Console.Height + 100.0;
            
            Label3.Text = string.Format("Console Size = ( {0}, {1} )",
                                        Console.Width, Console.HeightRequest);
            //Label2.Text = "Clicked " + Console.HeightRequest.ToString();
            ConsoleBack.ScrollToAsync(0, Console.HeightRequest, false);
            */
        }


        void Button03(object sender, EventArgs args)
        {
            //string code = File.ReadAllText("/Users/atsu/Projects/Xamarin/App/Prolog/Prolog/test.pl");            
            //File.WriteAllText("/Users/atsu/Projects/Xamarin/App/Prolog/Prolog/test.pl", code);

            string r = StringToList(" X = \"test\". ");
            Label3.Text = r;
        }

#elif PCLStorage
        // Identify Input Command and Execute it
        async void CommandExecute(string command)
        {
            string result = "";

            // Get the first three characters of command
            string com = command;
            if (command != null)
                if (command.Length > 3)
                    com = command.Substring(0, 3);


            // Identify the command from the first three characters            
            switch (com)
            {
                case "":
                case null:
                    AddPrompt(CurrentFolder.Path);
                    return;

                case "pro":
                    if (command == "prolog")
                    {
                        RunProlog = true;
                        prolog.Reset();
                        result = "\n";
                    }
                    else
                    {
                        result = "Command Invalid";
                    }
                    break;

                case "dir":
                    result = dir();
                    break;

                case "pwd":
                    result = CurrentFolder.Path + "\n";
                    break;

                case "mkd":
                    result = await mkdir(command);
                    break;

                case "cd ":
                    result = await cd(command);
                    break;

                case "del":
                    result = await del(command);
                    break;

                case "mor":
                    result = await more(command);
                    break;

                case "ed ":
                    result = await ed(command);
                    break;

                

                default:
                    result = "No Command" + "\n";
                    break;
            }

            // Display the Command Execute Result following the Command Line
            Console.Children.Add(new Label() { Text = result });

            // Store the result
            CommandLines.Last().Results = result;

            // Add a new Command Line
            //AddPrompt(CurrentFolder.Path);
            AddPrompt(GetPromptText());
        }


        // Display All Folders and Files
        string dir()
        {
            string result = "";
            Folder folder = CurrentFolder;

            foreach (Folder subfolder in folder.SubFolders)
                result += string.Format("<DIR>  {0}\n", subfolder.Name);

            foreach (File file in folder.Files)
                result += file.Name + "\n";

            return result;
        }


        // Make Directory
        async Task<string> mkdir(string command)
        {
            string result = " ";
            string name = "";
            Folder parent = CurrentFolder;

            if (command.Length < 6) return "Command Invalid";
            else if (command.Substring(0, 6) != "mkdir ")
                return "Command Invalid";
            else if (command.Substring(6) == "")
                return "No Folder Name";
            else name = command.Substring(6);

            Folder folder = new Folder()
            {
                Name = name,
                Parent = parent
            };

            // Check the existence of the IFolder corresponding to the Object "folder"
            bool IsExist = await ICheckFolderExist(name, parent.iFolder);
            if (IsExist) return name.ToString() + " already exists";

            folder.iFolder = await ICreateFolder(folder, parent.iFolder, false);
            parent.SubFolders.Add(folder);

            //Label3.Text = folder.iFolder.Name;

            return result;
        }

        // Change Directory
        async Task<string> cd(string command)
        {
            string result = " ";
            string name = "";

            if (command.Substring(3) == "")
                return "No Folder Name";
            else name = command.Substring(3);

            // Back to the Parent Folder
            if (name == "..")
            {
                if (CurrentFolder == LocalStorage) return "No Parent Folder";
                CurrentFolder = CurrentFolder.Parent;
                return result;
            }

            // Check the existence of the IFolder corresponding to the Object "folder"
            bool IsExist = await ICheckFolderExist(name, CurrentFolder.iFolder);
            if (!IsExist) return name + " doesn't exist";

            foreach (Folder folder in CurrentFolder.SubFolders)
            {
                if (folder.Name == name)
                {
                    CurrentFolder = folder;
                    break;
                }
            }

            return result;
        }

        // Delete a Folder or a File
        async Task<string> del(string command)
        {
            string result = " ";
            string name = "";

            if (command.Substring(0, 4) != "del ")
                return "Command Invalid";
            else if (command.Substring(4) == "")
                return "No Folder/File Name";
            else name = command.Substring(4);

            // Check the existence of the IFolder or the IFile
            bool IsExist1 = await ICheckFolderExist(name, CurrentFolder.iFolder);
            bool IsExist2 = await ICheckFileExist(name, CurrentFolder.iFolder);

            if (IsExist1)        // Delete Folder
            {
                Folder DelFolder = new Folder();
                foreach (Folder folder in CurrentFolder.SubFolders)
                {
                    if (folder.Name == name)
                    {
                        await IDeleteFolder(folder.iFolder, folder.IParent);
                        DelFolder = folder;
                        break;
                    }
                }
                CurrentFolder.SubFolders.Remove(DelFolder);
            }
            else if (IsExist2)   // Delete File
            {
                File DelFile = new File();
                foreach (File file in CurrentFolder.Files)
                {
                    if (file.Name == name)
                    {
                        await IDeleteFile(file.iFile, file.IParent);
                        DelFile = file;
                        break;
                    }
                }
                CurrentFolder.Files.Remove(DelFile);
            }
            else return name + " doesn't exist";

            return result;
        }


        // Display File Content
        async Task<string> more(string command)
        {
            //string result = " ";
            string name = "";
            Folder parent = CurrentFolder;

            if (command.Length < 5) return "Command Invalid";
            else if (command.Substring(0, 5) != "more ")
                return "Command Invalid";
            else if (command.Substring(5) == "")
                return "No File Name";
            else name = command.Substring(5);

            foreach (File file in parent.Files)
                if (file.Name == name)
                    return await IReadFile(file.iFile);

            return "No such File";
        }

        // Edit File
        string OriginalFile = "";
        async Task<string> ed(string command)
        {
            string result = " ";
            string name = "";
            Folder parent = CurrentFolder;

            if (command.Substring(3) == "")
                return "No File Name";
            else name = command.Substring(3);

            // Check the existence of the File
            bool IsExist = await ICheckFileExist(name, parent.iFolder);

            if (IsExist)
            {
                foreach (File file in parent.Files)
                {
                    if (file.Name == name)
                    {
                        FileEditor.Text = await IReadFile(file.iFile);
                        CurrentFile = file;
                        break;
                    }
                }
            }
            else
            {
                File file = new File()
                {
                    Name = name,
                    Parent = parent
                };
                file.iFile = await ICreateFile(file, parent.iFolder);
                parent.Files.Add(file);

                CurrentFile = file;
            }

            FileEditorTitle.Text = "File Name: " + CurrentFile.Name;
            FileEditorGrid.IsVisible = true;

            // Editor.Focus() doesn't work due to timing probelm(?), 
            // so maake it into Timer 
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                FileEditor.Focus();
                return false;   // Timer Cycle is only one time
            });

            return result;
        }

        
        // Buttons of FileEditor Menu: 
        //     save File & exit, save File, exit without saving file
        void SaveExitFile(object sender, EventArgs e)
        {
            // PCL Storage: Write content in IFile
            IWriteFile(CurrentFile.iFile, FileEditor.Text);

            FileEditorGrid.IsVisible = false;
            CommandLines.Last().CommandEntry.Focus();
        }
        void SaveFile(object sender, EventArgs e)
        {
            // PCL Storage: Write content in IFile
            IWriteFile(CurrentFile.iFile, FileEditor.Text);
        }
        async void CancelEditor(object sender, EventArgs e)
        {
            if (FileEditor.Text != OriginalFile)
                if (await DisplayAlert("Don't Save?", "", "Cancel", "OK"))
                    return;

            FileEditorGrid.IsVisible = false;
            CommandLines.Last().CommandEntry.Focus();
        }



        async void Button01(object sender, EventArgs args)
        {
            prolog.Reset();
            Label label = new Label();

            IFile iFile = await IGetFile("SyntaxChecker.pl", CurrentFolder);
            string code = await IReadFile(iFile);

            //label.Text = code;
            //Console.Children.Add(label);
            //AddPrompt(GetPromptText());

            if (SetClause(prolog, code, true))
                StatusLabel.Text = "Set Clauses";

        }

        
        void Button02(object sender, EventArgs args)
        {
            /*
            Clause clause = new Clause()
            {
                Predicate = "test"
                
            };

            Label2.Text = clause.Rule.ToString();
            */
        }
        


        void Button03(object sender, EventArgs args)
        {
            Label3.Text = ILocalStorage.Path;
        }

#endif


    }
}