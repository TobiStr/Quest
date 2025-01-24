using System;
using System.Threading.Tasks;

namespace TobiStr
{
    /// <summary>
    /// Defines the contract for building asynchronous quests with a fluent API.
    /// </summary>
    /// <typeparam name="T">The type of the payload associated with the quest.</typeparam>
    public interface IAsyncQuestBuilder<T>
    {
        /// <summary>
        /// Sets the payload for the asynchronous quest.
        /// </summary>
        /// <param name="payload">The payload to associate with the quest.</param>
        /// <returns>The current instance of the builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided payload is null.</exception>
        IAsyncQuestBuilder<T> WithPayload(T payload);

        /// <summary>
        /// Sets the asynchronous action to be invoked upon successful completion of the quest.
        /// </summary>
        /// <param name="onCompleteAsync">The asynchronous action to invoke when the quest is completed successfully.</param>
        /// <returns>The current instance of the builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided action is null.</exception>
        IAsyncQuestBuilder<T> OnCompleteAsync(Func<Task> onCompleteAsync);

        /// <summary>
        /// Sets the asynchronous action to be invoked if the quest encounters an error.
        /// </summary>
        /// <param name="onErrorAsync">The asynchronous action to invoke when an error occurs during the quest.</param>
        /// <returns>The current instance of the builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided action is null.</exception>
        IAsyncQuestBuilder<T> OnErrorAsync(Func<Exception, Task> onErrorAsync);

        /// <summary>
        /// Builds the asynchronous quest instance using the specified payload and handlers.
        /// </summary>
        /// <returns>A new instance of <see cref="IAsyncQuest{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if any required parameters, such as the payload or handlers, are missing.
        /// </exception>
        IAsyncQuest<T> Build();
    }
}
