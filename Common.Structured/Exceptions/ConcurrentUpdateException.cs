namespace Common.Structured.Exceptions;

public class ConcurrentUpdateException(string message) : Exception(message);