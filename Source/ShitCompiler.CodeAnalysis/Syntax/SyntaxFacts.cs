namespace ShitCompiler.CodeAnalysis.Syntax;


    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 6;

                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 4;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 3;

                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                    return 2;

                case SyntaxKind.AmpersandAmpersandToken:
                    return 1;

                default:
                    return 0;
            }
        }

        public static bool IsComment(this SyntaxKind kind)
        {
            return kind == SyntaxKind.CommentTrivia;
        }

        public static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "else":
                    return SyntaxKind.ElseKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "funk":
                    return SyntaxKind.FunkKeyword;
                case "if":
                    return SyntaxKind.IfKeyword;
                case "val":
                    return SyntaxKind.ValKeyword;
                case "return":
                    return SyntaxKind.ReturnKeyword;
                case "var":
                    return SyntaxKind.VarKeyword;
                
                case "string":
                    return SyntaxKind.StringType;
                case "double":
                    return SyntaxKind.DoubleType;
                case "long":
                    return SyntaxKind.LongType;
                case "char":
                    return SyntaxKind.CharacterType;
                case "bool":
                    return SyntaxKind.BooleanType;
                
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static bool IsType(this SyntaxKind kind)
        {
            return kind.ToString().EndsWith("Type");
        }

        
        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
        {
            var kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
            {
                if (GetUnaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
        {
            var kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
            {
                if (GetBinaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static string GetText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return "+";
                case SyntaxKind.MinusToken:
                    return "-";
                case SyntaxKind.StarToken:
                    return "*";
                case SyntaxKind.SlashToken:
                    return "/";
                case SyntaxKind.EqualsToken:
                    return "=";
                case SyntaxKind.LessThanToken:
                    return "<";
                case SyntaxKind.LessThanEqualsToken:
                    return "<=";
                case SyntaxKind.GreaterThanToken:
                    return ">";
                case SyntaxKind.GreaterThanEqualsToken:
                    return ">=";
                case SyntaxKind.AmpersandAmpersandToken:
                    return "&&";
                case SyntaxKind.BarBarToken:
                    return "||";
                case SyntaxKind.EqualsEqualsToken:
                    return "==";
                case SyntaxKind.OpenParenthesisToken:
                    return "(";
                case SyntaxKind.CloseParenthesisToken:
                    return ")";
                case SyntaxKind.OpenBraceToken:
                    return "{";
                case SyntaxKind.CloseBraceToken:
                    return "}";
                case SyntaxKind.ColonToken:
                    return ":";
                case SyntaxKind.CommaToken:
                    return ",";
                case SyntaxKind.ElseKeyword:
                    return "else";
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.FunkKeyword:
                    return "funk";
                case SyntaxKind.IfKeyword:
                    return "if";
                case SyntaxKind.ValKeyword:
                    return "val";
                case SyntaxKind.ReturnKeyword:
                    return "return";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.VarKeyword:
                    return "var";
                default:
                    return String.Empty;
            }
        }
        
        public static bool IsKeyword(this SyntaxKind kind)
        {
            return kind.ToString().EndsWith("Keyword");
        }

        public static bool IsToken(this SyntaxKind kind)
        {
            return (kind.IsKeyword() || kind.ToString().EndsWith("Token"));
        }
    }