using System.Runtime.CompilerServices;
using LLama.Abstractions;
using LLama.Common;
using Xunit;

namespace LLama.Unittest;

public sealed class ChatSessionTests
{
    [Fact]
    public void AddMessage_AllowsArbitraryUserAndAssistantInterleaving()
    {
        var executor = (ILLamaExecutor)RuntimeHelpers.GetUninitializedObject(typeof(TestExecutor));
        var session = new ChatSession(executor);

        session.AddUserMessage("multiply 2 by 3")
            .AddUserMessage("then divide by -1")
            .AddAssistantMessage("6")
            .AddAssistantMessage("-6");

        Assert.Collection(
            session.History.Messages,
            message => Assert.Equal((AuthorRole.User, "multiply 2 by 3"), (message.AuthorRole, message.Content)),
            message => Assert.Equal((AuthorRole.User, "then divide by -1"), (message.AuthorRole, message.Content)),
            message => Assert.Equal((AuthorRole.Assistant, "6"), (message.AuthorRole, message.Content)),
            message => Assert.Equal((AuthorRole.Assistant, "-6"), (message.AuthorRole, message.Content)));
    }

    private sealed class TestExecutor : StatefulExecutorBase
    {
        private TestExecutor() : base(null!) { }

        protected override Task<bool> GetLoopCondition(InferStateArgs args, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        protected override Task PreprocessInputs(string? text, InferStateArgs args, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        protected override Task<(bool, IReadOnlyList<string>)> PostProcess(IInferenceParams inferenceParams, InferStateArgs args, CancellationToken cancellationToken = default)
            => Task.FromResult<(bool, IReadOnlyList<string>)>((true, []));

        protected override Task InferInternal(IInferenceParams inferenceParams, InferStateArgs args, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public override Task SaveState(string filename, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public override ExecutorBaseState GetStateData()
            => new();

        public override Task LoadState(ExecutorBaseState data, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public override Task LoadState(string filename, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
