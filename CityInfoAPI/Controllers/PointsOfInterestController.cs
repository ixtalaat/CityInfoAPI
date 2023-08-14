using CityInfoAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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

		[HttpGet("{pointofinterestid}", Name = "GetPointsOfInterest")]
		public ActionResult<IEnumerable<List<PointOfInterestDto>>> GetPointsOfInterest(int cityId, int pointOfInterestId)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null) return NotFound();

			var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterest is null) return NotFound();

			return Ok(pointOfInterest);
		}

		[HttpPost]
		public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
		{
			//if (!ModelState.IsValid) return BadRequest(ModelState);

			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null) return NotFound();

			var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

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

			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null) return NotFound();

			var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterestFromStore is null) return NotFound();

			city.PointsOfInterest.Remove(pointOfInterestFromStore);

			return NoContent();
		}
	}
}
