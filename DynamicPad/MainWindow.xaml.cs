using System;
using System.Windows;
using System.Windows.Input;
using IronJS.Hosting;
using IronJS.Native;
using Exception = System.Exception;

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
            var script = textEditor.Text;
            try
            {
                var result = _ctx.Execute(script);
                output.Text += result.ToString();
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
