using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DynamicPad.Properties;
using Microsoft.Win32;

namespace DynamicPad
{
    //Getting the icons from http://www.iconfinder.com
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _currentFileName;
        private BackgroundWorker _backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            ProgressIndicator.Visibility = Visibility.Hidden;
            grid.KeyUp += WindowKeyUp;
            textEditor.ShowLineNumbers = true;
            PopulateConnectionStringsCombo();
            LanguageSelector.Items.Add("IronRuby");
            LanguageSelector.SelectedIndex = 0;
            textEditor.Focus();

            SetupCodeCompletion();
            textEditor.TextArea.TextEntered += new TextCompositionEventHandler(TextArea_TextEntered);
            InitializeBackgroundWorker();
        }

        private bool _textChangedSinceLastSave = false;
        void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            _textChangedSinceLastSave = true;
        }

        private void SetupCodeCompletion()
        {
            var textAreaCompletion = new TextAreaCompletion();
            textEditor.TextArea.TextEntering += textAreaCompletion.TextEditorTextAreaTextEntering;
            textEditor.TextArea.TextEntered += textAreaCompletion.TextEditorTextAreaTextEntered;
        }

        private void InitializeBackgroundWorker()
        {
            _backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
            _backgroundWorker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                var scriptArguments = args.Argument as ScriptArguments;
                new RubyScriptRunner().RunRubyScript(scriptArguments);
            };

            _backgroundWorker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    ProgressIndicator.Visibility = Visibility.Hidden;
                    statusText.Text = "Finished";
                }));
            };
        }

        private void PopulateConnectionStringsCombo()
        {
            PropertyInfo[] props = Settings.Default.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (!prop.Name.Contains("ConnectionString"))
                    continue;
                SettingsProperty sett = Settings.Default.Properties[prop.Name];
                if (sett != null)
                {
                    ConnectionStringSelector.Items.Add(sett.Name);
                }
            }
            ConnectionStringSelector.SelectedIndex = 0;
        }

        private void WindowKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                RunScript();

                e.Handled = true;
            }
            else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SaveFileCheckIfAlreadyLoaded();
            }
            else if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                OpenFileCheckIfFileSaved();
            }
        }

        private void SaveFileCheckIfAlreadyLoaded()
        {
            if (String.IsNullOrWhiteSpace(_currentFileName))
                SaveDialog();
            else
                Save(_currentFileName);
            _textChangedSinceLastSave = false;
        }

        private void ClearOutput()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                output.Text = String.Empty;
            }));
        }

        private void RunScript()
        {
            ClearOutput();
            ProgressIndicator.Visibility = Visibility.Visible;
            var connectionStringName = ConnectionStringSelector.SelectedItem.ToString();
            var connectionString = Settings.Default.Properties[connectionStringName].DefaultValue.ToString();
            var scriptArguments = new ScriptArguments
                                      {
                                          ConnectionString = connectionString,
                                          Script = textEditor.Text,
                                          Logger = new Log(SetOutput, ClearOutput)
                                      };
            
            if (_backgroundWorker.IsBusy)
                statusText.Text = "Busy";
            else
                _backgroundWorker.RunWorkerAsync(scriptArguments);
        }

        void SetOutput(string s)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                output.Text += s;
                ProgressIndicator.Visibility = Visibility.Visible;
            }));
        }

        private void ClearOutput__(object sender, RoutedEventArgs e)
        {
            ClearOutput();
        }

        private void SaveToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileCheckIfAlreadyLoaded();
        }

        private void SaveDialog()
        {
            var dlg = new SaveFileDialog();
            AddFileDialogSettings(dlg);

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                Save(dlg.FileName);
            }
            _currentFileName = dlg.FileName;
        }

        private void Save(string fileName)
        {
            TextWriter tw = new StreamWriter(fileName);

            tw.Write(textEditor.Text);
            tw.Close();

            SetTitle(fileName);
            return;
        }

        private void OpenToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileCheckIfFileSaved();
        }

        private void OpenFileCheckIfFileSaved()
        {
            if (_textChangedSinceLastSave)
            {
                var messageBoxResult = MessageBox.Show("Text not saved yet do you want to save", "Unsaved changes",
                                                       MessageBoxButton.YesNoCancel);
                if (messageBoxResult == MessageBoxResult.Yes)
                    SaveFileCheckIfAlreadyLoaded();
                else if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            OpenFileDialog();
        }

        private void OpenFileDialog()
        {
// Configure open file dialog box
            var dlg = new OpenFileDialog();
            AddFileDialogSettings(dlg);

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                Open(dlg.FileName);
            }
        }

        private void Open(string fileName)
        {
            var streamReader = new StreamReader(fileName);
            textEditor.Text = streamReader.ReadToEnd();
            streamReader.Close();
            SetTitle(fileName);
            _currentFileName = fileName;
        }

        private static void AddFileDialogSettings(FileDialog dlg)
        {
            dlg.FileName = "Document";
            dlg.DefaultExt = ".dpad";
            dlg.Filter = "dynamicpad scripts (.dpad)|*.dpad";
            dlg.InitialDirectory = GetDynamicPadDirectory();
        }

        private void SetTitle(string filename)
        {
            var fileInfo = new FileInfo(filename);
            Title = "Doing magic with " + fileInfo.Name;
        }

        public static string GetDynamicPadDirectory()
        {
            string dir = Path.Combine(GetMyDocumentsDir(), "DynamicPad Queries");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        public static string GetMyDocumentsDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        private void NewToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Text = string.Empty;
            _currentFileName = string.Empty;

            Title = "New Document";
        }

        private void PlayToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            RunScript();
        }

        private void StopToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            _backgroundWorker.CancelAsync();
            ProgressIndicator.Visibility = Visibility.Hidden;
            statusText.Text = "Stopped script run";
        }
    }
}