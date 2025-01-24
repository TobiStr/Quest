using System;
using System.Threading.Tasks;

namespace TobiStr
{
    /// <summary>
    /// A factory class for creating instances of quest builders.
    /// Provides methods to obtain builders for both synchronous and asynchronous quests.
    /// </summary>
    public static class QuestBuilder
    {
        /// <summary>
        /// Creates and returns a new builder for constructing synchronous quests.
        /// </summary>
        /// <typeparam name="T">The type of the payload associated with the quest.</typeparam>
        /// <returns>An instance of <see cref="IQuestBuilder{T}"/> for building synchronous quests.</returns>
        public static IQuestBuilder<T> GetSynchronousQuestBuilder<T>()
            where T : class
        {
            return new SyncQuestBuilder<T>();
        }

        /// <summary>
        /// Creates and returns a new builder for constructing asynchronous quests.
        /// </summary>
        /// <typeparam name="T">The type of the payload associated with the quest.</typeparam>
        /// <returns>An instance of <see cref="IAsyncQuestBuilder{T}"/> for building asynchronous quests.</returns>
        public static IAsyncQuestBuilder<T> GetAsynchronousQuestBuilder<T>()
            where T : class
        {
            return new AsyncQuestBuilder<T>();
        }
    }

    internal class SyncQuestBuilder<T> : IQuestBuilder<T>
        where T : class
    {
        private T payload;

        private Action onComplete;

        private Action<Exception> onError;

        public IQuestBuilder<T> WithPayload(T payload)
        {
            this.payload = payload;
            return this;
        }

        public IQuestBuilder<T> OnComplete(Action onComplete)
        {
            this.onComplete = onComplete ?? throw new ArgumentNullException(nameof(onComplete));
            return this;
        }

        public IQuestBuilder<T> OnError(Action<Exception> onError)
        {
            this.onError = onError ?? throw new ArgumentNullException(nameof(onError));
            return this;
        }

        public IQuest<T> Build()
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(payload, default))
                throw new InvalidOperationException(
                    "The payload must be set before building the quest."
                );

            if (onComplete == null)
                throw new InvalidOperationException(
                    "The onComplete action must be set before building the quest."
                );

            if (onError == null)
                throw new InvalidOperationException(
                    "The onError action must be set before building the quest."
                );

            return new Quest<T>(payload, onComplete, onError);
        }
    }

    internal class AsyncQuestBuilder<T> : IAsyncQuestBuilder<T>
        where T : class
    {
        private T payload;

        private Func<Task> onCompleteAsync;

        private Func<Exception, Task> onErrorAsync;

        public IAsyncQuestBuilder<T> WithPayload(T payload)
        {
            this.payload = payload;
            return this;
        }

        public IAsyncQuestBuilder<T> OnCompleteAsync(Func<Task> onCompleteAsync)
        {
            this.onCompleteAsync =
                onCompleteAsync ?? throw new ArgumentNullException(nameof(onCompleteAsync));
            return this;
        }

        public IAsyncQuestBuilder<T> OnErrorAsync(Func<Exception, Task> onErrorAsync)
        {
            this.onErrorAsync =
                onErrorAsync ?? throw new ArgumentNullException(nameof(onErrorAsync));
            return this;
        }

        public IAsyncQuest<T> Build()
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(payload, default))
                throw new InvalidOperationException(
                    "The payload must be set before building the quest."
                );

            if (onCompleteAsync == null)
                throw new InvalidOperationException(
                    "The onCompleteAsync action must be set before building the quest."
                );

            if (onErrorAsync == null)
                throw new InvalidOperationException(
                    "The onErrorAsync action must be set before building the quest."
                );

            return new Quest<T>(payload, onCompleteAsync, onErrorAsync);
        }
    }
}
