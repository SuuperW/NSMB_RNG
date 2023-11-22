namespace AngularTutorial
{
    internal static class CONST
    {
        public const string ApiRotuePrefix = "/asp/";
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container. (default comment)
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            // This is needed for attribute routes to actually be used.
            app.MapControllers();

            app.Run();
        }
    }
}