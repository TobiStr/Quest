namespace TobiStr
{
    /// <summary>
    /// Represents the state of a quest.
    /// </summary>
    public enum QuestState
    {
        /// <summary>
        /// The quest is still pending and has not been completed or failed.
        /// </summary>
        Pending,

        /// <summary>
        /// The quest has been successfully completed.
        /// </summary>
        Completed,

        /// <summary>
        /// The quest has failed due to an error or exception.
        /// </summary>
        Failed,
    }
}
