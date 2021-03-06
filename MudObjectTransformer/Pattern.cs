﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public partial class Pattern
    {
        public virtual PatternMatch Match(Token Start)
        {
            throw new NotImplementedException();
        }

        public static Token Advance(Token Start, int Count)
        {
            for (int i = 0; i < Count; ++i)
                if (Start != null) Start = Start.Next;
            return Start;
        }

        public static Token AdvanceAndSkipWhitespace(Token Start, int Count)
        {
            Start = Advance(Start, Count);
            while (Start.Type == TokenType.Whitespace)
                Start = Start.Next;
            return Start;
        }

        public static Token InsertAfter(Token Before, Token What)
        {
            if (Before != null)
            {
                What.Next = Before.Next;
                Before.Next = What;
            }

            if (What.Next != null)
                What.Next.Previous = What;

            What.Previous = Before;

            return What;
        }

        public static void Cut(Token Start, Token OnePastEnd)
        {
            if (Start.Previous != null)
                Start.Previous.Next = OnePastEnd;
            if (OnePastEnd != null)
                OnePastEnd.Previous = Start.Previous;
        }

        public static Token Replace(Token Start, Token OnePastEnd, Token With)
        {
            if (Start.Previous != null)
                Start.Previous.Next = With;
            if (OnePastEnd != null) OnePastEnd.Previous = With;
            With.Next = OnePastEnd;
            With.Previous = Start.Previous;
            return OnePastEnd;
        }

        public static IEnumerable<Token> EnumerateTokens(Token Start, Token End)
        {
            while (Start != End)
            {
                yield return Start;
                Start = Start.Next;
            }
        }

        private static List<Pattern> Patterns = null;

        public static void DiscoverPatterns()
        {
            Patterns = new List<Pattern>();
            try
            {
                foreach (var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
                    if (type.IsSubclassOf(typeof(Pattern)))
                        Patterns.Add(Activator.CreateInstance(type) as Pattern);
            } catch (System.Reflection.ReflectionTypeLoadException e)
            {
                Console.WriteLine(e.Message);
                for (int i = 0; i < e.Types.Length; ++i)
                    Console.WriteLine(
                        (e.Types[i] == null ? "??" : e.Types[i].Name) + " : " + (e.LoaderExceptions[i] == null ? "Loaded." : e.LoaderExceptions[i].Message));
                throw;
            }
        }

        public static String ProcessFile(String Data)
        {
            if (Patterns == null) DiscoverPatterns();

            var rootToken = TokenStream.TokenizeFile(Data);
            var current = rootToken;
            
            while (current.Type != TokenType.EndOfFile)
            {
                if (current.Type == TokenType.GeneratedBlock)
                {
                    current = current.Next;
                    continue;
                }

                bool patternMatched = false;
                foreach (var pattern in Patterns)
                {
                    var matchResult = pattern.Match(current);
                    if (matchResult.Matched)
                    {
                        patternMatched = true;
                        current = matchResult.NextToken;
                        break;
                    }
                }
                if (!patternMatched)
                    current = current.Next;
            }

            var builder = new StringBuilder();
            for (var start = rootToken; start != null; start = start.Next)
                builder.Append(start.Value);

            return builder.ToString();
        }
    }
}
