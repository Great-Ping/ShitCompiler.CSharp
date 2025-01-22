using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using ShitCompiler.CodeAnalysis.Lexicon;
using ShitCompiler.CodeAnalysis.Syntax;
using ShitCompiler.CodeAnalysis.Syntax.SyntaxNodes;

using ShitCompiler.CodeAnalysis;
using ShitCompiler.CodeAnalysis.Semantics.Errors;
using ShitCompiler.CodeAnalysis.Syntax.ErrorHandlers;

namespace ShitCompiler.CodeAnalysis.Semantics
{
    public class SemanticParser(ISyntaxErrorsHandler errorsHandler)
    {
        SymbolTable _symbolTable = new();
        ISyntaxErrorsHandler errorsHandler = errorsHandler;
        Dictionary<SyntaxNode, DataType> _dataTypes = new();

        DataType _currentReturnDataType = DataType.Unknown;
        FunctionDeclarationSyntax _currentFunction;

        bool _hasFunctionReturn = false;

        public void Parse(CompilationUnitSyntax compilationUnit, bool createScopeInBlock=true) 
        {
            _symbolTable.ResetBlock();
            HandleSyntaxNode(compilationUnit, createScopeInBlock);
        }
        private void HandleSyntaxNodes(params IEnumerable<ISyntaxNode> nodes)
        {
            foreach(ISyntaxNode node in nodes)
                HandleSyntaxNode(node);
        }

        private void HandleSyntaxNode(ISyntaxNode node, bool createScopeInBlock=true)
        {
            switch (node)
            {
                case BinaryExpressionSyntax binaryExpression:
                    HandleBinaryExpression(binaryExpression);
                    break;
                case LiteralExpressionSyntax literal:
                    HandleLiteralExpression(literal);
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
                case BlockStatementSyntax block:
                    HandleBlockStatement(block, createScope: createScopeInBlock);
                    break;
                case NameExpressionSyntax name:
                    HandleNameExpression(name);
                    break;
                case CallExpressionSyntax fuctionCall:
                    HandleCallExpression(fuctionCall);
                    break;
                case ReturnStatementSyntax returnState:
                    HandleReturnExpression(returnState);
                    break;
                default:
                    HandleSyntaxNodes(node.GetChildren());
                    break;
            };
        }

        private void HandleCallExpression(CallExpressionSyntax fuctionCall)
        {
            CheckIdentifierDeclaration(fuctionCall, fuctionCall.Identifier);
        }

        private void HandleIfStatement(IfStatementSyntax ifStatement)
        {
            _symbolTable.CreateNewSymbolBlock();
            HandleIfStatementCondition(ifStatement.Condition);
            HandleSyntaxNode(ifStatement.ThenStatement, false);
            _symbolTable.DismissBlock();

            if (ifStatement.ElseClause == null)
                return;

            _symbolTable.CreateNewSymbolBlock();
            HandleSyntaxNode(ifStatement.ElseClause.ElseStatement, false);
            _symbolTable.DismissBlock();
        }

        private void HandleIfStatementCondition(ExpressionSyntax condition)
        {
            HandleSyntaxNode(condition);
            DataType conditionType = _dataTypes.GetValueOrDefault(condition, DataType.Unknown);

            if (conditionType != DataType.Boolean){
                errorsHandler.Handle(
                    new SemanticError(
                        condition.Start,
                        "Waited boolean expression."
                    )
                );
            }
        }

