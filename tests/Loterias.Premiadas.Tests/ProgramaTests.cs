using System.Text.RegularExpressions;

namespace Loterias.Premiadas.Tests;

public class ProgramaTests
{
    [Fact]
    public void Main_ComQuantidadeValida_DeveApresentarUmJogoFormatado()
    {
        string saida = ExecutarPrograma($"1{Environment.NewLine}1");
        string linhaJogo = Assert.Single(
            saida.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries),
            linha => linha.StartsWith("Jogo 1: ", StringComparison.Ordinal));
        Match correspondencia = Regex.Match(linhaJogo, @"^Jogo 1: (?<numeros>\d{2}(?:, \d{2}){5})$");

        Assert.True(correspondencia.Success);
        int[] numeros = correspondencia.Groups["numeros"].Value
            .Split(", ")
            .Select(int.Parse)
            .ToArray();
        Assert.Equal(6, numeros.Length);
        Assert.Equal(6, numeros.Distinct().Count());
        Assert.Equal(numeros.Order(), numeros);
        Assert.All(numeros, numero => Assert.InRange(numero, 1, 60));
        Assert.DoesNotContain("Jogo 2:", saida);
    }

    [Fact]
    public void Main_ComLotofacil_DeveApresentarQuinzeNumerosFormatados()
    {
        string saida = ExecutarPrograma($"2{Environment.NewLine}1");
        string linhaJogo = Assert.Single(
            saida.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries),
            linha => linha.StartsWith("Jogo 1: ", StringComparison.Ordinal));
        Match correspondencia = Regex.Match(
            linhaJogo,
            @"^Jogo 1: (?<numeros>\d{2}(?:, \d{2}){14})$");

        Assert.True(correspondencia.Success);
        int[] numeros = correspondencia.Groups["numeros"].Value
            .Split(", ")
            .Select(int.Parse)
            .ToArray();
        Assert.Equal(15, numeros.Distinct().Count());
        Assert.Equal(numeros.Order(), numeros);
        Assert.All(numeros, numero => Assert.InRange(numero, 1, 25));
        AssertJogoLotofacilValido(numeros);
    }

    [Theory]
    [InlineData("1", 6)]
    [InlineData("2", 15)]
    public void Main_ComQuantidadeValida_DeveApresentarQuantidadeSolicitada(
        string modalidade,
        int quantidadeNumeros)
    {
        string entrada = $"{modalidade}{Environment.NewLine}3";
        string saida = ExecutarPrograma(entrada);
        string[] jogos = saida
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Where(linha => linha.StartsWith("Jogo ", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(3, jogos.Length);
        Assert.All(jogos, linha =>
            Assert.Equal(
                quantidadeNumeros,
                Regex.Matches(linha, @"\d{2}").Count));
        Assert.DoesNotContain("Jogo 4:", saida);
    }

    [Theory]
    [InlineData("")]
    [InlineData("texto")]
    [InlineData("1,5")]
    [InlineData(" ")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("1001")]
    public void Main_ComEntradaInvalida_DeveInformarEPermitirNovaTentativa(string entradaInvalida)
    {
        string saida = ExecutarPrograma($"1{Environment.NewLine}{entradaInvalida}{Environment.NewLine}1");
        string[] linhas = saida.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(2, linhas.Count(linha => linha == "Informe a quantidade de jogos (1 a 1000):"));
        Assert.Contains("A quantidade deve ser um número inteiro entre 1 e 1000.", saida);
        Assert.Single(linhas, linha => linha.StartsWith("Jogo 1: ", StringComparison.Ordinal));
        Assert.DoesNotContain("Jogo 2:", saida);
    }

    [Theory]
    [InlineData("")]
    [InlineData("3")]
    [InlineData("Lotomania")]
    public void Main_ComModalidadeInvalida_DevePermitirNovaTentativa(string modalidadeInvalida)
    {
        string saida = ExecutarPrograma(
            $"{modalidadeInvalida}{Environment.NewLine}1{Environment.NewLine}1");
        string[] linhas = saida.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(2, linhas.Count(linha =>
            linha == "Escolha a modalidade (1 - Mega-Sena, 2 - Lotofácil):"));
        Assert.Contains("A modalidade deve ser Mega-Sena ou Lotofácil.", saida);
        Assert.Single(linhas, linha => linha.StartsWith("Jogo 1: ", StringComparison.Ordinal));
    }

    [Fact]
    public void Main_ComFimDeEntradaNaSelecao_DeveEncerrarSemGerarJogos()
    {
        string saida = ExecutarPrograma(string.Empty);

        Assert.Equal(
            "Escolha a modalidade (1 - Mega-Sena, 2 - Lotofácil):",
            saida.Trim());
        Assert.DoesNotContain("A quantidade deve ser", saida);
        Assert.DoesNotContain("Jogo ", saida);
    }

    [Fact]
    public void Main_ComFimDeEntradaNaQuantidade_DeveEncerrarSemGerarJogos()
    {
        string saida = ExecutarPrograma("1");

        Assert.Contains("Informe a quantidade de jogos (1 a 1000):", saida);
        Assert.DoesNotContain("Jogo ", saida);
    }

    private static void AssertJogoLotofacilValido(int[] numeros)
    {
        Assert.Contains(numeros.Count(numero => numero % 2 != 0), new[] { 7, 8, 9 });
        Assert.InRange(numeros.Count(numero => numero is >= 1 and <= 6 or 10 or 11
            or 15 or 16 or >= 20 and <= 25), 9, 11);
        Assert.Equal(5, numeros.Select(numero => (numero - 1) / 5).Distinct().Count());
    }

    private static string ExecutarPrograma(string entrada)
    {
        TextReader entradaOriginal = Console.In;
        TextWriter saidaOriginal = Console.Out;
        using var leitor = new StringReader(entrada);
        using var escritor = new StringWriter();

        try
        {
            Console.SetIn(leitor);
            Console.SetOut(escritor);
            Program.Main();
            return escritor.ToString();
        }
        finally
        {
            Console.SetIn(entradaOriginal);
            Console.SetOut(saidaOriginal);
        }
    }
}
