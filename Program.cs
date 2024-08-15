using eBay.ApiClient.Auth.OAuth2;
using eBay.ApiClient.Auth.OAuth2.Model;
using ebay_authy.Helpers;
using Spectre.Console;
using Utils = ebay_authy.Utilities.Utils;

namespace ebay_authy;

class Program
{
    static readonly OAuthEnvironment ENVIRONMENT = OAuthEnvironment.PRODUCTION;
    static readonly IList<string> SCOPES = [
        "https://api.ebay.com/oauth/api_scope"
    ];

    static void Main(string[] args)
    {
        Utils.Greet();
        var keyset = Utils.AskKeyset();

        TryCredentialSetup(keyset);

        var api = new OAuth2Api();
        var appToken = GetApplicationToken(api);
        var authUrl = api.GenerateUserAuthorizationUrl(ENVIRONMENT, SCOPES, "");

        AnsiConsole.MarkupLine("[green]Please log in with the following URL:[/] " + authUrl);
        Utils.AskOpenUrl(authUrl);
    }

    private static void TryCredentialSetup(Keyset keyset)
    {
        var keysetStream = keyset.ConvertToStream();
        var reader = new StreamReader(keysetStream);

        try
        {
            CredentialUtil.Load(reader);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[red bold]Error while setting up credentials:[/] " + e.Message);
            Environment.Exit(1);
        }
    }

    private static string GetApplicationToken(OAuth2Api api)
    {
        OAuthResponse? response = null;
        try
        {
            response = api.GetApplicationToken(ENVIRONMENT, SCOPES);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[red bold]Error while getting application token:[/] " + e.Message);
            Environment.Exit(1);
        }

        if (response == null)
        {
            AnsiConsole.MarkupLine("[red bold]Got no response[/]");
            Environment.Exit(1);
        }

        if (response.ErrorMessage != null)
        {
            AnsiConsole.MarkupLine("[red bold]API error:[/] " + response.ErrorMessage);
            Environment.Exit(1);
        }

        return response.AccessToken.Token;
    }
}
