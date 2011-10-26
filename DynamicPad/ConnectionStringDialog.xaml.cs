using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DynamicPad
{
    /// <summary>
    /// Interaction logic for ConnectionStringDialog.xaml
    /// </summary>
    public partial class ConnectionStringDialog : Window
    {
        public ConnectionStringDialog()
        {
            InitializeComponent();
        }

        public Tuple<string, string> GetConnectionString()
        {
            return new Tuple<string, string>("key", "value");
        }
    }
}
