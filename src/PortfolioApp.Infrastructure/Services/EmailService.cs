using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PortfolioApp.Application.DTOs;

namespace PortfolioApp.Infrastructure.Services;

/// <summary>
/// Service for sending emails, such as contact form submissions or notifications.
/// This service abstracts the email sending logic and integrates with an SMTP server.
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    /// <summary>
    /// Initializes a new instance of the EmailService class.
    /// </summary>
    /// <param name="emailSettings">Email configuration settings.</param>
    public EmailService(EmailSettings emailSettings)
    {
        _emailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));

        // Basic validation of email settings
        if (string.IsNullOrWhiteSpace(_emailSettings.SmtpServer) ||
            _emailSettings.SmtpPort <= 0 ||
            string.IsNullOrWhiteSpace(_emailSettings.SenderEmail) ||
            string.IsNullOrWhiteSpace(_emailSettings.SenderName) ||
            string.IsNullOrWhiteSpace(_emailSettings.Password))
        {
            throw new ArgumentException("Invalid email settings provided.");
        }
    }

    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="toEmail">Recipient's email address.</param>
    /// <param name="toName">Recipient's name.</param>
    /// <param name="subject">Email subject.</param>
    /// <param name="body">Email body (HTML or plain text).</param>
    /// <param name="isHtml">True if the body is HTML, false for plain text.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if email sent successfully, false otherwise.</returns>
    public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            if (isHtml)
            {
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = body
                };
            }
            else
            {
                message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                {
                    Text = body
                };
            }

            using (var client = new SmtpClient())
            {
                // For demo purposes, accept all SSL certificates. In production, use proper validation.
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password, cancellationToken);
                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
            }
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception in a real application
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Sends a contact form submission email to the administrator.
    /// </summary>
    /// <param name="dto">The contact message DTO.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if email sent successfully, false otherwise.</returns>
    public async Task<bool> SendContactFormEmailAsync(CreateContactMessageDto dto, CancellationToken cancellationToken = default)
    {
        var subject = $"New Contact Message from {dto.SenderName}: {dto.Subject}";
        var body = $"<h1>New Contact Message</h1>"
                   + $"<p><strong>From:</strong> {dto.SenderName} ({dto.SenderEmail})</p>"
                   + $"<p><strong>Subject:</strong> {dto.Subject}</p>"
                   + $"<p><strong>Message:</strong></p>"
                   + $"<p>{dto.Message.Replace(Environment.NewLine, "<br/>")}</p>";

        return await SendEmailAsync(
            _emailSettings.AdminEmail, // Send to admin
            _emailSettings.SenderName, // Admin's name
            subject,
            body,
            isHtml: true,
            cancellationToken
        );
    }

    /// <summary>
    /// Sends a response email to the contact message sender.
    /// </summary>
    /// <param name="responseDto">The contact message response DTO.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if email sent successfully, false otherwise.</returns>
    public async Task<bool> SendContactResponseEmailAsync(ContactMessageResponseDto responseDto, CancellationToken cancellationToken = default)
    {
        var body = $"<h1>Your Message Response</h1>"
                   + $"<p>Dear {responseDto.ToName},</p>"
                   + $"<p>{responseDto.ResponseMessage.Replace(Environment.NewLine, "<br/>")}</p>"
                   + $"<hr>"
                   + $"<p><strong>Original Message:</strong></p>"
                   + $"<p>{responseDto.OriginalMessage.Replace(Environment.NewLine, "<br/>")}</p>";

        return await SendEmailAsync(
            responseDto.ToEmail,
            responseDto.ToName,
            responseDto.Subject,
            body,
            isHtml: true,
            cancellationToken
        );
    }
}

/// <summary>
/// Interface for email service operations.
/// </summary>
public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
    Task<bool> SendContactFormEmailAsync(CreateContactMessageDto dto, CancellationToken cancellationToken = default);
    Task<bool> SendContactResponseEmailAsync(ContactMessageResponseDto responseDto, CancellationToken cancellationToken = default);
}

/// <summary>
/// Configuration settings for email service.
/// </summary>
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty; // Email to send contact forms to
}

