using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Result;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        var result = Entegre.Extensions.Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        var result = Entegre.Extensions.Result.Failure(Error.Failure("Something went wrong"));

        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Message.Should().Be("Something went wrong");
    }

    [Fact]
    public void Success_WithValue_ShouldCreateSuccessResultWithValue()
    {
        var result = Entegre.Extensions.Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_WithValue_ShouldCreateFailureResultWithoutValue()
    {
        var result = Entegre.Extensions.Result.Failure<int>(Error.NotFound);

        result.IsFailure.Should().BeTrue();
        var action = () => _ = result.Value;
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Map_Success_ShouldTransformValue()
    {
        var result = Entegre.Extensions.Result.Success(10);

        var mapped = result.Map(x => x * 2);

        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be(20);
    }

    [Fact]
    public void Map_Failure_ShouldPropagateError()
    {
        var result = Result<int>.Failure(Error.Failure("error"));

        var mapped = result.Map(x => x * 2);

        mapped.IsFailure.Should().BeTrue();
        mapped.Error.Message.Should().Be("error");
    }

    [Fact]
    public void Bind_Success_ShouldChainResults()
    {
        var result = Entegre.Extensions.Result.Success(10);

        var bound = result.Bind(x =>
            x > 0 ? Result<int>.Success(x * 2) : Result<int>.Failure(Error.Failure("negative")));

        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be(20);
    }

    [Fact]
    public void Bind_Success_ShouldReturnFailure()
    {
        var result = Entegre.Extensions.Result.Success(-10);

        var bound = result.Bind(x =>
            x > 0 ? Result<int>.Success(x * 2) : Result<int>.Failure(Error.Failure("negative")));

        bound.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void GetValueOrDefault_Success_ShouldReturnValue()
    {
        var result = Entegre.Extensions.Result.Success(42);

        var value = result.GetValueOrDefault(0);

        value.Should().Be(42);
    }

    [Fact]
    public void GetValueOrDefault_Failure_ShouldReturnDefault()
    {
        var result = Result<int>.Failure(Error.Failure("error"));

        var value = result.GetValueOrDefault(0);

        value.Should().Be(0);
    }

    [Fact]
    public void GetValueOrThrow_Failure_ShouldThrow()
    {
        var result = Result<int>.Failure(Error.Failure("error"));

        var action = () => result.GetValueOrThrow();

        action.Should().Throw<InvalidOperationException>().WithMessage("error");
    }

    [Fact]
    public void OnSuccess_ShouldExecuteAction()
    {
        var executed = false;
        var result = Entegre.Extensions.Result.Success(42);

        result.OnSuccess(_ => executed = true);

        executed.Should().BeTrue();
    }

    [Fact]
    public void OnFailure_ShouldExecuteAction()
    {
        var executed = false;
        var result = Result<int>.Failure(Error.Failure("error"));

        result.OnFailure(_ => executed = true);

        executed.Should().BeTrue();
    }

    [Fact]
    public void Match_ShouldCallCorrectFunction()
    {
        var success = Entegre.Extensions.Result.Success(42);
        var failure = Result<int>.Failure(Error.Failure("error"));

        var successResult = success.Match(v => $"Value: {v}", e => $"Error: {e.Message}");
        var failureResult = failure.Match(v => $"Value: {v}", e => $"Error: {e.Message}");

        successResult.Should().Be("Value: 42");
        failureResult.Should().Be("Error: error");
    }

    [Fact]
    public void Combine_AllSuccess_ShouldReturnSuccess()
    {
        var result = Entegre.Extensions.Result.Combine(
            Entegre.Extensions.Result.Success(),
            Entegre.Extensions.Result.Success(),
            Entegre.Extensions.Result.Success());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Combine_AnyFailure_ShouldReturnFirstFailure()
    {
        var result = Entegre.Extensions.Result.Combine(
            Entegre.Extensions.Result.Success(),
            Entegre.Extensions.Result.Failure(Error.Failure("first")),
            Entegre.Extensions.Result.Failure(Error.Failure("second")));

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("first");
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateSuccess()
    {
        Result<int> result = 42;

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void ImplicitConversion_FromError_ShouldCreateFailure()
    {
        Result<int> result = Error.NotFound;

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("NotFound");
    }
}
