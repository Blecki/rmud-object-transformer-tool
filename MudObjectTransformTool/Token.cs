using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudObjectTransformTool
{
    public enum TokenType
    {
        Token,
        OpenParen,
        CloseParen,
        OpenBracket,
        CloseBracket,
        OpenBrace,
        CloseBrace,
        Whitespace,
        Comment,

        EndOfFile,
    }

    public class Token
    {
        public TokenType Type;
        public String Value;
        public Token Next;

        public static Token Create(TokenType Type, String Value)
        {
            return new Token{ Type = Type, Value = Value };
        }

        public override string ToString()
        {
            return "[" + Type + ": " + Value + "]";
        }
    }
}
