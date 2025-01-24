using System;
using System.Threading.Tasks;

namespace TobiStr
{
    /// <summary>
    /// Represents a task with immutable completion and error handlers, and a customizable payload.
    /// </summary>
    /// <typeparam name="T">The type of the payload associated with the task.</typeparam>
    public interface IAsyncQuest<out T>
    {
        /// <summary>
        /// Gets the payload associated with the task.
        /// </summary>
        T Payload { get; }

        /// <summary>
        /// Gets the current state of the quest, indicating whether it is pending, completed, or failed.
        /// </summary>
        QuestState State { get; }

        /// <summary>
        /// Executes the completion handler asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CompleteAsync();

        /// <summary>
        /// Executes the error handler asynchronously.
        /// </summary>
        /// <param name="exception">The exception that caused the error.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task FailAsync(Exception exception);
    }
}
