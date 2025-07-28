using PortfolioApp.Application.DTOs;
using PortfolioApp.Application.Mappers;
using PortfolioApp.Domain.Interfaces;

namespace PortfolioApp.Application.UseCases;

/// <summary>
/// Contains use cases for ContactMessage-related operations.
/// This class implements the business logic for contact message management,
/// following the Use Case Driven Design approach and Clean Architecture principles.
/// </summary>
public class ContactMessageUseCases
{
    private readonly IContactMessageRepository _contactMessageRepository;

    /// <summary>
    /// Initializes a new instance of the ContactMessageUseCases class.
    /// </summary>
    /// <param name="contactMessageRepository">Repository for contact message data access.</param>
    public ContactMessageUseCases(IContactMessageRepository contactMessageRepository)
    {
        _contactMessageRepository = contactMessageRepository ?? throw new ArgumentNullException(nameof(contactMessageRepository));
    }

    /// <summary>
    /// Creates a new contact message.
    /// </summary>
    /// <param name="dto">The contact message creation data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The created contact message DTO.</returns>
    public async Task<ContactMessageDto> CreateContactMessageAsync(CreateContactMessageDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        // Validate business rules
        ValidateContactMessageCreation(dto);

        // Map DTO to entity
        var contactMessage = ContactMessageMapper.ToEntity(dto);

        // Save to repository
        var createdMessage = await _contactMessageRepository.AddAsync(contactMessage, cancellationToken);

        // Map back to DTO and return
        return ContactMessageMapper.ToDto(createdMessage);
    }

