using System.Linq;
using Authorization.Application.Extensions;
using Authorization.Application.Extensions.Abstraction;
using Authorization.Application.Handlers.ConfirmationHandlerFolder;
using Authorization.Application.Handlers.RegistrationHandlerFolder;
using Authorization.Application.Handlers.UserHandlerFolder;
using Authorization.Application.Mapping;
using Authorization.Application.Models.Responses;
using Authorization.Application.Validators;
using Authorization.Domain.ConfigurationClasses;
using Authorization.Domain.Handlers;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services;
using Authorization.Domain.Services.Abstraction;
using Authorization.Host.Middleware;
using Authorization.Host.Policies;
using Authorization.Infrastructure.Logging.ConfigurationClasses;
using Authorization.Infrastructure.Logging.Middleware;
using Authorization.Infrastructure.Persistence.ConfigurationClasses;
using Authorization.Infrastructure.Persistence.Contexts;
using Authorization.Infrastructure.Persistence.Repositories;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IssueTokenHandler = Authorization.Application.Handlers.TokenHandlerFolder.IssueTokenHandler;
using LogoutHandler = Authorization.Application.Handlers.TokenHandlerFolder.LogoutHandler;
using RefreshIssueTokenHandler = Authorization.Application.Handlers.TokenHandlerFolder.RefreshIssueTokenHandler;

namespace Authorization.Host
{
    public class Startup
    {
        private const string PersistenceSectionName = "Persistence";
        private const string LoggingSectionName = "Logging";
        private const string TokensSectionName = "Tokens";
        private const string JwtSectionName = "Jwt";
        private const string EmailSectionName = "Email";
        private const string SendGridSectionName = "SendGrid";

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
            services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance)
                .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<RegisterUserCommandValidator>())
                .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<IssueTokenValidator>())
                .ConfigureApiBehaviorOptions(ConfigureFluentValidationResponse);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("scope-get-own-profile", policy =>
                {
                    policy.RequireClaim("scopes");
                    policy.AddRequirements(new ScopeRequirement("scope-get-own-profile"));
                    policy.AddRequirements(new UserProfileRequirement());
                });
                options.AddPolicy("scope-update-own-profile", policy =>
                {
                    policy.RequireClaim("scopes");
                    policy.AddRequirements(new ScopeRequirement("scope-update-own-profile"));
                    policy.AddRequirements(new UserProfileRequirement());
                });
            });
            services.AddScoped<IAuthorizationHandler, RequireScopeHandler>();
            services.AddScoped<IAuthorizationHandler, UserProfileHandler>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            services.AddHttpContextAccessor();
            RegisterConfiguration(Configuration, services);
            RegisterRepositories(services);
            RegisterServices(services);
            ExtensionServices(services);

            // auth
            services.AddMediatR(typeof(IssueTokenHandler).Assembly);
            services.AddMediatR(typeof(RefreshIssueTokenHandler).Assembly);
            services.AddMediatR(typeof(LogoutHandler).Assembly);

            // registration
            services.AddMediatR(typeof(RegisterUserHandler).Assembly);

            // confirmation
            services.AddMediatR(typeof(ConfirmRegistrationHandler).Assembly);
            services.AddMediatR(typeof(SendPasswordRecoveryEmailHandler).Assembly);
            services.AddMediatR(typeof(ConfirmResendHandler).Assembly);

            // user
            services.AddMediatR(typeof(GetOwnProfileHandler).Assembly);
            services.AddMediatR(typeof(UpdateOwnProfileHandler).Assembly);
            services.AddMediatR(typeof(ChangeOwnPasswordHandler).Assembly);

            services.AddDbContext<AuthContext>((sp, options) =>
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
            app.UseMiddleware<LoggingMiddleware>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

#pragma warning restore CA1822

        private static void RegisterConfiguration(IConfiguration configuration, IServiceCollection services)
        {
            var persistenceConfiguration = new PersistenceConfiguration();
            configuration.GetSection(PersistenceSectionName).Bind(persistenceConfiguration);

            var loggingConfiguration = new LoggingConfiguration();
            configuration.GetSection(LoggingSectionName).Bind(loggingConfiguration);

            var tokensConfiguration = new TokensConfiguration();
            configuration.GetSection(TokensSectionName).Bind(tokensConfiguration);
            configuration.GetSection(TokensSectionName).GetSection(JwtSectionName).Bind(tokensConfiguration.JwtConfiguration);

            var emailConfiguration = new EmailConfiguration();
            configuration.GetSection(EmailSectionName).Bind(emailConfiguration);

            var sendGridConfiguration = new SendGridConfiguration();
            configuration.GetSection(SendGridSectionName).Bind(sendGridConfiguration);

            services
                .AddSingleton<IPersistenceConfiguration>(persistenceConfiguration)
                .AddSingleton<ILoggingConfiguration>(loggingConfiguration)
                .AddSingleton<ITokensConfiguration>(tokensConfiguration)
                .AddSingleton<IEmailConfiguration>(emailConfiguration)
                .AddSingleton<ISendGridConfiguration>(sendGridConfiguration)
                .AddSingleton<IJwtConfiguration>(tokensConfiguration.JwtConfiguration);
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services
                .AddScoped<IClientRepository, ClientRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IRoleScopeRepository, RoleScopeRepository>()
                .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
                .AddScoped<IConfirmationRequestRepository, ConfirmationRequestRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services
                .AddScoped<IClientService, ClientService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IJwtService, JwtService>()
                .AddScoped<IRoleScopeService, RoleScopeService>()
                .AddScoped<IConfirmationRequestService, ConfirmationRequestService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<IRoleService, RoleService>()
                .AddScoped<IPasswordService, PasswordService>()
                .AddScoped<IHashGenerator, Pbkdf2HashGenerator>()
                .AddScoped<ITimeProvider, TimeProvider>();
        }

        private static void ExtensionServices(IServiceCollection services)
        {
            services.AddScoped<ICookiesExtensions, CookiesExtensions>();
        }

        private static void ConfigureFluentValidationResponse(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = c =>
            {
                var errors = c.ModelState.Values.Where(v => v.Errors.Count > 0)
                    .SelectMany(v => v.Errors)
                    .Select(v => v.ErrorMessage);

                var response = new ErrorResponse
                {
                    Code = ErrorCode.ValidationFailed.GetDisplayName(),
                    Message = errors.First() + errors.Last(),
                };

                return new BadRequestObjectResult(response);
            };
        }
    }
}
