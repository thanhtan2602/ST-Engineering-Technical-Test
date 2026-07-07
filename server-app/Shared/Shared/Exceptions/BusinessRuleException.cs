namespace Shared.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }

        public BusinessRuleException(string message, object? payload)
            : base(message)
        {
            Payload = payload;
        }

        public object? Payload { get; }
    }
}
