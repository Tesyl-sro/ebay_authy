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
        "https://api.ebay.com/oauth/api_scope",
        "https://api.ebay.com/oauth/api_scope/sell.account.readonly",
        "https://api.ebay.com/oauth/api_scope/sell.marketing",
        "https://api.ebay.com/oauth/api_scope/sell.inventory",
        "https://api.ebay.com/oauth/api_scope/sell.account",
        "https://api.ebay.com/oauth/api_scope/sell.fulfillment",
        "https://api.ebay.com/oauth/api_scope/sell.finances",
        "https://api.ebay.com/oauth/api_scope/sell.payment.dispute",
        "https://api.ebay.com/oauth/api_scope/commerce.identity.readonly",
        "https://api.ebay.com/oauth/api_scope/sell.reputation",
        "https://api.ebay.com/oauth/api_scope/commerce.notification.subscription",
        "https://api.ebay.com/oauth/api_scope/sell.stores",
    ];

    static void Main(string[] args)
    {
        Utils.Greet();
        var keyset = Utils.AskKeyset(ENVIRONMENT);

        TryCredentialSetup(keyset);

        var api = new OAuth2Api();
        var authUrl = api.GenerateUserAuthorizationUrl(ENVIRONMENT, SCOPES, "");
        AnsiConsole.MarkupLine("[green]Please log in with the following URL:[/] " + authUrl);
        Utils.AskOpenUrl(authUrl);

        AnsiConsole.MarkupLine("[green]Paste in the URL [bold]after[/] authentication:[/]");
        var redirectUrl = Console.ReadLine();
        var authCode = Utils.ParseAuthCode(redirectUrl!);

        if (authCode == null)
        {
            ErrorAndExit("Could not parse auth code");
        }

        var exchnageResponse = api.ExchangeCodeForAccessToken(ENVIRONMENT, authCode);
        HandleOathResponse(ref exchnageResponse);

        AnsiConsole.MarkupLine("\n[green bold]Success![/]");
        Console.WriteLine();
        Console.WriteLine(exchnageResponse!.AccessToken.Token);
    }

    private static void HandleOathResponse(ref OAuthResponse? response)
    {
        if (response == null)
        {
            AnsiConsole.MarkupLine("[red bold]Got no response[/]");
            Environment.Exit(1);
        }

        if (response.ErrorMessage != null)
        {
            ErrorAndExit("API error", response.ErrorMessage);
        }
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
            ErrorAndExit("Error while setting up credentials", e);
        }
    }

    private static void ErrorAndExit(string message)
    {
        AnsiConsole.MarkupLine("[red bold]" + message + "[/]");
        Environment.Exit(1);
    }

    private static void ErrorAndExit(string prefix, string errorMessage)
    {
        AnsiConsole.MarkupLine("[red bold]" + prefix + "[/]: " + errorMessage);
        Environment.Exit(1);
    }

    private static void ErrorAndExit(string prefix, Exception e)
    {
        ErrorAndExit(prefix, e.Message);
    }
}
