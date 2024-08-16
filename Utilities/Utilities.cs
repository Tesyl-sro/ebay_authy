using System.Diagnostics;
using System.Web;
using ebay_authy.Helpers;
using Spectre.Console;

namespace ebay_authy.Utilities;

public static class Utils
{
    public static void Greet()
    {
        AnsiConsole.MarkupLine("[red]e[/][blue]b[/][yellow]a[/][green]y[/][yellow] Authenticator[/]");
        AnsiConsole.MarkupLine("Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

#if DEBUG
        AnsiConsole.MarkupLine("[yellow]You are using a Debug build![/]");
#endif

        Console.WriteLine();
    }

    public static Keyset AskKeyset()
    {
        const string CHOICE_ENV = "Environment variables (EBAY_CLIENT_ID, EBAY_DEV_ID, EBAY_CERT_ID, EBAY_REDIRECT_URI)";
        const string CHOICE_MANUAL = "Enter manually";

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("How would you like to load your Ebay keyset (Client ID, Dev ID, Cert ID, Redirect URI)?")
                .AddChoices([
                    CHOICE_ENV,
                    CHOICE_MANUAL
                ])
        );

        switch (choice)
        {
            case CHOICE_ENV:
                {
                    return Keyset.FromEnvironment();
                }
            case CHOICE_MANUAL:
                {
                    return AskKeysetValues();
                }
            default:
                throw new Exception("unreachable");
        }
    }

    public static void AskOpenUrl(string url)
    {
        Console.WriteLine();

        var answer = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Would you like to open this URL with your default browser?")
                .AddChoices([
                    "Yes",
                    "No"
                ])
        );

        if (answer == "No")
        {
            return;
        }

        try
        {
            Process.Start(
            new ProcessStartInfo()
            {
                FileName = url,
                UseShellExecute = true
            }
        );
        }
        catch
        {
            AnsiConsole.MarkupLine("[red]Failed to open link. Please open it manually.[/]");
        }
    }

    public static string? ParseAuthCode(string redirectUrl)
    {
        var parsedUrl = redirectUrl.Split('?')[1];
        var paramsCollection = HttpUtility.ParseQueryString(parsedUrl);

        var code = paramsCollection.Get("code");

        return code;
    }

    private static Keyset AskKeysetValues()
    {
        string client_id = AnsiConsole.Prompt(new TextPrompt<string>("Enter client ID:", null));
        string dev_id = AnsiConsole.Prompt(new TextPrompt<string>("Enter dev ID:", null));
        string cert_id = AnsiConsole.Prompt(new TextPrompt<string>("Enter cert ID:", null));
        string redirect_uri = AnsiConsole.Prompt(new TextPrompt<string>("Enter redirect URI:", null));

        return new Keyset(client_id, dev_id, cert_id, redirect_uri);
    }
}