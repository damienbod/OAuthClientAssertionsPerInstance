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
    private string GetPublicKey(string sessionId)
    {
        return _publicKeyService.GetPublicKey(sessionId);

        throw new ArgumentNullException(nameof(sessionId), "something went wrong");
    }

    public void ProcessSessionAndEmail(string sessionId, string email)
    {
        var publicKey = GetPublicKey(sessionId);

        // TODO
        // Add or Update publicKey, sessionId to DB
    }

    internal void ProcessSessionAndPhoneNumber(string sessionId, string phoneNumber)
    {
        var publicKey = GetPublicKey(sessionId);

        // TODO
        // Update DB
    }

    internal void VerifyPhoneNumberWithSmsCode(string sessionId, string code)
    {
        // TODO
        // Update DB
    }
}
