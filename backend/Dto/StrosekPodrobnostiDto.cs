namespace Delez.Api.Dto;

public record StrosekPodrobnostiDto(int Id, string Opis, decimal Znesek, DateOnly Datum, int PlacnikId, List<int> ClaniIds);
