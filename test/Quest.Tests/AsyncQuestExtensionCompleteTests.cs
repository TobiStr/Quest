namespace TobiStr.Tests;

[TestFixture]
public class AsyncQuestExtensionsCompleteTests
{
    [Test]
    public async Task CompleteAsync_InvokesFinalActionAndCompletionAction_SetsStateToCompleted()
    {
        // Arrange
        bool finalActionInvoked = false;
        bool completionActionInvoked = false;
        var quest = new Quest<int>(
            42,
            async () => completionActionInvoked = true,
            async _ => Assert.Fail("Error action should not be invoked.")
        );

        // Act
        await ((IAsyncQuest<int>)quest).CompleteAsync(async payload =>
        {
            Assert.That(payload, Is.EqualTo(42));
            await Task.Delay(10);
            finalActionInvoked = true;
        });

        // Assert
        Assert.That(finalActionInvoked, Is.True, "Final action was not invoked.");
        Assert.That(completionActionInvoked, Is.True, "Completion action was not invoked.");
        Assert.That(quest.State, Is.EqualTo(QuestState.Completed));
    }

    [Test]
    public void CompleteAsync_WhenFinalActionThrowsError_InvokesErrorActionAndSetsStateToFailed()
    {
        // Arrange
        bool errorActionInvoked = false;
        var quest = new Quest<int>(
            42,
            async () => Assert.Fail("Completion action should not be invoked."),
            async ex =>
            {
                errorActionInvoked = true;
                Assert.That(ex?.Message, Is.EqualTo("Test async error"));
            }
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(
            async () =>
                await ((IAsyncQuest<int>)quest).CompleteAsync(async payload =>
                {
                    await Task.Delay(10);
                    throw new Exception("Test async error");
                })
        );

        Assert.That(ex?.Message, Is.EqualTo("Test async error"));
        Assert.That(errorActionInvoked, Is.True, "Error action was not invoked.");
        Assert.That(quest.State, Is.EqualTo(QuestState.Failed));
    }

    [Test]
    public async Task CompleteAsync_WithCancellationToken_InvokesFinalActionAndCompletionAction_SetsStateToCompleted()
    {
        // Arrange
        bool finalActionInvoked = false;
        bool completionActionInvoked = false;
        var quest = new Quest<int>(
            42,
            async () => completionActionInvoked = true,
            async _ => Assert.Fail("Error action should not be invoked.")
        );
        var cancellationToken = CancellationToken.None;

        // Act
        await ((IAsyncQuest<int>)quest).CompleteAsync(
            async (payload, token) =>
            {
                Assert.That(token, Is.EqualTo(cancellationToken));
                await Task.Delay(10, token);
                Assert.That(payload, Is.EqualTo(42));
                finalActionInvoked = true;
            },
            cancellationToken
        );

        // Assert
        Assert.That(finalActionInvoked, Is.True, "Final action was not invoked.");
        Assert.That(completionActionInvoked, Is.True, "Completion action was not invoked.");
        Assert.That(quest.State, Is.EqualTo(QuestState.Completed));
    }

    [Test]
    public void CompleteAsync_WithCancellationToken_WhenFinalActionThrowsError_InvokesErrorActionAndSetsStateToFailed()
    {
        // Arrange
        bool errorActionInvoked = false;
        var quest = new Quest<int>(
            42,
            async () => Assert.Fail("Completion action should not be invoked."),
            async ex =>
            {
                errorActionInvoked = true;
                Assert.That(ex?.Message, Is.EqualTo("Test async error with token"));
            }
        );
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(
            async () =>
                await ((IAsyncQuest<int>)quest).CompleteAsync(
                    async (payload, token) =>
                    {
                        await Task.Delay(10, token);
                        throw new Exception("Test async error with token");
                    },
                    cancellationToken
                )
        );

        Assert.That(ex?.Message, Is.EqualTo("Test async error with token"));
        Assert.That(errorActionInvoked, Is.True, "Error action was not invoked.");
        Assert.That(quest.State, Is.EqualTo(QuestState.Failed));
    }

    [Test]
    public void CompleteAsync_WithCancellationToken_WhenCancelled_ThrowsTaskCanceledException()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            async () => Assert.Fail("Completion action should not be invoked."),
            async ex => Assert.That(ex, Is.TypeOf<TaskCanceledException>())
        );
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        Assert.ThrowsAsync<TaskCanceledException>(
            async () =>
                await ((IAsyncQuest<int>)quest).CompleteAsync(
                    async (payload, token) =>
                    {
                        await Task.Delay(10, token);
                    },
                    cts.Token
                )
        );
        Assert.That(quest.State, Is.EqualTo(QuestState.Failed));
    }
}
