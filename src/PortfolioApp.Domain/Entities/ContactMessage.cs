using PortfolioApp.Domain.ValueObjects;

namespace PortfolioApp.Domain.Entities;

/// <summary>
/// Represents a contact message sent by visitors through the contact form.
/// This entity manages visitor inquiries and provides administrative capabilities
/// for managing and responding to contact requests.
/// </summary>
public class ContactMessage
{
    /// <summary>
    /// Unique identifier for the contact message. Primary key.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Name of the person sending the message.
    /// Required field for identification and response purposes.
    /// </summary>
    public string SenderName { get; private set; } = string.Empty;

    /// <summary>
    /// Email address of the sender. Implemented as a value object for validation.
    /// Required for response and communication purposes.
    /// </summary>
    public Email SenderEmail { get; private set; } = null!;

    /// <summary>
    /// Subject line of the message. Helps categorize and prioritize inquiries.
    /// </summary>
    public string Subject { get; private set; } = string.Empty;

    /// <summary>
    /// The actual message content from the sender.
    /// This is the main body of the inquiry.
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Timestamp when the message was sent/received.
    /// Used for sorting and response time tracking.
    /// </summary>
    public DateTime SentAt { get; private set; }

    /// <summary>
    /// Indicates whether the message has been read by an administrator.
    /// Used for inbox management and notification purposes.
    /// </summary>
    public bool IsRead { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework.
    /// </summary>
    private ContactMessage() { }

    /// <summary>
    /// Creates a new ContactMessage instance with required fields.
    /// This constructor enforces business rules and validates input.
    /// </summary>
    /// <param name="senderName">Name of the message sender</param>
    /// <param name="senderEmail">Email address of the sender</param>
    /// <param name="subject">Subject line of the message</param>
    /// <param name="message">The message content</param>
    /// <exception cref="ArgumentException">Thrown when required fields are invalid</exception>
    public ContactMessage(string senderName, Email senderEmail, string subject, string message)
    {
        if (string.IsNullOrWhiteSpace(senderName))
            throw new ArgumentException("Sender name cannot be empty", nameof(senderName));

        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException("Subject cannot be empty", nameof(subject));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));

        // Validate message length constraints
        if (senderName.Trim().Length > 100)
            throw new ArgumentException("Sender name cannot exceed 100 characters", nameof(senderName));

        if (subject.Trim().Length > 200)
            throw new ArgumentException("Subject cannot exceed 200 characters", nameof(subject));

        if (message.Trim().Length > 5000)
            throw new ArgumentException("Message cannot exceed 5000 characters", nameof(message));

