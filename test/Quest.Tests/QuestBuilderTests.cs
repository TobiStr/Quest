namespace TobiStr.Tests;

[TestFixture]
public class QuestBuilderTests
{
    [Test]
    public void WithPayload_SetsPayload_ReturnsBuilder()
    {
        // Arrange
        var builder = new QuestBuilder<int>();

        // Act
        var result = builder.WithPayload(42);

        // Assert
        Assert.That(
            result,
            Is.SameAs(builder),
            "WithPayload should return the same builder instance."
        );
    }

    [Test]
    public void OnComplete_SetsOnCompleteAction_ReturnsBuilder()
    {
        // Arrange
        var builder = new QuestBuilder<int>();
        Action onComplete = () => { };

        // Act
        var result = builder.OnComplete(onComplete);

        // Assert
        Assert.That(
            result,
            Is.SameAs(builder),
            "OnComplete should return the same builder instance."
        );
    }

    [Test]
    public void OnComplete_WhenNull_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = new QuestBuilder<int>();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => builder.OnComplete(null));
        Assert.That(ex?.ParamName, Is.EqualTo("onComplete"));
    }

    [Test]
    public void OnError_SetsOnErrorAction_ReturnsBuilder()
    {
        // Arrange
        var builder = new QuestBuilder<int>();
        Action<Exception> onError = ex => { };

        // Act
        var result = builder.OnError(onError);

        // Assert
        Assert.That(result, Is.SameAs(builder), "OnError should return the same builder instance.");
    }

    [Test]
    public void OnError_WhenNull_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = new QuestBuilder<int>();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => builder.OnError(null));
        Assert.That(ex?.ParamName, Is.EqualTo("onError"));
    }

    [Test]
    public void Build_WhenAllSettingsValid_CreatesQuestInstance()
    {
        // Arrange
        var builder = new QuestBuilder<int>()
            .WithPayload(42)
            .OnComplete(() => { })
            .OnError(ex => { });

        // Act
        var quest = builder.Build();

        // Assert
        Assert.That(quest, Is.Not.Null, "Build should create a valid Quest instance.");
        Assert.That(quest.Payload, Is.EqualTo(42));
        Assert.That(quest.State, Is.EqualTo(QuestState.Pending));
    }

    [Test]
    public void Build_WithoutPayload_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new QuestBuilder<int>().OnComplete(() => { }).OnError(ex => { });

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.That(ex?.Message, Is.EqualTo("The payload must be set before building the quest."));
    }

    [Test]
    public void Build_WithoutOnComplete_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new QuestBuilder<int>().WithPayload(42).OnError(ex => { });

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.That(
            ex?.Message,
            Is.EqualTo("The onComplete action must be set before building the quest.")
        );
    }

    [Test]
    public void Build_WithoutOnError_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new QuestBuilder<int>().WithPayload(42).OnComplete(() => { });

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.That(
            ex?.Message,
            Is.EqualTo("The onError action must be set before building the quest.")
        );
    }
}
