namespace WebApplication1.Dtos;

public record GameDto(
    int Id, 
    string Name, 
    string Genre, 
    decimal Price,
    DateOnly ReleaseDate
);