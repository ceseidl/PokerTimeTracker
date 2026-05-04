using CommunityToolkit.Mvvm.ComponentModel;
using TimePoker.Domain;

namespace TimePoker.Wpf.ViewModels;

/// <summary>
/// Representa uma linha da lista de níveis da estrutura, com status visual e
/// edição inline (SB / BB / Ante / Duração / Tipo).
///
/// Quando qualquer propriedade editável muda, dispara <see cref="AoMudar"/> —
/// o <see cref="MainViewModel"/> usa esse hook pra refletir a alteração no Domain.
/// </summary>
public partial class NivelLinhaViewModel : ObservableObject
{
    [ObservableProperty] private int _numero;
    [ObservableProperty] private int _smallBlind;
    [ObservableProperty] private int _bigBlind;
    [ObservableProperty] private int _ante;
    [ObservableProperty] private int _duracaoMinutos;
    [ObservableProperty] private TipoNivel _tipo;
    [ObservableProperty] private string _rotulo = string.Empty;
    [ObservableProperty] private StatusLinha _status = StatusLinha.Futuro;

    public bool EhBreak => Tipo == TipoNivel.Break;

    public string StatusTexto => Status switch
    {
        StatusLinha.Passou => "✓ passou",
        StatusLinha.Atual  => "● atual",
        _                  => ""
    };

    /// <summary>Disparado quando alguma propriedade editável muda.</summary>
    public Action? AoMudar { get; set; }

    partial void OnSmallBlindChanged(int value)     => AoMudar?.Invoke();
    partial void OnBigBlindChanged(int value)        => AoMudar?.Invoke();
    partial void OnAnteChanged(int value)            => AoMudar?.Invoke();
    partial void OnDuracaoMinutosChanged(int value)  => AoMudar?.Invoke();
    partial void OnTipoChanged(TipoNivel value)
    {
        OnPropertyChanged(nameof(EhBreak));
        AoMudar?.Invoke();
    }
    partial void OnStatusChanged(StatusLinha value)  => OnPropertyChanged(nameof(StatusTexto));

    /// <summary>Converte a linha de volta em um <see cref="Nivel"/> (Domain).</summary>
    public Nivel ParaNivel()
    {
        var dur = Math.Max(1, DuracaoMinutos);
        return Tipo == TipoNivel.Break
            ? Nivel.DeBreak(dur, string.IsNullOrWhiteSpace(Rotulo) ? "BREAK" : Rotulo)
            : Nivel.DeJogo(SmallBlind, BigBlind, dur, Ante);
    }

    public static NivelLinhaViewModel De(int numero, Nivel nivel, StatusLinha status) => new()
    {
        Numero = numero,
        SmallBlind = nivel.SmallBlind,
        BigBlind = nivel.BigBlind,
        Ante = nivel.Ante,
        DuracaoMinutos = (int)nivel.Duracao.TotalMinutes,
        Tipo = nivel.Tipo,
        Rotulo = nivel.Rotulo,
        Status = status
    };
}

public enum StatusLinha { Passou, Atual, Futuro }
