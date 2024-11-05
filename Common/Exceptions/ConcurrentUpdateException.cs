namespace Common.Exceptions;

public class ConcurrentUpdateException(string message) : Exception(message);