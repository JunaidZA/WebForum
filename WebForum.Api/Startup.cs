using Microsoft.EntityFrameworkCore;
using WebForum.Application.Interfaces;
using WebForum.Application.Services;
using WebForum.Domain.Interfaces;
using WebForum.Infrastructure.Context;
using WebForum.Infrastructure.Repositories;

namespace WebForum.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<WebForumDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IPostRepository, PostRepository>();

        services.AddScoped<IPostService, PostService>();

        services.AddControllers();
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        using (var scope = app.ApplicationServices.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WebForumDbContext>();
            db.Database.Migrate();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            if (env.IsDevelopment())
            {
                endpoints.MapOpenApi();
            }
        });

    }
}
