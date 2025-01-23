using System;
using System.Threading;
using System.Threading.Tasks;

namespace TobiStr
{
    /// <summary>
    /// Provides extension methods for working with <see cref="Quest{T}"/> objects.
    /// </summary>
    public static class QuestExtensions
    {
        /// <summary>
        /// Executes a synchronous action on the payload of the quest and returns the quest.
        /// </summary>
        /// <typeparam name="T">The type of the quest payload.</typeparam>
        /// <param name="quest">The quest to operate on.</param>
        /// <param name="action">The action to perform on the payload.</param>
        /// <returns>The original quest after the action is executed.</returns>
        /// <exception cref="Exception">Re-throws any exception encountered during the action.</exception>
        public static Quest<T> Tap<T>(this Quest<T> quest, Action<T> action)
        {
            try
            {
                action.Invoke(quest.Payload);
                return quest;
            }
            catch (Exception ex)
            {
                quest.ErrorAction(ex);
                throw;
            }
        }

        /// <summary>
        /// Executes an asynchronous action on the payload of the quest and returns the quest.
        /// </summary>
        /// <typeparam name="T">The type of the quest payload.</typeparam>
        /// <param name="quest">The quest to operate on.</param>
        /// <param name="asyncAction">The asynchronous action to perform on the payload.</param>
        /// <returns>A task representing the asynchronous operation, with the original quest as the result.</returns>
        /// <exception cref="Exception">Re-throws any exception encountered during the action.</exception>
        public static async Task<Quest<T>> TapAsync<T>(
            this Quest<T> quest,
            Func<T, Task> asyncAction
        )
        {
            try
            {
                await asyncAction.Invoke(quest.Payload);
                return quest;
            }
            catch (Exception ex)
            {
                quest.ErrorAction(ex);
                quest.State = QuestState.Failed;
                throw;
            }
        }

        /// <summary>
        /// Executes an asynchronous action on the payload of the quest with a cancellation token and returns the quest.
        /// </summary>
        /// <typeparam name="T">The type of the quest payload.</typeparam>
        /// <param name="quest">The quest to operate on.</param>
        /// <param name="asyncAction">The asynchronous action to perform on the payload.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with the original quest as the result.</returns>
        /// <exception cref="Exception">Re-throws any exception encountered during the action.</exception>
        public static async Task<Quest<T>> TapAsync<T>(
            this Quest<T> quest,
            Func<T, CancellationToken, Task> asyncAction,
            CancellationToken cancellationToken
        )
        {
            try
            {
                await asyncAction.Invoke(quest.Payload, cancellationToken);
                return quest;
            }
            catch (Exception ex)
            {
                quest.ErrorAction(ex);
                quest.State = QuestState.Failed;
                throw;
            }
        }

        /// <summary>
        /// Transforms the payload of the quest using a synchronous selector and returns a new quest.
        /// </summary>
        /// <typeparam name="T">The type of the original payload.</typeparam>
        /// <typeparam name="T2">The type of the transformed payload.</typeparam>
        /// <param name="quest">The quest to transform.</param>
        /// <param name="selector">The function to transform the payload.</param>
        /// <returns>A new quest with the transformed payload.</returns>
        /// <exception cref="Exception">Re-throws any exception encountered during the action.</exception>
        public static Quest<T2> Select<T, T2>(this Quest<T> quest, Func<T, T2> selector)
        {
            try
            {
                return new Quest<T2>(
                    selector.Invoke(quest.Payload),
                    quest.CompletionAction,
                    quest.ErrorAction
                );
            }
            catch (Exception ex)
            {
                quest.ErrorAction(ex);
                quest.State = QuestState.Failed;
                throw;
            }
        }

