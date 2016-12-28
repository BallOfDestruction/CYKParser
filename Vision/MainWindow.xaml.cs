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
        //Представление языка в форме Хомского
        private ChomskyNormalForm language = new ChomskyNormalForm();

        public MainWindow()
        {
            InitializeComponent();
            //Инициализация окна
            LoadButton.Click += ((sender, e) => Load(FilePathBox.Text));
            OpenButton.Click += ((sender, e) => OpenFile());
            RestoreButton.Click += ((sender, e) => RestoreLanguage());
            CheckButton.Click += ((sender, e) => Check());
        }
        //Открытие файла
        //Считываем только путь к файлу
        private void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePathBox.Text = dialog.FileName;
            }
        }
        //Загрузка языка из файла
        private void Load(string filepath)
        {
            Factory factory = new Factory(filepath);
            //Пытаемся прочитать из файла, если чего-то сломалось, то выводим ошибку на экран
            try
            {
                //Фабрикуем из файла язык
                language = factory.GetLanguage();

                //Нетерминальный алфавит
                string rightABS = "";
                foreach (string a in language.TerminalChars)
                {
                    rightABS += a + ", ";
                }
                RigthABC.Text = new string(rightABS.Take(rightABS.Length - 2).ToArray()); 
                //Терминальные алфавит
                string leftABC = "";
                foreach (string a in language.NonTerminalChars)
                {
                    leftABC += a + ", ";
                }
                LeftABC.Text = new string(leftABC.Take(leftABC.Length - 2).ToArray());
                //Начальный нетерминал
                StartSimbol.Text = language.Start;
                //Нетерминальные правила
                LeftRule.Document.Blocks.Clear();
                foreach (FirstRule rule in language.NonEndRules)
                {
                    LeftRule.Document.Blocks.Add(new Paragraph(new Run(rule.Left + " -> " + rule.RightOne + ", " + rule.RightTwo)));
                }
                //Терминальные правила
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
        //Обновить язык
        private void RestoreLanguage()
        {
            //Записываем в временный файл все данные в нужном формате, потом загружаем их же из него
            StringBuilder str = new StringBuilder();
            str.Append(RigthABC.Text + "\r\n");
            str.Append(LeftABC.Text + "\r\n");
            str.Append(StartSimbol.Text + "\r\n");
            str.Append(new TextRange(LeftRule.Document.ContentStart, LeftRule.Document.ContentEnd).Text + "\r\n");
            str.Append(new TextRange(RightRule.Document.ContentStart, RightRule.Document.ContentEnd).Text + "\r\n");
            language = Factory.FromString(str.ToString());
        }
        //Запуск парсера
        private void Check()
        {
            CYKParser parser = new CYKParser();
            //Слово для проверки
            var word = StringBox.Text.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            System.Windows.MessageBox.Show(parser.Check(word, language));
        }
    }
}
