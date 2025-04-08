namespace Idp.Models;

public class DeviceDetails
{
    public int Id { get; set; }
    public int ApplicationUserId { get; set; }
    public string AuthSession { get; set; }
    public string DevicePublicKey { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneNumberVerified { get; set; }
}
