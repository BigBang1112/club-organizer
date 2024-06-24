using ManiaAPI.NadeoAPI;

namespace ClubOrganizer;

public class EnvimixClubActivityComparer : IComparer<ClubActivity>
{
    private static readonly Dictionary<string, int> carPriority = new(StringComparer.OrdinalIgnoreCase)
    {
        { "CarSport", 1 },
        { "SnowCar", 2 },
        { "RallyCar", 3 },
        { "DesertCar", 4 }
    };

    private static readonly Dictionary<string, int> seasonMonths = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Winter", 1 },
        { "Spring", 4 },
        { "Summer", 7 },
        { "Fall", 10 }
    };

    private static readonly Dictionary<string, DateOnly> specialCampaignDates = new(StringComparer.OrdinalIgnoreCase)
    {
        { "S-Discover", new(2023, 11, 23) },
        { "S-Discovery", new(2023, 11, 23) },
        { "R-Discover", new(2024, 2, 27) },
        { "R-Discovery", new(2024, 2, 27) },
        { "D-Discover", new(2024, 5, 22) },
        { "D-Discovery", new(2024, 5, 22) },
        { "Training", new(2020, 6, 1) }
    };

    public int Compare(ClubActivity? x, ClubActivity? y)
    {
        if (x is null || y is null)
        {
            return 0;
        }

        if (x.Name.StartsWith("Welcome", StringComparison.OrdinalIgnoreCase)) return -1;
        if (y.Name.StartsWith("Welcome", StringComparison.OrdinalIgnoreCase)) return 1;

        if (x.Name.StartsWith("Envimix", StringComparison.OrdinalIgnoreCase)) return 1;
        if (y.Name.StartsWith("Envimix", StringComparison.OrdinalIgnoreCase)) return -1;

        var xActivity = CampaignActivity.Parse(x);
        var yActivity = CampaignActivity.Parse(y);

        if (xActivity.Released != yActivity.Released)
        {
            return yActivity.Released.CompareTo(xActivity.Released);
        }

        var xCarPriority = carPriority[xActivity.Car];
        var yCarPriority = carPriority[yActivity.Car];

        if (xCarPriority != yCarPriority)
        {
            return xCarPriority.CompareTo(yCarPriority);
        }

        return 0;
    }

    public sealed record CampaignActivity(ClubActivity Activity, string Car, DateOnly Released)
    {
        public static CampaignActivity Parse(ClubActivity activity)
        {
            var nameSplit = activity.Name.Split(' ');

            var year = 0;
            var hasExplicitYear = nameSplit.Length > 2 && int.TryParse(nameSplit[2].Replace("\'", "20"), out year);

            if (nameSplit.Length > 1)
            {
                var car = nameSplit[0];
                var type = nameSplit[1];

                if (hasExplicitYear && seasonMonths.TryGetValue(type, out var month))
                {
                    var released = new DateOnly(year, month, 1);

                    return new CampaignActivity(activity, car, released);
                }

                if (specialCampaignDates.TryGetValue(type, out var specialReleased))
                {
                    return new CampaignActivity(activity, car, specialReleased);
                }
            }

            throw new InvalidOperationException("Invalid activity name");
        }
    }
}
