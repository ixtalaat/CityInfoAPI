using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[Route("api/cities/{cityId}/pointsOfInterest")]
	[Authorize(Policy = "MustBeFromAntwerp")]
	[ApiController]
	public class PointsOfInterestController : ControllerBase
	{
		private readonly ILogger<PointsOfInterestController> _logger;
		private readonly IMailService _mailService;
		private readonly ICityInfoRepository _cityInfoRepository;
		private readonly IMapper _mapper;

		public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
			_cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<List<PointOfInterestDto>>>> GetPointsOfInterestAsync(int cityId)
		{
			try
			{
				//throw new Exception("Exception Sample.");

				var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

				if(!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
				{
					return Forbid();
				}

				if (!await _cityInfoRepository.CityExistsAsync(cityId))
				{
					_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
					return NotFound();
				}
				
				var pointsOfInterest = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
				
				return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest));
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
		public async Task<ActionResult<IEnumerable<List<PointOfInterestDto>>>> GetPointsOfInterestAsync(int cityId, int pointOfInterestId)
		{
			if (!await _cityInfoRepository.CityExistsAsync(cityId))
				return NotFound();
			
			var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
			if (pointOfInterest is null) return NotFound();

			return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
		}

		[HttpPost]
		public async Task<ActionResult> CreatePointOfInterestAsync(int cityId, PointOfInterestForCreationDto pointOfInterest)
		{
			//if (!ModelState.IsValid) return BadRequest(ModelState);

			if (!await _cityInfoRepository.CityExistsAsync(cityId))
				return NotFound();

			var finialPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

			await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finialPointOfInterest);
			
			await _cityInfoRepository.SaveChangesAsync();

			var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finialPointOfInterest);

			return CreatedAtRoute("GetPointsOfInterest",
				new
				{
					cityId,
					pointOfInterestId = createdPointOfInterestToReturn.Id
				},
				createdPointOfInterestToReturn);
		}


		[HttpPut("{pointOfInterestId}")]
		public async Task<ActionResult> CreatePointOfInterestAsync(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
		{
			//if (!ModelState.IsValid) return BadRequest(ModelState);

			if (!await _cityInfoRepository.CityExistsAsync(cityId))
				return NotFound();

			var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

			if (pointOfInterestEntity is null) return NotFound();

			_mapper.Map(pointOfInterest, pointOfInterestEntity);

			await _cityInfoRepository.SaveChangesAsync();

			return NoContent();
		}

		[HttpPatch("{pointOfInterestId}")]
		public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
		{
			if (!await _cityInfoRepository.CityExistsAsync(cityId))
				return NotFound();

			var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

			if (pointOfInterestEntity is null) return NotFound();


			var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);


			patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (!TryValidateModel(pointOfInterestToPatch)) return BadRequest(ModelState);

			_mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

			await _cityInfoRepository.SaveChangesAsync();

			return NoContent();

		}

		[HttpDelete("{pointOfInterestId}")]
		public async Task<ActionResult> DeletePointofInterest(int cityId, int pointOfInterestId)
		{
			if (!await _cityInfoRepository.CityExistsAsync(cityId))
				return NotFound();

			var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

			if (pointOfInterestEntity is null) return NotFound();

			_cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

			await _cityInfoRepository.SaveChangesAsync();

			_mailService.Send("point of interest deleted.", $"point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

			return NoContent();
		}
	}
}
