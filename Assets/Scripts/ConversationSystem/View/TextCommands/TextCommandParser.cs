// Assets/Scripts/ConversationSystem/View/TextCommands/TextCommandParser.cs
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Parses text containing inline commands in square bracket format.
    /// Example: "Hello [var:player_name], please [pause:0.5]wait here."
    /// </summary>
    public class TextCommandParser {
        // Pattern: [command] or [command:parameter] or [command:param1:param2]
        private static readonly Regex CommandPattern = new Regex(
            @"\[([a-zA-Z_][a-zA-Z0-9_]*)(?::([^\]]*))?\]",
            RegexOptions.Compiled
        );

        private readonly ITextCommandHandler _commandHandler;

        /// <summary>
        /// Creates a new text command parser.
        /// </summary>
        /// <param name="commandHandler">Handler for processing commands.</param>
        public TextCommandParser(ITextCommandHandler commandHandler = null) {
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Parses text and extracts all commands.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>List of processed segments.</returns>
        public List<ProcessedTextSegment> Parse(string text) {
            var segments = new List<ProcessedTextSegment>();

            if (string.IsNullOrEmpty(text)) {
                return segments;
            }

            int lastIndex = 0;
            MatchCollection matches = CommandPattern.Matches(text);

            foreach (Match match in matches) {
                // Add text before this command
                if (match.Index > lastIndex) {
                    string textBefore = text.Substring(lastIndex, match.Index - lastIndex);
                    if (!string.IsNullOrEmpty(textBefore)) {
                        segments.Add(ProcessedTextSegment.CreateText(textBefore));
                    }
                }

                // Parse the command
                TextCommandResult command = ParseCommand(match);
                segments.Add(ProcessedTextSegment.CreateCommand(command));

                lastIndex = match.Index + match.Length;
            }

            // Add remaining text after last command
            if (lastIndex < text.Length) {
                string textAfter = text.Substring(lastIndex);
                if (!string.IsNullOrEmpty(textAfter)) {
                    segments.Add(ProcessedTextSegment.CreateText(textAfter));
                }
            }

            return segments;
        }

        /// <summary>
        /// Parses a regex match into a TextCommandResult.
        /// </summary>
        private TextCommandResult ParseCommand(Match match) {
            TextCommandResult result = new() {
                CommandType = match.Groups[1].Value.ToLower(),
                OriginalText = match.Value,
                StartIndex = match.Index,
                Length = match.Length
            };

            // Parse parameters
            if (match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value)) {
                string paramString = match.Groups[2].Value;
                
                // Check for named parameters (key=value format)
                if (paramString.Contains("=")) {
                    ParseNamedParameters(paramString, result);
                }
                else if (paramString.Contains(":")) {
                    // Multiple positional parameters separated by :
                    string[] parts = paramString.Split(':');
                    result.Parameter = parts[0];
                    for (int i = 0; i < parts.Length; i++) {
                        result.Parameters[$"param{i}"] = parts[i];
                    }
                }
                else {
                    // Single parameter
                    result.Parameter = paramString;
                }
            }

            return result;
        }

        /// <summary>
        /// Parses named parameters in key=value format.
        /// </summary>
        private void ParseNamedParameters(string paramString, TextCommandResult result) {
            string[] pairs = paramString.Split(',');
            foreach (string pair in pairs) {
                string[] keyValue = pair.Split('=');
                if (keyValue.Length == 2) {
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();
                    result.Parameters[key] = value;
                    
                    // First parameter is also the default
                    if (string.IsNullOrEmpty(result.Parameter)) {
                        result.Parameter = value;
                    }
                }
            }
        }

        /// <summary>
        /// Processes text and returns the display string with variables substituted
        /// and a list of commands with their character positions.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <param name="variableProvider">Provider for variable values.</param>
        /// <returns>Processed text result.</returns>
        public ProcessedText ProcessText(string text, IConversationVariableProvider variableProvider = null) {
            ProcessedText result = new();
            List<ProcessedTextSegment> segments = Parse(text);
            StringBuilder displayBuilder = new();
            List<PositionedCommand> commands = new();

            foreach (var segment in segments) {
                if (segment.Type == SegmentType.Text) {
                    displayBuilder.Append(segment.Text);
                }
                else if (segment.Type == SegmentType.Command) {
                    TextCommandResult command = segment.Command;

                    // Handle variable substitution
                    if (command.CommandType == "var") {
                        string varValue = GetVariableValue(command.Parameter, variableProvider);
                        displayBuilder.Append(varValue);
                    }
                    else {
                        // Record command position for execution during typewriter
                        commands.Add(new PositionedCommand {
                            CharacterIndex = displayBuilder.Length,
                            Command = command
                        });
                    }
                }
            }

            result.DisplayText = displayBuilder.ToString();
            result.Commands = commands;

            return result;
        }

        /// <summary>
        /// Gets a variable value from the provider.
        /// </summary>
        private string GetVariableValue(string variableName, IConversationVariableProvider provider) {
            if (provider == null || string.IsNullOrEmpty(variableName)) {
                return $"[{variableName}]"; // Return original if can't resolve
            }

            // Try different types
            if (provider.TryGetString(variableName, out string strValue)) {
                return strValue;
            }
            if (provider.TryGetInt(variableName, out int intValue)) {
                return intValue.ToString();
            }
            if (provider.TryGetFloat(variableName, out float floatValue)) {
                return floatValue.ToString("F2");
            }
            if (provider.TryGetBool(variableName, out bool boolValue)) {
                return boolValue.ToString();
            }

            return $"[{variableName}]"; // Return original if not found
        }

        /// <summary>
        /// Strips all commands from text, returning plain text only.
        /// </summary>
        /// <param name="text">The text to strip.</param>
        /// <returns>Plain text without commands.</returns>
        public string StripCommands(string text) {
            if (string.IsNullOrEmpty(text)) {
                return text;
            }

            return CommandPattern.Replace(text, "");
        }

        /// <summary>
        /// Checks if text contains any commands.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if commands are present.</returns>
        public bool HasCommands(string text) {
            if (string.IsNullOrEmpty(text)) {
                return false;
            }

            return CommandPattern.IsMatch(text);
        }

        /// <summary>
        /// Extracts all commands from text without processing.
        /// </summary>
        /// <param name="text">The text to extract from.</param>
        /// <returns>List of command results.</returns>
        public List<TextCommandResult> ExtractCommands(string text) {
            var commands = new List<TextCommandResult>();
            
            if (string.IsNullOrEmpty(text)) {
                return commands;
            }

            MatchCollection matches = CommandPattern.Matches(text);
            foreach (Match match in matches) {
                commands.Add(ParseCommand(match));
            }

            return commands;
        }
    }

    /// <summary>
    /// Result of processing text with commands.
    /// </summary>
    public class ProcessedText {
        /// <summary>
        /// The text to display (with variables substituted, commands removed).
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// Commands to execute at specific character positions.
        /// </summary>
        public List<PositionedCommand> Commands { get; set; }

        public ProcessedText() {
            Commands = new List<PositionedCommand>();
        }
    }

    /// <summary>
    /// A command with its position in the display text.
    /// </summary>
    public class PositionedCommand {
        /// <summary>
        /// The character index where this command should execute.
        /// </summary>
        public int CharacterIndex { get; set; }

        /// <summary>
        /// The command to execute.
        /// </summary>
        public TextCommandResult Command { get; set; }
    }
}