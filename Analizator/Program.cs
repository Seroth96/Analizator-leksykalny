using System;
using System.Collections.Generic;
using System.IO;

namespace Analizator
{
    class Program
    {
        private static string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
        static void Main(string[] args)
        {
            List<LexerToken<OwlToken>> tokens = new List<LexerToken<OwlToken>>();
            var lexer = new OwlLexer();
            _filePath = Directory.GetParent(_filePath).FullName;
            String path = Directory.GetParent(Directory.GetParent(Directory.GetParent(_filePath).FullName).FullName).FullName;

            if (args.Length > 0)
            {
                path += "\\" + args[0];
            }

            using (StreamReader sr = File.OpenText(path))
            {                
                lexer.Initialise(sr);
                foreach (var token in lexer.TokenStream())
                {
                    tokens.Add(token);
                    Console.WriteLine(token.ToString());
                }
            }

            var parser = new Parser(tokens);
            try
            {
                parser.Parse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();


        }

    }
}
