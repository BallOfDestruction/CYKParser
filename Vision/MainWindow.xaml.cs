using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using Parser;
using System.IO;
using Language;

namespace Vision
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChomskyNormalForm language = new ChomskyNormalForm();

        public MainWindow()
        {
            InitializeComponent();
            LoadButton.Click += ((sender, e) => Load(FilePathBox.Text));
            OpenButton.Click += ((sender, e) => OpenFile());
            RestoreButton.Click += ((sender, e) => RestoreLanguage());
            CheckButton.Click += ((sender, e) => Check());
        }

        private void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePathBox.Text = dialog.FileName;
            }
        }

        private void Load(string filepath)
        {
            Factory factory = new Factory(filepath);
            try
            {
                language = factory.GetLanguage();

                string rightABS = "";
                foreach (string a in language.TerminalChars)
                {
                    rightABS += a + ", ";
                }
                RigthABC.Text = new string(rightABS.Take(rightABS.Length - 2).ToArray()); ;
                string leftABC = "";
                foreach (string a in language.NonTerminalChars)
                {
                    leftABC += a + ", ";
                }
                LeftABC.Text = new string(leftABC.Take(leftABC.Length - 2).ToArray());
                StartSimbol.Text = language.Start;
                LeftRule.Document.Blocks.Clear();
                foreach (FirstRule rule in language.NonEndRules)
                {
                    LeftRule.Document.Blocks.Add(new Paragraph(new Run(rule.Left + " -> " + rule.RightOne + ", " + rule.RightTwo)));
                }
                RightRule.Document.Blocks.Clear();
                foreach (SecondRule rule in language.EndRules)
                {
                    RightRule.Document.Blocks.Add(new Paragraph(new Run(rule.Left + "->" + rule.Right)));
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private void RestoreLanguage()
        {
            string filename = Environment.CurrentDirectory + "/1.txt";
            StreamWriter writer = new StreamWriter(filename, false);
            writer.WriteLine(RigthABC.Text);
            writer.WriteLine(LeftABC.Text);
            writer.WriteLine(StartSimbol.Text);
            writer.WriteLine(new TextRange(LeftRule.Document.ContentStart, LeftRule.Document.ContentEnd).Text);
            writer.Write(new TextRange(RightRule.Document.ContentStart, RightRule.Document.ContentEnd).Text);
            writer.Close();
            Load(filename);
        }

        private void Check()
        {
            CYKParser parser = new CYKParser();
            System.Windows.MessageBox.Show(parser.Check(StringBox.Text.Split(new char[] { ' ', ','}, StringSplitOptions.RemoveEmptyEntries).ToArray(), language));
        }
    }
}
