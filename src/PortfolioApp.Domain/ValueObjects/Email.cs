using System.Text.RegularExpressions;

namespace PortfolioApp.Domain.ValueObjects;

/// <summary>
/// Email value object that encapsulates email validation logic.
/// This follows DDD principles by representing email as a value object rather than a primitive string.
/// Value objects are immutable and defined by their attributes rather than identity.
/// </summary>
public class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// The validated email address value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new Email value object.
    /// </summary>
    /// <param name="value">The email address string to validate and encapsulate</param>
    /// <exception cref="ArgumentException">Thrown when the email format is invalid</exception>
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        var trimmedValue = value.Trim();
        
        if (!EmailRegex.IsMatch(trimmedValue))
            throw new ArgumentException($"Invalid email format: {trimmedValue}", nameof(value));

        if (trimmedValue.Length > 254) // RFC 5321 limit
            throw new ArgumentException("Email address is too long", nameof(value));

        Value = trimmedValue.ToLowerInvariant(); // Normalize to lowercase
    }

    /// <summary>
    /// Implicit conversion from string to Email for convenience.
    /// This allows string literals to be automatically converted to Email objects.
    /// </summary>
    /// <param name="email">Email string to convert</param>
    public static implicit operator Email(string email) => new(email);

    /// <summary>
    /// Implicit conversion from Email to string for convenience.
    /// This allows Email objects to be used where strings are expected.
    /// </summary>
    /// <param name="email">Email object to convert</param>
    public static implicit operator string(Email email) => email.Value;

    /// <summary>
    /// Determines whether two Email objects are equal.
    /// Value objects are equal if their values are equal.
    /// </summary>
    /// <param name="other">The other Email object to compare</param>
    /// <returns>True if the emails are equal, false otherwise</returns>
    public bool Equals(Email? other)
    {
        return other is not null && Value == other.Value;
    }

    /// <summary>
    /// Determines whether this Email object is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public override bool Equals(object? obj)
    {
        return obj is Email other && Equals(other);
    }

    /// <summary>
    /// Gets the hash code for this Email object.
    /// Required for proper behavior in collections and dictionaries.
    /// </summary>
    /// <returns>Hash code based on the email value</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Returns the string representation of the email address.
    /// </summary>
    /// <returns>The email address as a string</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Equality operator for Email objects.
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns>True if equal, false otherwise</returns>
    public static bool operator ==(Email? left, Email? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    /// <summary>
    /// Inequality operator for Email objects.
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns>True if not equal, false otherwise</returns>
    public static bool operator !=(Email? left, Email? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Validates an email string without creating an Email object.
    /// Useful for validation scenarios where you don't need the object.
    /// </summary>
    /// <param name="email">Email string to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var trimmedEmail = email.Trim();
        return trimmedEmail.Length <= 254 && EmailRegex.IsMatch(trimmedEmail);
    }

    /// <summary>
    /// Gets the domain part of the email address.
    /// </summary>
    /// <returns>The domain portion of the email</returns>
    public string GetDomain()
    {
        var atIndex = Value.LastIndexOf('@');
        return atIndex >= 0 ? Value.Substring(atIndex + 1) : string.Empty;
    }

    /// <summary>
    /// Gets the local part (username) of the email address.
    /// </summary>
    /// <returns>The local portion of the email</returns>
    public string GetLocalPart()
    {
        var atIndex = Value.LastIndexOf('@');
        return atIndex >= 0 ? Value.Substring(0, atIndex) : Value;
    }
}

