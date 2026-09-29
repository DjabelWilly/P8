using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private const double StatuteMilesPerNauticalMile = 1.15077945;
    private readonly int _defaultProximityBuffer = 10;
    private int _proximityBuffer;
    private readonly int _attractionProximityRange = 200;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardCentral _rewardsCentral;

    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral = rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    public void SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    public void SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }

    public async Task CalculateRewardsAsync(User user)
    {
        List<VisitedLocation> userLocations = user.VisitedLocations.ToList();
        List<Attraction> attractions = await _gpsUtil.GetAttractionsAsync();

        // Permet de vérifier si une attraction possède déjà une récompense.
        var existingRewardNames = user.UserRewards.Select(r => r.Attraction.AttractionName).ToHashSet();

        foreach (var visitedLocation in userLocations)
        {
            foreach (var attraction in attractions)
            {
                if (!existingRewardNames.Contains(attraction.AttractionName))
                {
                    if (IsNearAttraction(visitedLocation, attraction))
                    {
                        var reward = new UserReward(visitedLocation, attraction, GetRewardPoints(attraction, user.UserId));
                        user.AddUserReward(reward);

                        // Ajoute le nom dans le HashSet afin de ne pas recalculer une récompense pour cette attraction.
                        existingRewardNames.Add(attraction.AttractionName);
                    }
                }
            }
        }
    }

    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        double distance = GetDistance(attraction, location);

        Console.WriteLine(distance);

        return distance <= _attractionProximityRange;
    }

    private bool IsNearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        return GetDistance(attraction, visitedLocation.Location) <= _proximityBuffer;
    }
    public int GetRewardPoints(Attraction attraction, Guid userId)
    {
        return _rewardsCentral.GetAttractionRewardPoints(attraction.AttractionId, userId);
    }

    public double GetDistance(Locations loc1, Locations loc2)
    {
        double lat1 = Math.PI * loc1.Latitude / 180.0;
        double lon1 = Math.PI * loc1.Longitude / 180.0;
        double lat2 = Math.PI * loc2.Latitude / 180.0;
        double lon2 = Math.PI * loc2.Longitude / 180.0;

        double angle = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2)
                                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2));

        double nauticalMiles = 60.0 * angle * 180.0 / Math.PI;
        return StatuteMilesPerNauticalMile * nauticalMiles;
    }
}
