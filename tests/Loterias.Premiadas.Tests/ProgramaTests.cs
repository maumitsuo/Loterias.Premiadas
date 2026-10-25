using System.Text.RegularExpressions;

namespace Loterias.Premiadas.Tests;

public class ProgramaTests
{
    [Fact]
    public void Main_ComQuantidadeValida_DeveApresentarUmJogoFormatado()
    {
        string saida = ExecutarPrograma("1");
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
        string saida = ExecutarPrograma($"{entradaInvalida}{Environment.NewLine}1");
        string[] linhas = saida.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(2, linhas.Count(linha => linha == "Informe a quantidade de jogos (1 a 1000):"));
        Assert.Contains("A quantidade deve ser um número inteiro entre 1 e 1000.", saida);
        Assert.Single(linhas, linha => linha.StartsWith("Jogo 1: ", StringComparison.Ordinal));
        Assert.DoesNotContain("Jogo 2:", saida);
    }

    [Fact]
    public void Main_ComFimDeEntrada_DeveEncerrarSemGerarJogos()
    {
        string saida = ExecutarPrograma(string.Empty);

        Assert.Equal("Informe a quantidade de jogos (1 a 1000):", saida.Trim());
        Assert.DoesNotContain("A quantidade deve ser", saida);
        Assert.DoesNotContain("Jogo ", saida);
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
