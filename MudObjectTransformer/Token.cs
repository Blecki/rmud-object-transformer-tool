using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudObjectTransformer
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
        SemiColon,
        Whitespace,
        GeneratedBlock,
        Comment,

        EndOfFile,
    }

    public class Token
    {
        public TokenType Type;
        public String Value;
        public Token Previous;
        public Token Next;
        public Object Tag;

        public static Token Create(TokenType Type, String Value, Object Tag = null)
        {
            return new Token{ Type = Type, Value = Value, Tag = Tag };
        }

        public override string ToString()
        {
            return "[" + Type + ": " + Value + "]";
        }
    }
}
