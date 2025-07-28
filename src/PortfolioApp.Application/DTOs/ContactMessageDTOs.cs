namespace PortfolioApp.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new contact message.
/// This DTO contains all the required information for contact message creation.
/// </summary>
public class CreateContactMessageDto
{
    /// <summary>
    /// Name of the person sending the message. Required field.
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the sender. Required field.
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// Subject line of the message. Required field.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// The actual message content from the sender. Required field.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for contact message information display.
/// This DTO contains all contact message information formatted for presentation.
/// </summary>
public class ContactMessageDto
{
    /// <summary>
    /// Contact message's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the person who sent the message.
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the sender.
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// Domain part of the sender's email address.
    /// </summary>
    public string SenderDomain { get; set; } = string.Empty;

    /// <summary>
    /// Subject line of the message.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// The full message content.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Preview of the message content (truncated).
    /// </summary>
    public string MessagePreview { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the message was sent/received.
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Formatted sent date for display.
    /// </summary>
    public string FormattedSentDate { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the message has been read by an administrator.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Age of the message in days.
    /// </summary>
    public int AgeInDays { get; set; }

    /// <summary>
    /// Age of the message in hours.
    /// </summary>
    public int AgeInHours { get; set; }

    /// <summary>
    /// Indicates if the message is considered urgent.
    /// </summary>
    public bool IsUrgent { get; set; }

    /// <summary>
    /// Priority level of the message (1 = highest, 5 = lowest).
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Indicates if the message is potentially spam.
    /// </summary>
    public bool IsPotentialSpam { get; set; }

    /// <summary>
    /// Character count of the message.
    /// </summary>
    public int MessageLength { get; set; }

    /// <summary>
    /// Word count of the message.
    /// </summary>
    public int WordCount { get; set; }
}

/// <summary>
/// Data Transfer Object for contact message summary information.
/// This DTO contains minimal contact message information for list displays.
/// </summary>
public class ContactMessageSummaryDto
{
    /// <summary>
    /// Contact message's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the sender.
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the sender.
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// Subject line of the message.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Brief preview of the message content.
    /// </summary>
    public string MessagePreview { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the message was sent.
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Relative time string (e.g., "2 hours ago", "3 days ago").
    /// </summary>
    public string RelativeTime { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the message has been read.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Indicates if the message is urgent.
    /// </summary>
    public bool IsUrgent { get; set; }

    /// <summary>
    /// Priority level for sorting and display.
    /// </summary>
    public int Priority { get; set; }
}

/// <summary>
/// Data Transfer Object for contact message statistics.
/// This DTO contains aggregated statistics about contact messages.
/// </summary>
public class ContactMessageStatsDto
{
    /// <summary>
    /// Total number of contact messages.
    /// </summary>
    public int TotalMessages { get; set; }

    /// <summary>
    /// Number of unread messages.
    /// </summary>
    public int UnreadMessages { get; set; }

    /// <summary>
    /// Number of read messages.
    /// </summary>
    public int ReadMessages { get; set; }

    /// <summary>
    /// Number of urgent messages (unread and old).
    /// </summary>
    public int UrgentMessages { get; set; }

    /// <summary>
    /// Number of messages received today.
    /// </summary>
    public int TodayMessages { get; set; }

    /// <summary>
    /// Number of messages received this week.
    /// </summary>
    public int WeekMessages { get; set; }

    /// <summary>
    /// Number of messages received this month.
    /// </summary>
    public int MonthMessages { get; set; }

    /// <summary>
    /// Average messages per day over the last 30 days.
    /// </summary>
    public double AverageMessagesPerDay { get; set; }

    /// <summary>
    /// Percentage of messages that are read.
    /// </summary>
    public double ReadPercentage { get; set; }

    /// <summary>
    /// Average response time in hours (if tracking responses).
    /// </summary>
    public double? AverageResponseTimeHours { get; set; }

    /// <summary>
    /// Most common sender domains.
    /// </summary>
    public IEnumerable<DomainStatsDto> TopDomains { get; set; } = new List<DomainStatsDto>();

    /// <summary>
    /// Message volume by day for the last 30 days.
    /// </summary>
    public IEnumerable<DailyMessageStatsDto> DailyStats { get; set; } = new List<DailyMessageStatsDto>();
}

/// <summary>
/// Data Transfer Object for domain statistics.
/// This DTO contains statistics about sender email domains.
/// </summary>
public class DomainStatsDto
{
    /// <summary>
    /// Email domain name.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Number of messages from this domain.
    /// </summary>
    public int MessageCount { get; set; }

    /// <summary>
    /// Percentage of total messages from this domain.
    /// </summary>
    public double Percentage { get; set; }
}

/// <summary>
/// Data Transfer Object for daily message statistics.
/// This DTO contains message counts for specific dates.
/// </summary>
public class DailyMessageStatsDto
{
    /// <summary>
    /// Date for the statistics.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Number of messages received on this date.
    /// </summary>
    public int MessageCount { get; set; }

    /// <summary>
    /// Number of unread messages from this date.
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// Formatted date string for display.
    /// </summary>
    public string FormattedDate { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for contact message search and filtering.
/// This DTO contains search criteria for finding specific messages.
/// </summary>
public class ContactMessageSearchDto
{
    /// <summary>
    /// Search term for sender name, subject, or message content.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by sender email address.
    /// </summary>
    public string? SenderEmail { get; set; }

    /// <summary>
    /// Filter by read status (true = read, false = unread, null = all).
    /// </summary>
    public bool? IsRead { get; set; }

    /// <summary>
    /// Filter by urgent status.
    /// </summary>
    public bool? IsUrgent { get; set; }

    /// <summary>
    /// Start date for date range filter.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for date range filter.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Number of days to look back (alternative to date range).
    /// </summary>
    public int? DaysBack { get; set; }

    /// <summary>
    /// Number of results to return (for pagination).
    /// </summary>
    public int? Take { get; set; }

    /// <summary>
    /// Number of results to skip (for pagination).
    /// </summary>
    public int? Skip { get; set; }

    /// <summary>
    /// Sort order (e.g., "date_desc", "date_asc", "priority", "sender").
    /// </summary>
    public string? SortBy { get; set; }
}

/// <summary>
/// Data Transfer Object for bulk operations on contact messages.
/// This DTO contains information for performing bulk actions.
/// </summary>
public class BulkContactMessageActionDto
{
    /// <summary>
    /// Collection of message IDs to perform the action on.
    /// </summary>
    public IEnumerable<int> MessageIds { get; set; } = new List<int>();

    /// <summary>
    /// Action to perform (e.g., "mark_read", "mark_unread", "delete").
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Additional parameters for the action (if needed).
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// Data Transfer Object for contact message response template.
/// This DTO contains information for generating response emails.
/// </summary>
public class ContactMessageResponseDto
{
    /// <summary>
    /// ID of the original contact message.
    /// </summary>
    public int OriginalMessageId { get; set; }

    /// <summary>
    /// Recipient email address (sender of original message).
    /// </summary>
    public string ToEmail { get; set; } = string.Empty;

    /// <summary>
    /// Recipient name.
    /// </summary>
    public string ToName { get; set; } = string.Empty;

    /// <summary>
    /// Subject line for the response email.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Response message content.
    /// </summary>
    public string ResponseMessage { get; set; } = string.Empty;

    /// <summary>
    /// Pre-filled response template.
    /// </summary>
    public string ResponseTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Original message content for reference.
    /// </summary>
    public string OriginalMessage { get; set; } = string.Empty;
}

