namespace ShitCompiler.CodeAnalysis.Syntax;
public enum SyntaxKind
{
    /// <summary>
    /// Invalid Token
    /// </summary>
    BadToken,
    /// <summary>
    /// End of file
    /// </summary>
    EndToken,
    /// <summary>
    /// Some identifier
    /// </summary>
    IdentifierToken,
    /// <summary>
    /// Some string
    /// </summary>
    StringLiteral,
    /// <summary>
    /// Some Number 12312
    /// </summary>
    NumberToken,
    /// <summary>
    /// Some Real Number 0.000
    /// </summary>
    RealNumberToken,
    /// <summary>
    /// Some Character
    /// </summary>
    CharacterLiteral,

    /// <summary>
    /// if
    /// </summary>
    IfKeyword,
    /// <summary>
    /// else
    /// </summary>
    ElseKeyword,
    /// <summary>
    /// funk
    /// </summary>
    FunkKeyword,
    /// <summary>
    /// return
    /// </summary>
    ReturnKeyword,
    /// <summary>
    /// var
    /// </summary>
    VarKeyword,
    /// <summary>
    /// val
    /// </summary>
    ValKeyword,
    FalseKeyword,
    TrueKeyword,

    /// <summary>
    /// :
    /// </summary>
    ColonToken,
    /// <summary>
    /// ;
    /// </summary>
    SemicolonToken,

    /// <summary>
    /// ,
    /// </summary>
    CommaToken,
    /// <summary>
    /// .
    /// </summary>
    DotToken,

    /// <summary>
    /// =
    /// </summary>
    EqualsToken,
    /// <summary>
    /// +
    /// </summary>
    PlusToken,
    /// <summary>
    /// -
    /// </summary>
    MinusToken,
    /// <summary>
    /// *
    /// </summary>
    AsteriskToken,
    /// <summary>
    /// /
    /// </summary>
    SlashToken,

    /// <summary>
    /// &&
    /// </summary>
    AmpersandAmpersandToken,
    /// <summary>
    /// ||
    /// </summary>
    BarBarToken,
    /// <summary>
    /// !=
    /// </summary>
    ExclamationEqualsToken,
    /// <summary>
    /// ==
    /// </summary>
    EqualsEqualsToken,
    /// <summary>
    /// >
    /// </summary>
    GreaterThanToken,
    /// <summary>
    /// <
    /// </summary>
    LessThanToken,
    /// <summary>
    /// >=
    /// </summary>
    GreaterThanEqualsToken,
    /// <summary>
    /// <=
    /// </summary>
    LessThanEqualsToken,
    
    /// <summary>
    /// (
    /// </summary>
    OpenParenthesisToken,
    /// <summary>
    /// )
    /// </summary>
    CloseParenthesisToken,
    /// <summary>
    /// {
    /// </summary>
    OpenBraceToken,
    /// <summary>
    /// }
    /// </summary>
    CloseBraceToken,
    /// <summary>
    /// [
    /// </summary>
    OpenBracketToken,
    /// <summary>
    /// ]
    /// </summary>
    CloseBracketToken,
    CommentTrivia,
    CompilationUnitSyntax,
    Directive,
    GlobalStatement,
    ExpressionStatement,
    FunctionDeclaration,
    BlockStatement,
    VariableDeclaration,
    TypeClause,
    AssignmentExpression,
}