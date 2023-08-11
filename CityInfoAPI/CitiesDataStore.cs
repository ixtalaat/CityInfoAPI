using CityInfoAPI.Models;

namespace CityInfoAPI
{
	public class CitiesDataStore
	{
        public List<CityDto> Cities { get; set; }
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public CitiesDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto() 
                { 
                    Id = 1, 
                    Name = "Cairo",
                    Description = "Capital of Egypt, and one of the largest cities in Africa.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1, 
                            Name = "Giza Pyramids",
                            Description = "The Giza Pyramids, a UNESCO World Heritage Site, are a timeless wonder and one of the most iconic landmarks on Earth. These ancient tombs, built over 4,500 years ago, stand as a testament to the ingenuity and craftsmanship of the ancient Egyptians. The Great Pyramid of Giza, one of the Seven Wonders of the Ancient World, is the largest"
						},
                        new PointOfInterestDto()
                        {
							Id = 2,
							Name = "Cairo Tower",
							Description = "The Cairo Tower, an iconic landmark that graces the city's skyline, is a symbol of modernity and progress. Rising to a height of 187 meters, this tower offers panoramic views of Cairo and the Nile River."
						}
                    }
                },
                new CityDto() 
                { 
                    Id = 2,
                    Name = "Alexandria",
                    Description = "The city lies on the Mediterranean Sea at the western edge of the Nile River delta",
					PointsOfInterest = new List<PointOfInterestDto>()
					{
						new PointOfInterestDto()
						{
							Id = 1,
							Name = "Qaitbay Citadel",
							Description = "The Qaitbay Citadel stands as a historic fortress on the Mediterranean coastline, offering stunning sea views and a glimpse into Alexandria's past. Constructed in the 15th century, the citadel was built on the site of the ancient Lighthouse of Alexandria"
						},
						new PointOfInterestDto()
						{
							Id = 2,
							Name = "Montaza Palace",
							Description = "Montaza Palace, situated along the Mediterranean coast, is a splendid historical palace complex surrounded by beautifully landscaped gardens. Originally built as a royal residence, the palace is a blend of Turkish and Florentine architectural styles. "
						}
					}
				},
                new CityDto() 
                { 
                    Id = 3,
                    Name = "Mansoura",
                    Description = "It has long been thought of as a rural and underdeveloped part of Egypt. It lies on the east bank of the Damietta branch of the Nile, and is considered the capital of the Dakahlia Governorate",
					PointsOfInterest = new List<PointOfInterestDto>()
					{
						new PointOfInterestDto()
						{
							Id = 1,
							Name = "Mansoura University",
							Description = "Mansoura University, a prestigious educational institution, stands as a beacon of knowledge and academic excellence in the city. Founded in the early 1970s, the university's sprawling campus is a hub of learning and research, offering a wide range of disciplines and faculties."
						},
						new PointOfInterestDto()
						{
							Id = 2,
							Name = "Al-Mansouriya Canal",
							Description = "The Al-Mansouriya Canal, a historic waterway, has played a pivotal role in shaping the landscape and prosperity of Mansoura. Constructed during the medieval period, the canal served as an irrigation network, facilitating agriculture and trade in the region."
						}
					}
				},
            };
        }
    }
}
