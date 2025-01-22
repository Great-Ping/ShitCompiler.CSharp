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


        Dictionary<SyntaxNode, TypeInfo> _dataTypes = new();

        Dictionary<string, FunctionSemantic> mFunctions = new();

        TypeInfo _currentReturnDataType = DataType.Unknown;
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
                case ArrayExpressionSyntax arrayExpression:
                    HandleArrayExpression(arrayExpression);
                    break;
                case IndexExpressionSyntax indexExpression:
                    HandleIndexExpression(indexExpression);
                    break;
                case ArrayAssigmentExpressionSyntax arrayAssigmentExpression:
                    HandleArrayAssigment(arrayAssigmentExpression);
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

        private void HandleIndexExpression(IndexExpressionSyntax indexExpression)
        {
            HandleIdentifier(indexExpression.Identifier);
            
            TypeInfo type = _dataTypes.GetValueOrDefault(indexExpression.Identifier, DataType.Unknown);
            if (type.ArraySize.FirstOrDefault() <= indexExpression.Number.Value)
                errorsHandler.Handle(new SemanticError(indexExpression.Start, "Index out of range"));

            _dataTypes[indexExpression] = type.Type &(~DataType.Array);
        }

        private void HandleArrayAssigment(ArrayAssigmentExpressionSyntax arrayAssigment)
        {
            HandleIdentifier(arrayAssigment.Identifier);
            HandleSyntaxNode(arrayAssigment.Right);
            
            TypeInfo type = _dataTypes.GetValueOrDefault(arrayAssigment.Identifier, DataType.Unknown);
            if (type.ArraySize.FirstOrDefault() <= arrayAssigment.Number.Value)
                errorsHandler.Handle(new SemanticError(arrayAssigment.Start, "Index out of range"));
            
            //Костыли, как мы любим
            _dataTypes[arrayAssigment.Identifier] = type.Type & (~DataType.Array);
            PromoteType(arrayAssigment, arrayAssigment.Identifier, arrayAssigment.Right);
        }

        private void HandleArrayExpression(ArrayExpressionSyntax arrayExpression)
        {
            foreach (var expression in arrayExpression.Expressions)
                HandleSyntaxNode(expression);

            var types = arrayExpression.Expressions
                .Select(x => _dataTypes.GetValueOrDefault(x, DataType.Unknown).Type)
                .ToHashSet();
            
            if (types.Count == 1)
            {
                _dataTypes.Add(
                    arrayExpression,
                    new TypeInfo(types.First() | DataType.Array, [arrayExpression.Expressions.Count()])
                );
                return;    
            }
            
            _dataTypes.Add(arrayExpression, DataType.Unknown);
            errorsHandler.Handle(
                new SemanticError(
                    arrayExpression.Start,
                    $"Heterogeneous type of array elements"
                )
            );
        }

        private void HandleCallExpression(CallExpressionSyntax call)
        {
            CheckIdentifierDeclaration(call, call.Identifier);

            TypeInfo s = _dataTypes[call];
            if (s.Type == DataType.Unknown) {
                return;
            }

            FunctionSemantic sem = mFunctions[call.Identifier.OriginalValue];

            if (sem.Function.Parameters.Count < call.Arguments.Count) {
                errorsHandler.Handle(
                    new SemanticError(
                        call.Start,
                        $"A lot of parameters for function '{sem.Function.Identifier.OriginalValue}'({sem.Function.Parameters.Count})"
                    )
                );
                return;
            }

            if (sem.Function.Parameters.Count > call.Arguments.Count) {
                errorsHandler.Handle(
                    new SemanticError(
                        call.Start,
                        $"Not enough parameters for function '{sem.Function.Identifier.OriginalValue}'({sem.Function.Parameters.Count})"
                    )
                );
                return;
            }

            int i = 0;
            foreach (ExpressionSyntax arg in call.Arguments) {
                HandleSyntaxNode(arg);
                TypeInfo argType = _dataTypes[arg];
                TypeInfo funcArgType = sem.argTypes[i];
                if (argType != funcArgType) {
                    errorsHandler.Handle(
                        new SemanticError(
                            call.Start,
                            $"Parameter type error for called function '{sem.Function.Identifier.OriginalValue}: Waited: {funcArgType}; Found: {argType}"
                        )
                    );
                }

                i++;
            }
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
            TypeInfo conditionType = _dataTypes.GetValueOrDefault(condition, DataType.Unknown);

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

        private void HandleAssigmentExpresssion(AssignmentExpressionSyntax assignment)
        {
            Lexeme leftId = assignment.Identifier;
            HandleIdentifier(leftId);
            HandleSyntaxNode(assignment.Right);
            PromoteType(assignment, assignment.Identifier, assignment.Right);
        }

        private void HandleIdentifier(Lexeme identifier) 
        {
            Symbol? symbol = _symbolTable.Find(identifier);
            if (symbol != null) {
                _dataTypes.Add(
                    identifier,
                    new TypeInfo(symbol.DataType, symbol.ArraySize)
                );
                return;
            }
            
            errorsHandler.Handle(
                new SemanticError(
                    identifier.Start,
                    $"AssignmentExpression: No data type for ID - {identifier.OriginalValue}"
                )
            );
            
        }

        private void HandleVariable(VariableDeclarationSyntax variable)
        {
            Declarate(variable.Identifier, variable.TypeClause);
            HandleSyntaxNode(variable.Initializer);
            PromoteType(variable, variable.Identifier, variable.Initializer);
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
            _dataTypes.Add(identifier, type);
            _symbolTable.AddSymbol(
                new Symbol(
                    identifier,
                    type,
                    isFunk
                )
            );
        }

        private void PromoteType(SyntaxNode parent, SyntaxNode left, SyntaxNode right)
        {
            TypeInfo leftType = _dataTypes.GetValueOrDefault(left, DataType.Unknown);
            TypeInfo rightType = _dataTypes.GetValueOrDefault(right, DataType.Unknown);

            if (leftType.Equals(rightType)) {
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

            errorsHandler.Handle(
                new SemanticError(
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
                        MatchTypes(array.Identifier) | DataType.Array, 
                        [Convert.ToInt32(array.ArraySizeNumber.Value)]
                    );
                case IdentifierTypeSyntax identifier:
                    return (
                        MatchTypes(identifier.Identifier), 
                        []
                    );
                default:
                    throw new InvalidOperationException();
            }
        }

        private void HandleFunctionDeclaration(FunctionDeclarationSyntax funk)
        {
            Declarate(funk.Identifier, funk.TypeClause, true);

            _symbolTable.CreateNewSymbolBlock();

            List<TypeInfo> argParams = new();

            foreach (ParameterSyntax param in funk.Parameters) 
            {
                Declarate(param.Identifier, param.TypeClause);
                TypeInfo type = _dataTypes[param.Identifier];
                argParams.Add(type);
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

            mFunctions.Add(
                funk.Identifier.OriginalValue,
                new FunctionSemantic(
                    funcType.DataType,
                    funk,
                    argParams
                )
            );

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
            if (_currentReturnDataType.Type == DataType.Unit) {
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

            TypeInfo retType = _dataTypes[ret.Expression];
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