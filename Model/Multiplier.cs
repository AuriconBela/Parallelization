using BenchmarkDotNet.Engines;
using static System.Reflection.Metadata.BlobBuilder;

namespace Parallelization.Model;

public class Multiplier
{
    private readonly Matrix _a;
    private readonly Matrix _b;

    public Multiplier(Matrix a,Matrix b)
    {
        _a = a;
        _b = b;
    }

    public RunResult<Matrix> Multiply_Raw()
    {
        var size = _a.Size;
        var result = new Matrix(size, false);

        for (int i = 0; i < size; ++i)
            for (int j = 0; j < size; ++j)
                for (int k = 0; k < size; ++k)
                    result[i,j] += _a[i,k] * _b[k,j];

        return new Success<Matrix>(result);
    }

    public RunResult<Matrix> Multiply_Parallel()
    {
        var size = _a.Size;
        var result = new Matrix(size, false);

        Parallel.For(0, size, i =>
        {
            for (int j = 0; j < size; ++j)
                for (int k = 0; k < size; ++k)
                    result[i,j] += _a[i,k] * _b[k,j];
        });

        return new Success<Matrix>(result);
    }

    public RunResult<Matrix> Multiply_Parallel(int numberOfThreads)
    {
        var size = _a.Size;
        var result = new Matrix(size, false);

        Parallel.For(0, size, new ParallelOptions { MaxDegreeOfParallelism = numberOfThreads }, i =>
        {
            for (int j = 0; j < size; ++j)
                for (int k = 0; k < size; ++k)
                    result[i, j] += _a[i, k] * _b[k, j];
        });

        return new Success<Matrix>(result);
    }

    public async Task<RunResult<Matrix>> Multiply_Async()
    {
        return await Task.Run(() =>
        {
            var size = _a.Size;
            var result = new Matrix(size, false);

            Parallel.For(0, size, i =>
            {
                for (int j = 0; j < size; ++j)
                    for (int k = 0; k < size; ++k)
                        result[i, j] += _a[i, k] * _b[k, j];
            }
            );

            return new Success<Matrix>(result);
        });
    }

    public async Task<RunResult<Matrix>> Multiply_Async_AndCancel(CancellationTokenSource cts)
    {
        return await Task.Run(() =>
        {
            var size = _a.Size;
            RunResult<Matrix> resultWrapped;
            var result = new Matrix(size, false);

            try
            {
                Parallel.For(0, size, new ParallelOptions { CancellationToken = cts.Token }, i =>
                {
                    for (int j = 0; j < size; ++j)
                        for (int k = 0; k < size; ++k)
                            result[i, j] += _a[i, k] * _b[k, j];
                });
                resultWrapped = new Success<Matrix>(result);
            }
            catch (OperationCanceledException _)
            {
                resultWrapped = new Cancelled<Matrix>(null);
            }
            catch (Exception _)
            {
                resultWrapped = new Error<Matrix>(result, "Unknown error");
            }
            return resultWrapped;
        });
    }
}
