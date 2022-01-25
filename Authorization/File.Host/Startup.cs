using Amazon.S3;
using File.Application.Handlers;
using File.Application.Mapping;
using File.Domain.ConfigurationClasses;
using File.Domain.Repositories;
using File.Domain.Services;
using File.Domain.Services.Abstraction;
using File.Host.Middleware;
using File.Infrastructure.Persistence.ConfigurationClasses;
using File.Infrastructure.Persistence.Contexts;
using File.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace File.Host
{
    public class Startup
    {
        private const string AmazonWebServiceName = "AmazonWebService";
        private const string PersistenceSectionName = "Persistence";

        public Startup(
            IWebHostEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        private IWebHostEnvironment HostingEnvironment { get; }
        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAmazonS3>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IAmazonWebServicesS3Configuration>();
                var clientConfig = new AmazonS3Config
                {
                    AuthenticationRegion = configuration.AuthRegion,
                    ServiceURL = configuration.IpAddressAndPort,
                    ForcePathStyle = true,
                };
                return new AmazonS3Client(configuration.AccessKey, configuration.SecretKey, clientConfig);
            });

            AmazonWebServerConfiguration(Configuration, services);
            FilesConfiguration(Configuration, services);
            FilesRepositories(services);
            FilesServices(services);

            // file
            services.AddMediatR(typeof(GetSignedUrlHandler).Assembly);
            services.AddMediatR(typeof(UploadFileHandler).Assembly);
            services.AddMediatR(typeof(DeleteFileHandler).Assembly);
            services.AddMediatR(typeof(UploadMultipleFilesHandler).Assembly);

            services.AddDbContext<FileContext>((sp, options) =>
            {
                var configuration = sp.GetRequiredService<IPersistenceConfiguration>();
                var connectionString = configuration.GetConnectionString();
                options.UseNpgsql(connectionString);
            });

            services.AddAutoMapper(configAction => configAction.AddProfile(new MappingsProfile()), typeof(Startup));
        }

#pragma warning disable CA1822

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<BasicAuthMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

#pragma warning restore CA1822

        private static void AmazonWebServerConfiguration(IConfiguration configuration, IServiceCollection services)
        {
            var amazonWebServicesS3Configuration = new AmazonWebServicesS3Configuration();
            configuration.GetSection(AmazonWebServiceName).Bind(amazonWebServicesS3Configuration);
            services
                .AddSingleton<IAmazonWebServicesS3Configuration>(amazonWebServicesS3Configuration);
        }

        private static void FilesConfiguration(IConfiguration configuration, IServiceCollection services)
        {
            var persistenceConfiguration = new PersistenceConfiguration();
            configuration.GetSection(PersistenceSectionName).Bind(persistenceConfiguration);

            services
                .AddSingleton<IPersistenceConfiguration>(persistenceConfiguration);
        }

        private static void FilesRepositories(IServiceCollection services)
        {
            services
                .AddScoped<IFileRepository, FileRepository>()
                .AddScoped<IImageDetailsRepository, ImageDetailsRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void FilesServices(IServiceCollection services)
        {
            services
                .AddScoped<IFileService, FileService>()
                .AddScoped<IImageDetailsService, ImageDetailsService>()
                .AddScoped<ITimeProvider, TimeProvider>();
        }
    }
}
