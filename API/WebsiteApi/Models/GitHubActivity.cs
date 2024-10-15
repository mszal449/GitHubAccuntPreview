using System.ComponentModel.DataAnnotations;

namespace WebsiteApi.Models;

public class GitHubActivity
{
    [Key]
    public int Id { get; set; }
    
    public bool Private { get; set; } 
    
    public string RepoName { get; set; }
    
    public int CommitCount { get; set; } = 0;
    
    public DateTime ActivityDate { get; set; }
    

    public int UserId { get; set; }
    public User User { get; set; }
}