using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pustakalaya.Data;
using Pustakalaya.DTOs;
using Pustakalaya.Entities;
using Pustakalaya.Services.Interface;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pustakalaya.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return new AuthResponseDto { Success = false, Message = "Email already registered." };
            }

            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                return new AuthResponseDto { Success = false, Message = "Username already taken." };
            }

            //if (model.Role != UserRole.Member)
            //{
            //    return new AuthResponseDto { Success = false, Message = "Only Member role is allowed for registration." };
            //}

            // Generate salt and hash password
            var salt = GenerateSalt();
            var passwordHash = HashPassword(model.Password, salt);

            // Create user
            var user = new User
            {
                Email = model.Email,
                Username = model.Username,
                PasswordHash = passwordHash,
                Salt = salt,
                FullName = model.FullName,
                Role = model.Role,
                RegisteredDate = DateTime.UtcNow,
                RefreshToken = string.Empty
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role.ToString()
                }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto model)
        {
            // Find user by username or email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.UsernameOrEmail || u.Email == model.UsernameOrEmail);

            if (user == null)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid credentials" };
            }

            // Verify password
            var passwordHash = HashPassword(model.Password, user.Salt);
            if (passwordHash != user.PasswordHash)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid credentials" };
            }

            // Generate token
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role.ToString()
                }
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateSalt()
        {
            var randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        private string HashPassword(string password, string salt)
        {
            var combinedPassword = password + salt;
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}