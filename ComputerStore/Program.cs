using ComputerStore.Services;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public const string ConnectionString = "server=localhost;port=3306;database=computerstore;user=root;password=Linhdz123@;";
    public const string AuthenticationScheme = "MyCookieAuth";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .WithOrigins("http://localhost:8080")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        builder.Services.AddAuthentication(AuthenticationScheme)
            .AddCookie(AuthenticationScheme, options =>
            {
                options.Cookie.Name = "auth_cookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.LoginPath = "/api/auth/login";
                options.AccessDeniedPath = "/api/auth/denied";
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });
        builder.Services.AddAuthorization();
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
        });
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(
                ConnectionString,
                new MySqlServerVersion(new Version(8, 0, 36))
            );
        });
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.UseStaticFiles();
        app.MapControllers();
        app.Run();
    }
}