using DatRepacker.Models;
using DatRepacker.Services;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DatRepacker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppState _appState;

        public MainWindow()
        {
            InitializeComponent();
            _appState = AppState.Instance;
            this.DataContext = this;
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Nullable<bool> result;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(System.AppContext.BaseDirectory);
            dlg.Filter = "Dat file (*.dat)|*.dat|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            result = dlg.ShowDialog();
            // User didn't select a file so don't do anything
            if (result == false)
                return;

            // Load the file the user selected  
            Console.WriteLine(dlg.FileName);
            AddFile(dlg.FileName);
            return;
        }

        private void OpenFolderCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Nullable<bool> result;
            Microsoft.Win32.OpenFolderDialog dlg = new Microsoft.Win32.OpenFolderDialog();
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(System.AppContext.BaseDirectory);

            result = dlg.ShowDialog();
            // User didn't select a file so don't do anything
            if (result == false)
                return;

            // Load the file the user selected  
            Console.WriteLine(dlg.FolderName);
            AddFile(dlg.FolderName);
            return;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            return;
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bayonetta Cos Tool made by Lyzder.\nRepository: https://github.com/Lyzder/BayoCosTool\nFor modding questions, join the Infernal Warks discord server.", "About");
        }

        private void AddFile(string filename)
        {
            BayoDat? dat;
            string? error = null;
            try
            {
                FileAttributes attr = File.GetAttributes(filename);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    FolderContainer folder = new FolderContainer(filename, new DirectoryInfo(filename).Name, (uint)_appState.modContainers.Count + 1);
                    _appState.modContainers.Add(folder);
                }
                else
                {
                    dat = LoadDat(filename);
                    if (dat != null)
                    {
                        DatContainer container = new DatContainer(dat, System.IO.Path.GetFileName(filename), (uint)_appState.modContainers.Count + 1);
                        _appState.modContainers.Add(container);
                    }
                }
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Error: The file path cannot be null.");
                error = "Unable to open file.\nThe file path cannot be null.";
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: You do not have permission to read this file.");
                error = "Unable to open file.\nYou do not have permission to read this file.";
            }
            // Shows error message if error string was assigned
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Open file error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private BayoDat? LoadDat(string filename)
        {
            BayoDat? dat = null;
            DatHeader header;
            FileStream fileStream;
            EndianBinaryReader reader;
            string? error = null;

            try 
            {
                using (fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    // Checks if the file has the size to at least contain a header
                    if (fileStream.Length < (UInt16)Globals.HEADER_SIZE)
                    {
                        Console.WriteLine("The file is too short.");
                        throw new Exception("The file is too short.");
                    }
                    using (reader = new EndianBinaryReader(fileStream))
                    {
                        fileStream.Seek(8, SeekOrigin.Begin);
                        reader.IsBigEndian = false;
                        // Checking if little endian. The first offset is always 32, but if the value read is higher it means it's big endian
                        if (32 < reader.ReadUInt32())
                        {
                            reader.IsBigEndian = true;
                        }
                        fileStream.Seek(0, SeekOrigin.Begin);
                        header = LoadDatHeader(reader);
                        dat = LoadDatContents(reader, header);
                    }
                    fileStream.Close();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: The file was not found.");
                error = "Unable to open file.\nThe file was not found.";
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error: The specified directory was not found.");
                error = "Unable to open file.\nThe specified directory was not found.";
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: You do not have permission to read this file.");
                error = "Unable to open file.\nYou do not have permission to read this file.";
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("Error: The specified file path is too long.");
                error = "Unable to open file.\nThe specified file path is too long.";
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An I/O error occurred: {ex.Message}");
                error = $"Unable to open file.\nAn I/O error occurred: {ex.Message}";
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("Error: The file path format is invalid.");
                error = "Unable to open file.\nThe file path format is invalid.";
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Error: The file path cannot be null.");
                error = "Unable to open file.\nThe file path cannot be null.";
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Error: The file path contains invalid characters.");
                error = "Unable to open file.\nThe file path contains invalid characters.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                error = $"Unable to open file.\nAn unexpected error occurred: {ex.Message}";
            }
            // Shows error message if error string was assigned
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Open file error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return dat;
        }

        private DatHeader LoadDatHeader(EndianBinaryReader reader)
        {
            DatHeader header;
            char[] id = new char[4];
            uint fileNumber;
            uint offsetsOffset;
            uint extensionOffset;
            uint namesOffset;
            uint sizesOffset;
            uint hashMap;

            for (int i = 0; i < 4; i++)
            {
                id[i] = reader.ReadChar();
            }
            fileNumber = reader.ReadUInt32();
            offsetsOffset = reader.ReadUInt32();
            extensionOffset = reader.ReadUInt32();
            namesOffset = reader.ReadUInt32();
            sizesOffset = reader.ReadUInt32();
            hashMap = reader.ReadUInt32();

            header = new DatHeader(id, fileNumber, offsetsOffset, extensionOffset, namesOffset, sizesOffset, hashMap);

            return header;
        }

        private BayoDat LoadDatContents(EndianBinaryReader reader, DatHeader header)
        {
            BayoDat dat;
            List<uint> fileOffsets = [];
            List<char[]> fileExtensions = [];
            uint nameLength;
            List<List<char>> fileNames = [];
            List<uint> fileSizes = [];
            List<byte[]> files = [];
            int i, j;
            char[] chars;
            byte[] bytes;
            List<char> char_list;

            // Read file offsets
            reader.BaseStream.Position = header.fileOffsetsOffset;
            for (i = 0; i < header.fileNumber; i++)
            {
                fileOffsets.Add(reader.ReadUInt32());
            }
            // Read file extensions
            reader.BaseStream.Position = header.fileExtensionOffset;
            for (i = 0; i < header.fileNumber; i++)
            {
                chars = new char[4];
                for (j = 0; j < 4; j++)
                {
                    chars[j] = Convert.ToChar(reader.ReadByte());
                }
                fileExtensions.Add(chars);
            }
            // Read name length
            nameLength = reader.ReadUInt32();
            // Read file names
            for (i = 0; i < header.fileNumber; i++)
            {
                char_list = new List<char>();
                char_list.Capacity = (int)nameLength;
                for (j = 0; j < nameLength; ++j)
                {
                    char_list.Add(Convert.ToChar(reader.ReadByte()));
                }
                fileNames.Add(char_list);
            }
            // Read file sizes
            reader.BaseStream.Position = header.fileSizesOffset;
            for (i = 0; i < header.fileNumber; i++)
            {
                fileSizes.Add(reader.ReadUInt32());
            }
            // Read file data
            for (i = 0; i < header.fileNumber; i++)
            {
                bytes = new byte[fileSizes[i]];
                reader.BaseStream.Position = fileOffsets[i];
                for(j = 0; j < fileSizes[i]; j++)
                {
                    bytes[j] = reader.ReadByte();
                }
                files.Add(bytes);
            }
            // Build Dat
            dat = new BayoDat(header, nameLength, reader.IsBigEndian);
            dat.LoadContents(fileOffsets, fileExtensions, fileNames, fileSizes, files, nameLength);

            return dat;
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand OpenFolder = new RoutedUICommand(
            "Open Folder", "OpenFolder", typeof(CustomCommands));
    }
}