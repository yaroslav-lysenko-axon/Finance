using System;
using System.Threading.Tasks;
using Authorization.Domain.Exceptions;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services;
using Authorization.Domain.Services.Abstraction;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Authorization.Domain.Tests.Services
{
    public class UserServiceTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;
        private readonly IHashGenerator _hashGenerator;
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _roleService = Substitute.For<IRoleService>();
            _hashGenerator = Substitute.For<IHashGenerator>();

            _roleService.FindByName(Arg.Any<string>()).Returns(Task.FromResult(MockData.GetDefaultRole()));
            _userRepository.FindByEmail(Arg.Any<string>()).Returns(Task.FromResult<User>(null));
            var defaultUser = MockData.GetDefaultUser();
            _hashGenerator.CreateSalt().Returns(defaultUser.Salt);
            _hashGenerator.GenerateHash(Arg.Any<string>(), defaultUser.Salt).Returns(defaultUser.PasswordHash);

            _sut = new UserService(_userRepository, _roleService, _hashGenerator);
        }

        [Fact]
        public void RegisterUser_RoleNotFound_ThrowsRoleNotFoundException()
        {
            // Arrange
            _roleService.FindByName(Arg.Any<string>()).Returns(Task.FromResult<Role>(null));
            var (email, password, firstName, lastName) = GetRegistrationInput();

            // Act
            Func<Task> action = () => _sut.RegisterUser(email, password, firstName, lastName);

            // Assert
            action.Should().ThrowExactly<RoleNotFoundException>();
        }

        [Fact]
        public void RegisterUser_UserWithSuchMailAlreadyExists_ThrowsDuplicateEmailException()
        {
            // Arrange
            _userRepository.FindByEmail(Arg.Any<string>()).Returns(Task.FromResult(MockData.GetDefaultUser()));
            var (email, password, firstName, lastName) = GetRegistrationInput();

            // Act
            Func<Task> action = () => _sut.RegisterUser(email, password, firstName, lastName);

            // Assert
            action.Should().ThrowExactly<DuplicateEmailException>();
        }

        [Fact]
        public void RegisterUser_WithDefaultValues_CreatesARandomSaltViaCallingCreateSalt()
        {
            // Arrange
            var (email, password, firstName, lastName) = GetRegistrationInput();

            // Act
            _sut.RegisterUser(email, password, firstName, lastName);

            // Assert
            _hashGenerator.Received(1).CreateSalt();
        }

        [Fact]
        public void RegisterUser_WithDefaultValues_CallsHashGeneratorForPasswordHashing()
        {
            // Arrange
            var (email, password, firstName, lastName) = GetRegistrationInput();

            // Act
            _sut.RegisterUser(email, password, firstName, lastName);

            // Assert
            _hashGenerator.Received(1).GenerateHash(password, Arg.Any<string>());
        }

        [Fact]
        public void RegisterUser_WithDefaultValues_CallsRepositoryInsert()
        {
            // Arrange
            var (email, password, firstName, lastName) = GetRegistrationInput();

            // Act
            _sut.RegisterUser(email, password, firstName, lastName);

            // Assert
            _userRepository.Received(1).Insert(Arg.Any<User>());
        }

        [Fact]
        public async Task RegisterUser_WithDefaultValues_ReturnsInactiveUser()
        {
            // Arrange
            var (email, password, firstName, lastName) = GetRegistrationInput();

            // Act
            var user = await _sut.RegisterUser(email, password, firstName, lastName);

            // Assert
            user.Active.Should().BeFalse();
        }

        [Fact]
        public async Task RegisterUser_WithDefaultValues_CorrectlyPopulatesOtherUserProperties()
        {
            // Arrange
            var (email, password, firstName, lastName) = GetRegistrationInput();

            // Act
            var user = await _sut.RegisterUser(email, password, firstName, lastName);

            // Assert
            user.Email.Should().Be(email);
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Role.Should().NotBeNull();
        }

        [Fact]
        public void GetUser_WithWrongEmail_ThrowsWrongEmailUserException()
        {
            // Arrange
            var user = MockData.SetupDefaultUser();
            var email = user.Email;

            // Act
            Func<Task> action = async () => await _sut.GetUser(email);

            // Assert
            action.Should().ThrowExactly<UserNotFoundException>();
        }

        [Fact]
        public void GetUser_WithWrongUserId_ThrowsWrongUserIdUserException()
        {
            // Arrange
            var user = MockData.SetupDefaultUser();
            var userId = user.Id;

            // Act
            Func<Task> action = async () => await _sut.GetUserById(userId);

            // Assert
            action.Should().ThrowExactly<UserNotFoundException>();
        }

        [Fact]
        public void ChangePassword_WithDuplicatedPasswords_ThrowsDuplicatePasswordUserException()
        {
            // Arrange
            const string oldPassword = "defaultPassword";
            const string newPassword = "defaultPassword";
            Guid userId = Guid.NewGuid();

            // Act
            Func<Task> action = async () => await _sut.ChangeUserPassword(userId, oldPassword, newPassword);

            // Assert
            action.Should().ThrowExactly<PasswordChangeException>();
        }

        private static (string email, string password, string firstName, string lastName) GetRegistrationInput()
        {
            var user = MockData.SetupDefaultUser();
            var email = user.Email;
            var password = user.PasswordHash;
            var firstName = user.FirstName;
            var lastName = user.LastName;

            return (email, password, firstName, lastName);
        }
    }
}
