namespace Loterias.Premiadas.Tests;

public class GeradorDeJogosLotofacilTests
{
    [Theory]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11,12,13,16,21", 8, 10)]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11,12,13,16,22", 7, 10)]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11,12,13,17,21", 9, 9)]
    public void Gerar_DeveAceitarParidadesEContagensDeMolduraValidas(
        string valores,
        int imparesEsperados,
        int molduraEsperada)
    {
        int[] candidata = valores.Split(',').Select(int.Parse).ToArray();

        int[] jogo = GerarUmaCandidata(candidata);

        AssertJogoValido(jogo);
        Assert.Equal(imparesEsperados, jogo.Count(numero => numero % 2 != 0));
        Assert.Equal(molduraEsperada, jogo.Count(EhDaMoldura));
    }

    [Theory]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11,12,13,16,21")]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11,12,13,16,22")]
    [InlineData("1,2,3,4,5,6,7,8,9,10,11,12,13,17,21")]
    public void Gerar_DeveAceitarCadaUmaDasTresDistribuicoesDeParidade(string valores)
    {
        int[] candidata = valores.Split(',').Select(int.Parse).ToArray();

        int[] jogo = GerarUmaCandidata(candidata);

        AssertJogoValido(jogo);
    }

    [Fact]
    public void Gerar_DeveRetornarQuantidadeSolicitadaDeJogosValidos()
    {
        IReadOnlyList<int[]> jogos = GeradorDeJogosLotofacil.Gerar(1000, new Random(42));

        Assert.Equal(1000, jogos.Count);
        Assert.All(jogos, AssertJogoValido);
    }

    [Theory]
    [InlineData("1,4,6,7,10,11,12,13,14,17,18,20,21,22,24")]
    [InlineData("1,2,3,7,8,9,11,13,14,16,17,18,22,23,24")]
    [InlineData("2,3,4,5,6,7,9,10,11,12,13,14,15,17,20")]
    public void Gerar_DeveSubstituirCandidataQueViolaUmaRegraObrigatoria(string invalida)
    {
        int[] candidataInvalida = invalida.Split(',').Select(int.Parse).ToArray();
        int[] candidataValida = "1,2,5,6,7,10,11,12,14,16,17,19,21,22,24"
            .Split(',')
            .Select(int.Parse)
            .ToArray();

        int[] jogo = GerarUmaCandidata(candidataInvalida, candidataValida);

        AssertJogoValido(jogo);
        Assert.Equal(candidataValida, jogo);
    }

    [Fact]
    public void Gerar_DeveIgnorarNumeroRepetidoNaCandidata()
    {
        int[] valores = new[] { 1, 1 }.Concat(
                "2,5,6,7,10,11,12,14,16,17,19,21,22,24"
                    .Split(',')
                    .Select(int.Parse))
            .ToArray();

        int[] jogo = GerarUmaCandidata(valores);

        AssertJogoValido(jogo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1001)]
    public void Gerar_DeveRejeitarQuantidadeForaDoIntervalo(int quantidade)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => GeradorDeJogosLotofacil.Gerar(quantidade, new Random(1)));
    }

    [Fact]
    public void Gerar_DeveRejeitarFonteAleatoriaNula()
    {
        Assert.Throws<ArgumentNullException>(
            () => GeradorDeJogosLotofacil.Gerar(1, null!));
    }

    private static int[] GerarUmaCandidata(params int[] valores) =>
        GeradorDeJogosLotofacil.Gerar(1, new AleatorioControlado(valores)).Single();

    private static int[] GerarUmaCandidata(params int[][] candidatas) =>
        GeradorDeJogosLotofacil.Gerar(
            1,
            new AleatorioControlado(candidatas.SelectMany(candidata => candidata))).Single();

    private static void AssertJogoValido(int[] jogo)
    {
        Assert.Equal(15, jogo.Length);
        Assert.Equal(15, jogo.Distinct().Count());
        Assert.Equal(jogo.Order(), jogo);
        Assert.All(jogo, numero => Assert.InRange(numero, 1, 25));
        Assert.Contains(jogo.Count(numero => numero % 2 != 0), new[] { 7, 8, 9 });
        Assert.InRange(jogo.Count(EhDaMoldura), 9, 11);
        Assert.Equal(5, jogo.Select(numero => (numero - 1) / 5).Distinct().Count());
    }

    private static bool EhDaMoldura(int numero) =>
        numero is >= 1 and <= 6
            or 10 or 11
            or 15 or 16
            or >= 20 and <= 25;

    private sealed class AleatorioControlado(IEnumerable<int> valores) : Random
    {
        private readonly Queue<int> _valores = new(valores);

        public override int Next(int minValue, int maxValue) => _valores.Dequeue();
    }
}
