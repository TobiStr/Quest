namespace TobiStr.Tests;

[TestFixture]
public class QuestBuilderTests
{
    [Test]
    public void GetSynchronousQuestBuilder_ReturnsSyncQuestBuilder()
    {
        // Act
        var builder = QuestBuilder.GetSynchronousQuestBuilder<int>();

        // Assert
        Assert.That(builder, Is.Not.Null);
        Assert.That(builder, Is.InstanceOf<SyncQuestBuilder<int>>());
    }

    [Test]
    public void GetAsynchronousQuestBuilder_ReturnsAsyncQuestBuilder()
    {
        // Act
        var builder = QuestBuilder.GetAsynchronousQuestBuilder<int>();

        // Assert
        Assert.That(builder, Is.Not.Null);
        Assert.That(builder, Is.InstanceOf<AsyncQuestBuilder<int>>());
    }

    [Test]
    public void SyncQuestBuilder_BuildsValidQuest()
    {
        // Arrange
        var builder = QuestBuilder.GetSynchronousQuestBuilder<int>();
        bool onCompleteInvoked = false;
        bool onErrorInvoked = false;

        // Act
        var quest = builder
            .WithPayload(42)
            .OnComplete(() => onCompleteInvoked = true)
            .OnError(_ => onErrorInvoked = true)
            .Build();

        // Assert
        Assert.That(quest.Payload, Is.EqualTo(42));
        Assert.DoesNotThrow(() => quest.Complete());
        Assert.That(onCompleteInvoked, Is.True);
        Assert.DoesNotThrow(() => quest.Fail(new Exception()));
        Assert.That(onErrorInvoked, Is.True);
    }

    [Test]
    public void SyncQuestBuilder_ThrowsIfPayloadNotSet()
    {
        // Arrange
        var builder = QuestBuilder.GetSynchronousQuestBuilder<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => builder.OnComplete(() => { }).OnError(_ => { }).Build()
        );
    }

    [Test]
    public void SyncQuestBuilder_ThrowsIfOnCompleteNotSet()
    {
        // Arrange
        var builder = QuestBuilder.GetSynchronousQuestBuilder<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => builder.WithPayload(42).OnError(_ => { }).Build()
        );
    }

    [Test]
    public void SyncQuestBuilder_ThrowsIfOnErrorNotSet()
    {
        // Arrange
        var builder = QuestBuilder.GetSynchronousQuestBuilder<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => builder.WithPayload(42).OnComplete(() => { }).Build()
        );
    }

    [Test]
    public async Task AsyncQuestBuilder_BuildsValidAsyncQuest()
    {
        // Arrange
        var builder = QuestBuilder.GetAsynchronousQuestBuilder<int>();
        bool onCompleteInvoked = false;
        bool onErrorInvoked = false;

        // Act
        var quest = builder
            .WithPayload(42)
            .OnCompleteAsync(async () =>
            {
                await Task.Delay(10);
                onCompleteInvoked = true;
            })
            .OnErrorAsync(async _ =>
            {
                await Task.Delay(10);
                onErrorInvoked = true;
            })
            .Build();

        // Assert
        Assert.That(quest.Payload, Is.EqualTo(42));
        Assert.DoesNotThrowAsync(quest.CompleteAsync);
        Assert.That(onCompleteInvoked, Is.True);
        Assert.DoesNotThrowAsync(async () => await quest.FailAsync(new Exception()));
        Assert.That(onErrorInvoked, Is.True);
    }

    [Test]
    public void AsyncQuestBuilder_ThrowsIfPayloadNotSet()
    {
        // Arrange
        var builder = QuestBuilder.GetAsynchronousQuestBuilder<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () =>
                builder
                    .OnCompleteAsync(async () => await Task.CompletedTask)
                    .OnErrorAsync(async _ => await Task.CompletedTask)
                    .Build()
        );
    }

    [Test]
    public void AsyncQuestBuilder_ThrowsIfOnCompleteAsyncNotSet()
    {
        // Arrange
        var builder = QuestBuilder.GetAsynchronousQuestBuilder<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => builder.WithPayload(42).OnErrorAsync(async _ => await Task.CompletedTask).Build()
        );
    }

    [Test]
    public void AsyncQuestBuilder_ThrowsIfOnErrorAsyncNotSet()
    {
        // Arrange
        var builder = QuestBuilder.GetAsynchronousQuestBuilder<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () =>
                builder
                    .WithPayload(42)
                    .OnCompleteAsync(async () => await Task.CompletedTask)
                    .Build()
        );
    }
}
