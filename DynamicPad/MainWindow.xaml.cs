using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DynamicPad.Properties;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using IronRuby;
using Massive;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;

namespace DynamicPad
{
    //Getting the icons from http://www.iconfinder.com
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string OverrideRubyObject =
            @"
class Object
	def puts(str)
		log.Print(str)
	end

    def p(str)
		log.Print(str)
	end

    def clear()
        log.Clear
    end
    def Dump(obj)
        log.Dump(obj)
    end

    def DumpE(obj)
        log.DumpEnumerable(obj)
    end
end";

        private CompletionWindow _completionWindow;
        private ScriptEngine _scriptEngine;

        public MainWindow()
        {
            InitializeComponent();
            grid.KeyDown += WindowKeyDown;
            textEditor.ShowLineNumbers = true;
            PopulateConnectionStringsCombo();
            LanguageSelector.Items.Add("IronRuby");
            LanguageSelector.SelectedIndex = 0;
            textEditor.Focus();
            textEditor.TextArea.TextEntering += TextEditorTextAreaTextEntering;
            textEditor.TextArea.TextEntered += TextEditorTextAreaTextEntered;
        }

        private void TextEditorTextAreaTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                // Open code completion after the user has pressed dot:
                _completionWindow = new CompletionWindow(textEditor.TextArea);
                IList<ICompletionData> data = _completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("Query"));
                data.Add(new MyCompletionData("All()"));
                data.Add(new MyCompletionData("ToString()"));
                _completionWindow.Show();
                _completionWindow.Closed += delegate { _completionWindow = null; };
            }
        }

        private void TextEditorTextAreaTextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
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
            output.Text = String.Empty;
        }

        private void RunScript()
        {
            RunRubyScript();
        }

        private void RunRubyScript()
        {
            try
            {
                ScriptRuntime runtime = Ruby.CreateRuntime();
                ScriptEngine engine = runtime.GetEngine("IronRuby");

                _scriptEngine = engine;
                ScriptScope scriptScope = _scriptEngine.CreateScope();

                AddTbl(scriptScope);

                scriptScope.SetVariable("log", new Log(s => output.Text += s, () => output.Text = string.Empty));

                _scriptEngine.Execute(OverrideRubyObject, scriptScope);
                _scriptEngine.Execute(textEditor.Text, scriptScope);
            }
            catch (Exception exception)
            {
                output.Text += "\n--- Exception -------------------------------------------\n";
                output.Text += exception.ToString();
                output.Text += "\n--- Exception end----------------------------------------\n";
            }
        }

        private void AddTbl(ScriptScope scriptScope)
        {
            string selectedPropertyInfo = ConnectionStringSelector.SelectedItem.ToString();
            string selectedConnectionString = Settings.Default.Properties[selectedPropertyInfo].DefaultValue.ToString();
            var tbl = new DynamicModel(selectedConnectionString);
            scriptScope.SetVariable("tbl", tbl);
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

    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text)
        {
            Text = text;
        }

        #region ICompletionData Members

        public ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return Text; }
        }

        public object Description
        {
            get { return "Description for " + Text; }
        }

        public double Priority
        {
            get { return 1; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
                             EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }

        #endregion
    }
}