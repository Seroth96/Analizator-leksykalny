using System;
using System.Collections.Generic;
using System.Text;

namespace Analizator
{
    public class Parser
    {
        private List<LexerToken<OwlToken>> Tokens;        

        public Parser(List<LexerToken<OwlToken>> tokens)
        {
            Tokens = tokens;
        }

        public void Parse()
        {
            int i = 0;
            while (i < Tokens.Count)
            {
                if (Tokens[i].Type == OwlToken.Whitespace)
                {
                    i++;
                    continue;
                }

                Axioms(ref i);
            }
            Console.WriteLine("Parsing result - Finishing OK");
        }

        private void Axioms(ref int i)
        {
            if (Tokens[i].Type == OwlToken.SubClassOf ||
                Tokens[i].Type == OwlToken.EquivalentClasses ||
                Tokens[i].Type == OwlToken.DisjointClasses )
            {
                ClassAxiom(ref i);
            }
            else if (Tokens[i].Type == OwlToken.SameIndividual ||
                Tokens[i].Type == OwlToken.DifferentIndividuals)
            {
                Assertion(ref i);
            }
            else
            {
                throw new Exception($"Invalid Axiom at token {i} with value {Tokens[i].Value}");
            }

            Console.WriteLine("Parsing result - Axiom OK");
        }

        private void Assertion(ref int i)
        {
            if (Tokens[i].Type == OwlToken.SameIndividual)
            {
                SameIndividuals(ref i);
            }
            else if (Tokens[i].Type == OwlToken.DifferentIndividuals)
            {
                DifferentIndividuals(ref i);
            }
        }

        private void DifferentIndividuals(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.DifferentIndividuals);
            SkipNLorWhitespace(ref i);

            AtLeastNIri(ref i, 2);

            Console.WriteLine("Parsing result - DifferentIndividuals OK");
        }

        private void SameIndividuals(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.SameIndividual);
            SkipNLorWhitespace(ref i);

            AtLeastNIri(ref i, 2);

            Console.WriteLine("Parsing result - SameIndividual OK");
        }

        private void ClassAxiom(ref int i)
        {
            if (Tokens[i].Type == OwlToken.SubClassOf)
            {
                SubClassOf(ref i);
            }
            else if (Tokens[i].Type == OwlToken.EquivalentClasses)
            {
                EquivalentClasses(ref i);
            }
            else if (Tokens[i].Type == OwlToken.DisjointClasses)
            {
                DisjointClasses(ref i);
            }
            
            Console.WriteLine("Parsing result - ClassAxiom OK");
        }

        private void DisjointClasses(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.DisjointClasses);
            SkipNLorWhitespace(ref i);

            AtLeastTwoClassExpression(ref i);

            Console.WriteLine("Parsing result - DisjointClasses OK");
        }

        private void EquivalentClasses(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.EquivalentClasses);
            SkipNLorWhitespace(ref i);

            AtLeastTwoClassExpression(ref i);

            Console.WriteLine("Parsing result - EquivalentClasses OK");
        }

        private void SubClassOf(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.SubClassOf);
            SkipNLorWhitespace(ref i);
            AssertToken(Tokens[i++], OwlToken.LP);
            SkipNLorWhitespace(ref i);

            ClassExpression(ref i);
            SkipNLorWhitespace(ref i);
            ClassExpression(ref i);
            SkipNLorWhitespace(ref i);

            AssertToken(Tokens[i++], OwlToken.RP);
            SkipNLorWhitespace(ref i);

            Console.WriteLine("Parsing result - SubClassOf OK");
        }

        private void ClassExpression(ref int i)
        {
            if (Tokens[i].Type == OwlToken.Iri)
            {
                Klasa(ref i);
            }
            else if(Tokens[i].Type == OwlToken.ObjectIntersectionOf)
            {
                ObjectIntersectionOf(ref i);
            }
            else if (Tokens[i].Type == OwlToken.ObjectUnionOf)
            {
                ObjectUnionOf(ref i);
            }
            else if (Tokens[i].Type == OwlToken.ObjectComplementOf)
            {
                ObjectComplementOf(ref i);
            }
            else if (Tokens[i].Type == OwlToken.ObjectOneOf)
            {
                ObjectOneOf(ref i);
            }
            else
            {
                throw new Exception($"Invalid ClassExpression at token {i} with value {Tokens[i].Value}");
            }

            Console.WriteLine("Parsing result - ClassExpression OK");
        }

        private void ObjectIntersectionOf(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.ObjectIntersectionOf);
            SkipNLorWhitespace(ref i);

            AtLeastTwoClassExpression(ref i);

            Console.WriteLine("Parsing result - ObjectIntersectionOf OK");
        }

        private void ObjectUnionOf(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.ObjectUnionOf);
            SkipNLorWhitespace(ref i);

            AtLeastTwoClassExpression(ref i);

            Console.WriteLine("Parsing result - ObjectUnionOf OK");
        }

        private void ObjectComplementOf(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.ObjectComplementOf);
            SkipNLorWhitespace(ref i);

            AssertToken(Tokens[i++], OwlToken.LP);
            SkipNLorWhitespace(ref i);

            ClassExpression(ref i);
            SkipNLorWhitespace(ref i);

            AssertToken(Tokens[i++], OwlToken.RP);
            SkipNLorWhitespace(ref i);

            Console.WriteLine("Parsing result - ObjectComplementOf OK");
        }

        private void ObjectOneOf(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.ObjectOneOf);
            SkipNLorWhitespace(ref i);

            AtLeastNIri(ref i, 1);

            Console.WriteLine("Parsing result - ObjectOneOf OK");
        }

        private void Klasa(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.Iri);
            SkipNLorWhitespace(ref i);

            Console.WriteLine("Parsing result - Class OK");
        }

        private void AssertToken(LexerToken<OwlToken> token, OwlToken type)
        {
            if (token.Type != type)
            {
                throw new Exception($"The provided token \"{token.Value}\" : \"{token.Type}\" should be instead of type \"{type}\"");
            }
        }

        private void AtLeastTwoClassExpression(ref int i)
        {
            AssertToken(Tokens[i++], OwlToken.LP);
            SkipNLorWhitespace(ref i);

            ClassExpression(ref i);
            SkipNLorWhitespace(ref i);
            ClassExpression(ref i);
            SkipNLorWhitespace(ref i);

            while (i < Tokens.Count)
            {
                if (Tokens[i].Type == OwlToken.RP)
                {
                    i++;
                    return;
                }
                ClassExpression(ref i);
                SkipNLorWhitespace(ref i);
            }

            throw new Exception($"Missing token ')' at token {i} with value {Tokens[i].Value}");
        }

        private void AtLeastNIri(ref int i, int n)
        {
            AssertToken(Tokens[i++], OwlToken.LP);
            SkipNLorWhitespace(ref i);

            for (int k = 0; k < n; k++)
            {
                AssertToken(Tokens[i++], OwlToken.Iri);
                SkipNLorWhitespace(ref i);
            }
            while (i < Tokens.Count)
            {
                if (Tokens[i].Type == OwlToken.RP)
                {
                    i++;
                    return;
                }
                ClassExpression(ref i);

                SkipNLorWhitespace(ref i);
            }

            throw new Exception($"Missing token ')' at token {i} with value {Tokens[i].Value}");

        }

        private void SkipNLorWhitespace(ref int i)
        {
            while (i < Tokens.Count)
            {
                if (Tokens[i].Type == OwlToken.Whitespace ||
                    Tokens[i].Type == OwlToken.NL)
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
