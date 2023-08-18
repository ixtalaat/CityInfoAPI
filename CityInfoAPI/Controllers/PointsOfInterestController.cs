using CityInfoAPI.Models;
using CityInfoAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
	[Route("api/cities/{cityId}/pointsOfInterest")]
	[ApiController]
	public class PointsOfInterestController : ControllerBase
	{
		private readonly ILogger<PointsOfInterestController> _logger;
		private readonly IMailService _mailService;
		private readonly CitiesDataStore _citiesDataStore;

		public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
			_citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
		}

		[HttpGet]
		public ActionResult<IEnumerable<List<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
		{
			try
			{
				//throw new Exception("Exception Sample.");
				var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
				if (city is null)
				{
					_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
					return NotFound();
				}

				return Ok(city.PointsOfInterest);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(
					$"Exception while getting points of interest for city with id {cityId}.",
					ex);
				return StatusCode(500, "A problem happened while handling your request.");
			}
		}

		[HttpGet("{pointofinterestid}", Name = "GetPointsOfInterest")]
		public ActionResult<IEnumerable<List<PointOfInterestDto>>> GetPointsOfInterest(int cityId, int pointOfInterestId)
		{
			var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null) return NotFound();

			var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterest is null) return NotFound();

			return Ok(pointOfInterest);
		}

		[HttpPost]
		public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
		{
			//if (!ModelState.IsValid) return BadRequest(ModelState);

			var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null) return NotFound();

			var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

			var finialPointOfInterest = new PointOfInterestDto()
			{
				Id = ++maxPointOfInterestId,
				Name = pointOfInterest.Name,
				Description = pointOfInterest.Description
			};

			city.PointsOfInterest.Add(finialPointOfInterest);

			return CreatedAtRoute("GetPointsOfInterest",
				new
				{
					cityId = cityId,
					pointOfInterestId = finialPointOfInterest.Id
				},
				pointOfInterest);
		}


		[HttpPut("{pointOfInterestId}")]
		public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
		{
			//if (!ModelState.IsValid) return BadRequest(ModelState);

			var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null) return NotFound();

			var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterestFromStore is null) return NotFound();


			pointOfInterestFromStore.Name = pointOfInterest.Name;
			pointOfInterestFromStore.Description = pointOfInterest.Description;


			return NoContent();
		}

		[HttpPatch("{pointOfInterestId}")]
		public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
		{
			var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null) return NotFound();

			var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterestFromStore is null) return NotFound();


			var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
			{
				Name = pointOfInterestFromStore.Name,
				Description = pointOfInterestFromStore.Description
			};

			patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (!TryValidateModel(pointOfInterestToPatch)) return BadRequest(ModelState);

			pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
			pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;


			return NoContent();

		}

		[HttpDelete("{pointOfInterestId}")]
		public ActionResult<PointOfInterestDto> DeletePointOfInterest(int cityId, int pointOfInterestId)
		{
			var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null) return NotFound();

			var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterestFromStore is null) return NotFound();

			city.PointsOfInterest.Remove(pointOfInterestFromStore);

			_mailService.Send("Point Of interest deleted.", $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

			return NoContent();
		}
	}
}
