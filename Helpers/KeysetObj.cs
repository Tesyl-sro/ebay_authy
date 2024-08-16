using System.Text;
using eBay.ApiClient.Auth.OAuth2;
using eBay.ApiClient.Auth.OAuth2.Model;
using Spectre.Console;
using YamlDotNet.Core.Tokens;

namespace ebay_authy.Helpers;

public class Keyset(string client_id, string dev_id, string cert_id, string redirect_uri)
{
    private readonly string client_id = client_id;
    private readonly string dev_id = dev_id;
    private readonly string cert_id = cert_id;
    private readonly string redirect_uri = redirect_uri;

    public static Keyset FromConfigFile(string path, OAuthEnvironment env)
    {
        try
        {
            CredentialUtil.Load(path);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[red bold]Error while reading config file:[/] " + e.Message);
            Environment.Exit(1);
        }

        var credentials = CredentialUtil.GetCredentials(env);
        var clientId = credentials.Get(CredentialType.APP_ID);
        var devId = credentials.Get(CredentialType.DEV_ID);
        var certId = credentials.Get(CredentialType.CERT_ID);
        var redirectUrl = credentials.Get(CredentialType.REDIRECT_URI);

        return new Keyset(clientId, devId, certId, redirectUrl);
    }

    public static Keyset FromEnvironment()
    {
        string? clientId = Environment.GetEnvironmentVariable("EBAY_CLIENT_ID");
        string? devId = Environment.GetEnvironmentVariable("EBAY_DEV_ID");
        string? certId = Environment.GetEnvironmentVariable("EBAY_CERT_ID");
        string? redirectUrl = Environment.GetEnvironmentVariable("EBAY_REDIRECT_URI");

        if (clientId == null)
        {
            AnsiConsole.MarkupLine("[red bold]EBAY_CLIENT_ID[/] not defined");
            goto Error;
        }

        if (devId == null)
        {
            AnsiConsole.MarkupLine("[red bold]EBAY_DEV_ID[/] not defined");
            goto Error;
        }

        if (certId == null)
        {
            AnsiConsole.MarkupLine("[red bold]EBAY_CERT_ID[/] not defined");
            goto Error;
        }

        if (redirectUrl == null)
        {
            AnsiConsole.MarkupLine("[red bold]EBAY_REDIRECT_URI[/] not defined");
            goto Error;
        }

        goto Success;

    Error:
        Environment.Exit(1);

    Success:
        return new Keyset(clientId, devId, certId, redirectUrl);
    }

    public string GetClientId()
    {
        return client_id;
    }

    public string GetDevId()
    {
        return dev_id;
    }

    public string GetCertId()
    {
        return cert_id;
    }

    public string GetRedirectUri()
    {
        return redirect_uri;
    }

    public MemoryStream ConvertToStream()
    {
        var buffer = new StringBuilder();

        buffer.Append("name: ebay-config\n\n");
        buffer.AppendLine("api.ebay.com:");

        buffer.AppendFormat("    appid: {0}\n", client_id);
        buffer.AppendFormat("    certid: {0}\n", cert_id);
        buffer.AppendFormat("    devid: {0}\n", dev_id);
        buffer.AppendFormat("    redirecturi: {0}", redirect_uri);

        var compiled = buffer.ToString();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);

        writer.Write(compiled);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }
}