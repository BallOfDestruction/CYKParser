using System;
using System.Linq;
using System.IO;

namespace Language
{
    public class Factory
    {
        private readonly string _formText;

        public Factory(string fileName)
        {
            _formText = File.ReadAllText(fileName);
        }
        /// <summary>
        /// Получаем грамматику в нормальной форме Хомского
        /// </summary>
        public ChomskyNormalForm GetLanguage()
        {
            return FromString(_formText);
        }

        public static ChomskyNormalForm FromString(string dataFile)
        {
            var language = new ChomskyNormalForm();
            //Разделяем построчно
            var fileData = dataFile.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            //Разделяем в список терминальных символов
            language.TerminalChars = fileData[0].Split(new [] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //Разделяем в список нетерминальных символов
            language.NonTerminalChars = fileData[1].Split(new [] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            //Проверка на пересечение алфавитов
            foreach (var terminal in language.TerminalChars)
                if (language.NonTerminalChars.Any(nonTerminal => nonTerminal.Equals(terminal)))
                    throw new Exception("Alphabets is intersect: " + terminal);

            language.Start = fileData[2];

            //Проверка валидности стартового нетерминала
            if (!language.NonTerminalChars.Contains(fileData[2]))
                throw new Exception("Not valid format language: start simbol is not non-terminal");
            
            fileData = fileData.Skip(3).ToArray();
            //Считываем правила из оставшихся строк
            //Если справа два нетерминальных - в список NotEnd
            //Еcли один терминальный - в список End  
            foreach (var ruleString in fileData)
            {
                var simbols = ruleString.Split(new [] { '-', '>', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                if (!language.NonTerminalChars.Contains(simbols[0]))
                    throw new Exception("Not valid format language: " + ruleString);

                if (language.TerminalChars.Contains(simbols[1]))
                {
                    language.EndRules.Add(new SecondRule(simbols[0], simbols[1]));
                    continue;
                }

                if (!language.NonTerminalChars.Contains(simbols[1]) || !language.NonTerminalChars.Contains(simbols[2]))
                    throw new Exception("Not valid format language: " + ruleString);

                language.NonEndRules.Add(new FirstRule(simbols[0], simbols[1], simbols[2]));
            }
            return language;
        }
    }
}
