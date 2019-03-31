using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Analizator
{
    public class LexerRule<TokenType>
    {
        public readonly TokenType Type;
        public readonly Regex RegEx;
        public bool Enabled { get; set; } = true;

        public LexerRule(TokenType inName, string inRegEx)
        {
            if (!typeof(TokenType).IsEnum)
                throw new ArgumentException($"Error: inName must be an enum - {typeof(TokenType)} passed");

            Type = inName;
            RegEx = new Regex(inRegEx);
        }

        public bool Toggle()
        {
            Enabled = !Enabled;
            return Enabled;
        }
    }
}
