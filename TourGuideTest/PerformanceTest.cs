using System.Diagnostics;
using GpsUtil.Location;
using TourGuide.Users;
using Xunit.Abstractions;

namespace TourGuideTest
{
    public class PerformanceTest : IClassFixture<DependencyFixture>
    {
        /*
         * Note on performance improvements:
         * 
         * The number of generated users for high-volume tests can be easily adjusted using this method:
         * 
         *_fixture.Initialize(100000); (for example)
         * 
         * 
         * These tests can be modified to fit new solutions, as long as the performance metrics at the end of the tests remain consistent.
         * 
         * These are the performance metrics we aim to achieve:
         * 
         * highVolumeTrackLocation: 100,000 users within 15 minutes:
         * Assert.True(TimeSpan.FromMinutes(15).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
         *
         * highVolumeGetRewards: 100,000 users within 20 minutes:
         * Assert.True(TimeSpan.FromMinutes(20).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        */

        private readonly DependencyFixture _fixture;
        private readonly ITestOutputHelper _output;

        public PerformanceTest(DependencyFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [Fact]
        public async Task HighVolumeTrackLocation()
        {
            // Arrange
            _fixture.Initialize(0);

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            Stopwatch stopWatch = new();
            List<Task> tasks = new();

            // Act
            stopWatch.Start();

            foreach (User user in allUsers)
            {
                tasks.Add(_fixture.TourGuideService.TrackUserLocationAsync(user));
            }

            await Task.WhenAll(tasks);

            stopWatch.Stop();

            // Assert
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine(
                $"highVolumeTrackLocation: Time Elapsed: " +
                $"{stopWatch.Elapsed.TotalSeconds} seconds.");

            Assert.True(TimeSpan.FromMinutes(15).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }

        [Fact]
        public async Task HighVolumeGetRewards()
        {
            // Arrange
            _fixture.Initialize(0);

            List<Attraction> attractions = await _fixture.GpsUtil.GetAttractionsAsync();

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            foreach (User user in allUsers)
            {
                user.AddToVisitedLocations(
                    new VisitedLocation(
                        user.UserId,
                        attractions[0],
                        DateTime.Now));
            }

            List<Task> tasks = new();
            Stopwatch stopWatch = new();

            // Act
            stopWatch.Start();

            foreach (User user in allUsers)
            {
                tasks.Add(_fixture.RewardsService.CalculateRewardsAsync(user));
            }

            await Task.WhenAll(tasks);

            stopWatch.Stop();

            // Assert
            foreach (User user in allUsers)
            {
                Assert.True(user.UserRewards.Count > 0);
            }

            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine(
                $"highVolumeGetRewards: Time Elapsed: " +
                $"{stopWatch.Elapsed.TotalSeconds} seconds.");

            Assert.True(
                TimeSpan.FromMinutes(20).TotalSeconds >=
                stopWatch.Elapsed.TotalSeconds);
        }
    }
}
