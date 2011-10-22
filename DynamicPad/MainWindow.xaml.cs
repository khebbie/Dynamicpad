using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DynamicPad.Properties;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using System.Diagnostics;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

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
        Stopwatch stopwatch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            grid.KeyUp += WindowKeyUp;
            textEditor.ShowLineNumbers = true;
            PopulateConnectionStringsCombo();
            LanguageSelector.Items.Add("IronRuby");
            LanguageSelector.SelectedIndex = 0;
            textEditor.Focus();
            
            SetupCodeCompletion();
            textEditor.TextArea.TextEntered += new TextCompositionEventHandler(TextArea_TextEntered);
            InitializeBackgroundWorker();
            this.Closing += new CancelEventHandler(MainWindow_Closing);
            this.SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
            //Thanks to http://code.google.com/p/yuzhenpin-nemo/source/browse/trunk/lib/nemo/xshd/ruby.xml
            textEditor.SyntaxHighlighting = ResourceLoader.LoadHighlightingDefinition("ruby.xshd");
        }

        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            textEditor.Height = Height / 2;
            output.Height = (Height / 2 )- 120;
            HideProgressIndicator();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_textChangedSinceLastSave)
            {
                var showSaveYesNoDialog = ShowSaveYesNoDialog();
                if (showSaveYesNoDialog == MessageBoxResult.Yes)
                {
                    if (string.IsNullOrWhiteSpace(_currentFileName))
                        SaveFileCheckIfAlreadyLoaded();
                    else
                        Save(_currentFileName);
                }
                else if (showSaveYesNoDialog == MessageBoxResult.Cancel)
                {
                    //cancel exit
                    e.Cancel = true;
                }
            }
        }

        private bool _textChangedSinceLastSave = false;
        void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            _textChangedSinceLastSave = true;
            SetTitle(_currentFileName + " *");
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
                    HideProgressIndicator();
                    stopwatch.Stop();
                    var elapsed = stopwatch.ElapsedMilliseconds;
                    statusText.Text = "Finished";
                    timerStatus.Text = elapsed + " ms";
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
            else if(e.Key == Key.Escape)
            {
                textEditor.Focus();
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
            stopwatch.Reset();
            stopwatch.Start();
            ClearOutput();
            ShowProgressIndicator();
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
                ShowProgressIndicator();
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
                var messageBoxResult = ShowSaveYesNoDialog();
                if (messageBoxResult == MessageBoxResult.Yes)
                    SaveFileCheckIfAlreadyLoaded();
                else if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            OpenFileDialog();
        }

        private static MessageBoxResult ShowSaveYesNoDialog()
        {
            return MessageBox.Show("Text not saved yet do you want to save", "Unsaved changes",
                                   MessageBoxButton.YesNoCancel);
        }

        private void OpenFileDialog()
        {
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
            Title = "Doing magic with " + filename;
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
            HideProgressIndicator();
            statusText.Text = "Stopped script run";
        }

        private void ShowProgressIndicator()
        {
            ProgressIndicator.Visibility = Visibility.Visible;
            ProgressIndicator.Height = 130;
            ProgressIndicator.Width = 130;
        }

        private void HideProgressIndicator()
        {

            ProgressIndicator.Visibility = Visibility.Hidden;
            ProgressIndicator.Height = 0;
            ProgressIndicator.Width = 0;
        }

        
    }

    //http://stackoverflow.com/questions/5057210/how-do-i-create-an-avalonedit-syntax-file-xshd-and-embed-it-into-my-assembly
    public static class ResourceLoader
    {
        public static IHighlightingDefinition LoadHighlightingDefinition(string resourceName)
        {
            var type = typeof(ResourceLoader);
            var fullName = type.Namespace + "." + resourceName;
            using (var stream = type.Assembly.GetManifestResourceStream(fullName))
            using (var reader = new XmlTextReader(stream))
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
    }
}