using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Analizator
{
    public class Lexer<TokenType>
    {
        public List<LexerRule<TokenType>> Rules { get; private set; } = new List<LexerRule<TokenType>>();

        public bool Verbose { get; set; } = false;

        /// <summary>
        /// The number of the line that currently being scanned.
        /// </summary>
        public int CurrentLineNumber { get; private set; } = 0;
        /// <summary>
        /// The number of characters on the current line that have been scanned.
        /// </summary>
        /// <value>The current line position.</value>
        public int CurrentLinePos { get; private set; } = 0;
        /// <summary>
        /// The total number of characters currently scanned by this lexer instance.
        /// Only updated every newline!
        /// </summary>
        public int TotalCharsScanned { get; private set; } = 0;

        private StreamReader textStream;

        public Lexer()
        {

        }

        public void AddRule(LexerRule<TokenType> newRule)
        {
            Rules.Add(newRule);
        }

        public void AddRules(IEnumerable<LexerRule<TokenType>> newRules)
        {
            Rules.AddRange(newRules);
        }

        public void Initialise(StreamReader reader)
        {
            textStream = reader;
        }

        public IEnumerable<LexerToken<TokenType>> TokenStream()
        {
            string nextLine;
            List<LexerToken<TokenType>> matches = new List<LexerToken<TokenType>>();
            while ((nextLine = textStream.ReadLine()) != null)
            {
                CurrentLinePos = 0;

                while (CurrentLinePos < nextLine.Length)
                {
                    matches.Clear();
                    foreach (LexerRule<TokenType> rule in Rules)
                    {
                        if (!rule.Enabled) continue;

                        Match nextMatch = rule.RegEx.Match(nextLine, CurrentLinePos);
                        if (!nextMatch.Success) continue;

                        matches.Add(new LexerToken<TokenType>(rule, nextMatch));
                    }

                    if (matches.Count == 0)
                    {
                        string unknownTokenContent = nextLine.Substring(CurrentLinePos);
                        if (Verbose) Console.WriteLine("[Unknown Token: No matches found for this line] {0}", unknownTokenContent);
                        yield return new LexerToken<TokenType>(unknownTokenContent);
                        break;
                    }

                    matches.Sort((LexerToken<TokenType> a, LexerToken<TokenType> b) => {
                        // Match of offset position position
                        int result = nextLine.IndexOf(a.RegexMatch.Value, CurrentLinePos, StringComparison.CurrentCulture) -
                                    nextLine.IndexOf(b.RegexMatch.Value, CurrentLinePos, StringComparison.CurrentCulture);
                        // If they both start at the same position, then go with the longest one
                        if (result == 0)
                            result = b.RegexMatch.Length - a.RegexMatch.Length;

                        return result;
                    });

                    LexerToken<TokenType> selectedToken = matches[0];
                    int selectedTokenOffset = nextLine.IndexOf(selectedToken.RegexMatch.Value, CurrentLinePos) - CurrentLinePos;

                    if (selectedTokenOffset > 0)
                    {
                        string extraTokenContent = nextLine.Substring(CurrentLinePos, selectedTokenOffset);
                        CurrentLinePos += selectedTokenOffset;
                        if (Verbose) Console.WriteLine("[Unmatched content] '{0}'", extraTokenContent);
                        yield return new LexerToken<TokenType>(extraTokenContent);
                    }

                    CurrentLinePos += selectedToken.RegexMatch.Length;
                    if (Verbose) Console.WriteLine(selectedToken);
                    yield return selectedToken;

                }

                if (Verbose) Console.WriteLine("[Lexer] Next line");
                CurrentLineNumber++;
                TotalCharsScanned += CurrentLinePos;
            }
        }

        public void EnableRule(TokenType type)
        {
            SetRule(type, true);
        }

        public void DisableRule(TokenType type)
        {
            SetRule(type, false);
        }

        public void SetRule(TokenType type, bool state)
        {
            foreach (LexerRule<TokenType> rule in Rules)
            {
                // We have to do a string comparison here because of the generic type we're using in multiple nested
                // classes
                if (Enum.GetName(rule.Type.GetType(), rule.Type) == Enum.GetName(type.GetType(), type))
                {
                    rule.Enabled = state;
                    return;
                }
            }
        }

        public void EnableRulesByPrefix(string tokenTypePrefix)
        {
            SetRulesByPrefix(tokenTypePrefix, true);
        }

        public void DisableRulesByPrefix(string tokenTypePrefix)
        {
            SetRulesByPrefix(tokenTypePrefix, false);
        }

        public void SetRulesByPrefix(string tokenTypePrefix, bool state)
        {
            foreach (LexerRule<TokenType> rule in Rules)
            {
                // We have to do a string comparison here because of the generic type we're using in multiple nested
                // classes
                if (Enum.GetName(rule.Type.GetType(), rule.Type).StartsWith(tokenTypePrefix, StringComparison.CurrentCulture))
                {
                    rule.Enabled = state;
                }
            }
        }
    }
}
