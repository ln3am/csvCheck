using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using SpaltenCheck;

namespace csvCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public string hauptPfad = Path.Combine(@"C:\Users\leon.haufe\source\repos\csvCheck", "csvin");
        public string fileinputNameget = "";
        public bool autodelimiterkorrektur = false;
        public double writespeed = 0.5;
        public MainWindow()
        {
            InitializeComponent();
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

        
        public void stateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = (int)stateSlider.Value;
            switch (value)
            {
                case 0:
                    writespeed = 1;
                    break;
                case 1:
                    writespeed = 0.5;
                    break;
                case 2:
                    writespeed = 0.1;
                    break;
            }
        }
        
        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop));
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string file = files[0];
                Start(file);
            }
        }
        private void SaveCsvButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    Start(selectedFilePath);
                }
            }
            catch
            {
                MessageBox.Show("Input problem", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void SaveCsvButton2_Click(object sender, RoutedEventArgs e)
        {
            string fileinputname = @"\csvfalse\falselines.csv";
            Save(fileinputname);
        }

        private void SaveCsvButton1_Click(object sender, RoutedEventArgs e)
        {
            string fileinputname = @"\csvout\out.csv";
            Save(fileinputname);
        }
        
        private void PackageManager_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.DataContext = window1;
            window1.Show();
        }
        private void falsedel(object sender, RoutedEventArgs e)
        {
            autodelimiterkorrektur = autodelimiterkorrektur ? false : true;
            enablebutton.Content = autodelimiterkorrektur ? "Enable false delimiters" : "Disable false delimiters";
        }
        private void Save(string fileinputname)
        {
            string filePath = hauptPfad + fileinputname;
            Microsoft.Win32.SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = fileinputNameget,
                InitialDirectory = Path.GetDirectoryName(filePath)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string saveFilePath = saveFileDialog.FileName;

                try
                {
                    File.Copy(filePath, saveFilePath, true);
                    MessageBox.Show("CSV file saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving the file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Start(string file)
        {
            string fileName = @"\csvinput\csvinput.csv";
            fileinputNameget = Path.GetFileName(file);
            string targetFilePath = hauptPfad + fileName;
            if (File.Exists(targetFilePath))
            {
                File.Delete(targetFilePath);
            }
            File.Copy(file, targetFilePath);
            Console.SetOut(new ControlWriter(outputTextBox, writespeed));

            Logic log = new Logic();
            log.Cons(outputFalseText, hauptPfad, autodelimiterkorrektur, writespeed);
        }
    }
    public class ControlWriter : TextWriter
    {
            private TextBox textBox;
            private int currentIndex;
            private string textToDisplay;
            private DispatcherTimer timer;

        public ControlWriter(TextBox textBox, double writespeed)
        {
            this.textBox = textBox;
            textBox.Text = "";
            currentIndex = 0;
            textToDisplay = "";
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(writespeed); 
            timer.Tick += Timer_Tick;
        }
        public override void Write(char value)
        {
            textToDisplay += value;
            StartTimer();
        }
        public override void Write(string value)
        {
            textToDisplay += value;
            StartTimer();
        }
        public override Encoding Encoding => Encoding.ASCII;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentIndex < textToDisplay.Length)
            {
                textBox.AppendText(textToDisplay[currentIndex].ToString());
                currentIndex++;
            }
            else
            {
                StopTimer();
            }
        }

        private void StartTimer()
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }

        private void StopTimer()
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
        }
    }
    public class Logic
    {
        public void Cons(TextBox outputFalseText, string hauptPfad, bool autodelimiterkorrektur, double writespeed)
        {
            string csvFilePath = Path.Combine(hauptPfad, @"csvinput\csvinput.csv");
            string csvOut = Path.Combine(hauptPfad, @"csvout\out.csv");
            string csvfalselines = Path.Combine(hauptPfad, @"csvfalse\falselines.csv");
            string txtPackages = Path.Combine(hauptPfad, "txtpackages");
            Checker check = new Checker();
            check.SpaltenDelimiterÜberprüfung(csvFilePath, csvOut, txtPackages, csvfalselines, outputFalseText, autodelimiterkorrektur, writespeed);
        }
    }
}
