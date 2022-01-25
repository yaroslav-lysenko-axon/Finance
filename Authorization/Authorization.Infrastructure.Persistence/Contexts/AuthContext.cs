using Authorization.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Infrastructure.Persistence.Contexts
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> contextOptions)
            : base(contextOptions)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RoleScope> RoleScopes { get; set; }
        public DbSet<Scope> Scopes { get; set; }
        public DbSet<ConfirmationRequest> ConfirmationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<Role>().HasKey(role => role.Id);
            modelBuilder.Entity<Role>().Property(role => role.Id).HasColumnName("r_id");
            modelBuilder.Entity<Role>().HasIndex(role => role.Name).IsUnique();
            modelBuilder.Entity<Role>().Property(role => role.Name).HasColumnName("name");

            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<User>().HasKey(user => user.Id);
            modelBuilder.Entity<User>().Property(user => user.Id).HasColumnName("u_id");
            modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique();
            modelBuilder.Entity<User>().Property(user => user.Email).HasColumnName("email");
            modelBuilder.Entity<User>().Property(user => user.PasswordHash).HasColumnName("password");
            modelBuilder.Entity<User>().Property(user => user.Avatar).HasColumnName("avatar");
            modelBuilder.Entity<User>().Property(user => user.Salt).HasColumnName("salt");
            modelBuilder.Entity<User>().Property(user => user.FirstName).HasColumnName("first_name");
            modelBuilder.Entity<User>().Property(user => user.LastName).HasColumnName("last_name");
            modelBuilder.Entity<User>().Property(user => user.Active).HasColumnName("active");
            modelBuilder.Entity<User>().Property(user => user.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<User>().Property(user => user.RemovedAt).HasColumnName("removed_at");
            modelBuilder.Entity<User>().HasOne(user => user.Role).WithMany(role => role.Users).HasForeignKey("r_id");

            modelBuilder.Entity<Client>().ToTable("client");
            modelBuilder.Entity<Client>().HasKey(client => client.Id);
            modelBuilder.Entity<Client>().Property(client => client.Id).HasColumnName("c_id");
            modelBuilder.Entity<Client>().HasIndex(client => client.ClientSecret).IsUnique();
            modelBuilder.Entity<Client>().Property(client => client.ClientSecret).HasColumnName("secret");
            modelBuilder.Entity<Client>().HasIndex(client => client.Name).IsUnique();
            modelBuilder.Entity<Client>().Property(client => client.Name).HasColumnName("name");
            modelBuilder.Entity<Client>().Property(client => client.CreatedAt).HasColumnName("created_at");

            modelBuilder.Entity<RefreshToken>().ToTable("refresh_token");
            modelBuilder.Entity<RefreshToken>().HasKey(refreshToken => refreshToken.Id);
            modelBuilder.Entity<RefreshToken>().Property(refreshToken => refreshToken.Id).HasColumnName("rt_id");
            modelBuilder.Entity<RefreshToken>().Property(refreshToken => refreshToken.Token).HasColumnName("refresh_token");
            modelBuilder.Entity<RefreshToken>().Property(refreshToken => refreshToken.ExpireAt).HasColumnName("expire_at");
            modelBuilder.Entity<RefreshToken>().Property(refreshToken => refreshToken.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<RefreshToken>().Property(refreshToken => refreshToken.RevokedAt).HasColumnName("revoked_at");
            modelBuilder.Entity<RefreshToken>().Property(refreshToken => refreshToken.RevokeReason).HasColumnName("revoke_reason");
            modelBuilder.Entity<RefreshToken>().HasOne(refreshToken => refreshToken.Client).WithMany().HasForeignKey("c_id");
            modelBuilder.Entity<RefreshToken>().HasOne(refreshToken => refreshToken.User).WithMany().HasForeignKey("u_id");

            modelBuilder.Entity<RoleScope>().ToTable("role_scope");
            modelBuilder.Entity<RoleScope>().HasKey(roleScope => new { roleScope.RoleId, roleScope.ScopeId });
            modelBuilder.Entity<RoleScope>().Property(roleScope => roleScope.RoleId).HasColumnName("r_id");
            modelBuilder.Entity<RoleScope>().Property(roleScope => roleScope.ScopeId).HasColumnName("s_id");
            modelBuilder.Entity<RoleScope>().HasOne(roleScope => roleScope.Role).WithMany(role => role.RoleScopes).HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<RoleScope>().HasOne(roleScope => roleScope.Scope).WithMany(scope => scope.RoleScopes).HasForeignKey(x => x.ScopeId);

            modelBuilder.Entity<Scope>().ToTable("scope");
            modelBuilder.Entity<Scope>().HasKey(scope => scope.Id);
            modelBuilder.Entity<Scope>().Property(scope => scope.Id).HasColumnName("s_id");

            modelBuilder.Entity<ConfirmationRequest>().ToTable("confirmation_request");
            modelBuilder.Entity<ConfirmationRequest>().HasKey(confirmationRequest => confirmationRequest.Id);
            modelBuilder.Entity<ConfirmationRequest>().Property(confirmationRequest => confirmationRequest.Id).HasColumnName("cr_id");
            modelBuilder.Entity<ConfirmationRequest>().Property(confirmationRequest => confirmationRequest.Subject).HasColumnName("subject");
            modelBuilder.Entity<ConfirmationRequest>().Property(confirmationRequest => confirmationRequest.AdditionalSubject).HasColumnName("additional_subject");
            modelBuilder.Entity<ConfirmationRequest>().Property(confirmationRequest => confirmationRequest.Confirmed).HasColumnName("confirmed");
            modelBuilder.Entity<ConfirmationRequest>().Property(confirmationRequest => confirmationRequest.RequestType).HasColumnName("request_type");
            modelBuilder.Entity<ConfirmationRequest>().Property(confirmationRequest => confirmationRequest.Receiver).HasColumnName("receiver");
            modelBuilder.Entity<ConfirmationRequest>().Property(confirmationRequest => confirmationRequest.RevokedAt).HasColumnName("revoked_at");
            modelBuilder.Entity<ConfirmationRequest>().Property(confirmationRequest => confirmationRequest.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<ConfirmationRequest>().Property(confirmationRequest => confirmationRequest.ExpiredAt).HasColumnName("expired_at");
            modelBuilder.Entity<ConfirmationRequest>().HasOne(confirmationRequest => confirmationRequest.User).WithMany(user => user.ConfirmationRequests).HasForeignKey("u_id");
        }
    }
}
