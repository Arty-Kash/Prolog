//using System;
//using System.Linq;
//using System.Text;

#define PCLStorage

using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
#if PCLStorage
    using PCLStorage;
#endif

namespace Prolog
{
	public partial class MainPage : ContentPage
    {
#if PCLStorage
        // Set LocalStorage
        //    Name: ILocalStorage.Name, Path: ILocalStorage.Path
        static IFolder ILocalStorage = FileSystem.Current.LocalStorage;

        // Create Folder class of LocalStorage and RootFolder
        static Folder LocalStorage = new Folder()
        {
            Name = "LocalStorage",
            Parent = null,
            iFolder = ILocalStorage
        };
        static Folder RootFolder = new Folder()
        {
            Name = "RootFolder",
            Parent = LocalStorage
        };
        Folder CurrentFolder = RootFolder;


        // Folsder class
        class Folder
        {
            public string Name { get; set; }    // Folder Name           
            public Folder Parent { get; set; }  // Parent Folder
            private string path()
            {
                if (Name == "LocalStorage") return "LocalStorage>";
                else if (Name == "RootFolder") return ">";
                else if (Parent == RootFolder) return Name + ">";
                else return Parent.Path + Name + ">";
                /*
                if (Parent == null) return ">";
                else if(Parent == LocalStorage) return Name + ">";
                else return Parent.Path + Name + ">";
                */
            }
            public string Path { get { return path(); } } // Directory Path

            // SubFolder and File List in this Folder
            public List<Folder> SubFolders = new List<Folder>();
            public List<File> Files = new List<File>();
            
            public int NumChildren  // Number of Children in this folder
            {
                get { return SubFolders.Count + Files.Count; }
            }            
            public string NumChildrenText        // Text of NoChildren
            {
                get { return "(" + NumChildren.ToString() + ")"; }
            }
            
            // IFolder of this Folder and its Parent Folder
            public IFolder iFolder { get; set; }
            public IFolder IParent { get { return Parent.iFolder; } } // Parent IFolder
            /*
            private async Task<IFolder> ICreateFolder()
            {
                return await IParent.CreateFolderAsync(Name, CreationCollisionOption.ReplaceExisting);
            }
            //public Task<IFolder> iFolder { get { return ICreateFolder(); } }
            public IFolder iFolder { get { return (IFolder)ICreateFolder(); } }

            
            private async void ICreateFolder()
            {
                IFolder ifolder = await IParent.CreateFolderAsync(Name, CreationCollisionOption.ReplaceExisting);
                //return ifolder;
            }
            */

        }

        // File class
        class File
        {
            public string Name { get; set; }    // File Name
            public Folder Parent { get; set; }  // Parent Folder
            
            public string Path { get { return Parent.Path; } } // Directory Path

            // IFile of this File and its Parent Folder
            public IFile iFile { get; set; }
            public IFolder IParent { get { return Parent.iFolder; } } // Parent IFolder
        }
        File CurrentFile = new File();

        // Create the RootFolder in LocalStorage
        async void CreateRootFolder(bool replace = false)
        {
            CurrentFolder = RootFolder;

            // PCL Storage: Create the IFolder of RootFolder (RootFolder.iFolder)
            RootFolder.iFolder = await ICreateFolder(RootFolder, ILocalStorage, replace);
            LocalStorage.SubFolders.Add(RootFolder);

            IList<IFolder> iFolders = await IGetFolders(CurrentFolder.iFolder);
            foreach(IFolder iFolder in iFolders)
            {
                Folder folder = new Folder()
                {
                    Name = iFolder.Name,
                    Parent = CurrentFolder,
                    iFolder = iFolder
                };
                CurrentFolder.SubFolders.Add(folder);
            }
            
            IList<IFile> iFiles = await IGetFiles(CurrentFolder.iFolder);
            foreach(IFile iFile in iFiles)
            {
                File file = new File()
                {
                    Name = iFile.Name,
                    Parent = CurrentFolder,
                    iFile = iFile
                };
                CurrentFolder.Files.Add(file);
            }

        }


