using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Language
{
    public class Factory
    {
        private string fileName = "";

        public Factory(string fileName)
        {
            this.fileName = fileName;
        }
        /// <summary>
        /// Получаем грамматику в нормальной форме Хомского
        /// </summary>
        public ChomskyNormalForm GetLanguage()
        {
            string fileText = File.ReadAllText(fileName);
            return FromString(fileText);
        }

        public static ChomskyNormalForm FromString(string dataFile)
        {
            ChomskyNormalForm language = new ChomskyNormalForm();
            //Разделяем построчно
            var fileData = dataFile.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            //Разделяем в список терминальных символов
            language.TerminalChars = fileData[0].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //Разделяем в список нетерминальных символов
            language.NonTerminalChars = fileData[1].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            //Проверка на пересечение алфавитов
            foreach (string terminal in language.TerminalChars)
                foreach (string nonTerminal in language.NonTerminalChars)
                    if (nonTerminal.Equals(terminal))
                        throw new Exception("Alphabets is intersect: " + terminal);

            //Проверка валидности стартового нетерминала
            if (language.NonTerminalChars.Contains(fileData[2]))
                language.Start = fileData[2];
            else
                throw new Exception("Not valid format language: start simbol is not non-terminal");

            //Пропускаем первые три строки
            fileData = fileData.Skip(3).ToArray();
            //Считываем правила из оставшихся строк
            //Если справа два нетерминальных - в список NotEnd
            //Еcли один терминальный - в список End  
            foreach (string ruleString in fileData)
            {
                var simbols = ruleString.Split(new char[] { '-', '>', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                if (language.NonTerminalChars.Contains(simbols[0]))
                {
                    if (language.TerminalChars.Contains(simbols[1]))
                    {
                        language.EndRules.Add(new SecondRule(simbols[0], simbols[1]));
                        continue;
                    }
                    if (language.NonTerminalChars.Contains(simbols[1]) && language.NonTerminalChars.Contains(simbols[2]))
                    {
                        language.NonEndRules.Add(new FirstRule(simbols[0], simbols[1], simbols[2]));
                        continue;
                    }
                    throw new Exception("Not valid format language: " + ruleString);
                }
                else
                {
                    throw new Exception("Not valid format language: " + ruleString);
                }
            }
            return language;
        }
    }
}
