using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioApp.Application.DTOs;
using PortfolioApp.Application.UseCases;

namespace PortfolioApp.Web.Pages.Admin;

public class DashboardModel : PageModel
{
    private readonly UserUseCases _userUseCases;
    private readonly ProjectUseCases _projectUseCases;
    private readonly ContactMessageUseCases _contactMessageUseCases;

    public DashboardModel(
        UserUseCases userUseCases,
        ProjectUseCases projectUseCases,
        ContactMessageUseCases contactMessageUseCases)
    {
        _userUseCases = userUseCases;
        _projectUseCases = projectUseCases;
        _contactMessageUseCases = contactMessageUseCases;
    }

    public int TotalProjects { get; set; }
    public int TotalImages { get; set; }
    public int TotalVideos { get; set; }
    public int UnreadMessages { get; set; }
    public string TotalStorageUsed { get; set; } = "0 MB";
    public string ApplicationUptime { get; set; } = "Unknown";

    public IEnumerable<ProjectDto> RecentProjects { get; set; } = new List<ProjectDto>();
    public IEnumerable<ContactMessageSummaryDto> RecentMessages { get; set; } = new List<ContactMessageSummaryDto>();

    public async Task OnGetAsync()
    {
        try
        {
            // Get the first user (in a real app, you'd use authentication)
            var users = await _userUseCases.GetAllUsersAsync();
            var user = users.FirstOrDefault();

            if (user != null)
            {
                // Get projects statistics
                var allProjects = await _projectUseCases.GetProjectsByUserIdAsync(user.Id);
                TotalProjects = allProjects.Count();

                // Count images and videos
                TotalImages = allProjects.SelectMany(p => p.Images ?? new List<ImageDto>()).Count();
                TotalVideos = allProjects.SelectMany(p => p.Videos ?? new List<VideoDto>()).Count();

                // Get recent projects
                RecentProjects = allProjects.OrderByDescending(p => p.UpdatedAt).Take(5);
            }

            // Get messages statistics
            var allMessages = await _contactMessageUseCases.GetAllContactMessagesSummaryAsync();
            UnreadMessages = allMessages.Count(m => !m.IsRead);
            RecentMessages = allMessages.OrderByDescending(m => m.SentAt).Take(5);

            // Calculate storage usage (simplified)
            var totalSize = CalculateTotalStorageUsed();
            TotalStorageUsed = FormatFileSize(totalSize);

            // Calculate uptime (simplified)
            ApplicationUptime = CalculateUptime();
        }
        catch (Exception ex)
        {
            // Log error and set default values
            Console.WriteLine($"Error loading dashboard: {ex.Message}");
            TotalProjects = 0;
            TotalImages = 0;
            TotalVideos = 0;
            UnreadMessages = 0;
        }
    }

    private long CalculateTotalStorageUsed()
    {
        try
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (Directory.Exists(uploadsPath))
            {
                return GetDirectorySize(uploadsPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calculating storage: {ex.Message}");
        }
        return 0;
    }

    private long GetDirectorySize(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        return directoryInfo.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
    }

    private string FormatFileSize(long sizeInBytes)
    {
        if (sizeInBytes == 0) return "0 B";

        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = sizeInBytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    private string CalculateUptime()
    {
        try
        {
            // This is a simplified uptime calculation
            // In a real application, you might store the start time in a persistent location
            var startTime = DateTime.Now.AddHours(-2); // Assume 2 hours for demo
            var uptime = DateTime.Now - startTime;

            if (uptime.TotalDays >= 1)
            {
                return $"{(int)uptime.TotalDays} days, {uptime.Hours} hours";
            }
            else if (uptime.TotalHours >= 1)
            {
                return $"{uptime.Hours} hours, {uptime.Minutes} minutes";
            }
            else
            {
                return $"{uptime.Minutes} minutes";
            }
        }
        catch
        {
            return "Unknown";
        }
    }
}

