using System;

namespace Orbit.Exceptions {
    public class ParseValueException : Exception {
        public Type ParsedType { get; }
        public string ParsedString { get; }

        public ParseValueException(Type parsedType, string parsedString, string additionalMessage = "") :
            base($"Could not parse {parsedType.Name}: '{parsedString}'{(string.IsNullOrWhiteSpace(additionalMessage) ? "" : $"-- {additionalMessage}")}") {
            ParsedType = parsedType;
            ParsedString = parsedString;
        }
    }
}
