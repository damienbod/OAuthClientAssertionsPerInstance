using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Controllers;

[Authorize(Policy = "onboardinguserpolicy", AuthenticationSchemes = "onboardinguser")]
[Route("api/[controller]")]
public class OnboardingUserController : Controller
{
    private readonly OnboardingUserService _onboardingUserService;

    public OnboardingUserController(OnboardingUserService onboardingUserService)
    {
        _onboardingUserService = onboardingUserService;
    }

    /// <summary>
    /// Session exists in cache, get from cache and create
    /// </summary>
    [HttpPost("StartEmailVerification")]
    public async Task<IActionResult> StartEmailVerification(string sessionId, string email)
    {
        // TODO
        // AT must be verified and links to the sessionId
        // Probably re-use the cnf claim which needs to be signed then with the mobile public key and not a new one everything
        // better would be a cliam added to the AT when creating the AT
        var at = await HttpContext.GetTokenAsync("access_token");
        _onboardingUserService.ProcessSessionAndEmail(sessionId, email);

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
    public IActionResult StartSmsVerification(string sessionId, string phoneNumber)
    {
        _onboardingUserService.ProcessSessionAndPhoneNumber(sessionId, phoneNumber);

        // TODO
        // Send SMS verification

        return NoContent();
    }

    /// <summary>
    /// Session exists in DB, email is verified, now verify phone number
    /// </summary>
    [HttpPost("VerifyPhoneNumberWithSmsCode")]
    public IActionResult VerifyPhoneNumberWithSmsCode(string sessionId, string code)
    {
        _onboardingUserService.VerifyPhoneNumberWithSmsCode(sessionId, code);

        return NoContent();
    }
}
