using System.Text.Json;
using System.Xml.Schema;
using ShitCompiler.CodeAnalysis;
using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;

namespace ShitCompiler;

class Program
{
    static void Main(string[] args)
    {
        string testInput = 
        """
           funk abc(abc1:long, avs2:double): long
           {     
                return 1+1+2+3+4+avs2;
                return abc1;
           }
           
           abc();
           
           val sd:string="12312" + 123 + 5 + 3 +3 + 5+ 6+ 4 + 4;      
           
           if(12334 != (2345 + 0))
                sd = "abc";
        
            else 
                sd = "abc2";
            

        """;
        Console.WriteLine(testInput);
        TextCursor cursor = new(testInput.AsMemory());
        ILexer lexer = new SimpleLexer(cursor);

        //Lexeme? lexeme = null;
        //while (lexeme?.Kind is not SyntaxKind.EndToken or SyntaxKind.BadToken)
        //{
        //    lexeme = lexer.ScanNext();
        //    Console.WriteLine(lexeme);
        //}

        
        ISyntaxParser parser = new SimpleSyntaxParser(
            new LexemeQueue(
                lexer
            ),
            new SymbolTable(),
            new UebanErrorsHandlingStrategy()
        );
        
        Console.WriteLine(JsonSerializer.Serialize(
            parser.ParseCompilationUnit(),
            new JsonSerializerOptions()
            {
                WriteIndented = true,
            }
        ));
    }
}