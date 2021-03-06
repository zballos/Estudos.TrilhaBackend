using Amazon.S3;
using FileToS3Storage.Api.Database;
using FileToS3Storage.Api.Services;
using FileToS3Storage.Api.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FileToS3Storage.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAWSService<IAmazonS3>();
            services.AddSingleton(BindAppSettings());
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileToS3Storage", Version = "v1" });
            });

            services.AddDbContext<FileDbContext>(options => options.UseInMemoryDatabase("FileStorageDb"));
            services.AddScoped<IFileS3Repository, FileS3Repository>();
            services.AddScoped<IAwsS3Service, AwsS3Service>();
            services.AddScoped<IFileS3Service, FileS3Service>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileToS3Storage v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private AppSettingsConfig BindAppSettings()
        {
            var config = new AppSettingsConfig();
            Configuration.Bind("AppSettings", config);
            return config;
        }
    }
}
