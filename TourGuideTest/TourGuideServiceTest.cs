using GpsUtil.Location;
using TourGuide.Dtos;
using TourGuide.Users;
using TripPricer;

namespace TourGuideTest
{
    public class TourGuideServiceTour : IClassFixture<DependencyFixture>
    {
        private readonly DependencyFixture _fixture;

        public TourGuideServiceTour(DependencyFixture fixture)
        {
            _fixture = fixture;
        }

        public void Dispose()
        {
            _fixture.Cleanup();
        }

        [Fact]
        public async Task GetUserLocation()
        {
            // Arrange
            _fixture.Initialize(0);

            User user = new(
                    Guid.NewGuid(),
                    "jon",
                    "000",
                    "jon@tourGuide.com");

            // Act
            VisitedLocation visitedLocation = await _fixture.TourGuideService.TrackUserLocationAsync(user);

            _fixture.TourGuideService.Tracker.StopTracking();

            // Assert
            Assert.Equal(user.UserId, visitedLocation.UserId);
        }

        [Fact]
        public void AddUser()
        {
            // Arrange
            _fixture.Initialize(0);

            User user =
                new(
                    Guid.NewGuid(),
                    "jon",
                    "000",
                    "jon@tourGuide.com");

            User user2 =
                new(
                    Guid.NewGuid(),
                    "jon2",
                    "000",
                    "jon2@tourGuide.com");

            // Act
            _fixture.TourGuideService.AddUser(user);
            _fixture.TourGuideService.AddUser(user2);

            User? retrievedUser = _fixture.TourGuideService.GetUser(user.UserName);

            User? retrievedUser2 = _fixture.TourGuideService.GetUser(user2.UserName);

            _fixture.TourGuideService.Tracker.StopTracking();

            // Assert
            Assert.Equal(user, retrievedUser);
            Assert.Equal(user2, retrievedUser2);
        }

        [Fact]
        public void GetAllUsers()
        {
            // Arrange
            _fixture.Initialize(0);

            User user =
                new(
                    Guid.NewGuid(),
                    "jon",
                    "000",
                    "jon@tourGuide.com");

            User user2 =
                new(
                    Guid.NewGuid(),
                    "jon2",
                    "000",
                    "jon2@tourGuide.com");

            // Act
            _fixture.TourGuideService.AddUser(user);
            _fixture.TourGuideService.AddUser(user2);

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            _fixture.TourGuideService.Tracker.StopTracking();

            // Assert
            Assert.Contains(user, allUsers);
            Assert.Contains(user2, allUsers);
        }

        [Fact]
        public async Task TrackUser()
        {
            // Arrange
            _fixture.Initialize();

            User user =
                new(
                    Guid.NewGuid(),
                    "jon",
                    "000",
                    "jon@tourGuide.com");

            VisitedLocation visitedLocation = await _fixture.TourGuideService.TrackUserLocationAsync(user);

            // Act
            _fixture.TourGuideService.Tracker.StopTracking();

            // Assert
            Assert.Equal(user.UserId, visitedLocation.UserId);
        }

        [Fact]
        public async Task GetNearbyAttractions()
        {
            // Arrange
            _fixture.Initialize(0);

            User user = new(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");

            VisitedLocation visitedLocation = await _fixture.TourGuideService.TrackUserLocationAsync(user);

            // Act
            List<NearbyAttractionDto> attractions = await _fixture.TourGuideService.GetNearByAttractionsAsync(visitedLocation);

            _fixture.TourGuideService.Tracker.StopTracking();

            // Assert
            Assert.Equal(5, attractions.Count);
        }

        [Fact]
        public void GetTripDeals()
        {
            // Arrange
            _fixture.Initialize(0);

            User user =
                new(
                    Guid.NewGuid(),
                    "jon",
                    "000",
                    "jon@tourGuide.com");

            List<Provider> providers = _fixture.TourGuideService.GetTripDeals(user);

            _fixture.TourGuideService.Tracker.StopTracking();

            // Assert
            Assert.Equal(10, providers.Count);
        }
    }
}