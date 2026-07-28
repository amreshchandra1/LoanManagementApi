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

        // System Under Test (SUT)
        private LoanController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Instantiate mocks using NSubstitute
            _mockLoanRepository = Substitute.For<ILoan>();
            _mockLogger = Substitute.For<ILogger<Loan>>();
            _mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _mockLogin = Substitute.For<ILogin>();

            // Setup default HttpContext structure to prevent NullReferenceException in Authorization tests
            var httpContext = new DefaultHttpContext();
            _mockHttpContextAccessor.HttpContext.Returns(httpContext);

            // Construct the controller instance
            _controller = new LoanController(
                _mockLogger,
                _mockLoanRepository,
                _mockHttpContextAccessor,
                _mockLogin
            );
        }

        [TestMethod]
        public void UserRegistation_ValidPayload_ReturnsOkResult()
        {
            // Arrange
            var userReg = new UserRegistration {UserName="amresh" };
            // Act
            var result = _controller.UserRegistation(userReg);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            _mockLoanRepository.Received(1).UserRegistation(userReg);
        }

        [TestMethod]
        public void CreateLoanApplication_SuccessfulCreation_ReturnsOkResult()
        {
            // Arrange
            var loanApp = new LoanApplication { PrincipalAmount=1000 };
            string dummyToken = "Bearer text_token";
            string extractedUser = "john_doe";

            // Mocking HttpContext headers access
            _mockHttpContextAccessor.HttpContext.Request.Headers["Authorization"] = dummyToken;
            _mockLogin.ReadJWT(dummyToken).Returns(extractedUser);
            _mockLoanRepository.CreateLoanApplication(loanApp).Returns(1); // Greater than 0 means success

            // Act
            var result = _controller.CreateLoanApplication(loanApp);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(extractedUser, loanApp.UserRegistrationUserName);
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

            _mockLoanRepository.CalculateEmi(principal, rate, tenure).Returns(expectedEmi);

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
            var resultList = actionResult.Value.ToList();
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(targetUser, resultList[0].UserRegistrationUserName);
        }
    }
}
