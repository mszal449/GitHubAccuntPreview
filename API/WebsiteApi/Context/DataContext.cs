using Microsoft.EntityFrameworkCore;
using WebsiteApi.Controllers;
using WebsiteApi.Models;

namespace WebsiteApi.Context;

public class DataContext : DbContext 
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<GitHubActivity> Activities { get; set; }
    public DbSet<Repository> Repositories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Reward> Rewards { get; set; }
}