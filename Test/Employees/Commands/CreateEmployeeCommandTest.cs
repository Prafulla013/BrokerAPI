using Application.Common.Helper;
using Application.Employees.Commands;
using Common.Enumerations;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Test.Setup;
using Xunit;

namespace Test.Employees.Commands
{
    public class CreateEmployeeCommandTest : BaseTest
    {
        public CreateEmployeeCommandTest() : base(nameof(CreateEmployeeCommandTest) + _dbCount++) { }

        [Fact]
        public async Task Create_ShouldCreateEmployee()
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FirstName = "Emp FName",
                LastName = "Emp LName",
                Email = "emptest@testdomain.com",
                PhoneNumber = "9000888877",
                Username = "emptest",
                Street = "495 Grove Street",
                ZipCode = "10014",
                City = "New york",
                State = "NY",
                HasSystemAccess = true,
                UserType = UserType.Admin,
                BrokerId = SetupBroker.ID1,
                CurrentUser = "Test Current User",
                Subdomain = SetupBroker.SUBDOMAIN
            };
            var cancellationToken = new CancellationToken();
            var identityService = new IdentityService(_userManager, _roleManager, _jwtOptions);
            var stringHelper = new StringHelper(_appOptions);

            var handler = new CreateEmployeeHandler(_dbContext, identityService, stringHelper, null);

            // Act
            var result = await handler.Handle(command, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.Id));
            var dbUser = await _dbContext.UserProfiles.FindAsync(result.Id);
            Assert.NotNull(dbUser);
            Assert.Equal(command.FirstName, dbUser.FirstName);
            Assert.Equal(command.LastName, dbUser.LastName);
            Assert.Equal(command.Email, dbUser.Email);
            Assert.Equal(command.PhoneNumber, dbUser.PhoneNumber);
            Assert.Equal(command.State, dbUser.State);
            Assert.Equal(command.City, dbUser.City);
            Assert.Equal(command.Street, dbUser.Street);
            Assert.Equal(command.ZipCode, dbUser.ZipCode);
            Assert.Equal(command.HasSystemAccess, dbUser.HasSystemAccess);
            Assert.Equal(command.UserType, dbUser.Type);
            Assert.Equal(command.BrokerId, dbUser.BrokerId);
        }

        [Theory]
        [InlineData("", "", "", "", "", "", "", "", "", true, UserType.Employee, 0, "",
                    new string[] { "First name is required.",
                                   "Last name is required.",
                                   "Email is required.",
                                   "Phone number is required.",
                                   "Username is required.",
                                   "Street is required.",
                                   "Zip code is required.",
                                   "City is required.",
                                   "State is required.",
                                   "Invalid user request.",
                                   "Invalid user request.",
                                   "Designation id is required.",
                                   "Speciality is required.",
                                   "Certificate is required."})]
        [InlineData("This is very long first name. Total character length to test is 300. Do not change this value as this value should exceed the total maximum character length of 300 limited for name of broker. These are the random value here after to test the maximum character length. iyjjCu5oGwmMesxESsws - iyjjCu5oGwmMesxESsws",
                    "This is very long last name. Total character length to test is 300. Do not change this value as this value should exceed the total maximum character length of 300 limited for name of broker. These are the random value here after to test the maximum character length. iyjjCu5oGwmMesxESsws - iyjjCu5oGwmMesxESsws",
                    "max-character-length-test-for-email-max-limit-is-100-iyjjCu5oGwmMesxESsws-iyjjCu5oGwmMesxESsws@test100maxcharactermail.com",
                    "99999999999",
                    "maxCharacterLengthTestForUserNameMaxLimitIs100RandomValueHereAfteriyjjCu5oGwmMesxESswsiyjjCu5oGwmMesxESswstest100maxcharactermail",
                    "max character length test for street max limit is 300 random value herefater to exceed 300 characters iyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws",
                    "max character length test for zipcode max limit is 100 random value herefater to exceed 300 characters iyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws",
                    "max character length test for city max limit is 100 random value herefater to exceed 300 characters iyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws",
                    "max character length test for state max limit is 100 random value herefater to exceed 300 characters iyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws iyjjCu5oGwmMesxESsws test100maxcharacteriyjjCu5oGwmMesxESsws",
                     true, UserType.Employee, SetupBroker.ID1, SetupBroker.SUBDOMAIN,
                    new string[] { "Maximum character limit is 300.",
                                   "Maximum character limit is 300.",
                                   "Maximum character limit is 100.",
                                   "Maximum character limit is 10.",
                                   "Maximum character limit is 100.",
                                   "Maximum character limit is 300.",
                                   "Maximum character limit is 100.",
                                   "Maximum character limit is 100.",
                                   "Maximum character limit is 100."})]
        [InlineData("first-name", "last-name", "invaliemailaddress", "invalidno.", "11-invalid-username", "495 Grove Street",
                    "10014", "New York", "NY", true, UserType.Employee, SetupBroker.ID1, SetupBroker.SUBDOMAIN,
                    new string[] { "Invalid email address.",
                                   "Invalid phone number.",
                                   "Invalid userame. Must start with alphabet and can only have alphanumeric.",
                                   "Invalid speciality at index 1.",
                                   "Certificate id is required.",
                                   "Invalid expiration date." })]
        [InlineData("first-name", "last-name", "valid@email.com", "9000000000", "username", "495 Grove Street",
                    "10014", "New York", "NY", false, UserType.Admin, SetupBroker.ID1, SetupBroker.SUBDOMAIN,
                    new string[] { "Invalid user type." })]
        [InlineData("first-name", "last-name", "valid@email.com", "9000000000", "username", "495 Grove Street",
                    "10014", "New York", "NY",  false, UserType.Employee, SetupBroker.ID1, SetupBroker.SUBDOMAIN,
                    new string[] { })]
        public async Task Create_TestValidator(string firstName,
                                               string lastName,
                                               string email,
                                               string phoneNumber,
                                               string username,
                                               string street,
                                               string zipCode,
                                               string city,
                                               string state,
                                               bool hasSystemAccess,
                                               UserType userType,
                                               long brokerId,
                                               string subdomain,
                                               string[] errorMessages)
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                Username = username,
                State = state,
                Street = street,
                City = city,
                ZipCode = zipCode,
                HasSystemAccess = hasSystemAccess,
                UserType = userType,
                BrokerId = brokerId,
                Subdomain = subdomain,
                CurrentUser = "test user",
            };

            var validator = new CreateEmployeeValidator();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await validator.ValidateAsync(command, cancellationToken);

            // Assert
            Assert.NotNull(result);
            if (errorMessages.Length > 0)
            {
                Assert.False(result.IsValid);
                Assert.Equal(errorMessages.Length, result.Errors.Count);

                foreach (var errorMessage in errorMessages)
                {
                    Assert.Contains(result.Errors, a => a.ErrorMessage == errorMessage);
                }
            }
            else
            {
                Assert.True(result.IsValid);
            }
        }
    }
}
