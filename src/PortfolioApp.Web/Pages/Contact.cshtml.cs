using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioApp.Application.DTOs;
using PortfolioApp.Application.UseCases;
using PortfolioApp.Infrastructure.Services;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Web.Pages;

public class ContactModel : PageModel
{
    private readonly ContactMessageUseCases _contactMessageUseCases;
    private readonly UserUseCases _userUseCases;
    private readonly IEmailService _emailService;

    public ContactModel(
        ContactMessageUseCases contactMessageUseCases,
        UserUseCases userUseCases,
        IEmailService emailService)
    {
        _contactMessageUseCases = contactMessageUseCases;
        _userUseCases = userUseCases;
        _emailService = emailService;
    }

    [BindProperty]
    public CreateContactMessageDto ContactMessage { get; set; } = new();

    public UserDto? User { get; set; }
    public bool IsSuccess { get; set; }
    public bool HasErrors { get; set; }

    public async Task OnGetAsync()
    {
        // Get the first user for contact information display
        var users = await _userUseCases.GetAllUsersAsync();
        User = users.FirstOrDefault();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Get user for display
        var users = await _userUseCases.GetAllUsersAsync();
        User = users.FirstOrDefault();

        if (!ModelState.IsValid)
        {
            HasErrors = true;
            return Page();
        }

        try
        {
            // Save the contact message to database
            var createdMessage = await _contactMessageUseCases.CreateContactMessageAsync(ContactMessage);

            // Send email notification (optional - don't fail if email fails)
            try
            {
                await _emailService.SendContactFormEmailAsync(ContactMessage);
            }
            catch (Exception ex)
            {
                // Log email error but don't fail the entire operation
                Console.WriteLine($"Failed to send email notification: {ex.Message}");
            }

            IsSuccess = true;
            
            // Clear the form
            ContactMessage = new CreateContactMessageDto();
            
            return Page();
        }
        catch (Exception ex)
        {
            HasErrors = true;
            ModelState.AddModelError(string.Empty, "An error occurred while sending your message. Please try again.");
            Console.WriteLine($"Error creating contact message: {ex.Message}");
            return Page();
        }
    }
}

