namespace OpinionsAnalytics.WksLoadDwh
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.SqlServer;
    using OpinionsAnalytics.Persistence.Context;
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddDbContext<ResenasContext>(options =>
                options.UseSqlServer("Server=.;Database=opinionesClientes;Integrated Security=true;"));
            var host = builder.Build();
            host.Run();
        }
    }
}