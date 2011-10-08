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
        private BackgroundWorker _backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            ProgressIndicator.Visibility = Visibility.Hidden;
            grid.KeyDown += WindowKeyDown;
            textEditor.ShowLineNumbers = true;
            PopulateConnectionStringsCombo();
            LanguageSelector.Items.Add("IronRuby");
            LanguageSelector.SelectedIndex = 0;
            textEditor.Focus();

            SetupCodeCompletion();
            InitializeBackgroundWorker();
        }

        private void SetupCodeCompletion()
        {
            var textAreaCompletion = new TextAreaCompletion();
            textEditor.TextArea.TextEntering += textAreaCompletion.TextEditorTextAreaTextEntering;
            textEditor.TextArea.TextEntered += textAreaCompletion.TextEditorTextAreaTextEntered;
        }

        private void InitializeBackgroundWorker()
        {
            _backgroundWorker = new BackgroundWorker();
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

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                RunScript();

                e.Handled = true;
            }
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
            var dlg = new SaveFileDialog();
            AddFileDialogSettings(dlg);

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                TextWriter tw = new StreamWriter(filename);

                tw.Write(textEditor.Text);
                tw.Close();

                SetTitle(filename);
            }
        }

        private void OpenToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dlg = new OpenFileDialog();
            AddFileDialogSettings(dlg);

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                var streamReader = new StreamReader(filename);
                textEditor.Text = streamReader.ReadToEnd();
                streamReader.Close();
                SetTitle(filename);
            }
        }

        private static void AddFileDialogSettings(FileDialog dlg)
        {
            dlg.FileName = "Document";
            dlg.DefaultExt = ".dpad";
            dlg.Filter = "dynamicpad documents (.dpad)|*.dpad";
            dlg.InitialDirectory = GetDynamicPadDirectory();
        }

        private void SetTitle(string filename)
        {
            var fileInfo = new FileInfo(filename);
            Title = "Doing magic with" + fileInfo.Name;
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
            Title = "New Document";
        }

        private void PlayToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            RunScript();
        }
    }
}