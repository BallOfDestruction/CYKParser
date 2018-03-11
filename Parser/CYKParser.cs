using System.Collections.Generic;
using Language;
using System;
using System.Linq;
using System.Text;

namespace Parser
{
    public class CYKParser
    {
        private List<string>[][] _matrix;
        private string[] _word;
        private ChomskyNormalForm _language;

        private class ElPath
        {
            public string Name { get; }

            public bool IsEnd => J == 0;

            public int J { get; }

            public int I { get; }

            public ElPath(string name, int i, int j)
            {
                Name = name;
                I = i;
                J = j;
            }
        }

        private string GetPath()
        {
            var str = new StringBuilder();

            var result = new List<ElPath>();
            var elPath = new ElPath(_language.Start, 0, _word.Length - 1);
            result.Add(elPath);
            var p = 0;
            while (p <= _word.Length - 1)
            {
                elPath = result[p];
                //Если это символ, который переходит в терминальный - идем дальше
                if (elPath.IsEnd)
                {
                    p++;
                    continue;
                }
                foreach (var path in result)
                {
                    str.Append(path.Name);
                    str.Append(", ");
                }
                str.Remove(str.Length - 2, 2);
                str.Append(" -> ");
                var leftI = elPath.I;
                var rightI = elPath.I + 1;
                var leftJ = 0;
                var rightJ = elPath.J - 1;
                var goodPath = new List<ElPath>();
                while (leftJ < elPath.J)
                {
                    var left = _matrix[leftI][leftJ];
                    var right = _matrix[rightI][rightJ];
                    foreach (var leftElement in left)
                    {
                        foreach (var rightElement in right)
                        {
                            foreach (var rule in _language.NonEndRules)
                            {
                                if (!rule.Left.Equals(elPath.Name) || !rule.RightOne.Equals(leftElement) ||!rule.RightTwo.Equals(rightElement)) continue;

                                var leftEl = new ElPath(leftElement, leftI, leftJ);
                                var rightEl = new ElPath(rightElement, rightI, rightJ);
                                goodPath.Add(leftEl);
                                goodPath.Add(rightEl);
                            }
                        }
                    }
                    leftJ++;
                    rightI++;
                    rightJ--;
                }
                if (goodPath.Count == 0)
                {
                    throw new Exception("Exception: code1(70)");
                }
                result.RemoveAt(p);
                result.Insert(p, goodPath[1]);
                result.Insert(p, goodPath[0]);
            }
            foreach (var path in result)
            {
                str.Append(path.Name);
                str.Append(", ");
            }
            str.Remove(str.Length - 2, 2);
            str.Append(" -> ");
            foreach (var path in _word)
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
            _language = language;
            _word = word;
            var length = word.Length;
            _matrix = new List<string>[length][];

            //Инициализация матрицу как нижнеугловую
            for (var i = 0; i < length; i++)
            {
                _matrix[length - i - 1] = new List<string>[i + 1];
                for (var j = 0; j < _matrix[length - i - 1].Length; j++)
                {
                    _matrix[length - i - 1][j] = new List<string>();
                }
            }

            //Загружаем нижнию часть 
            LoadBottom();

            //Повышаем матрицу вверх
            for (var i = 1; i < word.Length; i++)
                Up(i);

            //Если вверху матрицы есть стартовый нетерминальный символ
            return _matrix[0][word.Length - 1].Contains(language.Start) ? GetPath() : "NOT OK";
        }

        //Прогружаем нижную строчку
        private void LoadBottom()
        {
            for (var i = 0; i < _word.Length; i++)
            {
                foreach (var rule in _language.EndRules)
                {
                    if ((_word[i].Equals(rule.Right)) && (!_matrix[i][0].Contains(rule.Left)))
                    {
                        _matrix[i][0].Add(rule.Left);
                    }
                }
            }
        }

        //Прогружаем все остальное по глубине входа( колличеству символов, для которых подбираем переборы)
        private void Up(int depth)
        {
            for (var i = 0; i < _word.Length - depth; i++)
            {
                var combo = new List<string>();
                for (var coef = 0; coef < depth; coef++)
                {
                    var lefts = _matrix[i][depth - 1 - coef];
                    var rights = _matrix[i + depth - coef][coef];
                    foreach (var left in lefts)
                    {
                        foreach (var right in rights)
                        {
                            var searched = SearchFirst(left, right);
                            foreach (var str in searched)
                            {
                                if (!combo.Contains(str))
                                {
                                    combo.Add(str);
                                }
                            }
                        }
                    }
                }
                _matrix[i][depth].AddRange(combo);
            }
        }

        //Ищет список всех нужных неконечных правил
        private IEnumerable<string> SearchFirst(string left, string right)
        {
            return _language.NonEndRules.Where(rule => rule.RightOne.Equals(left) && rule.RightTwo.Equals(right))
                                        .Select(rule => rule.Left).ToList();
        }
    }
}