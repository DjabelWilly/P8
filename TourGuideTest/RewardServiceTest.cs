using GpsUtil.Location;
using TourGuide.Users;

namespace TourGuideTest;

public class RewardServiceTest : IClassFixture<DependencyFixture>
{
    private readonly DependencyFixture _fixture;

    public RewardServiceTest(DependencyFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UserGetRewards()
    {
        // Arrange
        _fixture.Initialize(0);

        User user = new(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        
        List<Attraction> attractions = await _fixture.GpsUtil.GetAttractionsAsync();
        
        user.AddToVisitedLocations(new VisitedLocation(user.UserId, attractions.First(), DateTime.Now));

        // Act
        await _fixture.TourGuideService.TrackUserLocationAsync(user);

        List<UserReward> userRewards = user.UserRewards;

        // Assert
        Assert.Single(userRewards);

        _fixture.TourGuideService.Tracker.StopTracking();
    }

    [Fact]
    public async Task IsWithinAttractionProximity()
    {
        // Arrange
        List<Attraction> attractions = await _fixture.GpsUtil.GetAttractionsAsync();

        Attraction attraction = attractions.First();

        // Act
        bool result = _fixture.RewardsService.IsWithinAttractionProximity(attraction, attraction);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task NearAllAttractions()
    {
        // Arrange
        _fixture.Initialize(1);

        _fixture.RewardsService.SetProximityBuffer(int.MaxValue);

        User user = _fixture.TourGuideService.GetAllUsers().First();

        // Act
        await _fixture.RewardsService.CalculateRewardsAsync(user);

        List<UserReward> userRewards = _fixture.TourGuideService.GetUserRewards(user);

        // Assert
        List<Attraction> attractions = await _fixture.GpsUtil.GetAttractionsAsync();

        Assert.Equal(attractions.Count, userRewards.Count);

        _fixture.TourGuideService.Tracker.StopTracking();
    }
}
