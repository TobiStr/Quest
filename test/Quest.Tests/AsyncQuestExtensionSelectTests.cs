namespace TobiStr.Tests;

[TestFixture]
public class AsyncQuestExtensionsSelectTests
{
    [Test]
    public async Task SelectAsync_TransformsPayload_ReturnsNewQuest()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            async () => { },
            async _ => Assert.Fail("Error action should not be invoked.")
        );

        // Act
        var result = await ((IAsyncQuest<int>)quest).SelectAsync(async payload =>
        {
            await Task.Delay(10);
            return payload.ToString();
        });

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Payload, Is.EqualTo("42"));
    }

    [Test]
    public void SelectAsync_WhenAsyncSelectorThrowsError_InvokesErrorActionAndThrows()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            async () => { },
            async ex => Assert.That(ex?.Message, Is.EqualTo("Test async error"))
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(
            async () =>
                await ((IAsyncQuest<int>)quest).SelectAsync<int, string>(async _ =>
                {
                    await Task.Delay(10);
                    throw new Exception("Test async error");
                })
        );
        Assert.That(ex?.Message, Is.EqualTo("Test async error"));
        Assert.That(quest.State, Is.EqualTo(QuestState.Failed));
    }

    [Test]
    public async Task SelectAsync_WithCancellationToken_TransformsPayload_ReturnsNewQuest()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            async () => { },
            async _ => Assert.Fail("Error action should not be invoked.")
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ((IAsyncQuest<int>)quest).SelectAsync(
            async (payload, token) =>
            {
                Assert.That(token, Is.EqualTo(cancellationToken));
                await Task.Delay(10, token);
                return payload.ToString();
            },
            cancellationToken
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Payload, Is.EqualTo("42"));
    }

    [Test]
    public void SelectAsync_WithCancellationToken_WhenAsyncSelectorThrowsError_InvokesErrorActionAndThrows()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            async () => { },
            async ex => Assert.That(ex?.Message, Is.EqualTo("Test async error with token"))
        );
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(
            async () =>
                await ((IAsyncQuest<int>)quest).SelectAsync<int, string>(
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
    public void SelectAsync_WithCancellationToken_WhenCancelled_ThrowsTaskCanceledException()
    {
        // Arrange
        var quest = new Quest<int>(
            42,
            async () => { },
            async ex => Assert.That(ex, Is.TypeOf<TaskCanceledException>())
        );
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        Assert.ThrowsAsync<TaskCanceledException>(
            async () =>
                await ((IAsyncQuest<int>)quest).SelectAsync<int, string>(
                    async (payload, token) =>
                    {
                        await Task.Delay(10, token);
                        return payload.ToString();
                    },
                    cts.Token
                )
        );
        Assert.That(quest.State, Is.EqualTo(QuestState.Failed));
    }
}
