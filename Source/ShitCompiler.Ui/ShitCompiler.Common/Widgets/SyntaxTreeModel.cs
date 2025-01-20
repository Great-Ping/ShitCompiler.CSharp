using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using ShitCompiler.Pages;

namespace ShitCompiler.Widgets;

public partial class SyntaxTreeModel(SyntaxTreeNode root): ViewModelBase
{
    
    public SyntaxTreeNode Root
    {
        get => Nodes.First();
        set
        {
            Nodes.Clear();
            Nodes.Add(value);
        }
    }

    public ObservableCollection<SyntaxTreeNode> Nodes { get; } = [root];

    public SyntaxTreeModel()
        : this(SyntaxTreeNode.Empty)
    { }
}