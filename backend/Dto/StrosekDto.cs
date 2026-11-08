namespace Delez.Api.Dto;

public record StrosekDto(int Id, string Opis, decimal Znesek, DateOnly Datum, int PlacnikId, string PlacnikIme);
