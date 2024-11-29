using AutoMapper;
using WebApplication1.Dtos;
using WebApplication1.Entities;

namespace WebApplication1.Mappers
{
    public class GameMapper : Profile
    {
        public GameMapper()
        {
            // Map from CreateGameDto to Game (Entity)
            CreateMap<CreateGameDto, Game>()
                .ForMember(dest => dest.Genre, opt => opt.MapFrom<object>((src, dest) => null)) // Ensuring Genre is mapped correctly later in the handler
                .ForMember(dest => dest.GenreId, opt => opt.MapFrom(src => src.GenreId)); // Explicitly map GenreId

            // Map from Game (Entity) to GameDto
            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre!.Name)); // Mapping Genre name from related entity
        }
    }
}