using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace ShitCompiler.CodeAnalysis.Semantics
{
    public class SemanticsParser(ISyntaxErrorsHandler errorsHandler)
    {
        SymbolTable _symbolTable = new();
        ISyntaxErrorsHandler errorsHandler = errorsHandler;
        Dictionary<SyntaxNode, DataType> _dataTypes = new();


        public void Parse(CompilationUnitSyntax compilationUnit) 
        {
            _symbolTable.ResetBlock();
            ParsePrivate(compilationUnit);
        }

        private void ParsePrivate(SyntaxNode node) 
        {
            bool blockIsCreated = TryCreateSymbolBlock(node);
            Handle(node);

            foreach (SyntaxNode child in node.GetChildren()) 
            {
                ParsePrivate(child);
            }

            if (blockIsCreated)
                _symbolTable.DismissBlock();
        }

        private bool TryCreateSymbolBlock(SyntaxNode node) 
        {
            switch (node)
            {
                case BlockStatementSyntax:
                case FunctionDeclarationSyntax:
                    _symbolTable.CreateNewSymbolBlock();
                    return true;
            }

            return false;
        }

        private void Handle(SyntaxNode node)
        {
            switch (node)
            {
                case LiteralExpressionSyntax literal:
                    HandleLiteralExpression(literal);
                    break;
                case ArrayExpressionSyntax array:
                    HandleArrayExpression(array);
                    break;
                case BinaryExpressionSyntax binaryExpression:
                    HandleBinaryExpression(binaryExpression);
                    break;
                case FunctionDeclarationSyntax functionDeclaration:
                    HandleFunctionDeclaration(functionDeclaration);
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
            Lexeme leftId = assignmentExpression.Identifier;
            HandleIdentifier(leftId);
        }

        private void HandleIdentifier(Lexeme identifier) 
        {
            Symbol? symbol = _symbolTable.Find(identifier);
            if (symbol == null) {
                errorsHandler.Handle(
                    new SemanticError(
                        identifier.Start,
                        $"AssignmentExpression: No data type for ID - {identifier.OriginalValue}"
                    )
                );
                return;
            }
        }

        private void HandleIfStatement(IfStatementSyntax ifStatement)
        {
            // throw new NotImplementedException();
        }

        private void HandleVariable(VariableDeclarationSyntax variable)
        {
            HandleDeclaration(variable.Identifier, variable.TypeClause);
        }

        //Обрабатывает объявление какого либо идентификатора в текущем Scope
        private void HandleDeclaration(Lexeme identifier, TypeClauseSyntax typeClause, bool isFunk=false)
        {
            Symbol? symbol = _symbolTable.FindInBlock(identifier);
            if (symbol == null) 
            {
                _symbolTable.AddSymbol(
                    new Symbol(
                        identifier,
                        ParseType(typeClause.Type),
                        isFunk
                    )
                );
                return;
            }

            errorsHandler.Handle(
                new SemanticError(
                    identifier.Start,
                    $"The symbol variable has already been declared in this scope {identifier.OriginalValue}"
                )
            );
        }

        private DataType MatchTypes(Lexeme typeIdentifier)
        {
            DataType type = typeIdentifier.OriginalValue switch
            {
                "long" => DataType.Long,
                "double" => DataType.Double,
                "char" => DataType.Char,
                "bool" => DataType.Boolean,
                "string" => DataType.String,
                "unit" => DataType.Unit,
                _ => DataType.Unknown
            };

            if (type != DataType.Unknown)
                return type;

            errorsHandler.Handle(new SemanticError(
                typeIdentifier.Start,
                $"Unknown data type {typeIdentifier.OriginalValue}"
            ));

            return type;
        }

        private (DataType Type, int[] ArraySize) ParseType(TypeSyntax type)
        {
            switch (type) 
            {
                case ArrayTypeSyntax array:
                    return (
                        MatchTypes(array.Identifier), 
                        [Convert.ToInt32(array.ArraySizeNumber.Value)]
                    );
                case IdentifierTypeSyntax identifier:
                    return (
                        MatchTypes(identifier.Identifier), 
                        [0]
                    );
                default:
                    throw new InvalidOperationException();
            }
        }

        private void HandleArrayExpression(ArrayExpressionSyntax array)
        {
            // throw new NotImplementedException();
        }

        private void HandleFunctionDeclaration(FunctionDeclarationSyntax funk)
        {
            HandleDeclaration(funk.Identifier, funk.TypeClause, true);

            foreach (ParameterSyntax param in funk.Parameters) {
                HandleDeclaration(param.Identifier, param.TypeClause);
                _symbolTable.AddSymbol(
                    new Symbol(
                        param.Identifier,
                        ParseType(param.TypeClause.Type)
                    )
                );
            }
        }

        private void HandleBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            // throw new NotImplementedException();
        }

        private void HandleLiteralExpression(LiteralExpressionSyntax literal)
        {

        }

    }
}
