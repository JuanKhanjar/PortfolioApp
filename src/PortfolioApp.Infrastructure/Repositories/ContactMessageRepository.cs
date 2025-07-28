using Microsoft.EntityFrameworkCore;
using PortfolioApp.Domain.Entities;
using PortfolioApp.Domain.Interfaces;
using PortfolioApp.Infrastructure.Data;

namespace PortfolioApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for ContactMessage entity operations.
/// This class provides concrete implementation of IContactMessageRepository interface,
/// handling all data access operations for ContactMessage entities using Entity Framework Core.
/// </summary>
public class ContactMessageRepository : IContactMessageRepository
{
    private readonly PortfolioDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ContactMessageRepository class.
    /// </summary>
    /// <param name="context">The database context for data access</param>
    public ContactMessageRepository(PortfolioDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets a contact message by its unique identifier.
    /// </summary>
    /// <param name="id">The message's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The contact message if found, null otherwise</returns>
    public async Task<ContactMessage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.ContactMessages
                .AsNoTracking()
                .FirstOrDefaultAsync(cm => cm.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving contact message with ID {id}", ex);
        }
    }

    /// <summary>
    /// Gets all contact messages ordered by sent date (most recent first).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of all contact messages</returns>
    public async Task<IEnumerable<ContactMessage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.ContactMessages
                .OrderByDescending(cm => cm.SentAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving all contact messages", ex);
        }
    }

    /// <summary>
    /// Gets unread contact messages ordered by sent date (most recent first).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of unread contact messages</returns>
    public async Task<IEnumerable<ContactMessage>> GetUnreadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.ContactMessages
                .Where(cm => !cm.IsRead)
                .OrderByDescending(cm => cm.SentAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving unread contact messages", ex);
        }
    }

    /// <summary>
    /// Gets read contact messages ordered by sent date (most recent first).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of read contact messages</returns>
    public async Task<IEnumerable<ContactMessage>> GetReadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.ContactMessages
                .Where(cm => cm.IsRead)
                .OrderByDescending(cm => cm.SentAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving read contact messages", ex);
        }
    }

    /// <summary>
    /// Gets contact messages from a specific sender email address.
    /// </summary>
    /// <param name="senderEmail">Email address of the sender</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of messages from the specified sender</returns>
    public async Task<IEnumerable<ContactMessage>> GetBySenderEmailAsync(string senderEmail, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(senderEmail))
            throw new ArgumentException("Sender email cannot be null or empty", nameof(senderEmail));

        try
        {
            var normalizedEmail = senderEmail.Trim().ToLowerInvariant();
            return await _context.ContactMessages
                .Where(cm => cm.SenderEmail.Value == normalizedEmail)
                .OrderByDescending(cm => cm.SentAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving contact messages from sender {senderEmail}", ex);
        }
    }

    /// <summary>
    /// Gets contact messages within a specific date range.
    /// </summary>
    /// <param name="startDate">Start of the date range</param>
    /// <param name="endDate">End of the date range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of messages within the date range</returns>
    public async Task<IEnumerable<ContactMessage>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date cannot be before start date");

        try
        {
            return await _context.ContactMessages
                .Where(cm => cm.SentAt >= startDate && cm.SentAt <= endDate)
                .OrderByDescending(cm => cm.SentAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving contact messages between {startDate} and {endDate}", ex);
        }
    }

    /// <summary>
    /// Gets recent contact messages (within the last specified number of days).
    /// </summary>
    /// <param name="days">Number of days to look back</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of recent contact messages</returns>
    public async Task<IEnumerable<ContactMessage>> GetRecentAsync(int days = 7, CancellationToken cancellationToken = default)
    {
        if (days < 0)
            throw new ArgumentException("Days must be non-negative", nameof(days));

        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            return await _context.ContactMessages
                .Where(cm => cm.SentAt >= cutoffDate)
                .OrderByDescending(cm => cm.SentAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving recent contact messages from last {days} days", ex);
        }
    }

    /// <summary>
    /// Searches contact messages by sender name, subject, or message content.
    /// </summary>
    /// <param name="searchTerm">Term to search for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of messages matching the search term</returns>
    public async Task<IEnumerable<ContactMessage>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(cancellationToken); // Return all if search term is empty

        try
        {
            var searchLower = searchTerm.Trim().ToLowerInvariant();
            return await _context.ContactMessages
                .Where(cm => cm.SenderName.ToLower().Contains(searchLower) ||
                            cm.Subject.ToLower().Contains(searchLower) ||
                            cm.Message.ToLower().Contains(searchLower) ||
                            cm.SenderEmail.Value.ToLower().Contains(searchLower))
                .OrderByDescending(cm => cm.SentAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error searching contact messages with term '{searchTerm}'", ex);
        }
    }

    /// <summary>
    /// Gets urgent contact messages (unread messages older than specified threshold).
    /// </summary>
    /// <param name="urgentThresholdHours">Hours after which unread messages are considered urgent</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of urgent contact messages</returns>
    public async Task<IEnumerable<ContactMessage>> GetUrgentAsync(int urgentThresholdHours = 24, CancellationToken cancellationToken = default)
    {
        if (urgentThresholdHours < 0)
            throw new ArgumentException("Urgent threshold hours must be non-negative", nameof(urgentThresholdHours));

        try
        {
            var urgentCutoff = DateTime.UtcNow.AddHours(-urgentThresholdHours);
            return await _context.ContactMessages
                .Where(cm => !cm.IsRead && cm.SentAt <= urgentCutoff)
                .OrderBy(cm => cm.SentAt) // Oldest first for urgent messages
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving urgent contact messages with threshold {urgentThresholdHours} hours", ex);
        }
    }

    /// <summary>
    /// Gets contact messages with pagination support.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of messages per page</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Paginated collection of contact messages</returns>
    public async Task<IEnumerable<ContactMessage>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        try
        {
            return await _context.ContactMessages
                .OrderByDescending(cm => cm.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving paged contact messages (page {pageNumber}, size {pageSize})", ex);
        }
    }

    /// <summary>
    /// Adds a new contact message to the repository.
    /// </summary>
    /// <param name="contactMessage">The contact message entity to add</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The added contact message with generated ID</returns>
    public async Task<ContactMessage> AddAsync(ContactMessage contactMessage, CancellationToken cancellationToken = default)
    {
        if (contactMessage == null)
            throw new ArgumentNullException(nameof(contactMessage));

        try
        {
            _context.ContactMessages.Add(contactMessage);
            await _context.SaveChangesAsync(cancellationToken);
            return contactMessage;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error adding contact message", ex);
        }
    }

    /// <summary>
    /// Updates an existing contact message in the repository.
    /// </summary>
    /// <param name="contactMessage">The contact message entity to update</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated contact message</returns>
    public async Task<ContactMessage> UpdateAsync(ContactMessage contactMessage, CancellationToken cancellationToken = default)
    {
        if (contactMessage == null)
            throw new ArgumentNullException(nameof(contactMessage));

        try
        {
            _context.ContactMessages.Update(contactMessage);
            await _context.SaveChangesAsync(cancellationToken);
            return contactMessage;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error updating contact message with ID {contactMessage.Id}", ex);
        }
    }

    /// <summary>
    /// Marks a contact message as read.
    /// </summary>
    /// <param name="id">The ID of the message to mark as read</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if updated successfully, false if message not found</returns>
    public async Task<bool> MarkAsReadAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = await _context.ContactMessages.FindAsync(new object[] { id }, cancellationToken);
            if (message == null)
                return false;

            message.MarkAsRead();
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error marking contact message {id} as read", ex);
        }
    }

    /// <summary>
    /// Marks a contact message as unread.
    /// </summary>
    /// <param name="id">The ID of the message to mark as unread</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if updated successfully, false if message not found</returns>
    public async Task<bool> MarkAsUnreadAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = await _context.ContactMessages.FindAsync(new object[] { id }, cancellationToken);
            if (message == null)
                return false;

            message.MarkAsUnread();
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error marking contact message {id} as unread", ex);
        }
    }

    /// <summary>
    /// Marks multiple contact messages as read.
    /// </summary>
    /// <param name="ids">Collection of message IDs to mark as read</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of messages successfully updated</returns>
    public async Task<int> MarkMultipleAsReadAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        try
        {
            var idList = ids.ToList();
            if (!idList.Any())
                return 0;

            var messages = await _context.ContactMessages
                .Where(cm => idList.Contains(cm.Id) && !cm.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var message in messages)
            {
                message.MarkAsRead();
            }

            await _context.SaveChangesAsync(cancellationToken);
            return messages.Count;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error marking multiple contact messages as read", ex);
        }
    }

    /// <summary>
    /// Deletes a contact message from the repository.
    /// </summary>
    /// <param name="id">The ID of the message to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if deleted successfully, false if message not found</returns>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = await _context.ContactMessages.FindAsync(new object[] { id }, cancellationToken);
            if (message == null)
                return false;

            _context.ContactMessages.Remove(message);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error deleting contact message with ID {id}", ex);
        }
    }

    /// <summary>
    /// Deletes multiple contact messages from the repository.
    /// </summary>
    /// <param name="ids">Collection of message IDs to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of messages successfully deleted</returns>
    public async Task<int> DeleteMultipleAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        try
        {
            var idList = ids.ToList();
            if (!idList.Any())
                return 0;

            var messages = await _context.ContactMessages
                .Where(cm => idList.Contains(cm.Id))
                .ToListAsync(cancellationToken);

            _context.ContactMessages.RemoveRange(messages);
            await _context.SaveChangesAsync(cancellationToken);
            return messages.Count;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error deleting multiple contact messages", ex);
        }
    }

    /// <summary>
    /// Deletes old contact messages based on age.
    /// </summary>
    /// <param name="olderThanDays">Delete messages older than this many days</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of messages deleted</returns>
    public async Task<int> DeleteOldMessagesAsync(int olderThanDays, CancellationToken cancellationToken = default)
    {
        if (olderThanDays < 0)
            throw new ArgumentException("Days must be non-negative", nameof(olderThanDays));

        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
            var oldMessages = await _context.ContactMessages
                .Where(cm => cm.SentAt < cutoffDate)
                .ToListAsync(cancellationToken);

            _context.ContactMessages.RemoveRange(oldMessages);
            await _context.SaveChangesAsync(cancellationToken);
            return oldMessages.Count;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error deleting old contact messages older than {olderThanDays} days", ex);
        }
    }

    /// <summary>
    /// Gets the total count of contact messages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total number of contact messages</returns>
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.ContactMessages.CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error getting contact message count", ex);
        }
    }

    /// <summary>
    /// Gets the count of unread contact messages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of unread contact messages</returns>
    public async Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.ContactMessages.CountAsync(cm => !cm.IsRead, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error getting unread contact message count", ex);
        }
    }

    /// <summary>
    /// Gets contact message statistics for a date range.
    /// </summary>
    /// <param name="startDate">Start of the date range</param>
    /// <param name="endDate">End of the date range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Tuple containing (total, read, unread) counts</returns>
    public async Task<(int Total, int Read, int Unread)> GetStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date cannot be before start date");

        try
        {
            var messages = await _context.ContactMessages
                .Where(cm => cm.SentAt >= startDate && cm.SentAt <= endDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var total = messages.Count;
            var read = messages.Count(cm => cm.IsRead);
            var unread = total - read;

            return (total, read, unread);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting contact message statistics between {startDate} and {endDate}", ex);
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _context?.Dispose();
    }
}

