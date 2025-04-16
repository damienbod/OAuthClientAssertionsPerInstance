using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Controllers;

[Authorize(Policy = "onboardinguserpolicy", AuthenticationSchemes = "onboardinguser")]
[Route("api/[controller]")]
public class AuthorizationChallengeRequestController : Controller
{
    private readonly OnboardingUserService _onboardingUserService;

    public AuthorizationChallengeRequestController(OnboardingUserService onboardingUserService)
    {
        _onboardingUserService = onboardingUserService;
    }

    /// <summary>
    /// Session exists in cache, get from cache and create
    /// </summary>
    [HttpPost("StartEmailVerification")]
    public IActionResult StartEmailVerification(string email)
    {
        string authSession = GetAuthSession();

        _onboardingUserService.ProcessAuthSessionAndEmail(authSession, email);

        // TODO
        // Send email verification email
        // User MUST click link in email on phone
        // This connects the email to the mobile session

        return NoContent();
    }

    /// <summary>
    /// Session exists in DB, email is verified, now verify phone number
    /// </summary>
    [HttpPost("StartSmsVerification")]
    public IActionResult StartSmsVerification(string phoneNumber)
    {
        var authSession = GetAuthSession();
        _onboardingUserService.ProcessAuthSessionAndPhoneNumber(authSession, phoneNumber);

        // TODO
        // Send SMS verification

        return NoContent();
    }

    /// <summary>
    /// Session exists in DB, email is verified, now verify phone number
    /// </summary>
    [HttpPost("VerifyPhoneNumberWithSmsCode")]
    public IActionResult VerifyPhoneNumberWithSmsCode(string code)
    {
        var authSession = GetAuthSession();
        _onboardingUserService.VerifyPhoneNumberWithSmsCode(authSession, code);

        return NoContent();
    }

    private string GetAuthSession()
    {
        var authSessionClaim = User.Claims.FirstOrDefault(c => c.Type == "scope" && c.Value.StartsWith("auth_session"));
        var authSession = authSessionClaim.Value.Replace("auth_session:", "");
        return authSession;
    }
}
