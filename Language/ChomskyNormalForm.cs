using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class ChomskyNormalForm
    {
        public List<string> TerminalChars { get; set; } = new List<string>();

        public List<string> NonTerminalChars { get; set; } = new List<string>();

        public List<SecondRule> EndRules { get; set; } = new List<SecondRule>();

        public List<FirstRule> NonEndRules { get; set; } = new List<FirstRule>();

        public string Start { get; set; } = "";
    }
}
