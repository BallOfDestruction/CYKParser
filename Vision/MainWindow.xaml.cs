using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using Parser;
using Language;

namespace Vision
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //Представление языка в форме Хомского
        private ChomskyNormalForm _language = new ChomskyNormalForm();

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
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePathBox.Text = dialog.FileName;
            }
        }
        //Загрузка языка из файла
        private void Load(string filepath)
        {
            var factory = new Factory(filepath);
            //Пытаемся прочитать из файла, если чего-то сломалось, то выводим ошибку на экран
            try
            {
                //Фабрикуем из файла язык
                _language = factory.GetLanguage();

                //Нетерминальный алфавит
                var rightAbs = _language.TerminalChars.Aggregate("", (current, a) => current + (a + ", "));
                RigthABC.Text = new string(rightAbs.Take(rightAbs.Length - 2).ToArray()); 

                //Терминальные алфавит
                var leftAbc = _language.NonTerminalChars.Aggregate("", (current, a) => current + (a + ", "));
                LeftABC.Text = new string(leftAbc.Take(leftAbc.Length - 2).ToArray());

                //Начальный нетерминал
                StartSimbol.Text = _language.Start;
                //Нетерминальные правила
                LeftRule.Document.Blocks.Clear();
                foreach (var rule in _language.NonEndRules)
                {
                    LeftRule.Document.Blocks.Add(new Paragraph(new Run(rule.Left + " -> " + rule.RightOne + ", " + rule.RightTwo)));
                }
                //Терминальные правила
                RightRule.Document.Blocks.Clear();
                foreach (var rule in _language.EndRules)
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
            var str = new StringBuilder();
            str.Append(RigthABC.Text + "\r\n");
            str.Append(LeftABC.Text + "\r\n");
            str.Append(StartSimbol.Text + "\r\n");
            str.Append(new TextRange(LeftRule.Document.ContentStart, LeftRule.Document.ContentEnd).Text + "\r\n");
            str.Append(new TextRange(RightRule.Document.ContentStart, RightRule.Document.ContentEnd).Text + "\r\n");
            _language = Factory.FromString(str.ToString());
        }

        //Запуск парсера
        private void Check()
        {
            var parser = new CYKParser();
            //Слово для проверки
            AnsverBox.Document.Blocks.Clear();
            var word = StringBox.Text.Split(new [] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            AnsverBox.AppendText(parser.Check(word, _language));
        }
    }
}
