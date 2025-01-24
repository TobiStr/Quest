using System;
using System.Threading;
using System.Threading.Tasks;

namespace TobiStr
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IAsyncQuest{T}"/> objects.
    /// </summary>
    public static class AsyncQuestExtensions
    {
        /// <summary>
        /// Executes an asynchronous action on the payload of the quest and returns the quest.
        /// </summary>
        /// <typeparam name="T">The type of the quest payload.</typeparam>
        /// <param name="quest">The quest to operate on.</param>
        /// <param name="asyncAction">The asynchronous action to perform on the payload.</param>
        /// <returns>A task representing the asynchronous operation, with the original quest as the result.</returns>
        /// <exception cref="Exception">Re-throws any exception encountered during the action.</exception>
        public static async Task<IAsyncQuest<T>> TapAsync<T>(
            this IAsyncQuest<T> quest,
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
                await quest.FailAsync(ex);
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
        public static async Task<IAsyncQuest<T>> TapAsync<T>(
            this IAsyncQuest<T> quest,
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
                await quest.FailAsync(ex);
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
        public static async Task<IAsyncQuest<T2>> SelectAsync<T, T2>(
            this IAsyncQuest<T> quest,
            Func<T, Task<T2>> selector
        )
        {
            try
            {
                return new Quest<T2>(
                    await selector.Invoke(quest.Payload),
                    quest.CompleteAsync,
                    quest.FailAsync
                );
            }
            catch (Exception ex)
            {
                await quest.FailAsync(ex);
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
        public static async Task<IAsyncQuest<T2>> SelectAsync<T, T2>(
            this IAsyncQuest<T> quest,
            Func<T, CancellationToken, Task<T2>> selector,
            CancellationToken cancellationToken
        )
        {
            try
            {
                return new Quest<T2>(
                    await selector.Invoke(quest.Payload, cancellationToken),
                    quest.CompleteAsync,
                    quest.FailAsync
                );
            }
            catch (Exception ex)
            {
                await quest.FailAsync(ex);
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
            this IAsyncQuest<T> quest,
            Func<T, Task> finalAction = null
        )
        {
            try
            {
                if (finalAction != null)
                {
                    await finalAction.Invoke(quest.Payload);
                }

                await quest.CompleteAsync();
            }
            catch (Exception ex)
            {
                await quest.FailAsync(ex);
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
            this IAsyncQuest<T> quest,
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

                await quest.CompleteAsync();
            }
            catch (Exception ex)
            {
                await quest.FailAsync(ex);
                throw;
            }
        }
    }
}
