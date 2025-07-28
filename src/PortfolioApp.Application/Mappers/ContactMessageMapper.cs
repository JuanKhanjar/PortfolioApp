using PortfolioApp.Application.DTOs;
using PortfolioApp.Domain.Entities;
using PortfolioApp.Domain.ValueObjects;

namespace PortfolioApp.Application.Mappers;

/// <summary>
/// Provides manual mapping functionalities between ContactMessage entities and ContactMessage DTOs.
/// This mapper ensures a clear separation of concerns and avoids direct exposure
/// of domain entities to the presentation layer, adhering to Clean Architecture principles.
/// </summary>
public static class ContactMessageMapper
{
    /// <summary>
    /// Maps a ContactMessage entity to a ContactMessageDto.
    /// This method transforms the domain model into a presentation-friendly DTO.
    /// </summary>
    /// <param name="contactMessage">The ContactMessage entity to map.</param>
    /// <returns>A new ContactMessageDto instance.</returns>
    public static ContactMessageDto ToDto(ContactMessage contactMessage)
    {
        if (contactMessage == null)
        {
            throw new ArgumentNullException(nameof(contactMessage));
        }

        var ageInHours = (int)(DateTime.UtcNow - contactMessage.SentAt).TotalHours;
        var ageInDays = (int)(DateTime.UtcNow - contactMessage.SentAt).TotalDays;

        return new ContactMessageDto
        {
            Id = contactMessage.Id,
            SenderName = contactMessage.SenderName,
            SenderEmail = contactMessage.SenderEmail.Value,
            SenderDomain = ExtractDomain(contactMessage.SenderEmail.Value),
            Subject = contactMessage.Subject,
            Message = contactMessage.Message,
            MessagePreview = TruncateMessage(contactMessage.Message, 100),
            SentAt = contactMessage.SentAt,
            FormattedSentDate = FormatSentDate(contactMessage.SentAt),
            IsRead = contactMessage.IsRead,
            AgeInDays = ageInDays,
            AgeInHours = ageInHours,
            IsUrgent = DetermineUrgency(contactMessage.IsRead, ageInHours),
            Priority = CalculatePriority(contactMessage.IsRead, ageInHours, contactMessage.Subject, contactMessage.Message),
            IsPotentialSpam = DetectPotentialSpam(contactMessage.SenderEmail.Value, contactMessage.Subject, contactMessage.Message),
            MessageLength = contactMessage.Message.Length,
            WordCount = CountWords(contactMessage.Message)
        };
    }

    /// <summary>
    /// Maps a CreateContactMessageDto to a ContactMessage entity.
    /// This method transforms a DTO from the presentation layer into a domain entity.
    /// </summary>
    /// <param name="dto">The CreateContactMessageDto to map.</param>
    /// <returns>A new ContactMessage entity instance.</returns>
    public static ContactMessage ToEntity(CreateContactMessageDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        // Email is a ValueObject, so it needs to be created from the string
        var email = new Email(dto.SenderEmail);

        return new ContactMessage(
            dto.SenderName,
            email,
            dto.Subject,
            dto.Message
        );
    }

    /// <summary>
    /// Maps a ContactMessage entity to a ContactMessageSummaryDto.
    /// This method provides a lightweight DTO for summary views.
    /// </summary>
    /// <param name="contactMessage">The ContactMessage entity to map.</param>
    /// <returns>A new ContactMessageSummaryDto instance.</returns>
    public static ContactMessageSummaryDto ToSummaryDto(ContactMessage contactMessage)
    {
        if (contactMessage == null)
        {
            throw new ArgumentNullException(nameof(contactMessage));
        }

        var ageInHours = (int)(DateTime.UtcNow - contactMessage.SentAt).TotalHours;

        return new ContactMessageSummaryDto
        {
            Id = contactMessage.Id,
            SenderName = contactMessage.SenderName,
            SenderEmail = contactMessage.SenderEmail.Value,
            Subject = contactMessage.Subject,
            MessagePreview = TruncateMessage(contactMessage.Message, 80),
            SentAt = contactMessage.SentAt,
            RelativeTime = FormatRelativeTime(contactMessage.SentAt),
            IsRead = contactMessage.IsRead,
            IsUrgent = DetermineUrgency(contactMessage.IsRead, ageInHours),
            Priority = CalculatePriority(contactMessage.IsRead, ageInHours, contactMessage.Subject, contactMessage.Message)
        };
    }

