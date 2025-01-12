namespace ShitCompiler.Lexicon;
public enum LexemeKind
{
    Unknown,
    /// <summary>
    /// Invalid Token
    /// </summary>
    InvalidToken,
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
    /// var
    /// </summary>
    VarKeyword,
    /// <summary>
    /// val
    /// </summary>
    ValKeyword,

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
    OpenParenToken,
    /// <summary>
    /// )
    /// </summary>
    CloseParenToken,
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
}