namespace Language
{
    /// <summary>
    /// Правила "из нетерминала в два нетернинала"
    /// </summary>
    public class FirstRule : Rule
    {
        public string RightOne { get; set; }
        public string RightTwo { get; set; }

        public FirstRule(string left, string rightOne, string rightTwo) : base(left)
        {
            RightOne = rightOne;
            RightTwo = rightTwo;
        }
    }
}