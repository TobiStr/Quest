using System;
using System.Threading.Tasks;

namespace TobiStr
{
    /// <summary>
    /// Represents a task with immutable completion and error handlers, and a customizable payload.
    /// </summary>
    /// <typeparam name="T">The type of the payload associated with the task.</typeparam>
    internal class Quest<T> : IAsyncQuest<T>, IQuest<T>
    {
        private readonly Action onComplete;

        private readonly Action<Exception> onError;

        private readonly Func<Task> onCompleteAsync;

        private readonly Func<Exception, Task> onErrorAsync;

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
            this.onComplete = onComplete ?? throw new ArgumentNullException(nameof(onComplete));
            this.onError = onError ?? throw new ArgumentNullException(nameof(onError));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quest{T}"/> class with asynchrounus completion handlers.
        /// </summary>
        /// <param name="payload">The payload associated with the task.</param>
        /// <param name="onCompleteAsync">The asynchronous action to invoke upon successful completion of the task.</param>
        /// <param name="onErrorAsync">The asynchronous action to invoke if an error occurs during task execution.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="onComplete"/> or <paramref name="onError"/> is <c>null</c>.
        /// </exception>
        public Quest(T payload, Func<Task> onCompleteAsync, Func<Exception, Task> onErrorAsync)
        {
            Payload = payload;
            this.onCompleteAsync =
                onCompleteAsync ?? throw new ArgumentNullException(nameof(onCompleteAsync));
            this.onErrorAsync =
                onErrorAsync ?? throw new ArgumentNullException(nameof(onErrorAsync));
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
        /// Executes the completion handler.
        /// </summary>
        public void Complete()
        {
            if (onComplete == null)
                throw new InvalidOperationException(
                    "Synchronous method was called on an asynchronous Quest object."
                );
            State = QuestState.Completed;
            onComplete.Invoke();
        }

        /// <summary>
        /// Executes the error handler.
        /// </summary>
        /// <param name="exception">The exception that caused the error.</param>
        public void Fail(Exception exception)
        {
            State = QuestState.Failed;

            if (onError == null)
                throw new InvalidOperationException(
                    "Synchronous method was called on an asynchronous Quest object."
                );

            onError.Invoke(exception);
        }

        /// <summary>
        /// Executes the completion handler asynchronously.
        /// If an asynchronous handler is defined, it is invoked; otherwise, a synchronous fallback handler is invoked.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task CompleteAsync()
        {
            State = QuestState.Completed;

            if (onCompleteAsync != null)
                return onCompleteAsync();

            onComplete.Invoke();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the error handler asynchronously.
        /// If an asynchronous handler is defined, it is invoked; otherwise, a synchronous fallback handler is invoked.
        /// </summary>
        /// <param name="exception">The exception that caused the error.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task FailAsync(Exception exception)
        {
            State = QuestState.Failed;

            if (onErrorAsync != null)
                return onErrorAsync(exception);

            onError.Invoke(exception);

            return Task.CompletedTask;
        }
    }
}
