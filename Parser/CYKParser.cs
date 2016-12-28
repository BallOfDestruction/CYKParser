using System.Collections.Generic;
using Language;
using System;
using System.Text;

namespace Parser
{
    public class CYKParser
    {
        private List<string>[][] matrix;
        private string[] word;
        private ChomskyNormalForm language;

        private class ElPath
        {
            public string Name { get; set; } = "";

            public bool IsEnd { get { if (J == 0) return true; else return false; } }

            public int J { get; private set; } = 0;

            public int I { get; private set; } = 0;

            public ElPath(string Name, int i, int j)
            {
                this.Name = Name;
                this.I = i;
                this.J = j;
            }
        }

        public string GetPath()
        {
            StringBuilder str = new StringBuilder();

            List<ElPath> result = new List<ElPath>();
            ElPath elPath = new ElPath(language.Start, 0, word.Length - 1);
            result.Add(elPath);
            int p = 0;
            while (p <= word.Length - 1)
            {
                elPath = result[p];
                //Если это символ, который переходит в терминальный - идем дальше
                if (elPath.IsEnd)
                {
                    p++;
                    continue;
                }
                foreach (ElPath path in result)
                {
                    str.Append(path.Name);
                    str.Append(", ");
                }
                str.Remove(str.Length - 2, 2);
                str.Append(" -> ");
                int LeftI = elPath.I;
                int RightI = elPath.I + 1;
                int LeftJ = 0;
                int RightJ = elPath.J - 1;
                List<ElPath> goodPath = new List<ElPath>();
                while (LeftJ < elPath.J)
                {
                    List<string> left = matrix[LeftI][LeftJ];
                    List<string> right = matrix[RightI][RightJ];
                    foreach (string leftElement in left)
                    {
                        foreach (string rightElement in right)
                        {
                            foreach (FirstRule rule in language.NonEndRules)
                            {
                                if (rule.Left.Equals(elPath.Name) && rule.RightOne.Equals(leftElement) && rule.RightTwo.Equals(rightElement))
                                {
                                    ElPath leftEl = new ElPath(leftElement, LeftI, LeftJ);
                                    ElPath rightEl = new ElPath(rightElement, RightI, RightJ);
                                    goodPath.Add(leftEl);
                                    goodPath.Add(rightEl);
                                }
                            }
                        }
                    }
                    LeftJ++;
                    RightI++;
                    RightJ--;
                }
                if (goodPath.Count == 0)
                {
                    throw new Exception("Exception: code1(70)");
                }
                result.RemoveAt(p);
                result.Insert(p, goodPath[1]);
                result.Insert(p, goodPath[0]);
            }
            foreach (ElPath path in result)
            {
                str.Append(path.Name);
                str.Append(", ");
            }
            str.Remove(str.Length - 2, 2);
            str.Append(" -> ");
            foreach (string path in word)
            {
                str.Append(path);
                str.Append(", ");
            }
            str.Remove(str.Length - 2, 2);
            return str.ToString();
        }



        public string Check(string[] word, ChomskyNormalForm language)
        {
            //инициализируем
            this.language = language;
            this.word = word;
            int length = word.Length;
            matrix = new List<string>[length][];

            //Инициализация матрицу как нижнеугловую
            for (int i = 0; i < length; i++)
            {
                matrix[length - i - 1] = new List<string>[i+1];
                for (int j = 0; j < matrix[length - i - 1].Length; j++)
                {
                    matrix[length - i - 1][j] = new List<string>();
                }
            }

            //Загружаем нижнию часть 
            LoadBottom();

            //Повышаем матрицу вверх
            for (int i = 1; i < word.Length; i++)
                Up(i);

            //Если вверху матрицы есть стартовый нетерминальный символ
            if (matrix[0][word.Length - 1].Contains(language.Start))
            {
                return GetPath();
            }
            else
                return "NOT OK";
        }

        //Прогружаем нижную строчку
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

        //Прогружаем все остальное по глубине входа( колличеству символов, для которых подбираем переборы)
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
        //Ищет список всех нужных неконечных правил
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
