using System.ComponentModel.DataAnnotations;

namespace WebsiteApi.Models;

public class Repository
{
    [Key]
    public int RepositoryId { get; set; }
    
    public string Name { get; set; }
    
    public int GitHubId { get; set; }
    
    public string Description { get; set; }
    
    public string HtmlUrl { get; set; }
    
    
    public int UserId { get; set; }
    public User User { get; set; }
}