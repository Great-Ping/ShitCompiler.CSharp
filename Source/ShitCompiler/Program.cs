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
                           var abs = abstrd;
                           
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

        while (true)
        {
            ParseResult<Lexeme> result = lexer.ScanNext();
            Console.WriteLine(result);
            if (result.TrySuccess(out var lexeme) && lexeme.Kind == SyntaxKind.EndToken)
                break;
        }

        Console.WriteLine("Hello, World!");
    }
}