using Loterias.Premiadas.Services;

namespace Loterias.Premiadas.Tests;

public class MegasenaTests
{
    [Fact]
    public void Gerar_DeveRetornarQuantidadeSolicitadaDeJogosValidos()
    {
        IReadOnlyList<int[]> jogos = MegasenaService.Gerar(1000, new Random(42));

        Assert.Equal(1000, jogos.Count);
        foreach (int[] jogo in jogos)
        {
            AssertJogoValido(jogo);
        }
    }

    [Fact]
    public void Gerar_DeveAceitarQuandoParidadeEGruposSaoAtendidos()
    {
        int[] jogo = GerarUmaCandidata(1, 6, 12, 31, 32, 35);

        AssertJogoValido(jogo);
        Assert.True(jogo.Sum() < 150);
    }

    [Theory]
    [InlineData(2, 11, 13, 24, 35, 57, 2)]
    [InlineData(2, 11, 24, 33, 46, 48, 4)]
    public void Gerar_DeveAceitarLimitesInclusivosDeParidade(
        int primeiro,
        int segundo,
        int terceiro,
        int quarto,
        int quinto,
        int sexto,
        int quantidadeParesEsperada)
    {
        int[] jogo = GerarUmaCandidata(primeiro, segundo, terceiro, quarto, quinto, sexto);

        AssertJogoValido(jogo);
        Assert.Equal(quantidadeParesEsperada, jogo.Count(numero => numero % 2 == 0));
    }

    [Fact]
    public void Gerar_DeveAceitarQuandoParidadeESomaSaoAtendidas()
    {
        int[] jogo = GerarUmaCandidata(19, 20, 26, 27, 29, 30);

        AssertJogoValido(jogo);
        Assert.Single(jogo.Select(ObterGrupo).Distinct());
    }

    [Theory]
    [InlineData(11, 13, 15, 33, 35, 46)]
    [InlineData(2, 13, 24, 32, 46, 54)]
    public void Gerar_DeveAceitarQuandoGruposESomaSaoAtendidos(
        int primeiro,
        int segundo,
        int terceiro,
        int quarto,
        int quinto,
        int sexto)
    {
        int[] jogo = GerarUmaCandidata(primeiro, segundo, terceiro, quarto, quinto, sexto);

        AssertJogoValido(jogo);
        Assert.Contains(jogo.Count(numero => numero % 2 == 0), new[] { 1, 5 });
    }

    [Fact]
    public void Gerar_DeveSubstituirCandidataComTresNumerosConsecutivos()
    {
        int[] jogo = GerarUmaCandidata(
            new[] { 1, 2, 3, 20, 45, 58 },
            new[] { 2, 11, 24, 33, 36, 57 });

        AssertJogoValido(jogo);
        Assert.Equal(new[] { 2, 11, 24, 33, 36, 57 }, jogo);
    }

    [Fact]
    public void Gerar_DeveSubstituirCandidataComTresFinaisIguais()
    {
        int[] jogo = GerarUmaCandidata(
            new[] { 2, 12, 22, 31, 44, 55 },
            new[] { 2, 11, 24, 33, 36, 57 });

        AssertJogoValido(jogo);
        Assert.Equal(new[] { 2, 11, 24, 33, 36, 57 }, jogo);
    }

    [Fact]
    public void Gerar_DevePermitirUmParConsecutivo()
    {
        int[] jogo = GerarUmaCandidata(1, 2, 5, 20, 45, 58);

        AssertJogoValido(jogo);
        Assert.Contains(jogo.Zip(jogo.Skip(1)), par => par.Second == par.First + 1);
    }

    [Theory]
    [InlineData(1, 8, 13, 26, 47, 55, 150)]
    [InlineData(11, 20, 31, 42, 56, 60, 220)]
    public void Gerar_DeveAceitarLimitesInclusivosDaSoma(
        int primeiro,
        int segundo,
        int terceiro,
        int quarto,
        int quinto,
        int sexto,
        int somaEsperada)
    {
        int[] jogo = GerarUmaCandidata(primeiro, segundo, terceiro, quarto, quinto, sexto);

        AssertJogoValido(jogo);
        Assert.Equal(somaEsperada, jogo.Sum());
    }

    [Fact]
    public void Gerar_DeveSubstituirCandidataQueAtendeMenosDeDuasRegras()
    {
        int[] jogo = GerarUmaCandidata(
            new[] { 6, 7, 8, 9, 16, 17 },
            new[] { 2, 11, 24, 33, 36, 57 });

        AssertJogoValido(jogo);
    }

    [Fact]
    public void Gerar_DeveDescartarNumerosRepetidosNaCandidata()
    {
        int[] jogo = GerarUmaCandidata(2, 2, 11, 24, 33, 36, 57);

        AssertJogoValido(jogo);
        Assert.Equal(new[] { 2, 11, 24, 33, 36, 57 }, jogo);
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

    private static int[] GerarUmaCandidata(params int[] valores) =>
        MegasenaService.Gerar(1, new AleatorioControlado(valores)).Single();

    private static int[] GerarUmaCandidata(params int[][] candidatas) =>
        MegasenaService.Gerar(1, new AleatorioControlado(candidatas.SelectMany(candidata => candidata))).Single();

    private static void AssertJogoValido(int[] jogo)
    {
        Assert.Equal(6, jogo.Length);
        Assert.Equal(jogo.Order(), jogo);
        Assert.Equal(6, jogo.Distinct().Count());
        Assert.All(jogo, numero => Assert.InRange(numero, 1, 60));
        Assert.True(jogo.Count(numero => numero % 2 == 0) is 2 or 3 or 4
            || jogo.Select(ObterGrupo).Distinct().Count() >= 3
            || jogo.Sum() is >= 150 and <= 220);
        Assert.All(jogo.GroupBy(numero => numero % 10), grupo => Assert.True(grupo.Count() <= 2));
        Assert.DoesNotContain(Enumerable.Range(3, 58), numero =>
            jogo.Contains(numero - 2) && jogo.Contains(numero - 1) && jogo.Contains(numero));
    }

    private static int ObterGrupo(int numero) =>
        numero <= 30
            ? (numero - 1) % 10 < 5 ? 1 : 2
            : (numero - 31) % 10 < 5 ? 3 : 4;

    private sealed class AleatorioControlado(IEnumerable<int> valores) : Random
    {
        private readonly Queue<int> _valores = new(valores);

        public override int Next(int minValue, int maxValue) => _valores.Dequeue();
    }
}
