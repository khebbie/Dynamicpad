﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using IronRuby;
using Massive;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Utils;

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

                AddTbl(scriptScope);


                //tbl.Query("select top 1 * from Person")
                //tbl.methods.sort.join("\n").to_s+"\n\n"

                // result = tbl.All()
                // outp = ""
                //result.each do |bovs| 
                //    outp = outp + bovs.firstname
                //    outp = outp + bovs.lastname
                //    outp = outp + bovs.birthdate.ToString()
                //    outp = outp + "\n"
                //end

                //outp

                scriptScope.SetVariable("log", new Log());
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

        private static void AddTbl(ScriptScope scriptScope)
        {
            const string connectionString = @"Data Source=.\SQLExpress;Integrated Security=true; ;initial catalog=MassiveTest;";
            var tbl = new DynamicModel(connectionString, "Person");
            scriptScope.SetVariable("tbl", tbl);
        }

        private void ClearOutput__(object sender, RoutedEventArgs e)
        {
            ClearOutput();
        }

        private void SaveToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
                          {
                              FileName = "Document", 
                              DefaultExt = ".dpad", 
                              Filter = "dynamicpad documents (.dpad)|*.dpad"
                          };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                TextWriter tw = new StreamWriter(filename);

                tw.Write(textEditor.Text);

                tw.Close();
            }
        }

        private void OpenToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dlg = new Microsoft.Win32.OpenFileDialog
                          {
                              FileName = "Document", 
                              DefaultExt = ".dpad", 
                              Filter = "dynamicpad documents (.dpad)|*.dpad"
                          };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                var streamReader = new StreamReader(filename);
                textEditor.Text = streamReader.ReadToEnd();
                streamReader.Close();
            }
        }
    }
}