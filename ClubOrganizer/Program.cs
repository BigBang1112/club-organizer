using ClubOrganizer;
using ManiaAPI.NadeoAPI;
using Spectre.Console;
using TmEssentials;

using var cts = new CancellationTokenSource();

using var nls = new NadeoLiveServices();

var login = AnsiConsole.Ask<string>("Enter Ubisoft Connect [green]login[/]:");

var password = AnsiConsole.Prompt(
    new TextPrompt<string>("Enter Ubisoft Connect [green]password[/]:")
        .PromptStyle("red")
        .Secret());

await nls.AuthorizeAsync(login, password, AuthorizationMethod.UbisoftAccount);

AnsiConsole.MarkupLine("[green]Authorization successful[/]");

var clubs = (await nls.GetMyClubsAsync(10, 0, cts.Token)).ClubList;

var clubDict = clubs.ToDictionary(c => $"{TextFormatter.Deformat(c.Name)} ({c.Id})", c => c);

var clubNameId = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .PageSize(10)
        .MoreChoicesText("[grey](Move up and down to reveal more clubs)[/]")
        .AddChoices(clubDict.Keys));

var club = clubDict[clubNameId];

var activities = await nls.GetAllClubActivitiesAsync(club.Id, cts.Token).ToListAsync(cts.Token);

activities.Sort(new EnvimixClubActivityComparer());

var erroredActivities = new List<ClubActivity>();

for (int i = 0; i < activities.Count; i++)
{
    var activity = activities[i];

    AnsiConsole.WriteLine($"{i + 1}. {activity.Name} (move to {i})");

    try
    {
        await nls.EditClubActivityAsync(club.Id, activity.Id, new() { Position = i }, cts.Token);
    }
    catch (NadeoAPIResponseException ex)
    {
        AnsiConsole.WriteLine(activity.ToString());
        AnsiConsole.WriteException(ex);
        erroredActivities.Add(activity);
    }
}

if (erroredActivities.Count > 0)
{
    AnsiConsole.WriteLine();
    AnsiConsole.WriteLine("The following activities failed to update:");
    AnsiConsole.WriteLine();

    for (int i = 0; i < erroredActivities.Count; i++)
    {
        var activity = erroredActivities[i];

        AnsiConsole.WriteLine($"{i + 1}. {activity.Name}");
    }
}

foreach (var activity in erroredActivities)
{
    for (int i = activities.Count - 1; i >= 0; i--)
    {
        AnsiConsole.WriteLine($"{i + 1}. {activity.Name}");

        try
        {
            await nls.EditClubActivityAsync(club.Id, activity.Id, new() { Position = i }, cts.Token);
        }
        catch (NadeoAPIResponseException ex)
        {
            AnsiConsole.WriteException(ex);
            break;
        }
    }
}