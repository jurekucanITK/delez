namespace Delez.Api.Dto;

public record PredlogNakazilaDto(
    int PlacnikId,
    string PlacnikIme,
    string PlacnikEmail,
    int PrejemnikId,
    string PrejemnikIme,
    string PrejemnikEmail,
    decimal Znesek);
