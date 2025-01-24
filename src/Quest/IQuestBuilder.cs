using System;

namespace TobiStr
{
    /// <summary>
    /// Defines the contract for building synchronous quests with a fluent API.
    /// </summary>
    /// <typeparam name="T">The type of the payload associated with the quest.</typeparam>
    public interface IQuestBuilder<T>
    {
        /// <summary>
        /// Sets the payload for the quest.
        /// </summary>
        /// <param name="payload">The payload to associate with the quest.</param>
        /// <returns>The current instance of the builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided payload is null.</exception>
        IQuestBuilder<T> WithPayload(T payload);

        /// <summary>
        /// Sets the synchronous action to be invoked upon successful completion of the quest.
        /// </summary>
        /// <param name="onComplete">The action to invoke when the quest is completed successfully.</param>
        /// <returns>The current instance of the builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided action is null.</exception>
        IQuestBuilder<T> OnComplete(Action onComplete);

        /// <summary>
        /// Sets the synchronous action to be invoked if the quest encounters an error.
        /// </summary>
        /// <param name="onError">The action to invoke when an error occurs during the quest.</param>
        /// <returns>The current instance of the builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided action is null.</exception>
        IQuestBuilder<T> OnError(Action<Exception> onError);

        /// <summary>
        /// Builds the quest instance using the specified payload and handlers.
        /// </summary>
        /// <returns>A new instance of <see cref="IQuest{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if any required parameters, such as the payload or handlers, are missing.
        /// </exception>
        IQuest<T> Build();
    }
}
