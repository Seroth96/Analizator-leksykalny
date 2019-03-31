using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Analizator
{
    public class LexerToken<TokenType>
    {
        public readonly bool IsNullMatch = false;
        public readonly LexerRule<TokenType> Rule = null;
        public readonly Match RegexMatch;

        public TokenType Type
        {
            get
            {
                try
                {
                    return Rule.Type;
                }
                catch (NullReferenceException)
                {
                    return default(TokenType);
                }
            }
        }
        private string nullValueData;
        public string Value
        {
            get
            {
                return IsNullMatch ? nullValueData : RegexMatch.Value;
            }
        }

        public LexerToken(LexerRule<TokenType> inRule, Match inMatch)
        {
            Rule = inRule;
            RegexMatch = inMatch;
        }
        public LexerToken(string unknownData)
        {
            IsNullMatch = true;
            nullValueData = unknownData;
        }

        public override string ToString()
        {
            return string.Format("[LexerToken: Type={0}, Value={1}]", Type, Value);
        }
    }
}
