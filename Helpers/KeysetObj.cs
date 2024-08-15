using System.Text;
using Spectre.Console;
using YamlDotNet.Core.Tokens;

namespace ebay_authy.Helpers;

public class Keyset(string client_id, string dev_id, string cert_id, string redirect_uri)
{
    private readonly string client_id = client_id;
    private readonly string dev_id = dev_id;
    private readonly string cert_id = cert_id;
    private readonly string redirect_uri = redirect_uri;

    public static Keyset FromEnvironment()
    {
        string? client_id = Environment.GetEnvironmentVariable("EBAY_CLIENT_ID");
        string? dev_id = Environment.GetEnvironmentVariable("EBAY_DEV_ID");
        string? cert_id = Environment.GetEnvironmentVariable("EBAY_CERT_ID");
        string? redirect_uri = Environment.GetEnvironmentVariable("EBAY_REDIRECT_URI");

        if (client_id == null)
        {
            AnsiConsole.MarkupLine("[red bold]EBAY_CLIENT_ID[/] not defined");
            goto Error;
        }

        if (dev_id == null)
        {
            AnsiConsole.MarkupLine("[red bold]EBAY_DEV_ID[/] not defined");
            goto Error;
        }

        if (cert_id == null)
        {
            AnsiConsole.MarkupLine("[red bold]EBAY_CERT_ID[/] not defined");
            goto Error;
        }

        if (redirect_uri == null)
        {
            AnsiConsole.MarkupLine("[red bold]EBAY_REDIRECT_URI[/] not defined");
            goto Error;
        }

        goto Success;

    Error:
        Environment.Exit(1);

    Success:
        return new Keyset(client_id, dev_id, cert_id, redirect_uri);
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