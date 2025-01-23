using System;

namespace TobiStr
{
    /// <summary>
    /// Represents a task with immutable completion and error handlers, and a customizable payload.
    /// </summary>
    /// <typeparam name="T">The type of the payload associated with the task.</typeparam>
    public class Quest<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Quest{T}"/> class.
        /// </summary>
        /// <param name="payload">The payload associated with the task.</param>
        /// <param name="onComplete">The action to invoke upon successful completion of the task.</param>
        /// <param name="onError">The action to invoke if an error occurs during task execution.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="onComplete"/> or <paramref name="onError"/> is <c>null</c>.
        /// </exception>
        public Quest(T payload, Action onComplete, Action<Exception> onError)
        {
            Payload = payload;
            CompletionAction = onComplete ?? throw new ArgumentNullException(nameof(onComplete));
            ErrorAction = onError ?? throw new ArgumentNullException(nameof(onError));
        }

        /// <summary>
        /// Gets the payload associated with the task.
        /// </summary>
        public T Payload { get; }

        /// <summary>
        /// Gets the current state of the quest, indicating whether it is pending, completed, or failed.
        /// </summary>
        public QuestState State { get; internal set; } = QuestState.Pending;

        /// <summary>
        /// Gets the action to invoke upon successful completion of the task.
        /// </summary>
        public Action CompletionAction { get; }

        /// <summary>
        /// Gets the action to invoke if an error occurs during task execution.
        /// </summary>
        public Action<Exception> ErrorAction { get; }
    }
}
