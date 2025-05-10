namespace SteamHub.Web;

public class NoSslCertificateValidationHandler : HttpClientHandler
{
    public NoSslCertificateValidationHandler()
    {
        // Set callback once here
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
}
