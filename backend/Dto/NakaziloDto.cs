namespace Delez.Api.Dto;

public record NakaziloDto(
    int Id,
    decimal Znesek,
    DateOnly Datum,
    int PlacnikId,
    string PlacnikIme,
    int PrejemnikId,
    string PrejemnikIme);
