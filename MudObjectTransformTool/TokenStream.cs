using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudObjectTransformTool
{
	public class TokenStream
	{
        public static Token TokenizeFile(String Data)
        {
            var iterator = new StringIterator(Data);
            var first = Token.Create(TokenType.Whitespace, "");
            var current = first;
            while (current.Type != TokenType.EndOfFile)
            {
                current.Next = ParseToken(iterator);
                current = current.Next;
            }
            return first;
        }

        private static bool IsWhitespace(int c)
        {
            return " \t\r\n".Contains((char)c);
        }
        
		private static Token ParseToken(StringIterator Source)
		{
            if (Source.AtEnd) return Token.Create(TokenType.EndOfFile, "");

            if (Source.PeekCheck("//"))
            {
                var r = "";
                while (!Source.AtEnd && Source.Next != '\n')
                {
                    r += (char)Source.Next;
                    Source.Advance();
                }
                return Token.Create(TokenType.Comment, r);
            }
            else if (IsWhitespace(Source.Next))
            {
                var r = "";
                while (!Source.AtEnd && IsWhitespace(Source.Next))
                {
                    r += (char)Source.Next;
                    Source.Advance();
                }
                return Token.Create(TokenType.Whitespace, r);
            }
            else if (Source.Next == '(') { Source.Advance(); return Token.Create(TokenType.OpenParen, "("); }
			else if (Source.Next == ')') { Source.Advance(); return Token.Create(TokenType.CloseParen, ")"); }
			else if (Source.Next == '[') { Source.Advance(); return Token.Create(TokenType.OpenBracket, "["); }
			else if (Source.Next == ']') { Source.Advance(); return Token.Create(TokenType.CloseBracket, "]"); }
			else if (Source.Next == '{') { Source.Advance(); return Token.Create(TokenType.OpenBrace, "{"); }
			else if (Source.Next == '}') { Source.Advance(); return Token.Create(TokenType.CloseBrace, "}"); }
            else if (Source.Next == ';') { Source.Advance(); return Token.Create(TokenType.SemiColon, ";"); }
			else if (Source.Next == '\"')
            {
                Source.Advance();
                var literal = TokenizeStringLiteral(Source);
                Source.Advance();
                return Token.Create(TokenType.Token, "\"" + literal + "\"");
            }
            else
            {
                var token = "";
                while (!Source.AtEnd && !"[](){} \t\r\n\"".Contains((char)Source.Next))
                {
                    token += (char)Source.Next;
                    Source.Advance();
                }
                return Token.Create(TokenType.Token, token);
            }
		}

		private static String TokenizeStringLiteral(StringIterator Source)
		{
			var literal = "";
			while (!Source.AtEnd && Source.Next != '\"')
			{
				if (Source.Next == '\\')
				{
                    literal += "\\";
					Source.Advance();
                    if (Source.AtEnd) return literal;
                    literal += (char)Source.Next;
				}
				else
				{
                    literal += (char)Source.Next;
                    Source.Advance();
				}
			}
			return literal;
		}
	}
}
