using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace csvCheck
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        MainWindow main = new MainWindow();
        public ObservableCollection<CollectiveItem> FileNames { get; } = new ObservableCollection<CollectiveItem>();
        public ObservableCollection<CollevtiveItem2> FileNamesTemplates { get; } = new ObservableCollection<CollevtiveItem2>();
        private bool editdone = false;
        private bool toggle = false;
        private bool rename = false;
        private string renamename = "";
        private string editdonepath = "";
        private string torename = "nothere";
        private string renamekey = "renamekey will change";
        private TextBox edit = new TextBox();
        private Brush rectangleColor;
        private Visibility removevis = Visibility.Visible;
        private Visibility renamevis = Visibility.Visible;
        private Visibility textvis = Visibility.Visible;
        private Visibility renameboxvis = Visibility.Collapsed;
        private Visibility activatevis = Visibility.Collapsed;
        private Visibility deactivatevis = Visibility.Collapsed;
        public Window1()
        {
            InitializeComponent();
            LoadFiles();
            LoadMethodTemplates();
            fileListBox.ItemsSource = FileNames;
            templateList.ItemsSource = FileNamesTemplates;
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (ResizeMode != ResizeMode.CanResize &&
                    ResizeMode != ResizeMode.CanResizeWithGrip)
                {
                    return;
                }

                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
            else
            {
                mRestoreForDragMove = WindowState == WindowState.Maximized;
                DragMove();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (mRestoreForDragMove)
            {
                mRestoreForDragMove = false;

                var point = PointToScreen(e.MouseDevice.GetPosition(this));

                Left = point.X - (RestoreBounds.Width * 0.5);
                Top = point.Y;

                WindowState = WindowState.Normal;

                DragMove();
            }
        }
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mRestoreForDragMove = false;
        }
        private bool mRestoreForDragMove;
        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Editingwindowto()
        {
            edit.Text = editedtxtbox.Text;
        }
        private void Editingwindowback()
        {
            editedtxtbox.Text = edit.Text;
        }
        private void Addordonebutton()
        {
            addbutton.Content = editdone ? "  Done  " : "  Add  ";
        }
        private void Clear()
        {
            edit.Text = "";
            Editingwindowback();
        }
        private void RefreshFiles()
        {
            FileNames.Clear();
            LoadFiles();
            fileListBox.ItemsSource = FileNames;
        }
        private void Towindow(string filePath)
        {
                editdonepath = filePath;
                string[] lines = File.ReadAllLines(filePath);
                Clear();
                foreach (var line in lines)
                {
                    edit.Text += line + Environment.NewLine;
                }
                Editingwindowback();
                editdone = true;
                Addordonebutton();
        }
        
        private string[] FileSort(string[] txtfilenames)
        {
            Array.Sort(txtfilenames, new SecondLineNumberComparer());
            return txtfilenames;
        }
        private void RenameFile()
        {

                string directoryPath = Path.GetDirectoryName(torename);
                string newFilePath = Path.Combine(directoryPath, renamename + ".txt");
                if (!File.Exists(newFilePath))
                { try
                    {
                    File.Move(torename, newFilePath);
                    }
                    catch
                    {
                    MessageBox.Show("Parametererror choose a different name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("A file with the new name already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                renamekey = "renamekey will change";
                torename = "nothere";
                RefreshFiles();
        }
        private void LoadFiles()
        {
            string folderPath = main.hauptPfad + @"\txtpackages";

            if (Directory.Exists(folderPath))
            {
                string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");

                foreach (string txtFile in txtFiles)
                {
                    try
                    {
                        string fileName = Path.GetFileNameWithoutExtension(txtFile);
                        string[] lines = File.ReadAllLines(txtFile);
                        renameboxvis = Visibility.Collapsed;
                        textvis = Visibility.Visible;
                        if (fileName + ".txt" == renamekey)
                        {
                            if (rename)
                            {
                                renameboxvis = Visibility.Visible;
                                textvis = Visibility.Collapsed;
                            }
                            else
                            {
                                torename = txtFile;
                            }
                        }
                        if (bool.Parse(lines[2]))
                        {
                                rectangleColor = new SolidColorBrush(Colors.Green);

                                if (!toggle)
                                {
                                activatevis = Visibility.Collapsed;
                                deactivatevis = Visibility.Visible;
                                }
                        }
                        else
                        {
                                rectangleColor = new SolidColorBrush(Colors.Red);
                                if (!toggle)
                                {
                                deactivatevis = Visibility.Collapsed;
                                activatevis = Visibility.Visible;
                                }
                        }
                        FileNames.Add(new CollectiveItem { Text = fileName, RectangleColor = rectangleColor, Activatevis = activatevis, Deactivatevis = deactivatevis, Renamevis = renamevis, Removevis = removevis, Textvis = textvis, Renameboxvis = renameboxvis, RenameText = Path.GetFileNameWithoutExtension(txtFile) });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Line 3 of {Path.GetFileName(txtFile)} has no valid true or false {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void LoadMethodTemplates()
        {
            string folderPath = main.hauptPfad + @"\txtpackagetemplates";
            if (Directory.Exists(folderPath))
            {
                string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");
                txtFiles = FileSort(txtFiles);
                foreach (string txtFile in txtFiles)
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(txtFile);
                        string name = Path.GetFileNameWithoutExtension(txtFile);
                        FileNamesTemplates.Add(new CollevtiveItem2 { TextPackage = name, MethodNumber = lines[2] });
                    }
                    catch
                    {
                        MessageBox.Show($"Line 2 of {Path.GetFileName(txtFile)} has no valid true or false", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (editdone)
            {
                Editingwindowto();
                string topackage = edit.Text;
                File.WriteAllText(editdonepath, topackage);
                Clear();
                editdone = false;
                Addordonebutton();
                RefreshFiles();
                return;
            }
            try
            {
                string folderPath = main.hauptPfad + @"\txtpackages";
                Editingwindowto();
                string text = edit.Text;
                string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                if (lines.Length > 5)
                {
                    string header = lines[0];
                    string[] lin = new string[lines.Length - 1];
                    Array.Copy(lines, 1, lin, 0, lin.Length);
                    string filepath = Path.Combine(folderPath, header + ".txt");
                    string writeto = string.Join(Environment.NewLine, lin);
                    File.WriteAllText(filepath, writeto);
                    Clear();
                    RefreshFiles();
                }
                else
                {
                    MessageBox.Show("Each Method contains at least 4 Lines and a Title", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button removeButton = (Button)sender;
                CollectiveItem item = (CollectiveItem)removeButton.DataContext;
                string fileName = item.Text + ".txt";
                FileNames.Remove(item);
                string filePath = Path.Combine(main.hauptPfad + @"\txtpackages", fileName);
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }
        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button removeButton = (Button)sender;
                CollectiveItem item = (CollectiveItem)removeButton.DataContext;
                string fileName = item.Text + ".txt";
                string filePath = Path.Combine(main.hauptPfad + @"\txtpackages", fileName);
                string[] lines = File.ReadAllLines(filePath);
                lines[2] = "true";
                File.WriteAllLines(filePath, lines);
                RefreshFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Deactivate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button removeButton = (Button)sender;
                CollectiveItem item = (CollectiveItem)removeButton.DataContext;
                string fileName = item.Text + ".txt";
                string filePath = Path.Combine(main.hauptPfad + @"\txtpackages", fileName);
                string[] lines = File.ReadAllLines(filePath);
                lines[2] = "false";
                File.WriteAllLines(filePath, lines);
                RefreshFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ListEdit(object sender, RoutedEventArgs e)
        {
            ListBox clickedText = (ListBox)sender;
            CollectiveItem item = (CollectiveItem)clickedText.SelectedItem;
            if (item != null)
            {
                string name = item.Text + ".txt";
                string filePath = Path.Combine(main.hauptPfad + @"\txtpackages", name);
                try
                {
                    Towindow(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            rename = rename ? false : true;
            Button removeButton = (Button)sender;
            CollectiveItem item = (CollectiveItem)removeButton.DataContext;
            renamekey = item.Text + ".txt";
            renamename = item.RenameText;
            RefreshFiles();
            if (torename != "nothere")
            {
                RenameFile();
            }
        }
        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            toggle = toggle ? false : true;
            if (toggle)
            {
                activatevis = Visibility.Collapsed;
                deactivatevis = Visibility.Collapsed;
                removevis = Visibility.Collapsed;
                renamevis = Visibility.Collapsed;
            }
            else
            {
                removevis = Visibility.Visible;
                renamevis = Visibility.Visible;
            }
            RefreshFiles();
        }
        private void TemplateList_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = main.hauptPfad + @"\txtpackagetemplates";
            ListBox clickedText = (ListBox)sender;
            CollevtiveItem2 item = (CollevtiveItem2)clickedText.SelectedItem;
            if (item != null)
            {
                Clear();
                string name = item.TextPackage;
                string realpath = Path.Combine(folderPath, name + ".txt");
                string[] lines = File.ReadAllLines(realpath);
                foreach (var line in lines)
                {
                    edit.Text += line + Environment.NewLine;
                }
                editdone = false;
                Addordonebutton();
                Editingwindowback();
            }
        }
    }
    public class CollectiveItem
    {
        public string Text { get; set; }
        public Brush RectangleColor { get; set; }
        public Visibility Deactivatevis { get; set; }
        public Visibility Activatevis { get; set; }
        public Visibility Removevis { get; set; }
        public Visibility Renamevis { get; set; }
        public Visibility Textvis { get; set; }
        public Visibility Renameboxvis { get; set; }
        public string RenameText { get; set; }
    }
    public class CollevtiveItem2
    {
        public string TextPackage { get; set; }
        public string MethodNumber { get; set; }
    }
    public class Item3
    {
        public string addordone { get; set; }
    }
    class SecondLineNumberComparer : IComparer<string>
    {
        public int Compare(string file1, string file2)
        {
            int number1 = GetSecondLineNumber(file1);
            int number2 = GetSecondLineNumber(file2);
            return number1.CompareTo(number2);
        }
        private int GetSecondLineNumber(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            if (int.TryParse(lines[2], out int number))
            {
                return number;
            }
            else
            {
                MessageBox.Show("Missing template Number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }
    }
}
