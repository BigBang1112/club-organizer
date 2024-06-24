using ManiaAPI.NadeoAPI;
using System.Runtime.CompilerServices;

namespace ClubOrganizer;

public static class NadeoLiveServicesExtensions
{
    public static async IAsyncEnumerable<ClubActivity> GetAllClubActivitiesAsync(this NadeoLiveServices nls, int clubId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var offset = 0;
        var count = 50;
        var active = true;

        while (true)
        {
            var activityColl = await nls.GetClubActivitiesAsync(clubId, count, offset, active, cancellationToken);

            foreach (var activity in activityColl.ActivityList)
            {
                yield return activity;
            }

            if (activityColl.ActivityList.Length < count)
            {
                break;
            }

            offset += count;
        }
    }
}