    /// <summary>
    /// Maps a ContactMessage entity to a ContactMessageResponseDto.
    /// This method creates a DTO for generating response emails.
    /// </summary>
    /// <param name="contactMessage">The ContactMessage entity to map.</param>
    /// <returns>A new ContactMessageResponseDto instance.</returns>
    public static ContactMessageResponseDto ToResponseDto(ContactMessage contactMessage)
    {
        if (contactMessage == null)
        {
            throw new ArgumentNullException(nameof(contactMessage));
        }

        return new ContactMessageResponseDto
        {
            OriginalMessageId = contactMessage.Id,
            ToEmail = contactMessage.SenderEmail.Value,
            ToName = contactMessage.SenderName,
            Subject = GenerateResponseSubject(contactMessage.Subject),
            ResponseMessage = string.Empty, // To be filled by the user
            ResponseTemplate = GenerateResponseTemplate(contactMessage.SenderName),
            OriginalMessage = contactMessage.Message
        };
    }

    /// <summary>
    /// Maps a collection of ContactMessage entities to ContactMessageDto collection.
    /// </summary>
    /// <param name="contactMessages">Collection of ContactMessage entities to map.</param>
    /// <returns>Collection of ContactMessageDto instances.</returns>
    public static IEnumerable<ContactMessageDto> ToDtoCollection(IEnumerable<ContactMessage> contactMessages)
    {
        if (contactMessages == null)
        {
            throw new ArgumentNullException(nameof(contactMessages));
        }

        return contactMessages.Select(ToDto).ToList();
    }

    /// <summary>
    /// Maps a collection of ContactMessage entities to ContactMessageSummaryDto collection.
    /// </summary>
    /// <param name="contactMessages">Collection of ContactMessage entities to map.</param>
    /// <returns>Collection of ContactMessageSummaryDto instances.</returns>
    public static IEnumerable<ContactMessageSummaryDto> ToSummaryDtoCollection(IEnumerable<ContactMessage> contactMessages)
    {
        if (contactMessages == null)
        {
            throw new ArgumentNullException(nameof(contactMessages));
        }

        return contactMessages.Select(ToSummaryDto).ToList();
    }

    /// <summary>
    /// Truncates a message to a specified maximum length.
    /// </summary>
    /// <param name="message">The message to truncate.</param>
    /// <param name="maxLength">The maximum length of the truncated message.</param>
    /// <returns>A truncated message string.</returns>
    private static string TruncateMessage(string message, int maxLength)
    {
        if (string.IsNullOrEmpty(message))
        {
            return string.Empty;
        }

        if (message.Length <= maxLength)
        {
            return message;
        }

        // Find the last space before the max length to avoid cutting words
        var truncated = message.Substring(0, maxLength);
        var lastSpaceIndex = truncated.LastIndexOf(' ');

        if (lastSpaceIndex > 0)
        {
            truncated = truncated.Substring(0, lastSpaceIndex);
        }

        return truncated + "...";
    }

    /// <summary>
    /// Formats the sent date for display purposes.
    /// </summary>
    /// <param name="sentAt">The sent date to format.</param>
    /// <returns>A formatted date string.</returns>
    private static string FormatSentDate(DateTime sentAt)
    {
        var now = DateTime.UtcNow;
        var timeSpan = now - sentAt;

        if (timeSpan.TotalDays < 1)
        {
            return sentAt.ToString("HH:mm");
        }
        else if (timeSpan.TotalDays < 7)
        {
            return sentAt.ToString("ddd HH:mm");
        }
        else if (sentAt.Year == now.Year)
        {
            return sentAt.ToString("MMM dd HH:mm");
        }
        else
        {
            return sentAt.ToString("MMM dd, yyyy");
        }
    }

