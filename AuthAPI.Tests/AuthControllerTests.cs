using AuthAPI.Controller;
using AuthAPI.Models;
using AuthAPI.Models.Request;
using AuthAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AuthAPI.Tests;

public class AuthControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
    private readonly Mock<IJwtTokenService> _jwtTokenService;

    public AuthControllerTests()
    {
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _signInManager = new Mock<SignInManager<ApplicationUser>>(
            _userManager.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            null, null, null, null);

        _jwtTokenService = new Mock<IJwtTokenService>();
    }

    [Fact]
    public async Task Login_ValidUser_ReturnsOkResult()
    {
        // Arrange
        var controller = new AuthController(_userManager.Object, _signInManager.Object, _jwtTokenService.Object);
        var request = new LoginRequest { Email = "test@example.com", Password = "pass123", TraceId = "trace-001", UseCase = "login" };

        var user = new ApplicationUser { Email = request.Email };
        _userManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _signInManager.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
            .ReturnsAsync(SignInResult.Success);
        _jwtTokenService.Setup(x => x.GenerateToken(user, request.TraceId)).Returns("mocked-token");

        // Act
        var result = await controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
}