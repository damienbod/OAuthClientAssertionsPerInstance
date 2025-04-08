namespace Idp;

public class OnboardingUserService
{
    private readonly PublicKeyService _publicKeyService;

    public OnboardingUserService(PublicKeyService publicKeyService)
    {
        _publicKeyService = publicKeyService;
    }

    /// <summary>
    /// Get public key from cache
    /// </summary>
    private string GetPublicKey(string authSession)
    {
        return _publicKeyService.GetPublicKey(authSession);

        throw new ArgumentNullException(nameof(authSession), "something went wrong");
    }

    public void ProcessAuthSessionAndEmail(string authSession, string email)
    {
        var publicKey = GetPublicKey(authSession);

        // TODO
        // Add or Update publicKey, auth_session to DB
    }

    internal void ProcessAuthSessionAndPhoneNumber(string authSession, string phoneNumber)
    {
        var publicKey = GetPublicKey(authSession);

        // TODO
        // Update DB
    }

    internal void VerifyPhoneNumberWithSmsCode(string authSession, string code)
    {
        // TODO
        // Update DB
    }
}
