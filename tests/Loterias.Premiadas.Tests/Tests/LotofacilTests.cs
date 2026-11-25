using Loterias.Premiadas.Services;

namespace Loterias.Premiadas.Tests;

public class LotofacilTests
{
    [Fact]
    public void Gerar_DeveRetornarQuantidadeSolicitadaDeJogosValidos()
    {
        IReadOnlyList<int[]> jogos = LotofacilService.Gerar(1000, new Random(42));

        Assert.Equal(1000, jogos.Count);
        Assert.All(jogos, AssertJogoValido);
    }

    [Theory]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11,12,13,14,15")]
    [InlineData("1,3,5,7,9,11,13,15,17,19,2,4,6,8,10")]
    [InlineData("1,4,6,7,10,11,12,13,14,17,18,20,21,22,24")]
    public void Gerar_DeveAceitarCombinacaoAntesRejeitada(string valores)
    {
        int[] candidata = valores.Split(',').Select(int.Parse).ToArray();
        var aleatorio = new AleatorioControlado(candidata);

        int[] jogo = LotofacilService.Gerar(1, aleatorio).Single();

        AssertJogoValido(jogo);
        Assert.Equal(candidata.Order(), jogo);
        Assert.Equal(0, aleatorio.ValoresRestantes);
    }

    [Fact]
    public void Gerar_DeveIgnorarNumeroRepetidoNoJogo()
    {
        int[] distintos = "1,2,5,6,7,10,11,12,14,16,17,19,21,22,24"
            .Split(',')
            .Select(int.Parse)
            .ToArray();
        var aleatorio = new AleatorioControlado(new[] { 1 }.Concat(distintos));

        int[] jogo = LotofacilService.Gerar(1, aleatorio).Single();

        AssertJogoValido(jogo);
        Assert.Equal(distintos, jogo);
        Assert.Equal(0, aleatorio.ValoresRestantes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1001)]
    public void Gerar_DeveRejeitarQuantidadeForaDoIntervalo(int quantidade)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => LotofacilService.Gerar(quantidade, new Random(1)));
    }

    [Fact]
    public void Gerar_DeveRejeitarFonteAleatoriaNula()
    {
        Assert.Throws<ArgumentNullException>(
            () => LotofacilService.Gerar(1, null!));
    }

    private static void AssertJogoValido(int[] jogo)
    {
        Assert.Equal(15, jogo.Length);
        Assert.Equal(15, jogo.Distinct().Count());
        Assert.Equal(jogo.Order(), jogo);
        Assert.All(jogo, numero => Assert.InRange(numero, 1, 25));
    }

    private sealed class AleatorioControlado(IEnumerable<int> valores) : Random
    {
        private readonly Queue<int> _valores = new(valores);

        public int ValoresRestantes => _valores.Count;

        public override int Next(int minValue, int maxValue) => _valores.Dequeue();
    }
}