        // Check existence of IFolder "name" in IFolder "iparent"
        async Task<bool> ICheckFolderExist(string name, IFolder iparent)
        {
            ExistenceCheckResult IsExist = await iparent.CheckExistsAsync(name);
            if (IsExist == ExistenceCheckResult.FolderExists) return true;
            return false;
        }

        

        // Create a new IFolder of the Folder "folder" in the IFolder "iparent"
        async Task<IFolder> ICreateFolder(Folder folder, IFolder iparent, bool replace = false)
        {            
            if(replace) return await iparent.CreateFolderAsync(folder.Name, CreationCollisionOption.ReplaceExisting);
            //else return await iparent.CreateFolderAsync(folder.Name, CreationCollisionOption.GenerateUniqueName);
            else return await iparent.CreateFolderAsync(folder.Name, CreationCollisionOption.OpenIfExists);
        }      



        // Delete the IFolder "ifolder" in the IFolder "iparent"
        async Task IDeleteFolder(IFolder ifolder, IFolder iparent)
        {
            // Check the existence of the IFolder corresponding to the Object "folder"
            bool IsExist = await ICheckFolderExist(ifolder.Name, iparent);
            if (!IsExist)
            {
                await DisplayAlert("", ifolder.Name.ToString() + " doesn't exist", "OK");
                return;
            }

            await ifolder.DeleteAsync();
        }

        // Delete all of Folders and Files in the Folder "folder"
        async void IDleteAllObjects(Folder folder)
        {
            // Get ALl IFolders and IFiles in the Folder "folder"
            IList<IFolder> ifolders = await folder.iFolder.GetFoldersAsync();
            IList<IFile> ifiles = await folder.iFolder.GetFilesAsync();

            // Delete All IFolders
            foreach (IFolder ifolder in ifolders)
            {
                await ifolder.DeleteAsync();
            }

            // Delete All IFiles
            foreach (IFile ifile in ifiles)
            {
                await ifile.DeleteAsync();
            }

            // Delete All Folders & Files
            folder.SubFolders.Clear();
            folder.Files.Clear();

            // Recreate the new RootFolder
            CreateRootFolder();
        }



        


        // Get All IFolders in the IFolder "iparent"
        async Task<IList<IFolder>> IGetFolders(IFolder iparent)
        {
            return await iparent.GetFoldersAsync();
        }


        

        // Check existence of IFile "name" in IFolder "iparent"
        async Task<bool> ICheckFileExist(string name, IFolder iparent)
        {
            ExistenceCheckResult IsExist = await iparent.CheckExistsAsync(name);            
            if (IsExist == ExistenceCheckResult.FileExists) return true;
            return false;
        }

        //Create a new IFile of the File "file" in the IFolder "iparent" 
        //  (If the IFile already exists, open it.)
        async Task<IFile> ICreateFile(File file, IFolder iparent)
        {
            return await iparent.CreateFileAsync(file.Name, CreationCollisionOption.ReplaceExisting);
        }


        // Delete IFile "ifile" in IFolder "iparent"
        async Task IDeleteFile(IFile ifile, IFolder iparent)
        {
            // Check the existence of "file"
            bool IsExist = await ICheckFileExist(ifile.Name, iparent);
            if (!IsExist)
            {
                await DisplayAlert("", ifile.Name.ToString() + " doesn't exist", "OK");
                return;
            }

            await ifile.DeleteAsync();     // Delete the IFile "file"
        }



        // Get a IFile in in the IFolder "iparent"
        async Task<IFile> IGetFile(string name, Folder parent)
        {
            
            bool exist = await ICheckFileExist(name, parent.iFolder);
            if (exist) return await parent.iFolder.GetFileAsync(name);
            else
            {
                File file = new File()
                {
                    Name = name,
                    Parent = parent,
                    iFile = await parent.iFolder.GetFileAsync(name)
                };
                return file.iFile;
            }
            
        }

        // Get All IFiles in in the IFolder "iparent"
        async Task<IList<IFile>> IGetFiles(IFolder iparent)
        {
            return await iparent.GetFilesAsync();
        }

        // Write "content" to IFile "ifile"
        async void IWriteFile(IFile ifile, string content)
        {
            await ifile.WriteAllTextAsync(content);
        }

