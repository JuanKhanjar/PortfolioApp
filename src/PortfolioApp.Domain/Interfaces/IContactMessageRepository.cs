using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Domain.Interfaces;

/// <summary>
/// Repository interface for ContactMessage entity operations.
/// This interface defines the contract for data access operations related to contact messages.
/// It includes methods for managing visitor inquiries and administrative message handling.
/// </summary>
public interface IContactMessageRepository
{
    /// <summary>
    /// Gets a contact message by its unique identifier.
    /// </summary>
    /// <param name="id">The message's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The contact message if found, null otherwise</returns>
    Task<ContactMessage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all contact messages ordered by sent date (most recent first).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of all contact messages</returns>
    Task<IEnumerable<ContactMessage>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unread contact messages ordered by sent date (most recent first).
    /// This is useful for administrative dashboards and notification systems.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of unread contact messages</returns>
    Task<IEnumerable<ContactMessage>> GetUnreadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets read contact messages ordered by sent date (most recent first).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of read contact messages</returns>
    Task<IEnumerable<ContactMessage>> GetReadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets contact messages from a specific sender email address.
    /// Useful for tracking communication history with specific contacts.
    /// </summary>
    /// <param name="senderEmail">Email address of the sender</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of messages from the specified sender</returns>
    Task<IEnumerable<ContactMessage>> GetBySenderEmailAsync(string senderEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets contact messages within a specific date range.
    /// </summary>
    /// <param name="startDate">Start of the date range</param>
    /// <param name="endDate">End of the date range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of messages within the date range</returns>
    Task<IEnumerable<ContactMessage>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent contact messages (within the last specified number of days).
    /// </summary>
    /// <param name="days">Number of days to look back</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of recent contact messages</returns>
    Task<IEnumerable<ContactMessage>> GetRecentAsync(int days = 7, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches contact messages by sender name, subject, or message content.
    /// This method performs a case-insensitive search across message fields.
    /// </summary>
    /// <param name="searchTerm">Term to search for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of messages matching the search term</returns>
    Task<IEnumerable<ContactMessage>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets urgent contact messages (unread messages older than specified threshold).
    /// </summary>
    /// <param name="urgentThresholdHours">Hours after which unread messages are considered urgent</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of urgent contact messages</returns>
    Task<IEnumerable<ContactMessage>> GetUrgentAsync(int urgentThresholdHours = 24, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets contact messages with pagination support.
    /// Useful for administrative interfaces with large numbers of messages.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of messages per page</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Paginated collection of contact messages</returns>
    Task<IEnumerable<ContactMessage>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new contact message to the repository.
    /// </summary>
    /// <param name="contactMessage">The contact message entity to add</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The added contact message with generated ID</returns>
    Task<ContactMessage> AddAsync(ContactMessage contactMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing contact message in the repository.
    /// This is primarily used for marking messages as read/unread.
    /// </summary>
    /// <param name="contactMessage">The contact message entity to update</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated contact message</returns>
    Task<ContactMessage> UpdateAsync(ContactMessage contactMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a contact message as read.
    /// This is a convenience method that updates only the IsRead flag.
    /// </summary>
    /// <param name="id">The ID of the message to mark as read</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if updated successfully, false if message not found</returns>
    Task<bool> MarkAsReadAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a contact message as unread.
    /// This is a convenience method that updates only the IsRead flag.
    /// </summary>
    /// <param name="id">The ID of the message to mark as unread</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if updated successfully, false if message not found</returns>
    Task<bool> MarkAsUnreadAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks multiple contact messages as read.
    /// Useful for bulk operations in administrative interfaces.
    /// </summary>
    /// <param name="ids">Collection of message IDs to mark as read</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of messages successfully updated</returns>
    Task<int> MarkMultipleAsReadAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a contact message from the repository.
    /// </summary>
    /// <param name="id">The ID of the message to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if deleted successfully, false if message not found</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple contact messages from the repository.
    /// Useful for bulk cleanup operations.
    /// </summary>
    /// <param name="ids">Collection of message IDs to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of messages successfully deleted</returns>
    Task<int> DeleteMultipleAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes old contact messages based on age.
    /// Useful for automated cleanup of old messages.
    /// </summary>
    /// <param name="olderThanDays">Delete messages older than this many days</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of messages deleted</returns>
    Task<int> DeleteOldMessagesAsync(int olderThanDays, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of contact messages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total number of contact messages</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of unread contact messages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of unread contact messages</returns>
    Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets contact message statistics for a date range.
    /// Returns counts of total, read, and unread messages.
    /// </summary>
    /// <param name="startDate">Start of the date range</param>
    /// <param name="endDate">End of the date range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Tuple containing (total, read, unread) counts</returns>
    Task<(int Total, int Read, int Unread)> GetStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}

