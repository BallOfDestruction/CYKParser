using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Language;

namespace Parser
{
    public class CYKParser
    {
        private List<string>[][] matrix;
        private string[] word;
        private ChomskyNormalForm language;

        public string Check(string[] word, ChomskyNormalForm language)
        {
            this.language = language;
            this.word = word;
            int length = word.Length;
            matrix = new List<string>[length][];
            for (int i = 0; i < length; i++)
            {
                matrix[length - i - 1] = new List<string>[i+1];
                for (int j = 0; j < matrix[length - i - 1].Length; j++)
                {
                    matrix[length - i - 1][j] = new List<string>();
                }
            }
            LoadBottom();
            for (int i = 1; i < word.Length; i++)
            {
                Up(i);
            }
            if (matrix[0][word.Length - 1].Contains(language.Start))
            {
                return "OK";
            }
            return "NOT OK";
        }

        private void LoadBottom()
        {
            for (int i = 0; i < word.Length; i++)
            {
                foreach (SecondRule rule in language.EndRules)
                {
                    if ((word[i].Equals(rule.Right)) && (!matrix[i][0].Contains(rule.Left)))
                    {
                        matrix[i][0].Add(rule.Left);
                    }
                }
            }
        }

        private void Up(int depth)
        {
            for (int i = 0; i < word.Length-depth; i++)
            {
                List<string> combo = new List<string>();
                for (int coef = 0; coef < depth; coef++)
                {
                    var lefts = matrix[i][depth - 1 - coef];
                    var rights = matrix[i + depth - coef][coef];
                    foreach (string left in lefts)
                    {
                        foreach (string right in rights)
                        {
                            var searched = SearchFirst(left, right);
                            foreach (string str in searched)
                            {
                                if (!combo.Contains(str))
                                {
                                    combo.Add(str);
                                }
                            }
                        }
                    }
                }
                matrix[i][depth].AddRange(combo);
            }
        }

        private List<string> SearchFirst(string left, string right)
        {
            List<string> terms = new List<string>();
            foreach (FirstRule rule in language.NonEndRules)
            {
                if (rule.RightOne.Equals(left) && rule.RightTwo.Equals(right))
                {
                    terms.Add(rule.Left);
                }
            }
            return terms;
        }
    }
}
