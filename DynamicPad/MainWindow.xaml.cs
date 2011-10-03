using System;
using System.Windows;
using System.Windows.Input;
using IronRuby;
using Massive;
using Microsoft.Scripting.Hosting;

namespace DynamicPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            grid.KeyDown += WindowKeyDown;
            textEditor.ShowLineNumbers = true;
            LanguageSelector.Items.Add("IronRuby");
            LanguageSelector.SelectedIndex = 0;
            textEditor.Focus();
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
            output.Text = string.Empty;
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

                var scriptScope = engine.CreateScope();


                const string connectionString = @"Data Source=.\SQLExpress;Integrated Security=true; ;initial catalog=MassiveTest;";
                var tbl = new DynamicModel(connectionString);
                //tbl.Query("select top 1 * from Person")
                //tbl.methods.sort.join("\n").to_s+"\n\n"
                scriptScope.SetVariable("tbl", tbl);

                var execute = engine.Execute(textEditor.Text, scriptScope);
                output.Text = execute.ToString();
            }
            catch (Exception exception)
            {
                output.Text += "\n--- Exception -------------------------------------------\n";
                output.Text += exception.ToString();
                output.Text += "\n--- Exception end----------------------------------------\n";
            }
        }

        private void ClearOutput__(object sender, RoutedEventArgs e)
        {
            ClearOutput();
        }
    }
}