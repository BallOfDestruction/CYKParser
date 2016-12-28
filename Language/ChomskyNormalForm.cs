using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class ChomskyNormalForm
    {
        /// <summary>
        /// Список терминальных символов
        /// </summary>
        public List<string> TerminalChars { get; set; } = new List<string>();
        /// <summary>
        /// Список нетерминальных символов
        /// </summary>
        public List<string> NonTerminalChars { get; set; } = new List<string>(); 
        /// <summary>
        /// Правила с терминальным символом справа
        /// </summary>
        public List<SecondRule> EndRules { get; set; } = new List<SecondRule>() ;  
        /// <summary>
        /// Правила с двумя нетерминалами справа
        /// </summary>
        public List<FirstRule> NonEndRules { get; set; } = new List<FirstRule>();
        /// <summary>
        /// Начальный нетерминал
        /// </summary>
        public string Start { get; set; } = "";
    }
}
