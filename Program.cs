using eBay.ApiClient.Auth.OAuth2;
using eBay.ApiClient.Auth.OAuth2.Model;

namespace ebay_authy;

class Program
{
    static void Main(string[] args)
    {
        CredentialUtil.Load("ebay-config.yml");
        Console.WriteLine("Credentials loaded");

        var env = OAuthEnvironment.PRODUCTION;
        IList<string> scopes = ["https://api.ebay.com/oauth/api_scope"];

        var api = new OAuth2Api();
        var response = api.GetApplicationToken(env, scopes);

        var url = api.GenerateUserAuthorizationUrl(env, scopes, "");
        Console.Write("Log in: ");
        Console.WriteLine(url);

        Console.Write("Enter response code: ");

        var response_code = Console.ReadLine();
        var access_token_request = api.ExchangeCodeForAccessToken(env, response_code);
        var access_token = api.GetAccessToken(env, access_token_request.RefreshToken.Token, scopes);

        Console.WriteLine(access_token.AccessToken.Token);
    }
}
