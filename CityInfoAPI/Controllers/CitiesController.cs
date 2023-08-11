using CityInfoAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
	[Route("api/cities")]
	[ApiController]
	public class CitiesController : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<CityDto>> GetCities()
		{
			return Ok(CitiesDataStore.Current.Cities); 
		}

		[HttpGet("{id}")]
		public ActionResult<CityDto> GetCity(int id)
		{
			var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
			if( cityToReturn is null) return NotFound();

			return Ok(cityToReturn);
		}
	}
}
