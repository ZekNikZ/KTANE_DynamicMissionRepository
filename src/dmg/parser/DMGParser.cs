using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace DMG.Parser
{
    class DMGParser
    {
        public DMGMission Parse(string source)
        {
            var rawTokens = new Tokenizer().Tokenize(source);
            var tokens = new Lexer().LexTokens(rawTokens);
            var actions = ParseTokens(tokens);
            return CreateMission(actions);
        }

        private List<DMGAction> ParseTokens(List<Token> tokens)
        {
            var result = new List<DMGAction>();

            var statement = new List<Token>();
            var inSubBomb = false;
            var bombRepititions = 1;
            var reduceDuplicates = false;
            var statements = new List<List<Token>>();

            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    // Debug
                    case TokenType.COMMENT_INLINE:
                    case TokenType.COMMENT_MULTILINE:
                        Debug.LogFormat("Found comment on line {0} {1}", token.LineNumber, token.StringValue);
                        break;

                    // Bomb group open
                    case TokenType.OP_BOMB_GROUP_OPEN:
                        if (statement.First().TokenType == TokenType.OP_DUPLICATE_REDUCTION_FLAG)
                        {
                            reduceDuplicates = true;
                            statement.RemoveAt(0);
                        }
                        if (statement.Count == 2 && statement[0].TokenType == TokenType.VALUE_INT && statement[1].TokenType == TokenType.OP_REPEAT)
                        {
                            bombRepititions = (int)statement[0].IntValue;
                            statement.Clear();
                        }
                        if (statement.Count != 0)
                        {
                            throw new ParseException(token, "Cannot open bomb group in the middle of a statement");
                        }
                        else if (inSubBomb)
                        {
                            throw new ParseException(token, "Cannot nest bomb groups");
                        }
                        inSubBomb = true;
                        statements.Clear();
                        break;

                    // Bomb group close
                    case TokenType.OP_BOMB_GROUP_CLOSE:
                        if (!inSubBomb)
                        {
                            throw new ParseException(token, "Cannot close bomb group when not in one");
                        }
                        inSubBomb = false;

                        if (statement.Count > 0)
                        {
                            statements.Add(new List<Token>(statement));
                            statement.Clear();
                        }
                        result.Add(new BombDefinitionAction(reduceDuplicates, bombRepititions, statements.Select(ParseStatement).ToList()));
                        bombRepititions = 1;
                        reduceDuplicates = false;
                        break;

                    // Statement separator
                    case TokenType.OP_STATEMENT_SEPARATOR:
                        if (inSubBomb)
                        {
                            statements.Add(new List<Token>(statement));
                        }
                        else
                        {
                            result.Add(ParseStatement(statement));
                            bombRepititions = 1;
                            reduceDuplicates = false;
                        }
                        statement.Clear();
                        break;

                    default:
                        statement.Add(token);
                        break;
                }
            }

            if (statement.Count > 0)
            {
                result.Add(ParseStatement(statement));
            }

            return result.Where(action => !(action is NoopAction)).ToList();
        }

        private DMGAction ParseStatement(List<Token> tokens)
        {
            if (tokens.Count == 0)
            {
                return new NoopAction();
            }

            var modulePoolRepititions = 1;
            var reducedDuplicates = false;

            if (tokens[0].TokenType == TokenType.OP_DUPLICATE_REDUCTION_FLAG)
            {
                reducedDuplicates = true;
                tokens.RemoveAt(0);

                if (tokens.Count == 0)
                {
                    throw new ParseException(tokens[0], "Expected bomb or module pool following a duplicate reduction flag");
                }
            }

            if (tokens[0].TokenType == TokenType.VALUE_INT && tokens.Count >= 2 && tokens[1].TokenType == TokenType.OP_REPEAT)
            {
                modulePoolRepititions = (int)tokens[0].IntValue;
                tokens.RemoveAt(0);
                tokens.RemoveAt(0);

                if (tokens.Count == 0)
                {
                    throw new ParseException(tokens[0], "Expected bomb or module pool following a repeat directive");
                }
            }

            switch (tokens[0].TokenType)
            {
                case TokenType.VALUE_STRING:
                    // Strike count
                    if (DMGSettings.STRIKE_COUNT_REGEX.IsMatch(tokens[0].StringValue))
                    {
                        if (reducedDuplicates)
                        {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a duplicate reduction flag");
                        }
                        else if (modulePoolRepititions != 1)
                        {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a repeat directive");
                        }
                        else if (tokens.Count != 1)
                        {
                            throw new ParseException(tokens[0], "Detected strike count setting but extra tokens provided");
                        }

                        return new SetSettingAction(DMGSettings.GENERATOR_SETTINGS["strikes"], int.Parse(tokens[0].StringValue.Substring(0, tokens[0].StringValue.Length - 1)));
                    }
                    // TODO: time setting
                    // Generator setting
                    else if (DMGSettings.GENERATOR_SETTINGS.ContainsKey(tokens[0].StringValue))
                    {
                        if (reducedDuplicates)
                        {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a duplicate reduction flag");
                        }
                        else if (modulePoolRepititions != 1)
                        {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a repeat directive");
                        }

                        return HandleSettingStatement(DMGSettings.GENERATOR_SETTINGS, "generator", tokens);
                    }
                    // DMG setting
                    else if (DMGSettings.DMG_SETTINGS.ContainsKey(tokens[0].StringValue))
                    {
                        if (reducedDuplicates)
                        {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a duplicate reduction flag");
                        }
                        else if (modulePoolRepititions != 1)
                        {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a repeat directive");
                        }

                        return HandleSettingStatement(DMGSettings.DMG_SETTINGS, "DMG", tokens);
                    }
                    // Tweaks setting
                    else if (DMGSettings.TWEAKS_SETTINGS.ContainsKey(tokens[0].StringValue))
                    {
                        if (reducedDuplicates)
                        {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a duplicate reduction flag");
                        }
                        else if (modulePoolRepititions != 1)
                        {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a repeat directive");
                        }

                        return HandleSettingStatement(DMGSettings.TWEAKS_SETTINGS, "Tweaks", tokens);
                    }
                    // Single module pool
                    else
                    {
                        if (tokens.Count != 1)
                        {
                            throw new ParseException(tokens[0], "Detected singleton module pool but extra tokens provided");
                        }
                        else
                        {
                            return new AddModulePoolAction(reducedDuplicates, modulePoolRepititions, tokens[0].StringValue);
                        }
                    }

                // Module pool
                case TokenType.OP_MODULE_GROUP_OPEN:
                    if (tokens.Any(token => token.TokenType == TokenType.OP_MODULE_GROUP_CLOSE) && tokens.Last().TokenType != TokenType.OP_MODULE_GROUP_CLOSE)
                    {
                        throw new ParseException(tokens[0], "Modules specified after module pool closed");
                    }
                    else if (tokens.Last().TokenType != TokenType.OP_MODULE_GROUP_CLOSE)
                    {
                        throw new ParseException(tokens[0], "Module pool left unclosed");
                    }
                    else if (tokens.Count == 2)
                    {
                        throw new ParseException(tokens[0], "Illegal empty module pool");
                    }
                    else
                    {
                        var modules = new List<string>();
                        for (int i = 1; i < tokens.Count - 1; i += 2)
                        {
                            if (tokens[i].TokenType != TokenType.VALUE_STRING)
                            {
                                throw new ParseException(tokens[0], "Invalid token in module pool");
                            }
                            else if (i < tokens.Count - 2 && tokens[i + 1].TokenType != TokenType.OP_ITEM_SEPARATOR)
                            {
                                throw new ParseException(tokens[0], "Separator expected between modules in module pool");
                            }
                            modules.Add(tokens[i].StringValue);
                        }
                        return new AddModulePoolAction(reducedDuplicates, modulePoolRepititions, modules);
                    }

                default:
                    throw new ParseException(tokens[0], "Unexpected token");
            }
        }

        private DMGAction HandleSettingStatement(Dictionary<string, DMGSetting> settings, string type, List<Token> tokens)
        {
            var setting = settings[tokens[0].StringValue];
            object value;

            if (setting.TakesValue)
            {
                if (tokens.Count == 2 && tokens[1].TokenType == TokenType.OP_VALUE_ASSIGNMENT)
                {
                    throw new ParseException(tokens[0], $"Detected {type} setting but missing value");
                }
                else if (tokens.Count == 3 && tokens[1].TokenType == TokenType.OP_VALUE_ASSIGNMENT)
                {
                    switch (tokens[2].TokenType)
                    {
                        case TokenType.VALUE_STRING:
                            value = tokens[2].StringValue;
                            break;
                        case TokenType.VALUE_INT:
                            value = tokens[2].IntValue;
                            break;
                        case TokenType.VALUE_FLOAT:
                            value = tokens[2].FloatValue;
                            break;
                        default:
                            throw new ParseException(tokens[0], $"Detected {type} setting but provided with invalid value");
                    }
                }
                else
                {
                    throw new ParseException(tokens[0], $"Detected {type} setting but invalid tokens provided");
                }
            }
            else
            {
                if (tokens.Count > 1)
                {
                    throw new ParseException(tokens[0], $"Detected boolean {type} setting but statement contains more tokens");
                }

                value = true;
            }

            return new SetSettingAction(setting, value);
        }

        private DMGMission CreateMission(List<DMGAction> actions)
        {
            return null;
        }
    }
}