    /// <summary>
    /// Formats the sent date as relative time (e.g., "2 hours ago").
    /// </summary>
    /// <param name="sentAt">The sent date to format.</param>
    /// <returns>A relative time string.</returns>
    private static string FormatRelativeTime(DateTime sentAt)
    {
        var timeSpan = DateTime.UtcNow - sentAt;

        if (timeSpan.TotalMinutes < 1)
        {
            return "Just now";
        }
        else if (timeSpan.TotalHours < 1)
        {
            var minutes = (int)timeSpan.TotalMinutes;
            return $"{minutes} minute{(minutes == 1 ? "" : "s")} ago";
        }
        else if (timeSpan.TotalDays < 1)
        {
            var hours = (int)timeSpan.TotalHours;
            return $"{hours} hour{(hours == 1 ? "" : "s")} ago";
        }
        else if (timeSpan.TotalDays < 30)
        {
            var days = (int)timeSpan.TotalDays;
            return $"{days} day{(days == 1 ? "" : "s")} ago";
        }
        else if (timeSpan.TotalDays < 365)
        {
            var months = (int)(timeSpan.TotalDays / 30);
            return $"{months} month{(months == 1 ? "" : "s")} ago";
        }
        else
        {
            var years = (int)(timeSpan.TotalDays / 365);
            return $"{years} year{(years == 1 ? "" : "s")} ago";
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

    /// <summary>
    /// Determines if a message is urgent based on read status and age.
    /// </summary>
    /// <param name="isRead">Whether the message has been read.</param>
    /// <param name="ageInHours">Age of the message in hours.</param>
    /// <returns>True if the message is urgent, false otherwise.</returns>
    private static bool DetermineUrgency(bool isRead, int ageInHours)
    {
        // Unread messages older than 24 hours are considered urgent
        return !isRead && ageInHours > 24;
    }

    /// <summary>
    /// Calculates the priority of a message (1 = highest, 5 = lowest).
    /// </summary>
    /// <param name="isRead">Whether the message has been read.</param>
    /// <param name="ageInHours">Age of the message in hours.</param>
    /// <param name="subject">The message subject.</param>
    /// <param name="message">The message content.</param>
    /// <returns>Priority level from 1 to 5.</returns>
    private static int CalculatePriority(bool isRead, int ageInHours, string subject, string message)
    {
        var priority = 3; // Default priority

        // Increase priority for unread messages
        if (!isRead)
        {
            priority -= 1;
        }

        // Increase priority for older messages
        if (ageInHours > 72) // More than 3 days
        {
            priority -= 1;
        }
        else if (ageInHours > 24) // More than 1 day
        {
            priority -= 0; // Keep same priority
        }

        // Increase priority for urgent keywords
        var urgentKeywords = new[] { "urgent", "asap", "important", "emergency", "help", "problem", "issue", "bug", "error" };
        var subjectLower = subject.ToLowerInvariant();
        var messageLower = message.ToLowerInvariant();

        if (urgentKeywords.Any(keyword => subjectLower.Contains(keyword) || messageLower.Contains(keyword)))
        {
            priority -= 1;
        }

        // Decrease priority for potential spam
        if (DetectPotentialSpam(string.Empty, subject, message))
        {
            priority += 2;
        }

        // Ensure priority is within valid range (1-5)
        return Math.Max(1, Math.Min(5, priority));
    }

    /// <summary>
    /// Detects potential spam messages based on common patterns.
    /// </summary>
    /// <param name="email">The sender's email address.</param>
    /// <param name="subject">The message subject.</param>
    /// <param name="message">The message content.</param>
    /// <returns>True if the message is potentially spam, false otherwise.</returns>
    private static bool DetectPotentialSpam(string email, string subject, string message)
    {
        var spamKeywords = new[]
        {
            "free", "win", "winner", "congratulations", "prize", "lottery", "casino",
            "viagra", "pharmacy", "pills", "weight loss", "make money", "work from home",
            "click here", "act now", "limited time", "offer expires", "100% free",
            "no obligation", "risk free", "satisfaction guaranteed"
        };

        var subjectLower = subject.ToLowerInvariant();
        var messageLower = message.ToLowerInvariant();

        // Check for spam keywords
        var spamKeywordCount = spamKeywords.Count(keyword => 
            subjectLower.Contains(keyword) || messageLower.Contains(keyword));

        if (spamKeywordCount >= 2)
        {
            return true;
        }

        // Check for excessive capitalization
        if (subject.Length > 10 && subject.Count(char.IsUpper) > subject.Length * 0.7)
        {
            return true;
        }

        // Check for excessive exclamation marks
        if (subject.Count(c => c == '!') > 3 || message.Count(c => c == '!') > 10)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Counts the number of words in a message.
    /// </summary>
    /// <param name="message">The message to count words in.</param>
    /// <returns>The number of words.</returns>
    private static int CountWords(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return 0;
        }

        return message.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    /// <summary>
    /// Generates a response subject line based on the original subject.
    /// </summary>
    /// <param name="originalSubject">The original message subject.</param>
    /// <returns>A response subject line.</returns>
    private static string GenerateResponseSubject(string originalSubject)
    {
        if (string.IsNullOrWhiteSpace(originalSubject))
        {
            return "Re: Your Message";
        }

        if (originalSubject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
        {
            return originalSubject;
        }

        return $"Re: {originalSubject}";
    }

    /// <summary>
    /// Generates a response template for replying to a contact message.
    /// </summary>
    /// <param name="senderName">The name of the original sender.</param>
    /// <returns>A response template string.</returns>
    private static string GenerateResponseTemplate(string senderName)
    {
        return $"Hi {senderName},\n\nThank you for reaching out to me. I appreciate your message and will get back to you as soon as possible.\n\nBest regards,\n[Your Name]";
    }
}

