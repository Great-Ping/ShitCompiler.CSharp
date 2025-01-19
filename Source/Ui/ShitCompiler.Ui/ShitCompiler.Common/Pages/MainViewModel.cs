using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.Errors;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using ShitCompiler.Widgets;

namespace ShitCompiler.Pages;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] 
    private string _codeInput = String.Empty;
    
    [ObservableProperty]
    private string _infoOutput = String.Empty;

    public SyntaxTreeModel SyntaxTree { get; } = new();

    partial void OnCodeInputChanged(string value)
    { 
        SimpleLexer lexer = new(
            new TextCursor(value.AsMemory())
        );
        LexemeQueue lexems = new(lexer);
        List<ParseError> errors = new();
        
        SimpleSyntaxParser parser = new SimpleSyntaxParser(
            lexems, 
            new SymbolTable(), 
            new AccumulatingErrorsHandlingStrategy(errors)
        );
        
        SyntaxTree.Root = SyntaxTreeNode.FromSyntaxNode(
            parser.ParseCompilationUnit()
        );
        
        InfoOutput = String.Join(
            "\n", 
            errors.Select(
                x=>$"({x.Location.LineIndex}, {x.Location.SymbolIndex}): {x.Message}"
            )
        );
    }
}