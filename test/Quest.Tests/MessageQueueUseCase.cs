using Moq;

namespace TobiStr.Tests;

public interface IMessageQueue
{
    MockMessage PeekLockMessage();
    void UnlockMessage(string id);
    void CompleteMessage(string id);
}

public record MockMessage(string Id, string Content);

[TestFixture]
public class MessageQueueUseCase
{
    [Test]
    public async Task MessageQueue_ProcessingSteps_SuccessfulCompletion()
    {
        // Arrange
        var mockQueue = new Mock<IMessageQueue>();
        var message = new MockMessage(Guid.NewGuid().ToString(), "Message from Queue");

        mockQueue.Setup(q => q.PeekLockMessage()).Returns(message);

        // Build a Quest that completes the locked message on completion
        // or unlocks it after failure
        var quest = QuestBuilder
            .GetSynchronousQuestBuilder<string>()
            .WithPayload(message.Content)
            .OnComplete(() => mockQueue.Object.CompleteMessage(message.Id))
            .OnError(ex =>
            {
                Console.WriteLine($"Error: {ex.Message}");
                mockQueue.Object.UnlockMessage(message.Id);
            })
            .Build();

        // Act
        // These processors transform and act on the payload
        var processors = new List<Func<IQuest<string>, Task<IQuest<string>>>>()
        {
            async quest =>
                await quest.SelectAsync(async payload =>
                {
                    await Task.Delay(10);
                    return payload + " -> Step 1 Processed";
                }),
            async quest =>
                await quest.SelectAsync(async payload =>
                {
                    await Task.Delay(10);
                    return payload + " -> Step 2 Processed";
                }),
        };

        foreach (var processor in processors)
        {
            quest = await processor(quest);
        }

        // This processor completes the Quest
        await quest.CompleteAsync(async payload =>
        {
            await Task.Delay(10);
            Console.WriteLine(payload + " -> Step 3 Completed");
        });

        // Assert
        // Verify that CompleteMessage has been called on the MessageQueue
        mockQueue.Verify(q => q.CompleteMessage(message.Id), Times.Once);
        mockQueue.Verify(q => q.UnlockMessage(It.IsAny<string>()), Times.Never);
        Assert.Pass("Quest completed successfully.");
    }

    [Test]
    public async Task MessageQueue_ProcessingSteps_FailureInLastStep()
    {
        // Arrange
        var mockQueue = new Mock<IMessageQueue>();
        var message = new MockMessage(Guid.NewGuid().ToString(), "Message from Queue");

        mockQueue.Setup(q => q.PeekLockMessage()).Returns(message);

        // Build a Quest that completes the locked message on completion
        // or unlocks it after failure
        var quest = QuestBuilder
            .GetSynchronousQuestBuilder<string>()
            .WithPayload(message.Content)
            .OnComplete(() => mockQueue.Object.CompleteMessage(message.Id))
            .OnError(ex =>
            {
                Console.WriteLine($"Error: {ex.Message}");
                mockQueue.Object.UnlockMessage(message.Id);
            })
            .Build();

        // Act
        // These processors transform and act on the payload
        // The last one fails with an exception
        var processors = new List<Func<IQuest<string>, Task<IQuest<string>>>>()
        {
            async quest =>
                await quest.SelectAsync(async payload =>
                {
                    await Task.Delay(10);
                    return payload + " -> Step 1 Processed";
                }),
            async quest =>
                await quest.SelectAsync(async payload =>
                {
                    await Task.Delay(10);
                    return payload + " -> Step 2 Processed";
                }),
            async quest =>
                await quest.SelectAsync<string, string>(async payload =>
                {
                    await Task.Delay(10);
                    throw new InvalidOperationException("Simulated Exception");
                }),
        };

        foreach (var processor in processors)
        {
            try
            {
                quest = await processor(quest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught Exception: {ex.Message}");
                break;
            }
        }

        // Assert
        // Verify that UnlockMessage has been called on the MessageQueue
        mockQueue.Verify(q => q.CompleteMessage(It.IsAny<string>()), Times.Never);
        mockQueue.Verify(q => q.UnlockMessage(message.Id), Times.Once);
        Assert.Pass("Quest failed and the message was unlocked.");
    }
}
