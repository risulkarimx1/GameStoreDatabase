using GameStore.Api.Entities;

namespace GameStore.Api.Dtos;

public record GameDto(int Id, string Name, string Genre, decimal Price, DateOnly ReleaseDate);

public record GameDetailsDto (
    int Id, 
    string Name, 
    int GenreId, 
    decimal Price, 
    DateOnly ReleaseDate);
    
public record GameSummaryDto (
    int Id, 
    string Name, 
    int GenreId, 
    decimal Price, 
    DateOnly ReleaseDate);
