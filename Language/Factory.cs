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

        public ChomskyNormalForm GetLanguage()
        {
            ChomskyNormalForm language = new ChomskyNormalForm();
            string fileText = File.ReadAllText(fileName);
            var fileData = fileText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            language.TerminalChars = fileData[0].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            language.NonTerminalChars = fileData[1].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (string terminal in language.TerminalChars)
            {
                foreach (string nonTerminal in language.NonTerminalChars)
                {
                    if (nonTerminal.Equals(terminal))
                    {
                        throw new Exception("Alphabets is intersect: " + terminal);
                    }
                }
            }

            if (language.NonTerminalChars.Contains(fileData[2]))
            {
                language.Start = fileData[2];
            }
            else
            {
                throw new Exception("Not valid format language: start simbol is not non-terminal");
            }
            fileData = fileData.Skip(3).ToArray();
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

        public static List<SecondRule> GetSecondRule(string str, List<string> terminal, List<string> nonTerminal)
        {
            var secondRules = new List<SecondRule>();
            var rules = str.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (string rule in rules)
            {
                var simbols = rule.Split(new char[] { '-', '>', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                if (terminal.Contains(simbols[1]) && nonTerminal.Contains(simbols[0]))
                {
                    secondRules.Add(new SecondRule(simbols[0], simbols[1]));
                }
                else
                {
                    throw new Exception("Not valid format language " + rule);
                }
            }
            return secondRules;
        }

        public static List<FirstRule> GetFirstRule(string str, List<string> nonTerminal)
        {
            var firstRules = new List<FirstRule>();
            var rules = str.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (string rule in rules)
            {
                var simbols = rule.Split(new char[] { '-', '>', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                if (nonTerminal.Contains(simbols[1]) && nonTerminal.Contains(simbols[0]) && nonTerminal.Contains(simbols[2]))
                {
                    firstRules.Add(new FirstRule(simbols[0], simbols[1], simbols[2]));
                }
                else
                {
                    throw new Exception("Not valid format language " + rule);
                }
            }
            return firstRules;
        }
    }
}
