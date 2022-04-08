using System;
using System.Collections.Generic;

namespace Chroma.Commander
{
    internal class Tokenizer
    {
        public static string[] Tokenize(string input)
        {
            var ret = new List<string>();
            var currentToken = string.Empty;

            for (var i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '\\':
                    {
                        if (i + 1 >= input.Length)
                        {
                            currentToken += '\\';
                        }
                        else
                        {
                            i++;
                            currentToken += input[i];
                        }

                        break;
                    }

                    case '"':
                        if (i + 1 >= input.Length)
                        {
                            currentToken += '"';
                        }
                        else
                        {
                            i++;

                            while (input[i] != '"')
                            {
                                if (i + 1 >= input.Length)
                                    throw new FormatException("Unterminated string.");

                                if (input[i] == '\\')
                                {
                                    if (i + 1 >= input.Length)
                                        throw new FormatException("Unterminated string.");

                                    i++;
                                    switch (input[i])
                                    {
                                        case '\\':
                                            currentToken += '\\';
                                            i++;
                                            break;

                                        case 'n':
                                            currentToken += '\n';
                                            i++;
                                            break;

                                        case 'r':
                                            currentToken += '\r';
                                            i++;
                                            break;

                                        case '"':
                                            currentToken += '"';
                                            i++;
                                            break;
                                        default: throw new FormatException("Unrecognized escape sequence.");
                                    }
                                }
                                else
                                {
                                    currentToken += input[i++];
                                }
                            }
                        }

                        break;

                    case ' ':
                        if (!string.IsNullOrWhiteSpace(currentToken))
                            ret.Add(currentToken);

                        currentToken = string.Empty;
                        break;

                    default:
                        currentToken += input[i];
                        break;
                }
            }

            if (currentToken.Length > 0)
                ret.Add(currentToken);

            return ret.ToArray();
        }
    }
}