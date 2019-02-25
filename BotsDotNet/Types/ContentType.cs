using System;

namespace BotsDotNet
{
    /// <summary>
    /// Represents a category for mime-types of messages
    /// </summary>
    [Flags]
    public enum ContentType
    {
        /// <summary>
        /// General text message
        /// </summary>
        Text = 1,
        /// <summary>
        /// Rich message containing HTML
        /// </summary>
        Rich = 2,
        /// <summary>
        /// Voice message or some audio format
        /// </summary>
        Audio = 4,
        /// <summary>
        /// Any image format including gifs.
        /// </summary>
        Image = 8,
        /// <summary>
        /// Markup enabled content.
        /// </summary>
        Markup = 16,
        /// <summary>
        /// Any file other than images, audio, and HTML
        /// </summary>
        File = 32,
        /// <summary>
        /// A content-type other than what is already listed.
        /// </summary>
        Other = 64
    }
}
