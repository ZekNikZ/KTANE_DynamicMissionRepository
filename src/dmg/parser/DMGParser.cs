using System;
using System.Collections.Generic;
using System.Linq;

namespace DMG.Parser {
    public class DMGParser {
        private readonly bool OnlyBaseFeatures;

        public DMGParser(bool onlyBaseFeatures = false) {
            OnlyBaseFeatures = onlyBaseFeatures;
        }

        public DMGMission Parse(string source) {
            var rawTokens = new Tokenizer().Tokenize(source);
            var tokens = new Lexer().LexTokens(rawTokens);
            var actions = ParseTokens(tokens);
            return CreateMission(actions);
        }

        private List<DMGAction> ParseTokens(List<Token> tokens) {
            var result = new List<DMGAction>();

            var statement = new List<Token>();
            var inSubBomb = false;
            var bombRepititions = 1;
            var reduceDuplicates = false;
            var statements = new List<List<Token>>();

            foreach (var token in tokens) {
                switch (token.TokenType) {
                    // Debug
                    case TokenType.COMMENT_INLINE:
                    case TokenType.COMMENT_MULTILINE:
                        // TODO: Debug.LogFormat("Found comment on line {0} {1}", token.LineNumber, token.StringValue);
                        break;

                    // Bomb group open
                    case TokenType.OP_BOMB_GROUP_OPEN:
                        if (statement.First().TokenType == TokenType.OP_DUPLICATE_REDUCTION_FLAG) {
                            reduceDuplicates = true;
                            statement.RemoveAt(0);
                        }
                        if (statement.Count == 2 && statement[0].TokenType == TokenType.VALUE_INT && statement[1].TokenType == TokenType.OP_REPEAT) {
                            bombRepititions = (int) statement[0].IntValue;
                            statement.Clear();
                        }
                        if (statement.Count != 0) {
                            throw new ParseException(token, "Cannot open bomb group in the middle of a statement");
                        } else if (inSubBomb) {
                            throw new ParseException(token, "Cannot nest bomb groups");
                        }
                        inSubBomb = true;
                        statements.Clear();
                        break;

                    // Bomb group close
                    case TokenType.OP_BOMB_GROUP_CLOSE:
                        if (!inSubBomb) {
                            throw new ParseException(token, "Cannot close bomb group when not in one");
                        }
                        inSubBomb = false;

                        if (statement.Count > 0) {
                            statements.Add(new List<Token>(statement));
                            statement.Clear();
                        }
                        result.Add(new BombDefinitionAction(reduceDuplicates, bombRepititions, statements.Select(ParseStatement).ToList()));
                        bombRepititions = 1;
                        reduceDuplicates = false;
                        break;

                    // Statement separator
                    case TokenType.OP_STATEMENT_SEPARATOR:
                        if (inSubBomb) {
                            statements.Add(new List<Token>(statement));
                        } else {
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

            if (statement.Count > 0) {
                result.Add(ParseStatement(statement));
            }

            return result.Where(action => !(action is NoopAction)).ToList();
        }

        private DMGAction ParseStatement(List<Token> tokens) {
            if (tokens.Count == 0) {
                return new NoopAction();
            }

            var modulePoolRepititions = 1;
            var reducedDuplicates = false;

            if (tokens[0].TokenType == TokenType.OP_DUPLICATE_REDUCTION_FLAG) {
                reducedDuplicates = true;
                tokens.RemoveAt(0);

                if (tokens.Count == 0) {
                    throw new ParseException(tokens[0], "Expected bomb or module pool following a duplicate reduction flag");
                }
            }

            if (tokens[0].TokenType == TokenType.VALUE_INT && tokens.Count >= 2 && tokens[1].TokenType == TokenType.OP_REPEAT) {
                modulePoolRepititions = (int) tokens[0].IntValue;
                tokens.RemoveAt(0);
                tokens.RemoveAt(0);

                if (tokens.Count == 0) {
                    throw new ParseException(tokens[0], "Expected bomb or module pool following a repeat directive");
                }
            }

            switch (tokens[0].TokenType) {
                case TokenType.VALUE_STRING:
                    // Strike count
                    if (Regexes.STRIKE_COUNT_REGEX.IsMatch(tokens[0].StringValue)) {
                        if (reducedDuplicates) {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a duplicate reduction flag");
                        } else if (modulePoolRepititions != 1) {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a repeat directive");
                        } else if (tokens.Count != 1) {
                            throw new ParseException(tokens[0], "Detected strike count setting but extra tokens provided");
                        }

                        return new SetSettingAction(ModuleRegistry.Instance.GetSetting("strikes"), int.Parse(tokens[0].StringValue.Substring(0, tokens[0].StringValue.Length - 1)));
                    }
                    // Timer
                    // TODO: figure out how to make more efficient
                    else if (Regexes.TIMER_REGEX.IsMatch(tokens[0].StringValue)) {
                        var time = 0;
                        var groups = Regexes.TIMER_REGEX.Match(tokens[0].StringValue).Groups;
                        if (groups["One"].Success) {
                            time += int.Parse(groups["One"].Value) * 60;
                        }
                        if (groups["Two"].Success) {
                            time += int.Parse(groups["Two"].Value);
                        }
                        if (groups["Three"].Success) {
                            time *= 60;
                            time += int.Parse(groups["Three"].Value);
                        }

                        return new SetSettingAction(ModuleRegistry.Instance.GetSetting("time"), time);
                    }
                    // Settings
                    else if (ModuleRegistry.Instance.TryGetSetting(tokens[0].StringValue, out DMGSetting setting, OnlyBaseFeatures)) {
                        if (reducedDuplicates) {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a duplicate reduction flag");
                        } else if (modulePoolRepititions != 1) {
                            throw new ParseException(tokens[0], "Expected bomb or module pool following a repeat directive");
                        }

                        return HandleSettingStatement(tokens, setting);
                    }
                    // Single module pool
                    else {
                        if (tokens.Count != 1) {
                            if (tokens[1].TokenType == TokenType.OP_VALUE_ASSIGNMENT) {
                                throw new ParseException(tokens[0], "Unknown setting");
                            } else {
                                throw new ParseException(tokens[0], "Detected singleton module pool but extra tokens provided");
                            }
                        } else {
                            return new AddModulePoolAction(reducedDuplicates, modulePoolRepititions, tokens[0].StringValue);
                        }
                    }

                // Module pool
                case TokenType.OP_MODULE_GROUP_OPEN:
                    if (tokens.Any(token => token.TokenType == TokenType.OP_MODULE_GROUP_CLOSE) && tokens.Last().TokenType != TokenType.OP_MODULE_GROUP_CLOSE) {
                        throw new ParseException(tokens[0], "Modules specified after module pool closed");
                    } else if (tokens.Last().TokenType != TokenType.OP_MODULE_GROUP_CLOSE) {
                        throw new ParseException(tokens[0], "Module pool left unclosed");
                    } else if (tokens.Count == 2) {
                        throw new ParseException(tokens[0], "Illegal empty module pool");
                    } else {
                        var modules = new List<string>();
                        for (int i = 1; i < tokens.Count - 1; i += 2) {
                            if (tokens[i].TokenType != TokenType.VALUE_STRING) {
                                throw new ParseException(tokens[0], "Invalid token in module pool");
                            } else if (i < tokens.Count - 2 && tokens[i + 1].TokenType != TokenType.OP_ITEM_SEPARATOR) {
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

        private DMGAction HandleSettingStatement(List<Token> tokens, DMGSetting setting) {
            object value;

            if (setting.TakesValue) {
                if (tokens.Count == 2 && tokens[1].TokenType == TokenType.OP_VALUE_ASSIGNMENT) {
                    throw new ParseException(tokens[0], $"Detected setting but missing value");
                } else if (tokens.Count == 3 && tokens[1].TokenType == TokenType.OP_VALUE_ASSIGNMENT) {
                    switch (tokens[2].TokenType) {
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
                            throw new ParseException(tokens[0], $"Detected setting but provided with invalid value");
                    }
                } else {
                    throw new ParseException(tokens[0], $"Detected setting but invalid tokens provided");
                }
            } else {
                if (tokens.Count > 1) {
                    throw new ParseException(tokens[0], $"Detected boolean setting but statement contains more tokens");
                }

                value = true;
            }

            return new SetSettingAction(setting, value);
        }

        private DMGMission CreateMission(List<DMGAction> actions) {
            foreach (var action in actions) {
                Console.WriteLine(action);
            }

            MissionFactory factory = MissionFactory.Create();

            foreach (var action in actions) {
                HandleAction(factory, action);
            }

            return factory.Build();
        }

        private void HandleAction(MissionFactory factory, DMGAction action) {
            action.Apply(factory, OnlyBaseFeatures);
        }
    }
}