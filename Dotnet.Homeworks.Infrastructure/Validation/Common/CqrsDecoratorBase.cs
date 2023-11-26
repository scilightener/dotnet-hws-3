using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Common;

public abstract class CqrsDecoratorBase<TResponse> where TResponse : Result
{
    private protected static TResponse GenerateFailedResult(string error)
    {
        return GenerateResult(false, error);
    }

    private protected static TResponse GenerateSucceedResult()
    {
        return GenerateResult(true, null);
    }

    private static TResponse GenerateResult(bool isSuccess, string? error)
    {
        if (typeof(TResponse) == typeof(Result))
            return (new Result(isSuccess, error) as TResponse)!;

        var valueType = typeof(TResponse).GenericTypeArguments[0];
        var resultGeneric = typeof(Result<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(valueType)
            .GetConstructor(new[] { valueType, typeof(bool), typeof(string) })!
            .Invoke(new object?[] { null, isSuccess, error });

        return (TResponse)resultGeneric;
    }
}