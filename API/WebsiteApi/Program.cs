using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebsiteApi;
using WebsiteApi.Context;
using WebsiteApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Environment variables, appsettings and secret values
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddUserSecrets<Program>();
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json");
}

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme = "github";
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
    }) // cookie authentication middleware first
    .AddOAuth("github", options =>
        {
            // Oauth authentication middleware is second
            
            // When a user needs to sign in, they will be redirected to the authorize endpoint
            options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
            
            // scopes
            options.Scope.Add("user");

            // After the user signs in, an authorization code will be sent to a callback
            // in this app. The OAuth middleware will intercept it
            options.CallbackPath = new PathString("/github-cb");

            // The OAuth middleware will send the ClientId, ClientSecret, and the
            // authorization code to the token endpoint, and get an access token in return
            options.ClientId = Secrets.ClientId;
            options.ClientSecret = Secrets.ClientSecret; 
            options.TokenEndpoint = "https://github.com/login/oauth/access_token";

            // Below we call the userinfo endpoint to get information about the user
            options.UserInformationEndpoint = "https://api.github.com/user";

            // Describe how to map the user info we receive to user claims
            options.ClaimActions.MapJsonKey("GitHubId", "id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");


            options.Events.OnCreatingTicket = async context =>
            {
                // Get user info from the userinfo endpoint and use it to populate user claims
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Add("Authorization", $"Bearer {context.AccessToken}");

                var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(jsonResponse);
                var user = jsonDoc.RootElement;
                context.Identity?.AddClaim(new Claim("github", "user"));
                
                // authentication complete, now check database for this user
                var userService = context.HttpContext.RequestServices.GetService<UserService>();
                var userId = user.GetProperty("id").ToString();
                // check if the user exists
                var existingUser = await userService?.GetUserByGitHubId(userId)!;

                if (existingUser is null || context.AccessToken is null)
                {
                    return;
                } 
                
                await userService.SetAccessToken(existingUser.UserId, context.AccessToken, 60 * 60 * 8);
                
                context.Identity?.AddClaim(new Claim("id", existingUser.UserId.ToString()));
                context.RunClaimActions(user);
            };
        }
    );

builder.Services.AddAuthorization(b =>
{
    b.AddPolicy("github-enabled", pb =>
    {
        pb.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireClaim("github", "user")
            .RequireAuthenticatedUser();
    });
});

builder.Services.AddControllers();
builder.Services.AddScoped<UserService>();
builder.Services.AddHttpClient();
    
// Database connection string
var connection = builder.Environment.IsDevelopment() 
    ? builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING") 
    : Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");


// Register DbContext
builder.Services.AddDbContext<DataContext>(optionsBuilder =>
{
    optionsBuilder.UseSqlServer(connection, options => options.EnableRetryOnFailure());
});


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
// Use CORS
// app.UseCors();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();



// Simple API routes
app.MapGet("/", (HttpContext context) =>
{
    if (context.User.Identity?.IsAuthenticated ?? false) // Check for authenticated user
    {
        // Return the user's claims as JSON
        return Results.Json(context.User.Claims.Select(c => new { c.Type, c.Value }));
    }

    // Return a message indicating the user is not logged in
    return Results.Text("not logged in");
});

app.MapGet("/gh", (HttpContext context) =>
{
    var response = new
    {
        message = "private route",
        user = new
        {
            id = context.User.FindFirstValue("id"),
            githubUserName = context.User.FindFirstValue(ClaimTypes.Name)
        }
    };

    return Results.Json(response);
}).RequireAuthorization("github-enabled");


app.MapGet("/github/info", async (HttpContext ctx) =>
{
    var user = ctx.User;
    var dbId = user.FindFirstValue("id");
    var userIdNumber = int.Parse(dbId); 
    var userService = ctx.RequestServices.GetService<UserService>();
    string? accessToken = await userService.GetAcccessToken(userIdNumber);
    
    var client = new HttpClient();
    using var req = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
    req.Headers.Add("Authorization", $"Bearer {accessToken}");
    req.Headers.Add("User-Agent", "ProfileSummaryApi");
    var res = await client.SendAsync(req);
    res.EnsureSuccessStatusCode();
    
    var content = await res.Content.ReadAsStringAsync();
    var jsonresponse = JsonNode.Parse(content);
    return Results.Json(jsonresponse);

}).RequireAuthorization("github-enabled");

app.MapControllers();
// Use middleware
app.Run();
