// Model nagrody

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebsiteApi.Models;

public class Reward
{
    [Key]
    public int RewardId { get; set; }

    [MaxLength(255)]
    public required string RewardDescription { get; set; }
    
    [Column(TypeName = "decimal(10, 2)")]
    public required decimal RewardAmount { get; set; }
    
    public DateTime GrantedAt { get; set; } = DateTime.Now;
    
    
    public int UserId { get; set; }
    public User User { get; set; }
}