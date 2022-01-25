using System;
using System.Threading.Tasks;
using Authorization.Domain.Exceptions;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;

namespace Authorization.Domain.Services
{
    public class UserService : IUserService
    {
        private const string UserRegistrationRole = "UNCONFIRMED_USER";

        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;
        private readonly IHashGenerator _hashGenerator;

        public UserService(
            IUserRepository userRepository,
            IRoleService roleService,
            IHashGenerator hashGenerator)
        {
            _userRepository = userRepository;
            _roleService = roleService;
            _hashGenerator = hashGenerator;
        }

        public async Task<User> RegisterUser(string email, string password, string firstName, string lastName)
        {
            var role = await _roleService.FindByName(UserRegistrationRole);
            if (role == null)
            {
                throw new RoleNotFoundException(UserRegistrationRole);
            }

            var existingUser = await _userRepository.FindByEmail(email);
            if (existingUser != null)
            {
                throw new DuplicateEmailException(email);
            }

            var salt = _hashGenerator.CreateSalt();
            var passwordHash = _hashGenerator.GenerateHash(password, salt);
            var user = new User
            {
                Email = email,
                PasswordHash = passwordHash,
                Avatar = Guid.NewGuid(),
                Salt = salt,
                FirstName = firstName,
                LastName = lastName,
                Active = false,
                Role = role,
            };

            await _userRepository.Insert(user);

            return user;
        }

        public async Task<User> GetUser(string email, string password)
        {
            var user = await _userRepository.FindByEmail(email);
            if (user == null)
            {
                throw new UserNotFoundException(email);
            }

            var passwordHash = _hashGenerator.GenerateHash(password, user.Salt);
            return user.PasswordHash == passwordHash
                ? user
                : throw new PasswordValidationException();
        }

        public async Task<User> GetUser(string email)
        {
            var user = await _userRepository.FindByEmail(email);
            if (user == null)
            {
                throw new UserNotFoundException(email);
            }

            return user;
        }

        public async Task<User> GetUserById(Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            return user;
        }

        public async Task<User> GetInactiveUser(string email)
        {
            var user = await _userRepository.FindByEmailIgnoreCaseAndActiveFalse(email);
            if (user == null)
            {
                throw new UserNotFoundException(email);
            }

            return user;
        }

        public async Task UpdateUserProfile(User user)
        {
            await _userRepository.UpdateProfile(user);
        }

        public async Task<User> ChangeUserPassword(Guid userId, string oldPassword, string newPassword)
        {
            var userInfo = await _userRepository.FindById(userId);

            if (oldPassword == newPassword)
            {
                throw new PasswordChangeException();
            }

            if (!_hashGenerator.VerifyPassword(oldPassword, userInfo.Salt, userInfo.PasswordHash))
            {
                throw new PasswordValidationException();
            }

            var salt = _hashGenerator.CreateSalt();
            var passwordHash = _hashGenerator.GenerateHash(newPassword, salt);
            var user = new User
            {
                PasswordHash = passwordHash,
                Salt = salt,
            };

            return user;
        }
    }
}
