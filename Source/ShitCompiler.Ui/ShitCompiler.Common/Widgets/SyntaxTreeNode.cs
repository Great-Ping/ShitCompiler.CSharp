using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.Widgets;

public class SyntaxTreeNode(
    string nodeTitle,
    IEnumerable<SyntaxTreeNode> children
)
{
    public string NodeTitle { get; } = nodeTitle;
    public ObservableCollection<SyntaxTreeNode> Children { get; } = new(children);

    public static SyntaxTreeNode Empty => new(
        String.Empty,
        []
    );

    public static SyntaxTreeNode FromSyntaxNode(ISyntaxNode node)
    {
        return new SyntaxTreeNode(
            node.Kind.ToString(),
            node.GetChildren().Select(x => FromSyntaxNode(x))
        );
    }
}