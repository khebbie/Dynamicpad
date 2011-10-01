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
        }

        private void InitializeIronJs()
        {
            _ctx = new CSharp.Context();
            Action<string> alert = message => MessageBox.Show(message);
            _ctx.SetGlobal("alert", Utils.CreateFunction(_ctx.Environment, 1, alert));
        }


        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                var script = textEditor.Text;
                try
                {
                    var result = _ctx.Execute(script);
                    output.Text = result.ToString();
                }
                catch (Exception exception)
                {

                    output.Text = exception.ToString();
                }
                
                e.Handled = true;
            }
        }
    }
}
