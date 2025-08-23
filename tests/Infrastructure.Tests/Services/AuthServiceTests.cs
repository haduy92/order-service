using Application.Contracts.Identity;
using Application.Contracts.Persistence.Identities;
using Application.Models.Auth;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shared.Exceptions;
using Xunit;

namespace Infrastructure.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ISignInManagerWrapper> _signInManagerWrapperMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly AuthServiceTestable _authService;

    public AuthServiceTests()
    {
        // Create mocks for UserManager dependencies
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _signInManagerWrapperMock = new Mock<ISignInManagerWrapper>();
        _tokenServiceMock = new Mock<ITokenService>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

        _authService = new AuthServiceTestable(
            _userManagerMock.Object,
            _signInManagerWrapperMock.Object,
            _tokenServiceMock.Object,
            _refreshTokenRepositoryMock.Object);
    }

    [Fact]
    public async Task WhenSignInAsync_GivenValidCredentials_ThenReturnIdentityResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "ValidPassword123!";
        var user = new ApplicationUser 
        { 
            Id = "test-user-id", 
            Email = email, 
            UserName = email 
        };
        var accessToken = "generated-access-token";
        var refreshTokenDto = new RefreshTokenDto
        {
            RefreshToken = "generated-refresh-token",
            Expires = DateTime.UtcNow.AddDays(7)
        };

        _signInManagerWrapperMock
            .Setup(x => x.PasswordSignInAsync(email, password, false, false))
            .ReturnsAsync(SignInResult.Success)
            .Verifiable();

        _userManagerMock
            .Setup(x => x.FindByNameAsync(email))
            .ReturnsAsync(user)
            .Verifiable();

        _tokenServiceMock
            .Setup(x => x.GenerateAccessToken(user.Id, email))
            .Returns(accessToken)
            .Verifiable();

        _tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns(refreshTokenDto)
            .Verifiable();

        _refreshTokenRepositoryMock
            .Setup(x => x.InsertAsync(It.IsAny<RefreshToken>()))
            .ReturnsAsync((RefreshToken token) => token)
            .Verifiable();

        // Act
        var result = await _authService.SignInAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(refreshTokenDto.RefreshToken);

        _signInManagerWrapperMock.Verify();
        _userManagerMock.Verify();
        _tokenServiceMock.Verify();
        _refreshTokenRepositoryMock.Verify();
        
        _refreshTokenRepositoryMock.Verify(x => x.InsertAsync(It.Is<RefreshToken>(rt => 
            rt.Token == refreshTokenDto.RefreshToken && 
            rt.Expires == refreshTokenDto.Expires)), Times.Once);
    }

    [Fact]
    public async Task WhenSignInAsync_GivenInvalidCredentials_ThenThrowUnauthorizedException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "WrongPassword";

        _signInManagerWrapperMock
            .Setup(x => x.PasswordSignInAsync(email, password, false, false))
            .ReturnsAsync(SignInResult.Failed)
            .Verifiable();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(
            () => _authService.SignInAsync(email, password));
        
        exception.Message.Should().Be("Invalid email or password");

        _signInManagerWrapperMock.Verify();
        _userManagerMock.Verify(x => x.FindByNameAsync(It.IsAny<string>()), Times.Never);
        _tokenServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenServiceMock.Verify(x => x.GenerateRefreshToken(), Times.Never);
        _refreshTokenRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<RefreshToken>()), Times.Never);
    }

    [Fact]
    public async Task WhenSignInAsync_GivenLockedOutAccount_ThenThrowUnauthorizedException()
    {
        // Arrange
        var email = "locked@example.com";
        var password = "ValidPassword123!";

        _signInManagerWrapperMock
            .Setup(x => x.PasswordSignInAsync(email, password, false, false))
            .ReturnsAsync(SignInResult.LockedOut)
            .Verifiable();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(
            () => _authService.SignInAsync(email, password));
        
        exception.Message.Should().Be("Account is locked out");

        _signInManagerWrapperMock.Verify();
        _userManagerMock.Verify(x => x.FindByNameAsync(It.IsAny<string>()), Times.Never);
        _tokenServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenServiceMock.Verify(x => x.GenerateRefreshToken(), Times.Never);
        _refreshTokenRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<RefreshToken>()), Times.Never);
    }

    public void Dispose()
    {
        // Cleanup any resources if needed
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Interface to wrap SignInManager for easier testing
    /// </summary>
    public interface ISignInManagerWrapper
    {
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
    }

    /// <summary>
    /// Testable version of AuthService that accepts an ISignInManagerWrapper instead of concrete SignInManager
    /// </summary>
    public class AuthServiceTestable : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISignInManagerWrapper _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthServiceTestable(
            UserManager<ApplicationUser> userManager,
            ISignInManagerWrapper signInManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<IdentityResponse> SignInAsync(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
            {
                // If login was successful, get the user and their roles.
                var user = await _userManager.FindByNameAsync(email);
                var accessToken = _tokenService.GenerateAccessToken(user!.Id, email);
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _refreshTokenRepository.InsertAsync(new RefreshToken
                {
                    Token = refreshToken.RefreshToken,
                    Expires = refreshToken.Expires
                });

                return new()
                {
                    UserId = user!.Id,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.RefreshToken
                };
            }
            else
            {
                if (result.IsLockedOut)
                {
                    throw new UnauthorizedException("Account is locked out");
                }
                if (result.IsNotAllowed)
                {
                    throw new UnauthorizedException("Account is not allowed");
                }
                throw new UnauthorizedException("Invalid email or password");
            }
        }

        // Implement other IAuthService methods as needed for testing
        public Task<IdentityResponse> SignUpAsync(string email, string password) => throw new NotImplementedException();
        public Task<IdentityResponse> RefreshTokenAsync(string refreshToken) => throw new NotImplementedException();
        public Task SignOutAsync(string userId) => throw new NotImplementedException();
        public Task<UserDto?> GetProfileAsync(string userId) => throw new NotImplementedException();
    }
}
