using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.Dtos;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;

    public TourGuideController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    [HttpGet("getLocation")]
    public async Task<ActionResult<VisitedLocation>> GetLocationAsync([FromQuery] string userName)
    {
        var user = GetUser(userName);

        if (user == null)
        {
            return NotFound($"User '{userName}' not found.");
        }

        var location = await _tourGuideService.GetUserLocationAsync(user);

        return Ok(location);
    }

    [HttpGet("getNearbyAttractions")]
    public async Task<ActionResult<List<NearbyAttractionDto>>> GetNearbyAttractionsAsync([FromQuery] string userName)
    {
        var user = GetUser(userName);

        if (user == null)
        {
            return NotFound($"User '{userName}' not found.");
        }

        var visitedLocation = await _tourGuideService.GetUserLocationAsync(user);

        var attractions = await _tourGuideService.GetNearByAttractionsAsync(visitedLocation);

        return Ok(attractions);
    }

    [HttpGet("getRewards")]
    public ActionResult<List<UserReward>> GetRewards([FromQuery] string userName)
    {
        var user = GetUser(userName);
        if (user == null)
        {
            return NotFound($"User '{userName}' not found.");

        }

        var rewards = _tourGuideService.GetUserRewards(user);

        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public ActionResult<List<Provider>> GetTripDeals([FromQuery] string userName)
    {
        var user = GetUser(userName);
        if (user == null)
        {
            return NotFound($"User '{userName}' not found.");
        }

        var deals = _tourGuideService.GetTripDeals(user);

        return Ok(deals);
    }

    private User? GetUser(string userName)
    {
        return _tourGuideService.GetUser(userName);
    }
}