    /// <summary>
    /// Gets a contact message by ID.
    /// </summary>
    /// <param name="id">The contact message ID.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The contact message DTO if found, null otherwise.</returns>
    public async Task<ContactMessageDto?> GetContactMessageByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var message = await _contactMessageRepository.GetByIdAsync(id, cancellationToken);
        return message != null ? ContactMessageMapper.ToDto(message) : null;
    }

    /// <summary>
    /// Gets all contact messages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of contact message DTOs.</returns>
    public async Task<IEnumerable<ContactMessageDto>> GetAllContactMessagesAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _contactMessageRepository.GetAllAsync(cancellationToken);
        return ContactMessageMapper.ToDtoCollection(messages);
    }

    /// <summary>
    /// Gets all contact messages as summary DTOs.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of contact message summary DTOs.</returns>
    public async Task<IEnumerable<ContactMessageSummaryDto>> GetAllContactMessagesSummaryAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _contactMessageRepository.GetAllAsync(cancellationToken);
        return ContactMessageMapper.ToSummaryDtoCollection(messages);
    }

    /// <summary>
    /// Gets unread contact messages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of unread contact message DTOs.</returns>
    public async Task<IEnumerable<ContactMessageDto>> GetUnreadContactMessagesAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _contactMessageRepository.GetUnreadAsync(cancellationToken);
        return ContactMessageMapper.ToDtoCollection(messages);
    }

    /// <summary>
    /// Gets read contact messages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of read contact message DTOs.</returns>
    public async Task<IEnumerable<ContactMessageDto>> GetReadContactMessagesAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _contactMessageRepository.GetReadAsync(cancellationToken);
        return ContactMessageMapper.ToDtoCollection(messages);
    }

    /// <summary>
    /// Gets urgent contact messages (unread messages older than 24 hours).
    /// </summary>
    /// <param name="urgentThresholdHours">Hours after which unread messages are considered urgent.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of urgent contact message DTOs.</returns>
    public async Task<IEnumerable<ContactMessageDto>> GetUrgentContactMessagesAsync(int urgentThresholdHours = 24, CancellationToken cancellationToken = default)
    {
        var messages = await _contactMessageRepository.GetUrgentAsync(urgentThresholdHours, cancellationToken);
        return ContactMessageMapper.ToDtoCollection(messages);
    }

    /// <summary>
    /// Gets recent contact messages (within the last specified number of days).
    /// </summary>
    /// <param name="days">Number of days to look back.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of recent contact message DTOs.</returns>
    public async Task<IEnumerable<ContactMessageDto>> GetRecentContactMessagesAsync(int days = 7, CancellationToken cancellationToken = default)
    {
        var messages = await _contactMessageRepository.GetRecentAsync(days, cancellationToken);
        return ContactMessageMapper.ToDtoCollection(messages);
    }

    /// <summary>
    /// Gets contact messages from a specific sender.
    /// </summary>
    /// <param name="senderEmail">Email address of the sender.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of contact message DTOs from the specified sender.</returns>
    public async Task<IEnumerable<ContactMessageDto>> GetContactMessagesBySenderAsync(string senderEmail, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(senderEmail))
        {
            throw new ArgumentException("Sender email cannot be null or empty.", nameof(senderEmail));
        }

        var messages = await _contactMessageRepository.GetBySenderEmailAsync(senderEmail, cancellationToken);
        return ContactMessageMapper.ToDtoCollection(messages);
    }

    /// <summary>
    /// Gets contact messages within a specific date range.
    /// </summary>
    /// <param name="startDate">Start of the date range.</param>
    /// <param name="endDate">End of the date range.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of contact message DTOs within the date range.</returns>
    public async Task<IEnumerable<ContactMessageDto>> GetContactMessagesByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var messages = await _contactMessageRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        return ContactMessageMapper.ToDtoCollection(messages);
    }

    /// <summary>
    /// Searches contact messages based on various criteria.
    /// </summary>
    /// <param name="searchDto">The search criteria.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of contact message DTOs matching the criteria.</returns>
    public async Task<IEnumerable<ContactMessageDto>> SearchContactMessagesAsync(ContactMessageSearchDto searchDto, CancellationToken cancellationToken = default)
    {
        if (searchDto == null)
        {
            throw new ArgumentNullException(nameof(searchDto));
        }

        IEnumerable<Domain.Entities.ContactMessage> messages;

        if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
        {
            messages = await _contactMessageRepository.SearchAsync(searchDto.SearchTerm, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(searchDto.SenderEmail))
        {
            messages = await _contactMessageRepository.GetBySenderEmailAsync(searchDto.SenderEmail, cancellationToken);
        }
        else if (searchDto.StartDate.HasValue && searchDto.EndDate.HasValue)
        {
            messages = await _contactMessageRepository.GetByDateRangeAsync(searchDto.StartDate.Value, searchDto.EndDate.Value, cancellationToken);
        }
        else if (searchDto.DaysBack.HasValue)
        {
            messages = await _contactMessageRepository.GetRecentAsync(searchDto.DaysBack.Value, cancellationToken);
        }
        else if (searchDto.IsRead == false)
        {
            messages = await _contactMessageRepository.GetUnreadAsync(cancellationToken);
        }
        else if (searchDto.IsRead == true)
        {
            messages = await _contactMessageRepository.GetReadAsync(cancellationToken);
        }
        else if (searchDto.IsUrgent == true)
        {
            messages = await _contactMessageRepository.GetUrgentAsync(24, cancellationToken);
        }
        else
        {
            messages = await _contactMessageRepository.GetAllAsync(cancellationToken);
        }

        // Apply pagination if requested
        if (searchDto.Skip.HasValue)
        {
            messages = messages.Skip(searchDto.Skip.Value);
        }
        if (searchDto.Take.HasValue)
        {
            messages = messages.Take(searchDto.Take.Value);
        }

        return ContactMessageMapper.ToDtoCollection(messages);
    }

    /// <summary>
    /// Gets contact messages with pagination support.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of messages per page.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Paginated collection of contact message DTOs.</returns>
    public async Task<IEnumerable<ContactMessageDto>> GetPagedContactMessagesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException("Page number must be greater than 0.", nameof(pageNumber));
        }
        if (pageSize < 1)
        {
            throw new ArgumentException("Page size must be greater than 0.", nameof(pageSize));
        }

        var messages = await _contactMessageRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return ContactMessageMapper.ToDtoCollection(messages);
    }

    /// <summary>
    /// Marks a contact message as read.
    /// </summary>
    /// <param name="id">The ID of the message to mark as read.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if updated successfully, false if message not found.</returns>
    public async Task<bool> MarkAsReadAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _contactMessageRepository.MarkAsReadAsync(id, cancellationToken);
    }

    /// <summary>
    /// Marks a contact message as unread.
    /// </summary>
    /// <param name="id">The ID of the message to mark as unread.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if updated successfully, false if message not found.</returns>
    public async Task<bool> MarkAsUnreadAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _contactMessageRepository.MarkAsUnreadAsync(id, cancellationToken);
    }

    /// <summary>
    /// Marks multiple contact messages as read.
    /// </summary>
    /// <param name="ids">Collection of message IDs to mark as read.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Number of messages successfully updated.</returns>
    public async Task<int> MarkMultipleAsReadAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        if (ids == null)
        {
            throw new ArgumentNullException(nameof(ids));
        }

        return await _contactMessageRepository.MarkMultipleAsReadAsync(ids, cancellationToken);
    }

    /// <summary>
    /// Performs bulk actions on contact messages.
    /// </summary>
    /// <param name="actionDto">The bulk action data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Number of messages affected by the action.</returns>
    public async Task<int> PerformBulkActionAsync(BulkContactMessageActionDto actionDto, CancellationToken cancellationToken = default)
    {
        if (actionDto == null)
        {
            throw new ArgumentNullException(nameof(actionDto));
        }

        if (!actionDto.MessageIds.Any())
        {
            return 0;
        }

        switch (actionDto.Action.ToLowerInvariant())
        {
            case "mark_read":
                return await _contactMessageRepository.MarkMultipleAsReadAsync(actionDto.MessageIds, cancellationToken);

            case "delete":
                return await _contactMessageRepository.DeleteMultipleAsync(actionDto.MessageIds, cancellationToken);

            default:
                throw new ArgumentException($"Unknown action: {actionDto.Action}", nameof(actionDto.Action));
        }
    }

    /// <summary>
    /// Deletes a contact message by ID.
    /// </summary>
    /// <param name="id">The contact message ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if deleted successfully, false if message not found.</returns>
    public async Task<bool> DeleteContactMessageAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _contactMessageRepository.DeleteAsync(id, cancellationToken);
    }

    /// <summary>
    /// Deletes old contact messages based on age.
    /// </summary>
    /// <param name="olderThanDays">Delete messages older than this many days.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Number of messages deleted.</returns>
    public async Task<int> DeleteOldMessagesAsync(int olderThanDays, CancellationToken cancellationToken = default)
    {
        if (olderThanDays < 0)
        {
            throw new ArgumentException("Days must be non-negative.", nameof(olderThanDays));
        }

        return await _contactMessageRepository.DeleteOldMessagesAsync(olderThanDays, cancellationToken);
    }

    /// <summary>
    /// Gets contact message statistics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Contact message statistics DTO.</returns>
    public async Task<ContactMessageStatsDto> GetContactMessageStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalMessages = await _contactMessageRepository.GetCountAsync(cancellationToken);
        var unreadMessages = await _contactMessageRepository.GetUnreadCountAsync(cancellationToken);
        var readMessages = totalMessages - unreadMessages;

        var urgentMessages = (await _contactMessageRepository.GetUrgentAsync(24, cancellationToken)).Count();
        var todayMessages = (await _contactMessageRepository.GetRecentAsync(1, cancellationToken)).Count();
        var weekMessages = (await _contactMessageRepository.GetRecentAsync(7, cancellationToken)).Count();
        var monthMessages = (await _contactMessageRepository.GetRecentAsync(30, cancellationToken)).Count();

        var readPercentage = totalMessages > 0 ? (double)readMessages / totalMessages * 100 : 0;
        var averageMessagesPerDay = monthMessages / 30.0;

        // Get domain statistics (simplified - would need more complex query in real implementation)
        var allMessages = await _contactMessageRepository.GetAllAsync(cancellationToken);
        var domainStats = allMessages
            .GroupBy(m => ExtractDomain(m.SenderEmail.Value))
            .Select(g => new DomainStatsDto
            {
                Domain = g.Key,
                MessageCount = g.Count(),
                Percentage = totalMessages > 0 ? (double)g.Count() / totalMessages * 100 : 0
            })
            .OrderByDescending(d => d.MessageCount)
            .Take(5)
            .ToList();

        // Get daily statistics for the last 30 days (simplified)
        var dailyStats = new List<DailyMessageStatsDto>();
        for (int i = 29; i >= 0; i--)
        {
            var date = DateTime.UtcNow.Date.AddDays(-i);
            var dayMessages = allMessages.Where(m => m.SentAt.Date == date).ToList();
            dailyStats.Add(new DailyMessageStatsDto
            {
                Date = date,
                MessageCount = dayMessages.Count,
                UnreadCount = dayMessages.Count(m => !m.IsRead),
                FormattedDate = date.ToString("MMM dd")
            });
        }

        return new ContactMessageStatsDto
        {
            TotalMessages = totalMessages,
            UnreadMessages = unreadMessages,
            ReadMessages = readMessages,
            UrgentMessages = urgentMessages,
            TodayMessages = todayMessages,
            WeekMessages = weekMessages,
            MonthMessages = monthMessages,
            AverageMessagesPerDay = averageMessagesPerDay,
            ReadPercentage = readPercentage,
            TopDomains = domainStats,
            DailyStats = dailyStats
        };
    }

    /// <summary>
    /// Gets contact message statistics for a specific date range.
    /// </summary>
    /// <param name="startDate">Start of the date range.</param>
    /// <param name="endDate">End of the date range.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Tuple containing (total, read, unread) counts.</returns>
    public async Task<(int Total, int Read, int Unread)> GetContactMessageStatsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _contactMessageRepository.GetStatisticsAsync(startDate, endDate, cancellationToken);
    }

    /// <summary>
    /// Creates a response DTO for replying to a contact message.
    /// </summary>
    /// <param name="originalMessageId">ID of the original contact message.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Contact message response DTO if original message found, null otherwise.</returns>
    public async Task<ContactMessageResponseDto?> CreateResponseAsync(int originalMessageId, CancellationToken cancellationToken = default)
    {
        var originalMessage = await _contactMessageRepository.GetByIdAsync(originalMessageId, cancellationToken);
        if (originalMessage == null)
        {
            return null;
        }

        return ContactMessageMapper.ToResponseDto(originalMessage);
    }

    /// <summary>
    /// Validates contact message creation business rules.
    /// </summary>
    /// <param name="dto">The contact message creation data.</param>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    private static void ValidateContactMessageCreation(CreateContactMessageDto dto)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.SenderName))
        {
            throw new ArgumentException("Sender name is required.", nameof(dto.SenderName));
        }

        if (string.IsNullOrWhiteSpace(dto.SenderEmail))
        {
            throw new ArgumentException("Sender email is required.", nameof(dto.SenderEmail));
        }

        if (string.IsNullOrWhiteSpace(dto.Subject))
        {
            throw new ArgumentException("Subject is required.", nameof(dto.Subject));
        }

        if (string.IsNullOrWhiteSpace(dto.Message))
        {
            throw new ArgumentException("Message is required.", nameof(dto.Message));
        }

        // Validate field lengths
        if (dto.SenderName.Length > 100)
        {
            throw new ArgumentException("Sender name cannot exceed 100 characters.", nameof(dto.SenderName));
        }

        if (dto.SenderEmail.Length > 254)
        {
            throw new ArgumentException("Sender email cannot exceed 254 characters.", nameof(dto.SenderEmail));
        }

        if (dto.Subject.Length > 200)
        {
            throw new ArgumentException("Subject cannot exceed 200 characters.", nameof(dto.Subject));
        }

        if (dto.Message.Length > 5000)
        {
            throw new ArgumentException("Message cannot exceed 5000 characters.", nameof(dto.Message));
        }

        // Validate minimum lengths
        if (dto.SenderName.Trim().Length < 2)
        {
            throw new ArgumentException("Sender name must be at least 2 characters.", nameof(dto.SenderName));
        }

        if (dto.Subject.Trim().Length < 3)
        {
            throw new ArgumentException("Subject must be at least 3 characters.", nameof(dto.Subject));
        }

        if (dto.Message.Trim().Length < 10)
        {
            throw new ArgumentException("Message must be at least 10 characters.", nameof(dto.Message));
        }

        // Basic email validation
        if (!dto.SenderEmail.Contains("@") || !dto.SenderEmail.Contains("."))
        {
            throw new ArgumentException("Invalid email format.", nameof(dto.SenderEmail));
        }
    }

    /// <summary>
    /// Extracts the domain from an email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <returns>The domain part of the email.</returns>
    private static string ExtractDomain(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return string.Empty;
        }

        var atIndex = email.LastIndexOf('@');
        if (atIndex >= 0 && atIndex < email.Length - 1)
        {
            return email.Substring(atIndex + 1).ToLowerInvariant();
        }

        return string.Empty;
    }
}

