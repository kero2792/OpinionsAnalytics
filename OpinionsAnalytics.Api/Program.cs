
using Microsoft.EntityFrameworkCore;
using OpinionsAnalytics.Api.Context;
using OpinionsAnalytics.Api.Interface;
using OpinionsAnalytics.Api.Repository;

namespace OpinionsAnalytics.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ComentariosContex>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("OpinionesDatabase")));

            builder.Services.AddScoped<IViewComentariosRepository, ComentariosRepository>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
