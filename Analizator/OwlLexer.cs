using System;
using System.Collections.Generic;
using System.Text;

namespace Analizator
{

    public enum OwlToken
    {
        Unknown = 0,

        Whitespace,
        Iri,
        ObjectIntersectionOf,
        ObjectUnionOf,
        ObjectComplementOf,
        ObjectOneOf,
        SameIndividual,
        DifferentIndividuals,
        DisjointClasses,
        EquivalentClasses,
        SubClassOf,
        NL,
        LP,
        RP
    }

    public class OwlLexer : Lexer<OwlToken>
    {
        public OwlLexer()
        {
            AddRules(new List<LexerRule<OwlToken>>() {
            new LexerRule<OwlToken>(OwlToken.Whitespace,    @"\s+"),
            new LexerRule<OwlToken>(OwlToken.NL,    "\n"),
            new LexerRule<OwlToken>(OwlToken.LP,    @"\("),
            new LexerRule<OwlToken>(OwlToken.RP,    @"\)"),
            new LexerRule<OwlToken>(OwlToken.Iri,    @"\w*[:]\w*"),
            new LexerRule<OwlToken>(OwlToken.ObjectIntersectionOf,    @"ObjectIntersectionOf"),
            new LexerRule<OwlToken>(OwlToken.ObjectUnionOf,    @"ObjectUnionOf"),
            new LexerRule<OwlToken>(OwlToken.ObjectComplementOf,    @"ObjectComplementOf"),
            new LexerRule<OwlToken>(OwlToken.ObjectOneOf,    @"ObjectOneOf"),
            new LexerRule<OwlToken>(OwlToken.SameIndividual,    @"SameIndividual"),
            new LexerRule<OwlToken>(OwlToken.DifferentIndividuals,    @"DifferentIndividuals"),
            new LexerRule<OwlToken>(OwlToken.DisjointClasses,    @"DisjointClasses"),
            new LexerRule<OwlToken>(OwlToken.EquivalentClasses,    @"EquivalentClasses"),
            new LexerRule<OwlToken>(OwlToken.SubClassOf,    @"SubClassOf"),
        });
        }
    }
}
