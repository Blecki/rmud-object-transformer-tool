using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public partial class Pattern
    {
        /// <summary>
        /// Given a start token, which is the first token after the 'when' or 'do' keyword, find the 
        /// first token after the end of the clause.
        /// </summary>
        /// <param name="Start"></param>
        /// <returns></returns>
        public static Token FindEndOfBodyClause(Token Start)
        {
            var end = Start;

            while (end.Type != TokenType.EndOfFile && end.Type != TokenType.SemiColon && end.Value.ToUpper() != "WHEN" && end.Value.ToUpper() != "DO")
            {
                if (end.Type == TokenType.OpenBrace || end.Type == TokenType.OpenBracket || end.Type == TokenType.OpenParen)
                    end = FindMatchingBracket(end);
                else
                    end = Advance(end, 1);
            }

            return end;
        }

        /// <summary>
        /// Given a token that is a brace, bracket, or paren, find the token after the token
        /// that closes the pair.
        /// </summary>
        /// <param name="Start"></param>
        /// <returns></returns>
        public static Token FindMatchingBracket(Token Start)
        {
            var closeType = TokenType.CloseParen;
            if (Start.Type == TokenType.OpenBrace) closeType = TokenType.CloseBrace;
            else if (Start.Type == TokenType.OpenBracket) closeType = TokenType.CloseBracket;
            else if (Start.Type == TokenType.OpenParen) closeType = TokenType.CloseParen;

            Start = Advance(Start, 1);
            while (Start.Type != TokenType.EndOfFile && Start.Type != closeType)
            {
                if (Start.Type == TokenType.OpenBrace || Start.Type == TokenType.OpenBracket || Start.Type == TokenType.OpenParen)
                    Start = FindMatchingBracket(Start);
                else
                    Start = Advance(Start, 1);
            }
            return Advance(Start, 1);
        }
    }
}
