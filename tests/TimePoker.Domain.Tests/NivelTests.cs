using TimePoker.Domain;
using Xunit;

namespace TimePoker.Domain.Tests;

public class NivelTests
{
    [Fact]
    public void DeJogo_CriaNivelDoTipoCertoComBlinds()
    {
        var n = Nivel.DeJogo(100, 200, 20);
        Assert.Equal(TipoNivel.Jogo, n.Tipo);
        Assert.Equal(100, n.SmallBlind);
        Assert.Equal(200, n.BigBlind);
        Assert.Equal(0, n.Ante);
        Assert.Equal(TimeSpan.FromMinutes(20), n.Duracao);
        Assert.False(n.EhBreak());
    }

    [Fact]
    public void DeJogo_ComAnte_PreservaAnte()
    {
        var n = Nivel.DeJogo(200, 400, 15, ante: 50);
        Assert.Equal(50, n.Ante);
    }

    [Fact]
    public void DeBreak_CriaBreakComRotuloPadrao()
    {
        var n = Nivel.DeBreak(10);
        Assert.Equal(TipoNivel.Break, n.Tipo);
        Assert.True(n.EhBreak());
        Assert.Equal("BREAK", n.Rotulo);
    }

    [Fact]
    public void FormatarBlinds_NivelDeJogo_RetornaSmallBarBig()
    {
        var n = Nivel.DeJogo(200, 400, 20);
        Assert.Equal("200 / 400", n.FormatarBlinds());
    }

    [Fact]
    public void FormatarBlinds_ComAnte_IncluiAnteEntreParenteses()
    {
        var n = Nivel.DeJogo(200, 400, 20, ante: 50);
        Assert.Equal("200 / 400 (50)", n.FormatarBlinds());
    }

    [Fact]
    public void FormatarBlinds_NivelBreak_RetornaRotulo()
    {
        var n = Nivel.DeBreak(10, "BREAK FINAL");
        Assert.Equal("BREAK FINAL", n.FormatarBlinds());
    }
}
