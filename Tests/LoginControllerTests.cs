using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using LoanManagementApi.Controllers;
using LoanManagementApi.Repository;

namespace LoanManagementApi.Tests
{
    [TestClass]
    public class LoginControllerTests
    {
        // Mock Dependency
        private ILogin _mockLoginRepository;

        // System Under Test (SUT)
        private LoginController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Create a mock instance of the repository
            _mockLoginRepository = Substitute.For<ILogin>();

            // Inject the mock into the controller
            _controller = new LoginController(_mockLoginRepository);
        }

        [TestMethod]
        public void SignIn_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            string username = "valid_user";
            string password = "SecurePassword123";
            var expectedResult = new OkObjectResult(new { token = "mocked_jwt_token_here" });

            // Setup the mock to return an Ok result when these specific credentials match
            _mockLoginRepository.GenerateToken(username, password).Returns(expectedResult);

            // Act
            var result = _controller.SignIn(username, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            // Verify the repository was called exactly once with the expected parameters
            _mockLoginRepository.Received(1).GenerateToken(username, password);
        }

        [TestMethod]
        public void SignIn_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            string username = "wrong_user";
            string password = "WrongPassword";
            var expectedResult = new UnauthorizedObjectResult("Invalid credentials");

            // Setup the mock to return an Unauthorized result for bad credentials
            _mockLoginRepository.GenerateToken(username, password).Returns(expectedResult);

            // Act
            var result = _controller.SignIn(username, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));

            var unauthorizedResult = result;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
            Assert.AreEqual("Invalid credentials", unauthorizedResult.Value);

            // Verify execution path
            _mockLoginRepository.Received(1).GenerateToken(username, password);
        }
    }
}
