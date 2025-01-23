using System;

namespace TobiStr
{
    /// <summary>
    /// A builder for creating instances of <see cref="Quest{T}"/> with a fluent API.
    /// </summary>
    /// <typeparam name="T">The type of the payload associated with the quest.</typeparam>
    public class QuestBuilder<T>
    {
        private T payload;
        private Action onComplete;
        private Action<Exception> onError;

        /// <summary>
        /// Sets the payload for the quest.
        /// </summary>
        /// <param name="payload">The payload to associate with the quest.</param>
        /// <returns>The current <see cref="QuestBuilder{T}"/> instance.</returns>
        public QuestBuilder<T> WithPayload(T payload)
        {
            this.payload = payload;
            return this;
        }

        /// <summary>
        /// Sets the completion action for the quest.
        /// </summary>
        /// <param name="onComplete">The action to invoke upon successful completion of the quest.</param>
        /// <returns>The current <see cref="QuestBuilder{T}"/> instance.</returns>
        public QuestBuilder<T> OnComplete(Action onComplete)
        {
            this.onComplete = onComplete ?? throw new ArgumentNullException(nameof(onComplete));
            return this;
        }

        /// <summary>
        /// Sets the error action for the quest.
        /// </summary>
        /// <param name="onError">The action to invoke if an error occurs during the quest.</param>
        /// <returns>The current <see cref="QuestBuilder{T}"/> instance.</returns>
        public QuestBuilder<T> OnError(Action<Exception> onError)
        {
            this.onError = onError ?? throw new ArgumentNullException(nameof(onError));
            return this;
        }

        /// <summary>
        /// Builds a new instance of <see cref="Quest{T}"/> with the specified configuration.
        /// </summary>
        /// <returns>A new <see cref="Quest{T}"/> instance.</returns>
        public Quest<T> Build()
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
}