        /// <summary>
        /// Transforms the payload of the quest using an asynchronous selector and returns a new quest.
        /// </summary>
        /// <typeparam name="T">The type of the original payload.</typeparam>
        /// <typeparam name="T2">The type of the transformed payload.</typeparam>
        /// <param name="quest">The quest to transform.</param>
        /// <param name="selector">The asynchronous function to transform the payload.</param>
        /// <returns>A task representing the asynchronous operation, with a new quest as the result.</returns>
        /// <exception cref="Exception">Re-throws any exception encountered during the action.</exception>
        public static async Task<Quest<T2>> SelectAsync<T, T2>(
            this Quest<T> quest,
            Func<T, Task<T2>> selector
        )
        {
            try
            {
                return new Quest<T2>(
                    await selector.Invoke(quest.Payload),
                    quest.CompletionAction,
                    quest.ErrorAction
                );
            }
            catch (Exception ex)
            {
                quest.ErrorAction(ex);
                quest.State = QuestState.Failed;
                throw;
            }
        }

        /// <summary>
        /// Transforms the payload of the quest using an asynchronous selector with a cancellation token and returns a new quest.
        /// </summary>
        /// <typeparam name="T">The type of the original payload.</typeparam>
        /// <typeparam name="T2">The type of the transformed payload.</typeparam>
        /// <param name="quest">The quest to transform.</param>
        /// <param name="selector">The asynchronous function to transform the payload.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a new quest as the result.</returns>
        /// <exception cref="Exception">Re-throws any exception encountered during the action.</exception>
        public static async Task<Quest<T2>> SelectAsync<T, T2>(
            this Quest<T> quest,
            Func<T, CancellationToken, Task<T2>> selector,
            CancellationToken cancellationToken
        )
        {
            try
            {
                return new Quest<T2>(
                    await selector.Invoke(quest.Payload, cancellationToken),
                    quest.CompletionAction,
                    quest.ErrorAction
                );
            }
            catch (Exception ex)
            {
                quest.ErrorAction(ex);
                quest.State = QuestState.Failed;
                throw;
            }
        }

        /// <summary>
        /// Completes the quest by invoking the optional final action and the completion action.
        /// </summary>
        /// <typeparam name="T">The type of the quest payload.</typeparam>
        /// <param name="quest">The quest to complete.</param>
        /// <param name="finalAction">An optional action to invoke with the payload before completion.</param>
        /// <exception cref="Exception">Re-throws any exception encountered during the action.</exception>
        public static void Complete<T>(this Quest<T> quest, Action<T> finalAction = null)
        {
            try
            {
                if (finalAction != null)
                {
                    finalAction.Invoke(quest.Payload);
                }

                quest.CompletionAction.Invoke();
                quest.State = QuestState.Completed;
            }
            catch (Exception ex)
            {
                quest.ErrorAction(ex);
                quest.State = QuestState.Failed;
                throw;
            }
        }

        /// <summary>
        /// Completes the quest asynchronously by invoking the optional final action and the completion action.
        /// </summary>
        /// <typeparam name="T">The type of the quest payload.</typeparam>
        /// <param name="quest">The quest to complete.</param>
        /// <param name="finalAction">An optional asynchronous action to invoke with the payload before completion.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Re-throws any exception encountered during the action.</exception>
        public static async Task CompleteAsync<T>(
            this Quest<T> quest,
            Func<T, Task> finalAction = null
        )
        {
            try
            {
                if (finalAction != null)
                {
                    await finalAction.Invoke(quest.Payload);
                }

                quest.CompletionAction.Invoke();
                quest.State = QuestState.Completed;
            }
            catch (Exception ex)
            {
                quest.ErrorAction(ex);
                quest.State = QuestState.Failed;
                throw;
            }
        }

        /// <summary>
        /// Completes the quest asynchronously by invoking the final action with a cancellation token and the completion action.
        /// </summary>
        /// <typeparam name="T">The type of the quest payload.</typeparam>
        /// <param name="quest">The quest to complete.</param>
        /// <param name="finalAction">An optional asynchronous action to invoke with the payload before completion.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Re-throws any exception encountered during the action.</exception>
        public static async Task CompleteAsync<T>(
            this Quest<T> quest,
            Func<T, CancellationToken, Task> finalAction,
            CancellationToken cancellationToken
        )
        {
            try
            {
                if (finalAction != null)
                {
                    await finalAction.Invoke(quest.Payload, cancellationToken);
                }

                quest.CompletionAction.Invoke();
                quest.State = QuestState.Completed;
            }
            catch (Exception ex)
            {
                quest.ErrorAction(ex);
                quest.State = QuestState.Failed;
                throw;
            }
        }
    }
}
