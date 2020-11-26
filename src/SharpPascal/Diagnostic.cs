using SharpPascal.Parsing;

namespace SharpPascal
{
    public class Diagnostic
    {
        public bool IsError { get; }
        public string Message { get; }
        public Location Location { get; }

        private Diagnostic(bool isError, string message, Location location)
        {
            IsError = isError;
            Message = message;
            Location = location;
        }

        public static Diagnostic Error(string message, Location location) => new Diagnostic(true, message, location);
    }
}
