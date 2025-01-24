using System;

namespace TobiStr
{
    /// <summary>
    /// Represents a task with immutable completion and error handlers, and a customizable payload.
    /// </summary>
    /// <typeparam name="T">The type of the payload associated with the task.</typeparam>
    public interface IQuest<out T>
        where T : class
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
        /// Executes the completion handler.
        /// </summary>
        void Complete();

        /// <summary>
        /// Executes the error handler.
        /// </summary>
        /// <param name="exception">The exception that caused the error.</param>
        void Fail(Exception exception);
    }
}
