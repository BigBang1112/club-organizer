using ManiaAPI.NadeoAPI;
using Spectre.Console;

using var nls = new NadeoLiveServices();

var login = AnsiConsole.Ask<string>("Enter Ubisoft Connect [green]login[/]:");

var password = AnsiConsole.Prompt(
    new TextPrompt<string>("Enter Ubisoft Connect [green]password[/]:")
        .PromptStyle("red")
        .Secret());

await nls.AuthorizeAsync(login, password, AuthorizationMethod.UbisoftAccount);

AnsiConsole.MarkupLine("[green]Authorization successful[/]");

var clubs = await nls.GetMyClubsAsync(20);

foreach (var club in clubs.ClubList)
{
    AnsiConsole.WriteLine(club.Name);
}