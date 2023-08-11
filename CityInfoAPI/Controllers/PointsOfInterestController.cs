using CityInfoAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
	[Route("api/cities/{cityId}/pointsOfInterest")]
	[ApiController]
	public class PointsOfInterestController : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<List<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
		{	
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null) return NotFound();

			return Ok(city.PointsOfInterest);
		}

		[HttpGet("{pointOfInterestId}")]
		public ActionResult<IEnumerable<List<PointOfInterestDto>>> GetPointsOfInterest(int cityId, int pointOfInterestId)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null) return NotFound();

			var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterest is null) return NotFound();

			return Ok(pointOfInterest);
		}
	}
}
