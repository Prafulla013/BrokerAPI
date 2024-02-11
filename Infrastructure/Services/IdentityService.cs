using Application.Common.Interfaces;
using Common.Configurations;
using Common.Enumerations;
using Common.Exceptions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly JwtConfiguration _jwtConfig;
        public IdentityService(UserManager<User> userManager,
                               RoleManager<Role> roleManager,
                               IOptions<JwtConfiguration> jwtOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtConfig = jwtOptions.Value;
        }

        public async Task<bool> ActivateAsync(User dbUser, string token, string password)
        {
            var codeEncodedBytes = WebEncoders.Base64UrlDecode(token);
            var codeEncoded = Encoding.UTF8.GetString(codeEncodedBytes);

            var result = await _userManager.ConfirmEmailAsync(dbUser, codeEncoded);
            if (!result.Succeeded)
            {
                var errorMessage = JsonSerializer.Serialize(result.Errors.Select(s => s.Description).ToArray());
                throw new BadRequestException(errorMessage);
            }

            // Always add password after the email has been confirmed.
            await _userManager.AddPasswordAsync(dbUser, password);

            return result.Succeeded;
        }

        public async Task<bool> ResetPasswordAsync(User dbUser, string token, string password)
        {
            var codeEncodedBytes = WebEncoders.Base64UrlDecode(token);
            var codeEncoded = Encoding.UTF8.GetString(codeEncodedBytes);

            var result = await _userManager.ResetPasswordAsync(dbUser, codeEncoded, password);
            if (!result.Succeeded)
            {
                var errorMessage = JsonSerializer.Serialize(result.Errors.Select(s => s.Description).ToArray());
                throw new BadRequestException(errorMessage);
            }
            return result.Succeeded;
        }

        public async Task<bool> CheckPasswordAsync(User dbUser, string password)
        {
            var result = await _userManager.CheckPasswordAsync(dbUser, password);
            if (!result)
            {
                throw new BadRequestException("Invalid username or password.");
            }
            return result;
        }

        public async Task<bool> CreateAsync(User dbUser, string roleId)
        {
            var dbRole = await _roleManager.FindByIdAsync(roleId);
            if (dbRole == null)
            {
                throw new BadRequestException("Invalid role id.");
            }

            var dbUserbyEmail = await _userManager.FindByEmailAsync(dbUser.Email);
            if (dbUserbyEmail != null)
            {
                throw new BadRequestException("User already registered with email address.");
            }

            await ValidateUsernameThrowExceptionAsync(dbUser);

            var result = await _userManager.CreateAsync(dbUser);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(dbUser, dbRole.Name);
                return true;
            }

            var errorMessage = JsonSerializer.Serialize(result.Errors.Select(s => s.Description).ToArray());
            throw new BadRequestException(errorMessage);
        }

        public async Task<bool> CreateAsync(User dbUser)
        {
            var dbUserbyEmail = await _userManager.FindByEmailAsync(dbUser.Email);
            if (dbUserbyEmail != null)
            {
                throw new BadRequestException("User already registered with email address.");
            }

            await ValidateUsernameThrowExceptionAsync(dbUser);

            var result = await _userManager.CreateAsync(dbUser);
            if (result.Succeeded)
            {
                return true;
            }

            var errorMessage = JsonSerializer.Serialize(result.Errors.Select(s => s.Description).ToArray());
            throw new BadRequestException(errorMessage);
        }

        private async Task ValidateUsernameThrowExceptionAsync(User dbUser)
        {
            var dbUserbyUsername = await _userManager.Users.Include(i => i.Profile)
                                                           .FirstOrDefaultAsync(fd => fd.NormalizedUserName == dbUser.UserName.ToUpper());
            if (dbUserbyUsername != null && dbUserbyUsername.Profile.BrokerId == dbUser.Profile.BrokerId)
            {
                throw new BadRequestException("Username already taken.");
            }
        }

        public async Task<bool> DeleteAsync(User dbUser)
        {
            var result = await _userManager.DeleteAsync(dbUser);
            if (!result.Succeeded)
            {
                var errorMessage = JsonSerializer.Serialize(result.Errors.Select(s => s.Description).ToArray());
                throw new BadRequestException(errorMessage);
            }

            return result.Succeeded;
        }

        public async Task<bool> ReconfirmUserEmailAsync(string userId, string clientUrl, string lastUpdatedBy)
        {
            var dbUser = await _userManager.Users.Include(i => i.Profile).FirstOrDefaultAsync(fd => fd.Id == userId);
            dbUser.ClientUrl = clientUrl;
            dbUser.LastUpdateDateTime = DateTimeOffset.UtcNow;
            dbUser.LastUpdatedBy = lastUpdatedBy;
            dbUser.Activity = ActivityLog.RequestReinvite;
            dbUser.IsActive = false;

            var hasPassword = await _userManager.HasPasswordAsync(dbUser);
            if (hasPassword)
            {
                await _userManager.RemovePasswordAsync(dbUser);
            }

            var result = await _userManager.UpdateAsync(dbUser);
            if (!result.Succeeded)
            {
                var errorMessage = JsonSerializer.Serialize(result.Errors.Select(s => s.Description).ToArray());
                throw new BadRequestException(errorMessage);
            }

            return result.Succeeded;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User dbUser)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(dbUser);
            byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(token);
            var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

            return codeEncoded;
        }

        public async Task<string> GenerateResetPasswordTokenAsync(User dbUser)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(dbUser);
            byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(token);
            var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

            return codeEncoded;
        }

        public async Task<string> GenerateTokenAsync(User dbUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var secretKey = Encoding.UTF8.GetBytes(_jwtConfig.Key);
            var securityKey = new SymmetricSecurityKey(secretKey);
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, $"{Guid.NewGuid()}"),
                new Claim(ClaimTypes.NameIdentifier, $"{dbUser.Id}"),
                new Claim(ClaimTypes.Name, $"{dbUser.Profile.FirstName} {dbUser.Profile.LastName}"),
                new Claim(ClaimTypes.System, $"{dbUser.Profile.BrokerId}")
            };

            var claimsIdentity = new ClaimsIdentity(claims);
            var dbRoles = await _userManager.GetRolesAsync(dbUser);
            claimsIdentity.AddClaims(dbRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtConfig.Audience,
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpireInMinutes),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512),
                Issuer = _jwtConfig.Issuer
            };

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var dbUser = await _userManager.Users.Include(i => i.Profile)
                                                 .FirstOrDefaultAsync(fd => fd.NormalizedEmail == email.ToUpper(), cancellationToken);
            if (dbUser == null)
            {
                throw new NotFoundException("Invalid user request.");
            }
            return dbUser;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var dbUser = await _userManager.FindByIdAsync(id);
            if (dbUser == null)
            {
                throw new NotFoundException("Invalid user request.");
            }
            return dbUser;
        }

        public async Task<User> GetByNameAsync(string username, CancellationToken cancellationToken)
        {
            var dbUser = await _userManager.Users.Include(i => i.Profile)
                                                 .FirstOrDefaultAsync(fd => fd.NormalizedUserName == username.ToUpper(), cancellationToken);
            if (dbUser == null)
            {
                throw new NotFoundException("Invalid user request.");
            }
            return dbUser;
        }

        public async Task<User> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            var dbUser = await _userManager.Users.Include(i => i.Profile)
                                                 .FirstOrDefaultAsync(fd => fd.Id == userId, cancellationToken);
            if (dbUser == null)
            {
                throw new NotFoundException("Invalid user request.");
            }
            return dbUser;
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            var dbRole = await _roleManager.FindByNameAsync(roleName);
            return dbRole;
        }

        public async Task<IList<string>> GetUserRolesAsync(User dbUser)
        {
            var roles = await _userManager.GetRolesAsync(dbUser);
            return roles;
        }

        public async Task<List<Role>> ListRolesAsync(CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.OrderBy(h => h.Priority)
                                                .ToListAsync(cancellationToken);

            return roles;
        }

        public async Task<bool> UpdateAsync(User dbUser)
        {
            var result = await _userManager.UpdateAsync(dbUser);
            if (result.Succeeded)
            {
                return true;
            }

            var errorMessage = JsonSerializer.Serialize(result.Errors.Select(s => s.Description).ToArray());
            throw new BadRequestException(errorMessage);
        }
    }
}
