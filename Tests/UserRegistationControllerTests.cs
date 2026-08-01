using FluentValidation;
using FluentValidation.Results;
using LoanManagementApi.Controllers;
using LoanManagementApi.Model;
using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LoanManagementApi.Tests.Controllers
{
    [TestClass]
    public class UserRegistrationControllerTests
    {
        private ILogger<UserRegistrationController> _subLogger;
        private IValidator<UserRegistration> _subValidator;
        private IUser _subUserRepository;
        private IAuditLog _subAuditRepository;
        private UserRegistrationController _controller;

        [TestInitialize]
        public void Setup()
        {
            // 1. Create substitutes for all dependencies using NSubstitute
            _subLogger = Substitute.For<ILogger<UserRegistrationController>>();
            _subValidator = Substitute.For<IValidator<UserRegistration>>();
            _subUserRepository = Substitute.For<IUser>();
            _subAuditRepository = Substitute.For<IAuditLog>();

            // 2. Instantiate the controller with substitutes
            _controller = new UserRegistrationController(
                _subLogger,
                _subValidator,
                _subUserRepository,
                _subAuditRepository
            );
        }

        [TestMethod]
        public async Task UserRegistration_ShouldReturnBadRequest_WhenValidationFails()
        {
            var invalidUser = new UserRegistration { UserName = "" };

            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("UserName", "Username is required.")
            };
            var validationResult = new ValidationResult(validationErrors);

            _subValidator
                .ValidateAsync(invalidUser, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(validationResult));

            var result = await _controller.UserRegistation(invalidUser);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult");
            Assert.IsNotNull(badRequestResult.Value);

            _subUserRepository.DidNotReceive().UserRegistation(Arg.Any<UserRegistration>());
            _subAuditRepository.DidNotReceive().LogAction(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [TestMethod]
        public async Task UserRegistration_ShouldReturnOk_WhenRegistrationSucceeds()
        {
            var validUser = new UserRegistration { UserName = "Amresh" };
            var expectedResultPayload = validUser;

            _subValidator
                .ValidateAsync(validUser, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new ValidationResult()));

            _subUserRepository.UserRegistation(validUser).Returns(validUser);

            var result = await _controller.UserRegistation(validUser);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.AreEqual(expectedResultPayload, okResult.Value);

            _subAuditRepository.Received(1).LogAction(
                "New User",
                "New User",
                Arg.Is<string>(msg => msg.Contains("Amresh"))
            );
        }

        [TestMethod]
        public async Task UserRegistration_ShouldReturn500InternalServerError_WhenRepositoryReturnsNull()
        {
            var validUser = new UserRegistration { UserName = "Amresh" };

            _subValidator
                .ValidateAsync(validUser, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new ValidationResult()));

            _subUserRepository.UserRegistation(validUser).Returns((object)null!);

            var result = await _controller.UserRegistation(validUser);

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult, "Expected ObjectResult");
            Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Assert.AreEqual("Failed to create User Registation", statusCodeResult.Value);

            _subAuditRepository.DidNotReceive().LogAction(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
