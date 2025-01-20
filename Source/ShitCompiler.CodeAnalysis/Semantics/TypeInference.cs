using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Syntax;

public class TypeInference(ISyntaxErrorsHandler errrorHandler)
{
    public const string DataTypeKey = "DataType";
    public const string ItemTypeKey = "ItemType";
    public const string ArraySizeKey = "ArraySize";
    
    public void Handle(SyntaxNode node)
    {
        switch(node)
        {
            case LiteralExpressionSyntax literal:
                HandleLiteralExpression(literal);
                break;
            case ArrayExpressionSyntax array:
                HandleArrayExpression(array);
                break;
            case BinaryExpressionSyntax binaryExpression:
                HandleBinatyExpression(binaryExpression);
                break;
            case FunctionDeclarationSyntax ifStatement:
                HandleFunctionDeclaration(ifStatement);
                break;
            case VariableDeclarationSyntax variable:
                HandleVariable(variable);
                break;
            case AssignmentExpressionSyntax assignmentExpression:
                HandleAssigmentExpresssion(assignmentExpression);
                break;
            case IfStatementSyntax ifStatement:
                HandleIfStatement(ifStatement);
                break;
        }
    }

    private void HandleAssigmentExpresssion(AssignmentExpressionSyntax assignmentExpression)
    {
        throw new NotImplementedException();
    }

    private void HandleIfStatement(IfStatementSyntax ifStatement)
    {
        throw new NotImplementedException();
    }

    private void HandleVariable(VariableDeclarationSyntax variable)
    {
        throw new NotImplementedException();
    }

    private void HandleArrayExpression(ArrayExpressionSyntax array)
    {
        throw new NotImplementedException();
    }

    private void HandleFunctionDeclaration(FunctionDeclarationSyntax ifStatement)
    {
        throw new NotImplementedException();
    }

    private void HandleBinatyExpression(BinaryExpressionSyntax binaryExpression)
    {
        throw new NotImplementedException();
    }

    private void HandleLiteralExpression(LiteralExpressionSyntax literal)
    {
        switch (literal)
        {
            case LiteralExpressionSyntax<long>:
                literal.Metadata[DataTypeKey] = DataType.Long;
                break;
            case LiteralExpressionSyntax<double>:
                literal.Metadata[DataTypeKey] = DataType.Double;
                break;
            case LiteralExpressionSyntax<Char>:
                literal.Metadata[DataTypeKey] = DataType.Char;
                break;
            case LiteralExpressionSyntax<String>:
                literal.Metadata[DataTypeKey] = DataType.String;
                break;
            case LiteralExpressionSyntax<bool>:
                literal.Metadata[DataTypeKey] = DataType.Boolean;
                break;
            default:
                literal.Metadata[DataTypeKey] = DataType.Unknown;
                break;
        }
    }
}