        private void HandleBlockStatement(BlockStatementSyntax block, bool createScope = false)
        {
            if (!createScope)
            {
                HandleSyntaxNodes(block.GetChildren());
                return;
            }

            _symbolTable.CreateNewSymbolBlock();
            HandleSyntaxNodes(block.GetChildren());
            _symbolTable.DismissBlock();
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

        private void HandleVariable(VariableDeclarationSyntax variable)
        {
            Declarate(variable.Identifier, variable.TypeClause);
            HandleSyntaxNode(variable.Initializer);
            PromoteType(variable, variable.Identifier, variable.Initializer);
            ///TODO TYPE MATCHING
        }

        //Обрабатывает объявление какого либо идентификатора в текущем Scope
        private void Declarate(Lexeme identifier, TypeClauseSyntax typeClause, bool isFunk=false)
        {
            Symbol? symbol = _symbolTable.FindInBlock(identifier);
            if (symbol != null) 
            {
                errorsHandler.Handle(
                    new SemanticError(
                        identifier.Start,
                        $"The symbol variable has already been declared in this scope {identifier.OriginalValue}"
                    )
                );
                return;
            }

            var type = ParseType(typeClause.Type);
            _symbolTable.AddSymbol(
                new Symbol(
                    identifier,
                    type,
                    isFunk
                )
            );
            _dataTypes.Add(identifier, type.Type);
        }

        private void PromoteType(SyntaxNode parent, SyntaxNode left, SyntaxNode right)
        {
            DataType leftType = _dataTypes.GetValueOrDefault(left, DataType.Unknown);
            DataType rightType = _dataTypes.GetValueOrDefault(right, DataType.Unknown);

            if (leftType == rightType) {
                _dataTypes.Add(parent, leftType);
                return;
            }
            _dataTypes.Add(parent,  DataType.Unknown);

            errorsHandler.Handle(
                new SemanticError(
                    parent.Start,
                    $"Type mismatch. Left type - {leftType}. Right type - {rightType}"
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

        private void HandleFunctionDeclaration(FunctionDeclarationSyntax funk)
        {
            Declarate(funk.Identifier, funk.TypeClause, true);

            _symbolTable.CreateNewSymbolBlock();

            foreach (ParameterSyntax param in funk.Parameters) 
            {
                Declarate(param.Identifier, param.TypeClause);
            }

            Symbol? funcType = _symbolTable.Find(
                funk.Identifier
            );

            if (funcType == null) {
                errorsHandler.Handle(
                    new SemanticError(
                        funk.Identifier.Start,
                        $"No function data type - {funk.Identifier.OriginalValue}"
                    )
                );
                return;
            }

            _currentFunction = funk;
            _currentReturnDataType = funcType.DataType;
            _hasFunctionReturn = false;
            HandleSyntaxNode(funk.Block, false);

            if (!_hasFunctionReturn && funcType.DataType != DataType.Unit) {
                errorsHandler.Handle(
                    new SemanticError(
                        funk.Identifier.Start,
                        $"No function return statement for {funk.Identifier.OriginalValue}"
                    )
                );
            }

            _symbolTable.DismissBlock();
        }
        
        private void HandleBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            HandleSyntaxNode(
                binaryExpression.Left
            );

            HandleSyntaxNode(
                binaryExpression.Right
            );

            PromoteType(binaryExpression, binaryExpression.Left, binaryExpression.Right);           
        }

        private void HandleNameExpression(
            NameExpressionSyntax name
        ) {
            CheckIdentifierDeclaration(name, name.Identifier);
        }

        private void CheckIdentifierDeclaration(SyntaxNode parent, Lexeme identifier)
        {
            Symbol? found = _symbolTable.Find(
                identifier
            );

            if (found != null){
                _dataTypes.Add(parent, found.DataType);
                return;
            }

            _dataTypes.Add(parent, DataType.Unknown);
            errorsHandler.Handle(
                new SemanticError(
                    parent.Start,
                    $"Identifier not found {identifier.OriginalValue}."
                )
            );
        }

        private void HandleLiteralExpression(LiteralExpressionSyntax literal)
        {
            _dataTypes.Add(
                literal,
                literal.Type
            );
        }

        private void HandleReturnExpression(
            ReturnStatementSyntax ret
        ) {
            _hasFunctionReturn = true;
            if (_currentReturnDataType == DataType.Unit) {
                return;
            }

            if (ret.Expression == null) {
                errorsHandler.Handle(
                    new SemanticError(
                        _currentFunction.Identifier.Start,
                        $"Function {_currentFunction.Identifier.OriginalValue} should return value with data type {_currentReturnDataType}"
                    )
                );
                return;
            }

            HandleSyntaxNode(
                ret.Expression,
                false
            );

            DataType retType = _dataTypes[ret.Expression];
            if (retType == _currentReturnDataType) {
                return;
            }

            errorsHandler.Handle(
                new SemanticError(
                    _currentFunction.Identifier.Start,
                    $"Function: {_currentFunction.Identifier.OriginalValue} has invalid return value: {retType}. Waited: {_currentReturnDataType}"
                )
            );

        }

    }
}