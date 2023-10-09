namespace Parallelization.Model;

public abstract record RunResult<T>(T? Data);

public record Success<T>(T? Data) : RunResult<T>(Data);
public record Cancelled<T>(T? Data) : RunResult<T>(Data);
public record Error<T>(T? Data, string Message) : RunResult<T>(Data);
