using FluentValidation;
using LoanManagementApi.Controllers;
using LoanManagementApi.Model;
using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LoanManagementApi.Tests
{
    [TestClass]
    public class LoanControllerTests
    {
        // Mock Dependencies
        private ILoan _mockLoanRepository;
        private ILogger<Loan> _mockLogger;
        private IHttpContextAccessor _mockHttpContextAccessor;
        private ILogin _mockLogin;
        private IHelper _mockHelper;
        // System Under Test (SUT)
        private LoanController _controller;
        private IAuditLog _mockAuditLog;
        private IValidator<UserRegistration> _mockValidator;

        [TestInitialize]
        public void Setup()
        {
            // Instantiate mocks using NSubstitute
            _mockLoanRepository = Substitute.For<ILoan>();
            _mockLogger = Substitute.For<ILogger<Loan>>();
            _mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _mockLogin = Substitute.For<ILogin>();
            _mockHelper= Substitute.For<IHelper>();
            var httpContext = new DefaultHttpContext();
            _mockHttpContextAccessor.HttpContext.Returns(httpContext);
            _mockAuditLog=Substitute.For<IAuditLog>();
            _mockValidator = Substitute.For<IValidator<UserRegistration>>();

            // Construct the controller instance
            _controller = new LoanController(
                _mockLogger,
                _mockLoanRepository,
                _mockHttpContextAccessor,
                _mockLogin,
                _mockHelper,
                _mockAuditLog,
                _mockValidator
            );
        }

        [TestMethod]
        public async Task UserRegistation_ValidPayload_ReturnsOkWithSavedEntity()
        {
            // Arrange
            var userReg = new UserRegistration { UserName = "amresh", Email = "test@test.com" };
            var savedUser = new UserRegistration { Id = 1, UserName = "amresh", Email = "test@test.com" };

            _mockValidator.ValidateAsync(userReg).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));
            _mockLoanRepository.ValidateUserRegistation(userReg).Returns(false);
            _mockLoanRepository.UserRegistation(userReg).Returns(savedUser);

            // Act
            var result = await _controller.UserRegistation(userReg);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;

            // Verify the mock repository and audit methods executed exactly once
            Assert.AreEqual(savedUser, okResult.Value);
            _mockLoanRepository.Received(1).UserRegistation(userReg);
            _mockAuditLog.Received(1).LogAction(
                "New User",
                "New User",
                "User Registation created successfully for user: amresh"
            );
        }

        [TestMethod]
        public void CreateLoanApplication_SuccessfulCreation_ReturnsOkResult()
        {
            // Arrange
            var loanApp = new LoanApplication { PrincipalAmount=1000,UserRegistrationUserName="amresh" };
            string dummyToken = "Bearer text_token";
            string extractedUser = "amresh";

            // Mocking HttpContext headers access
            _mockHttpContextAccessor.HttpContext.Request.Headers["Authorization"] = dummyToken;
            _mockLogin.ReadJWT(dummyToken).Returns(extractedUser);
            _mockLoanRepository.CreateLoanApplication(Arg.Any<LoanApplication>()).Returns(loanApp); 

            // Act
            var result = _controller.CreateLoanApplication(loanApp) as OkObjectResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsNotNull(result.Value);
            _mockLoanRepository.Received(1).CreateLoanApplication(loanApp);
        }

        [TestMethod]
        public void ApproveReject_UpdateSucceeds_ReturnsOkObjectResultWithMessage()
        {
            // Arrange
            var id = Guid.NewGuid();
            var status = LoanStatus.ApplicationSubmitted; // Assumed enum value
            _mockLoanRepository.ApproveReject(id, status).Returns(1);

            // Act
            var result = _controller.ApproveReject(id, status) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual($"Record Updated {id}", result.Value);
        }

        [TestMethod]
        public void ApproveReject_UpdateFails_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var status = LoanStatus.DisbursementPending;
            _mockLoanRepository.ApproveReject(id, status).Returns(0); // 0 records updated

            // Act
            var result = _controller.ApproveReject(id, status) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("No record updated", result.Value);
        }

        [TestMethod]
        public void CalculateEmi_ValidParameters_ReturnsOkObjectResultWithEmiValue()
        {
            // Arrange
            double principal = 50000;
            double rate = 10.5;
            int tenure = 12;
            decimal expectedEmi = 4408.40m;

            _mockHelper.CalculateEmi(principal, rate, tenure).Returns(expectedEmi);

            // Act
            var result = _controller.CalculateEmi(principal, rate, tenure) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedEmi, result.Value);
        }

        [TestMethod]
        public void ViewLoanHistoryByUserName_RecordsExist_ReturnsListOfApplications()
        {
            // Arrange
            string targetUser = "amresh";
            var dataList = new List<LoanApplication>
            {
                new LoanApplication {PrincipalAmount=1000,UserRegistrationUserName=targetUser },
                new LoanApplication {PrincipalAmount=1000 ,UserRegistrationUserName= targetUser }
            };

            _mockLoanRepository.ViewLoanHistoryByUserName(targetUser).Returns(dataList);

            // Act
            var actionResult = _controller.ViewLoanHistoryByUserName(targetUser);

            // Assert
            Assert.IsNotNull(actionResult);
            var okResult = (OkObjectResult)actionResult.Result;
            Assert.IsNotNull(okResult);

            var resultList = (okResult.Value as IEnumerable<LoanApplication>).ToList();

            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(targetUser, resultList[0].UserRegistrationUserName);
        }
    }
}
