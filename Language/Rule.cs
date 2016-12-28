using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    //Правила формы Хомского
    public class Rule
    {
        public readonly string Left;


        public Rule(string left)
        {
            this.Left = left;
        }
    }
    /// <summary>
    /// Правила "из нетерминала в два нетернинала"
    /// </summary>
    public class FirstRule : Rule
    {
        public readonly string RightOne;
        public readonly string RightTwo;

        public FirstRule(string left, string rightOne, string rightTwo) : base(left)
        {
            this.RightOne = rightOne;
            this.RightTwo = rightTwo;
        }
    }
    /// <summary>
    /// Правила "из нетерминала в терминал"
    /// </summary>
    public class SecondRule : Rule
    {
        public readonly string Right;

        public SecondRule(string left, string right) : base(left)
        {
            this.Right = right;
        }
    }
}
