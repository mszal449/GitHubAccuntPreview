using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using WebsiteApi.Controllers;

namespace WebsiteApi.Models;

public class User
{
    [Key]
    public int UserId { get; set; }
    
    [Column(TypeName="varchar")]
    public string GitHubId { get; set; }

    [MaxLength(255)]
    public required string GitHubUsername { get; set; }
    
    [MaxLength(255)]
    public required string DisplayName { get; set; }

    [MaxLength(255)]
    [EmailAddress]
    public required string Email { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal CurrencyBalance { get; set; } = 0;

    [MaxLength(255)]
    public string? AccessToken { get; set; }

    public DateTime TokenExpiration { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? LastLogin { get; set; }
    
    
    // Database Relations
    public virtual ICollection<GitHubActivity> GitHubActivities { get; set; } = new List<GitHubActivity>();
    public virtual ICollection<Repository> Repositories { get; set; } = new List<Repository>();
    public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();
    public virtual ICollection<User> Friends { get; set; } = new List<User>();
    public virtual ICollection<User> FriendOf { get; set; } = new List<User>();
    
    public UserController.UserResponse ToUserResponse()
    {
        return new UserController.UserResponse(
            Id: UserId,
            UserName: GitHubUsername,
            Email: Email,
            CurrentBalance: CurrencyBalance
        );
    }
}