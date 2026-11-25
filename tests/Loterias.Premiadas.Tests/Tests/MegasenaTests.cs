using Loterias.Premiadas.Services;

namespace Loterias.Premiadas.Tests;

public class MegasenaTests
{
    [Fact]
    public void Gerar_DeveRetornarQuantidadeSolicitadaDeJogosValidos()
    {
        IReadOnlyList<int[]> jogos = MegasenaService.Gerar(1000, new Random(42));

        Assert.Equal(1000, jogos.Count);
        Assert.All(jogos, AssertJogoValido);
    }

    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6)]
    [InlineData(2, 4, 6, 8, 10, 12)]
    [InlineData(2, 12, 22, 31, 44, 55)]
    public void Gerar_DeveAceitarCombinacaoAntesRejeitada(
        int primeiro,
        int segundo,
        int terceiro,
        int quarto,
        int quinto,
        int sexto)
    {
        int[] valores = { sexto, primeiro, quinto, segundo, quarto, terceiro };
        var aleatorio = new AleatorioControlado(valores);

        int[] jogo = MegasenaService.Gerar(1, aleatorio).Single();

        AssertJogoValido(jogo);
        Assert.Equal(valores.Order(), jogo);
        Assert.Equal(0, aleatorio.ValoresRestantes);
    }

    [Fact]
    public void Gerar_DeveIgnorarNumeroRepetidoNoJogo()
    {
        var aleatorio = new AleatorioControlado(new[] { 2, 2, 11, 24, 33, 36, 57 });

        int[] jogo = MegasenaService.Gerar(1, aleatorio).Single();

        AssertJogoValido(jogo);
        Assert.Equal(new[] { 2, 11, 24, 33, 36, 57 }, jogo);
        Assert.Equal(0, aleatorio.ValoresRestantes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1001)]
    public void Gerar_DeveRejeitarQuantidadeForaDoIntervalo(int quantidade)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MegasenaService.Gerar(quantidade, new Random(1)));
    }

    [Fact]
    public void Gerar_DeveRejeitarFonteAleatoriaNula()
    {
        Assert.Throws<ArgumentNullException>(() => MegasenaService.Gerar(1, null!));
    }

    private static void AssertJogoValido(int[] jogo)
    {
        Assert.Equal(6, jogo.Length);
        Assert.Equal(jogo.Order(), jogo);
        Assert.Equal(6, jogo.Distinct().Count());
        Assert.All(jogo, numero => Assert.InRange(numero, 1, 60));
    }

    private sealed class AleatorioControlado(IEnumerable<int> valores) : Random
    {
        private readonly Queue<int> _valores = new(valores);

        public int ValoresRestantes => _valores.Count;

        public override int Next(int minValue, int maxValue) => _valores.Dequeue();
    }
}
