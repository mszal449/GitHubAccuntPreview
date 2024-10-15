using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebsiteApi.Context;
using WebsiteApi.Models;

namespace WebsiteApi.Services;

public class UserService(DataContext context)
{
    private readonly DataContext _context = context;

    public async Task<User?> GetUser(int userId) 
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetUserByGitHubId(string gitHubId)
    {
        var sql = "SELECT * FROM Users WHERE GitHubId = @GitHubId";
        return await _context.Users
            .FromSqlRaw(sql, new SqlParameter("@GitHubId", gitHubId))
            .FirstOrDefaultAsync();
    }



    public async Task<List<User>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddNewUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteUser(User user)
    {
        var userToDelete = await _context.Users.FindAsync(user);
        if (userToDelete == null) return false;
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string?> GetAcccessToken(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.AccessToken == null || DateTime.Now > user.TokenExpiration) return null;
        return user.AccessToken;
    }
    
    public async Task<bool?> SetAccessToken(int userId, string token, int expiresInSeconds)
    {
        // Find the user by ID
        var user = await _context.Users.FindAsync(userId);

        // Check if the user exists
        if (user == null) return null;

        // Update the user's token and expiration
        user.AccessToken = token;
        user.TokenExpiration = DateTime.Now.AddSeconds(expiresInSeconds);

        // Save the changes to the database
        await _context.SaveChangesAsync();

        // Return true to indicate success
        return true;
    }


    
    public async Task<List<User>> GetFriends(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Friends)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return user?.Friends.ToList() ?? new List<User>();
    }
    
    public async Task<List<GitHubActivity>> GetActivities(int userId)
    {
        var user = await _context.Users
            .Include(u => u.GitHubActivities)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return user?.GitHubActivities.ToList() ?? new List<GitHubActivity>();
    }
    
    public async Task<List<Repository>> GetRepositories(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Repositories)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return user?.Repositories.ToList() ?? new List<Repository>();
    }
}