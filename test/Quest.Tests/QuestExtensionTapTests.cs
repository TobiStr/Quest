namespace TobiStr.Tests;

[TestFixture]
public class QuestExtensionTapTests
{
    [Test]
    public void Tap_ExecutesActionOnPayload_ReturnsSameQuest()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            () => { },
            ex => Assert.Fail("Error action should not be invoked.")
        );
        bool actionInvoked = false;

        // Act
        var result = quest.Tap(payload =>
        {
            actionInvoked = true;
            Assert.That(payload, Is.EqualTo(42));
        });

        // Assert
        Assert.That(result, Is.SameAs(quest));
        Assert.That(actionInvoked, Is.True, "Action was not invoked on the payload.");
    }

    [Test]
    public void Tap_WhenActionThrowsError_InvokesErrorActionAndThrows()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            () => { },
            ex => Assert.That(ex?.Message, Is.EqualTo("Test error"))
        );

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => quest.Tap(_ => throw new Exception("Test error")));
        Assert.That(ex?.Message, Is.EqualTo("Test error"));
    }

    [Test]
    public async Task TapAsync_ExecutesAsyncActionOnPayload_ReturnsSameQuest()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            () => { },
            ex => Assert.Fail("Error action should not be invoked.")
        );
        bool actionInvoked = false;

        // Act
        var result = await quest.TapAsync(async payload =>
        {
            await Task.Delay(10);
            actionInvoked = true;
            Assert.That(payload, Is.EqualTo(42));
        });

        // Assert
        Assert.That(result, Is.SameAs(quest));
        Assert.That(actionInvoked, Is.True, "Async action was not invoked on the payload.");
    }

    [Test]
    public void TapAsync_WhenAsyncActionThrowsError_InvokesErrorActionAndThrows()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            () => { },
            ex => Assert.That(ex?.Message, Is.EqualTo("Test async error"))
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(
            async () =>
                await quest.TapAsync(async _ =>
                {
                    await Task.Delay(10);
                    throw new Exception("Test async error");
                })
        );
        Assert.That(ex?.Message, Is.EqualTo("Test async error"));
        Assert.That(quest.State, Is.EqualTo(QuestState.Failed));
    }

    [Test]
    public async Task TapAsync_WithCancellationToken_ExecutesAsyncActionOnPayload_ReturnsSameQuest()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            () => { },
            ex => Assert.Fail("Error action should not be invoked.")
        );
        bool actionInvoked = false;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await quest.TapAsync(
            async (payload, token) =>
            {
                Assert.That(token, Is.EqualTo(cancellationToken));
                await Task.Delay(10, token);
                actionInvoked = true;
                Assert.That(payload, Is.EqualTo(42));
            },
            cancellationToken
        );

        // Assert
        Assert.That(result, Is.SameAs(quest));
        Assert.That(
            actionInvoked,
            Is.True,
            "Async action with cancellation token was not invoked."
        );
    }

    [Test]
    public void TapAsync_WithCancellationToken_WhenAsyncActionThrowsError_InvokesErrorActionAndThrows()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            () => { },
            ex => Assert.That(ex?.Message, Is.EqualTo("Test async error with token"))
        );
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(
            async () =>
                await quest.TapAsync(
                    async (payload, token) =>
                    {
                        await Task.Delay(10, token);
                        throw new Exception("Test async error with token");
                    },
                    cancellationToken
                )
        );

        Assert.That(ex?.Message, Is.EqualTo("Test async error with token"));
        Assert.That(quest.State, Is.EqualTo(QuestState.Failed));
    }

    [Test]
    public void TapAsync_WhenCancelled_ThrowsTaskCanceledException()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            () => { },
            ex => Assert.That(ex, Is.TypeOf<TaskCanceledException>())
        );
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        Assert.ThrowsAsync<TaskCanceledException>(
            async () =>
                await quest.TapAsync(
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