        // Read all text of IFile "ifile"
        async Task<string> IReadFile(IFile ifile)
        {
            string content = await ifile.ReadAllTextAsync();
            return content;
        }

        /*
        // Open IFile corresponding to the Object "file", 
        //          and set its content in the Editor "editor"
        async void IOpenFile(Object file, Editor editor)
        {
            string content = await (file.iFile).ReadAllTextAsync();
            editor.Text = content;
        }
        */

        // Get All IFolders and IFiles in IFolder "parent"
        async void IGetAllObjects(IFolder parent)
        {


            await DisplayAlert("", "Get All IFolders and IFiles in parent", "OK");
        }

        // Copy, Cut, Paste IFolder
        async void ICopyFolder()
        {
            await DisplayAlert("", "Copy IFolder", "OK");
        }
        async void ICutFolder()
        {
            await DisplayAlert("", "Cut IFolder", "OK");
        }
        async void IPasteFolder()
        {
            await DisplayAlert("", "Paste IFolder", "OK");
        }

        // Copy, Cut, Paste IFile
        async void ICopyFile()
        {
            await DisplayAlert("", "Copy IFile", "OK");
        }
        async void ICutFile()
        {
            await DisplayAlert("", "Cut IFile", "OK");
        }
        async void IPasteFile()
        {
            await DisplayAlert("", "Paste IFile", "OK");
        }
#endif
    }
}



/*
        // Delete all of IFolders and IFiles in IFolder "iparent"
        async void IDleteAllObjects(IFolder iparent)
        {
            // Get ALl IFolders and IFiles in the IFolder "parent"
            IList<IFolder> ifolders = await iparent.GetFoldersAsync();
            IList<IFile> ifiles = await iparent.GetFilesAsync();

            // Delete All IFolders
            foreach (IFolder ifolder in ifolders)
            {
                await ifolder.DeleteAsync();
            }

            // Delete All IFiles
            foreach (IFile ifile in ifiles)
            {
                await ifile.DeleteAsync();
            }

            // Clear All Objects (Folders&Files)
            CurrentDisplayObjects.Clear();
            AllObjects.Clear();
            UpdateFinder(CurrentDisplayObjects);
        }
        */

/*
        // Define Object(Folder/File) class        
        class Object    // : INotifyPropertyChanged
        {
            public string Name { get; set; }    // Name of Object
            public string NameWithExtension     // If File, add "txt" extention to the name
            {
                get { return IsFolder ? Name : Name + ".txt"; }
            }

            public bool IsFolder { get; set; }  // Folder -> true, File -> false

            public Object Parent { get; set; }  // Parent Folder of this Object

            public int NoChildren { get; set; } // Number of Children in this Object            
            public string NoChildrenText        // Text of NoChildren
            {
                get { return IsFolder ? "(" + NoChildren.ToString() + ")" : ""; }
            }

            public string Path { get; set; }    // Directory Path

            public string Color                 // Text Color: Folder->Blue, File->Black
            {
                get { return IsFolder ? "Blue" : "Black"; }
            }


            // PCL Storage: IFolder/IFile of this Object
            public IFolder iFolder { get; set; }
            public IFile iFile { get; set; }

            
            // Notify the change of IsExpanded
            private bool isexpanded;
            public event PropertyChangedEventHandler PropertyChanged;
            public bool IsExpanded
            {
                get { return this.isexpanded; }
                set
                {
                    if (this.isexpanded != value)
                    {
                        this.isexpanded = value;
                        OnPropertyChanged("IsExpanded");
                        OnPropertyChanged("Icon");
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

            public string Icon          // Icon: Expanded Folder -> expanded.png
                                        //       Collapsed Folder -> collapsed.png
                                        //       File -> file-hyphen.png
            {
                get { return IsFolder ? (IsExpanded ? "expanded.png" : "collapsed.png") : "file-hyphen.png"; }
            }
            
            public int Depth { get; set; }  // Depth in Folder Structure            
            public int Indent               // Indent Display Position according to the Depth
            {
                get { return Depth * 15; }
            }
            
            public string OpenAction    // ContextAction Menu: Folder->Open, File->Edit
            {
                get { return IsFolder ? "Open" : "Edit"; }
            }
            
        }
        */