        SenderName = senderName.Trim();
        SenderEmail = senderEmail ?? throw new ArgumentNullException(nameof(senderEmail));
        Subject = subject.Trim();
        Message = message.Trim();
        SentAt = DateTime.UtcNow;
        IsRead = false; // New messages are unread by default
    }

    /// <summary>
    /// Marks the message as read.
    /// This method is used when an administrator views the message.
    /// </summary>
    public void MarkAsRead()
    {
        IsRead = true;
    }

    /// <summary>
    /// Marks the message as unread.
    /// This method can be used to flag messages that need follow-up.
    /// </summary>
    public void MarkAsUnread()
    {
        IsRead = false;
    }

    /// <summary>
    /// Gets the age of the message in days.
    /// Useful for prioritizing older messages or cleanup operations.
    /// </summary>
    /// <returns>Number of days since the message was sent</returns>
    public int GetAgeInDays()
    {
        return (DateTime.UtcNow - SentAt).Days;
    }

    /// <summary>
    /// Gets the age of the message in hours.
    /// Useful for recent message handling and response time tracking.
    /// </summary>
    /// <returns>Number of hours since the message was sent</returns>
    public int GetAgeInHours()
    {
        return (int)(DateTime.UtcNow - SentAt).TotalHours;
    }

    /// <summary>
    /// Determines if the message is considered urgent based on age.
    /// Messages older than 24 hours might be considered urgent for response.
    /// </summary>
    /// <param name="urgentThresholdHours">Hours after which a message is considered urgent</param>
    /// <returns>True if urgent, false otherwise</returns>
    public bool IsUrgent(int urgentThresholdHours = 24)
    {
        return GetAgeInHours() >= urgentThresholdHours && !IsRead;
    }

    /// <summary>
    /// Gets a preview of the message content.
    /// Useful for displaying in lists or notifications.
    /// </summary>
    /// <param name="maxLength">Maximum length of the preview</param>
    /// <returns>Truncated message content</returns>
    public string GetMessagePreview(int maxLength = 100)
    {
        if (string.IsNullOrEmpty(Message))
            return string.Empty;

        if (Message.Length <= maxLength)
            return Message;

        return Message.Substring(0, maxLength).TrimEnd() + "...";
    }

    /// <summary>
    /// Gets the sender's domain from their email address.
    /// Useful for spam detection or sender categorization.
    /// </summary>
    /// <returns>Domain portion of the sender's email</returns>
    public string GetSenderDomain()
    {
        return SenderEmail.GetDomain();
    }

    /// <summary>
    /// Validates the message content for potential spam indicators.
    /// This is a basic implementation that can be enhanced with more sophisticated rules.
    /// </summary>
    /// <returns>True if the message appears to be spam, false otherwise</returns>
    public bool IsPotentialSpam()
    {
        // Basic spam detection rules
        var spamKeywords = new[] { "viagra", "casino", "lottery", "winner", "congratulations", "click here", "free money" };
        var messageContent = (Subject + " " + Message).ToLowerInvariant();

        // Check for excessive capitalization
        var capitalLetters = Message.Count(char.IsUpper);
        var totalLetters = Message.Count(char.IsLetter);
        var capitalRatio = totalLetters > 0 ? (double)capitalLetters / totalLetters : 0;

        if (capitalRatio > 0.5 && totalLetters > 20) // More than 50% capitals in messages longer than 20 letters
            return true;

        // Check for spam keywords
        if (spamKeywords.Any(keyword => messageContent.Contains(keyword)))
            return true;

        // Check for suspicious patterns
        if (Message.Count(c => c == '!') > 5) // Excessive exclamation marks
            return true;

        if (messageContent.Contains("http://") || messageContent.Contains("https://")) // Contains URLs
            return true;

        return false;
    }

    /// <summary>
    /// Gets the message priority based on various factors.
    /// This can be used for sorting and administrative workflow.
    /// </summary>
    /// <returns>Priority level (1 = highest, 5 = lowest)</returns>
    public int GetPriority()
    {
        // High priority for unread urgent messages
        if (!IsRead && IsUrgent())
            return 1;

        // Medium-high priority for unread recent messages
        if (!IsRead && GetAgeInHours() < 24)
            return 2;

        // Medium priority for unread older messages
        if (!IsRead)
            return 3;

        // Low priority for read recent messages
        if (GetAgeInDays() < 7)
            return 4;

        // Lowest priority for old read messages
        return 5;
    }

    /// <summary>
    /// Creates a response template with sender information pre-filled.
    /// This can be used to generate email responses.
    /// </summary>
    /// <returns>Email response template</returns>
    public string CreateResponseTemplate()
    {
        return $"Dear {SenderName},\n\n" +
               $"Thank you for your message regarding \"{Subject}\".\n\n" +
               $"[Your response here]\n\n" +
               $"Best regards,\n" +
               $"[Your name]";
    }

    /// <summary>
    /// Validates that the message meets business rules for acceptance.
    /// This can be used before saving to ensure data quality.
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValid()
    {
        // Check required fields
        if (string.IsNullOrWhiteSpace(SenderName) || 
            string.IsNullOrWhiteSpace(Subject) || 
            string.IsNullOrWhiteSpace(Message))
            return false;

        // Check length constraints
        if (SenderName.Length > 100 || Subject.Length > 200 || Message.Length > 5000)
            return false;

        // Check for potential spam (optional - might want to flag rather than reject)
        // if (IsPotentialSpam())
        //     return false;

        return true;
    }
}

