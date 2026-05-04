using TimePoker.Domain;
using Xunit;

namespace TimePoker.Domain.Tests;

public class PreSetsTests
{
    [Fact]
    public void Turbo_TemNiveisCurtos()
    {
        var e = PreSets.Turbo();
        Assert.NotEmpty(e.Niveis);
        Assert.All(e.Niveis.Where(n => !n.EhBreak()),
            n => Assert.Equal(TimeSpan.FromMinutes(10), n.Duracao));
    }

    [Fact]
    public void Padrao_TemNiveisDe15A20Min()
    {
        var e = PreSets.Padrao();
        Assert.NotEmpty(e.Niveis);
        Assert.All(e.Niveis.Where(n => !n.EhBreak()),
            n => Assert.InRange(n.Duracao.TotalMinutes, 15, 20));
    }

    [Fact]
    public void DeepStack_TemNiveisDe30Min()
    {
        var e = PreSets.DeepStack();
        Assert.NotEmpty(e.Niveis);
        Assert.All(e.Niveis.Where(n => !n.EhBreak()),
            n => Assert.Equal(TimeSpan.FromMinutes(30), n.Duracao));
    }

    [Fact]
    public void Customizada_VemVazia()
    {
        var e = PreSets.Customizada();
        Assert.True(e.EstaVazia());
    }

    [Theory]
    [InlineData("Turbo")]
    [InlineData("Padrão")]
    [InlineData("DeepStack")]
    public void PreSets_TemBuyInEValorRebuyConfigurados(string preset)
    {
        Estrutura e = preset switch
        {
            "Turbo" => PreSets.Turbo(),
            "Padrão" => PreSets.Padrao(),
            _ => PreSets.DeepStack()
        };
        Assert.True(e.BuyIn > 0);
        Assert.True(e.ValorRebuy > 0);
    }

    [Fact]
    public void DuracaoTotal_DePreSetPadrao_BateComSomaDosNiveis()
    {
        var e = PreSets.Padrao();
        var soma = TimeSpan.Zero;
        foreach (var n in e.Niveis) soma += n.Duracao;
        Assert.Equal(soma, e.DuracaoTotal());
    }
}
