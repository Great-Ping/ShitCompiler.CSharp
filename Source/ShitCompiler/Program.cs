using System.Xml.Schema;
using ShitCompiler.CodeAnalysis;
using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;

namespace ShitCompiler;

class Program
{
    static void Main(string[] args)
    {
        string testInput = """
                           0.000
                           .0000
                           0.1000
                           00.00.00
                           132424
                           :
                           var abs = "abstrd";
                           
                           ["Строка" 'с' 'Инвалид'] 
                           // Это скипаем () ][
                           /*
                           Тут тоже будет скип
                           */
                           / 
                           |
                           &
                           &&
                           ||
                           >>
                           >=
                           <=
                           +-*
                           {[()]}
                           /* А это инвалид                               
                           """;
        Console.WriteLine(testInput);
        TextCursor cursor = new(testInput.AsMemory());
        ILexer lexer = new SimpleLexer(cursor);

        ISyntaxParser parser = new SimpleSyntaxParser(
            new LexemeQueue(
                lexer
            ),
            new SymbolTable(),
            new UebanErrorsHandlingStrategy()
        );

        while (true)
        {
            Lexeme result = lexer.ScanNext();
            if (result.Kind == SyntaxKind.EndToken)
                break;
            //parser.ParseCompilationUnit();
            Console.WriteLine(result);
        }

        Console.WriteLine("Hello, World!");
    }
}