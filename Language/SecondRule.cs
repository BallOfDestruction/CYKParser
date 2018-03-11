namespace Language
{
    /// <summary>
    /// Правила "из нетерминала в терминал"
    /// </summary>
    public class SecondRule : Rule
    {
        public string Right { get; set; }

        public SecondRule(string left, string right) : base(left)
        {
            Right = right;
        }
    }
}