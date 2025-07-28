using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioApp.Application.DTOs;
using PortfolioApp.Application.UseCases;

namespace PortfolioApp.Web.Pages;

public class IndexModel : PageModel
{
    private readonly UserUseCases _userUseCases;
    private readonly ProjectUseCases _projectUseCases;

    public IndexModel(UserUseCases userUseCases, ProjectUseCases projectUseCases)
    {
        _userUseCases = userUseCases;
        _projectUseCases = projectUseCases;
    }

    public UserDto? User { get; set; }
    public IEnumerable<ProjectDto> FeaturedProjects { get; set; } = new List<ProjectDto>();
    public IEnumerable<ProjectDto> RecentProjects { get; set; } = new List<ProjectDto>();
    public IEnumerable<string> Technologies { get; set; } = new List<string>();

    public async Task OnGetAsync()
    {
        // For demo purposes, get the first user (in a real app, you'd have authentication)
        var users = await _userUseCases.GetAllUsersAsync();
        User = users.FirstOrDefault();

        if (User != null)
        {
            // Get user's profile with projects and technologies
            var profile = await _userUseCases.GetUserProfileAsync(User.Id);
            if (profile != null)
            {
                FeaturedProjects = profile.FeaturedProjects?.Cast<ProjectDto>() ?? new List<ProjectDto>();
                RecentProjects = profile.RecentProjects?.Cast<ProjectDto>() ?? new List<ProjectDto>();
                Technologies = profile.Technologies ?? new List<string>();
            }
        }
    }
}

