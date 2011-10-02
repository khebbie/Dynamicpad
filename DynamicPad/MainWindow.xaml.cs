using System;
using System.Windows;
using System.Windows.Input;
using IronJS.Hosting;
using IronJS.Native;
using IronRuby;
using Microsoft.Scripting.Hosting;

namespace DynamicPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CSharp.Context _ctx;

        public MainWindow()
        {
            InitializeComponent();
            grid.KeyDown += WindowKeyDown;
            textEditor.ShowLineNumbers = true;
            LanguageSelector.Items.Add("IronJS");
            LanguageSelector.Items.Add("IronRuby");
            LanguageSelector.SelectedIndex = 0;
            InitializeIronJs();
            textEditor.Focus();
        }

        private void InitializeIronJs()
        {
            _ctx = new CSharp.Context();
            Action<string> alert = message => MessageBox.Show(message);
            _ctx.SetGlobal("alert", Utils.CreateFunction(_ctx.Environment, 1, alert));

            Action<string> log = message => output.Text += message + "\n";
            _ctx.SetGlobal("log", Utils.CreateFunction(_ctx.Environment, 1, log));
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
            if (LanguageSelector.SelectedIndex == 0)
                RunJS();
            else
                RunRubyScript();
        }

        private void RunJS()
        {
            string script = textEditor.Text;
            try
            {
                object result = _ctx.Execute(script);
                output.Text += result.ToString();
            }
            catch (Exception exception)
            {
                output.Text += "\n--- Exception -------------------------------------------\n";
                output.Text += exception.ToString();
                output.Text += "\n--- Exception end----------------------------------------\n";
            }
        }

        private void RunRubyScript()
        {
            try
            {
                //http://stackoverflow.com/questions/5341111/how-do-you-add-a-string-to-a-c-iliststring-from-ironruby
                ScriptRuntime runtime = Ruby.CreateRuntime();
                ScriptEngine engine = runtime.GetEngine("IronRuby");
                var execute = engine.Execute(textEditor.Text);
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