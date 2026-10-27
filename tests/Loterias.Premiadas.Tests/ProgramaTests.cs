using Loterias.Premiadas.Services;
using Spectre.Console.Testing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Loterias.Premiadas.Tests;

[CollectionDefinition("Console interaction", DisableParallelization = true)]
public sealed class ConsoleInteractionCollection
{
}

[Collection("Console interaction")]
public class ProgramaTests
{
    [Theory]
    [InlineData(ConsoleKey.Enter, "Mega-Sena", 1)]
    [InlineData(ConsoleKey.Enter, "Mega-Sena", 1000)]
    [InlineData(ConsoleKey.DownArrow, "Lotofácil", 1)]
    [InlineData(ConsoleKey.DownArrow, "Lotofácil", 1000)]
    public void Executar_ComSolicitacaoValida_DeveGerarEApresentarQuantidadeEscolhida(
        ConsoleKey teclaModalidade,
        string modalidade,
        int quantidade)
    {
        using var console = new TestConsole().Interactive().Colors(Spectre.Console.ColorSystem.NoColors);
        console.Input.PushKey(teclaModalidade);
        console.Input.PushKey(ConsoleKey.Enter);
        console.Input.PushTextWithEnter(quantidade.ToString());
        console.Input.PushTextWithEnter("n");
        string modalidadeGerada = string.Empty;
        int quantidadeGerada = 0;

        Program.Executar(
            console,
            (modalidadeEscolhida, quantidadeEscolhida) =>
            {
                modalidadeGerada = modalidadeEscolhida;
                quantidadeGerada = quantidadeEscolhida;
                return Enumerable.Repeat(new[] { 1, 2 }, quantidadeEscolhida);
            });

        Assert.Equal(modalidade, modalidadeGerada);
        Assert.Equal(quantidade, quantidadeGerada);
        Assert.Contains(modalidade, console.Output);
        Assert.Contains($"{quantidade} jogo(s) solicitado(s)", console.Output);
        Assert.Contains($"{quantidade} apresentado(s)", console.Output);
        int[] identificadores = Regex.Matches(console.Output, @"Jogo (\d+)")
            .Select(correspondencia => int.Parse(correspondencia.Groups[1].Value, CultureInfo.InvariantCulture))
            .ToArray();
        Assert.Equal(Enumerable.Range(1, quantidade), identificadores);
        Assert.Contains("01, 02", console.Output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Executar_QuandoGeracaoFalha_DeveDescartarParcialEReiniciarSolicitacao(bool falhaDuranteEnumeracao)
    {
        using var console = new TestConsole().Interactive();
        AdicionarSolicitacao(console, "1", 2);
        AdicionarSolicitacao(console, "1", 1);
        console.Input.PushTextWithEnter("n");
        int chamadas = 0;

        Program.Executar(
            console,
            (_, quantidade) =>
            {
                chamadas++;
                if (chamadas == 1)
                {
                    if (falhaDuranteEnumeracao)
                    {
                        return JogosParciaisComFalha();
                    }

                    throw new InvalidOperationException("falha simulada");
                }

                return Enumerable.Repeat(new[] { 7, 8 }, quantidade);
            });

        Assert.Equal(2, chamadas);
        Assert.Contains("falha simulada", console.Output);
        Assert.Equal(2, console.Output.Split("Escolha Mega-Sena ou Lotofácil", StringSplitOptions.None).Length - 1);
        Assert.Single(Regex.Matches(console.Output, @"Jogo \d+\s").Cast<Match>());
        Assert.DoesNotContain("Jogo 2", console.Output);
    }

    [Theory]
    [InlineData("")]
    [InlineData("texto")]
    [InlineData("0")]
    [InlineData("1001")]
    public void Executar_ComQuantidadeInvalida_DeveRepetirQuantidade(string quantidadeInvalida)
    {
        using var console = new TestConsole().Interactive();
        AdicionarModalidade(console, "1");
        console.Input.PushTextWithEnter(quantidadeInvalida);
        console.Input.PushTextWithEnter("1");
        console.Input.PushTextWithEnter("n");
        int chamadas = 0;

        Program.Executar(console, (_, quantidade) =>
        {
            chamadas++;
            return Enumerable.Repeat(new[] { 1 }, quantidade);
        });

        Assert.Equal(1, chamadas);
        Assert.Contains("inteiro entre 1 e 1.000", console.Output);
        Assert.Equal(2, console.Output.Split("Quantidade de jogos", StringSplitOptions.None).Length - 1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("3")]
    [InlineData("Lotomania")]
    public void Executar_ComModalidadeInvalida_DeveRepetirSomenteModalidade(string modalidadeInvalida)
    {
        using var console = new TestConsole()
            .Colors(Spectre.Console.ColorSystem.NoColors)
            .SupportsAnsi(false);
        TextReader entradaOriginal = Console.In;
        using var leitor = new StringReader(
            $"{modalidadeInvalida}{Environment.NewLine}1{Environment.NewLine}1{Environment.NewLine}n");
        int chamadas = 0;

        try
        {
            Console.SetIn(leitor);
            Program.Executar(console, (_, quantidade) =>
            {
                chamadas++;
                return Enumerable.Repeat(new[] { 1 }, quantidade);
            }, interativo: false);
        }
        finally
        {
            Console.SetIn(entradaOriginal);
        }

        Assert.Equal(1, chamadas);
        Assert.Contains("A modalidade deve ser Mega-Sena ou Lotofácil.", console.Output);
        Assert.Contains("Informe a quantidade de jogos", console.Output);
    }

    [Fact]
    public void Executar_ComFalhaSemMensagem_DeveExibirErroGenerico()
    {
        using var console = new TestConsole().Interactive();
        AdicionarSolicitacao(console, "1", 1);
        AdicionarSolicitacao(console, "1", 1);
        console.Input.PushTextWithEnter("n");
        int chamadas = 0;

        Program.Executar(console, (_, quantidade) =>
        {
            chamadas++;
            return chamadas == 1
                ? throw new InvalidOperationException(string.Empty)
                : Enumerable.Repeat(new[] { 1 }, quantidade);
        });

        Assert.Contains("Falha ao gerar os jogos", console.Output);
    }

    [Fact]
    public void Executar_ComQuantidadeIncompleta_DeveTratarComoFalha()
    {
        using var console = new TestConsole().Interactive();
        AdicionarSolicitacao(console, "1", 2);
        AdicionarSolicitacao(console, "1", 1);
        console.Input.PushTextWithEnter("n");
        int chamadas = 0;

        Program.Executar(console, (_, quantidade) =>
        {
            chamadas++;
            return Enumerable.Repeat(new[] { 1 }, chamadas == 1 ? quantidade - 1 : quantidade);
        });

        Assert.Contains("Falha ao gerar os jogos", console.Output);
        Assert.Single(Regex.Matches(console.Output, @"Jogo \d+\s").Cast<Match>());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Executar_AposResultado_DevePermitirRepetirOuEncerrar(bool continuar)
    {
        using var console = new TestConsole().Interactive();
        AdicionarSolicitacao(console, "1", 1);
        if (continuar)
        {
            console.Input.PushTextWithEnter("s");
            AdicionarSolicitacao(console, "2", 1);
            console.Input.PushTextWithEnter("n");
        }
        else
        {
            console.Input.PushTextWithEnter("n");
        }

        int chamadas = 0;
        Program.Executar(console, (_, quantidade) =>
        {
            chamadas++;
            return Enumerable.Repeat(new[] { 1 }, quantidade);
        });

        Assert.Equal(continuar ? 2 : 1, chamadas);
    }

    [Fact]
    public void Executar_ComSaidaRedirecionada_DeveUsarFluxoTextual()
    {
        using var console = new TestConsole()
            .Colors(Spectre.Console.ColorSystem.NoColors)
            .SupportsAnsi(false);
        TextReader entradaOriginal = Console.In;
        using var leitor = new StringReader($"1{Environment.NewLine}1{Environment.NewLine}n");

        try
        {
            Console.SetIn(leitor);
            Program.Executar(
                console,
                (_, quantidade) => Enumerable.Repeat(new[] { 3, 5 }, quantidade),
                interativo: false);
        }
        finally
        {
            Console.SetIn(entradaOriginal);
        }

        Assert.Contains("Escolha a modalidade", console.Output);
        Assert.Contains("Jogo 1", console.Output);
    }

    [Fact]
    public void Executar_ComFimDeEntrada_DeveEncerrarSemGerar()
    {
        using var console = new TestConsole();
        TextReader entradaOriginal = Console.In;
        using var leitor = new StringReader(string.Empty);
        int chamadas = 0;

        try
        {
            Console.SetIn(leitor);
            Program.Executar(
                console,
                (_, quantidade) =>
                {
                    chamadas++;
                    return Enumerable.Repeat(new[] { 1 }, quantidade);
                },
                interativo: false);
        }
        finally
        {
            Console.SetIn(entradaOriginal);
        }

        Assert.Equal(0, chamadas);
        Assert.DoesNotContain("Jogo", console.Output);
    }

    [Fact]
    public void Executar_ComFimDeEntradaNaQuantidade_DeveEncerrarSemGerar()
    {
        using var console = new TestConsole();
        TextReader entradaOriginal = Console.In;
        using var leitor = new StringReader($"1{Environment.NewLine}");
        int chamadas = 0;

        try
        {
            Console.SetIn(leitor);
            Program.Executar(
                console,
                (_, quantidade) =>
                {
                    chamadas++;
                    return Enumerable.Repeat(new[] { 1 }, quantidade);
                },
                interativo: false);
        }
        finally
        {
            Console.SetIn(entradaOriginal);
        }

        Assert.Equal(0, chamadas);
        Assert.DoesNotContain("Jogo", console.Output);
    }

    [Fact]
    public void Executar_ComFimDeEntradaNaContinuacao_DeveEncerrarAposResultado()
    {
        using var console = new TestConsole();
        TextReader entradaOriginal = Console.In;
        using var leitor = new StringReader($"1{Environment.NewLine}1{Environment.NewLine}");

        try
        {
            Console.SetIn(leitor);
            Program.Executar(
                console,
                (_, quantidade) => Enumerable.Repeat(new[] { 1 }, quantidade),
                interativo: false);
        }
        finally
        {
            Console.SetIn(entradaOriginal);
        }

        Assert.Contains("Jogo 1", console.Output);
    }

    [Fact]
    public void Executar_ComEntradaTextualInvalida_DeveRepetirCamposEPermitirContinuar()
    {
        using var console = new TestConsole();
        TextReader entradaOriginal = Console.In;
        using var leitor = new StringReader(
            $"x{Environment.NewLine}1{Environment.NewLine}texto{Environment.NewLine}0{Environment.NewLine}" +
            $"1001{Environment.NewLine}1{Environment.NewLine}" +
            $"?{Environment.NewLine}s{Environment.NewLine}2{Environment.NewLine}1{Environment.NewLine}n{Environment.NewLine}");
        int chamadas = 0;

        try
        {
            Console.SetIn(leitor);
            Program.Executar(
                console,
                (_, quantidade) =>
                {
                    chamadas++;
                    return Enumerable.Repeat(new[] { 1, 2 }, quantidade);
                },
                interativo: false);
        }
        finally
        {
            Console.SetIn(entradaOriginal);
        }

        Assert.Equal(2, chamadas);
        Assert.Contains("A modalidade deve ser Mega-Sena ou Lotofácil.", console.Output);
        Assert.Contains("A quantidade deve ser um número inteiro entre 1 e 1.000.", console.Output);
        Assert.Contains("Responda s para continuar ou n para encerrar.", console.Output);
        Assert.Equal(2, console.Output.Split("jogo(s) solicitado(s)", StringSplitOptions.None).Length - 1);
    }

    [Fact]
    public void Executar_ComGeradorNulo_DeveMostrarFalhaEReiniciar()
    {
        using var console = new TestConsole().Interactive();
        AdicionarSolicitacao(console, "1", 1);
        AdicionarSolicitacao(console, "1", 1);
        console.Input.PushTextWithEnter("n");
        int chamadas = 0;

        Program.Executar(console, (_, quantidade) =>
        {
            chamadas++;
            return chamadas == 1
                ? null!
                : Enumerable.Repeat(new[] { 1 }, quantidade);
        });

        Assert.Contains("A geração não retornou jogos", console.Output);
        Assert.Equal(2, chamadas);
    }

    [Fact]
    public void Executar_ComJogoNulo_DeveMostrarFalhaSemApresentarResultadoParcial()
    {
        using var console = new TestConsole().Interactive();
        AdicionarSolicitacao(console, "1", 1);
        AdicionarSolicitacao(console, "1", 1);
        console.Input.PushTextWithEnter("n");
        int chamadas = 0;

        Program.Executar(console, (_, quantidade) =>
        {
            chamadas++;
            return chamadas == 1
                ? new[] { (int[])null! }
                : Enumerable.Repeat(new[] { 1 }, quantidade);
        });

        Assert.Contains("A geração retornou um jogo inválido", console.Output);
        Assert.Single(Regex.Matches(console.Output, @"Jogo \d+\s").Cast<Match>());
    }

    [Fact]
    public void Executar_ComDependenciaNula_DeveLancarArgumentNullException()
    {
        using var console = new TestConsole();

        Assert.Throws<ArgumentNullException>(() =>
            Program.Executar(null!, (_, _) => Array.Empty<int[]>()));
        Assert.Throws<ArgumentNullException>(() =>
            Program.Executar(console, null!));
    }

    [Theory]
    [InlineData("1", 60)]
    [InlineData("2", 25)]
    public void Main_ComCadaModalidade_DeveUsarGeradorExistente(
        string modalidade,
        int numeroMaximo)
    {
        TextReader entradaOriginal = Console.In;
        TextWriter saidaOriginal = Console.Out;
        using var leitor = new StringReader(
            $"{modalidade}{Environment.NewLine}1{Environment.NewLine}n{Environment.NewLine}");
        using var escritor = new StringWriter(CultureInfo.InvariantCulture);

        try
        {
            Console.SetIn(leitor);
            Console.SetOut(escritor);
            Program.Main();
        }
        finally
        {
            Console.SetIn(entradaOriginal);
            Console.SetOut(saidaOriginal);
        }

        Assert.Contains(modalidade == "1" ? "Mega-Sena" : "Lotofácil", escritor.ToString());
        string linhaJogo = escritor.ToString()
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Single(linha => linha.Contains("Jogo 1", StringComparison.Ordinal));
        int[] numeros = Regex.Matches(linhaJogo, @"(?<!\d)\d{2}(?!\d)")
            .Select(correspondencia => int.Parse(correspondencia.Value, CultureInfo.InvariantCulture))
            .ToArray();
        Assert.Equal(modalidade == "1" ? 6 : 15, numeros.Length);
        Assert.All(numeros, numero => Assert.InRange(numero, 1, numeroMaximo));
        Assert.Equal(numeros.Order(), numeros);
    }

    [Fact]
    public void Main_ComFimDeEntrada_DeveEncerrarSemGerar()
    {
        TextReader entradaOriginal = Console.In;
        TextWriter saidaOriginal = Console.Out;
        using var leitor = new StringReader(string.Empty);
        using var escritor = new StringWriter();

        try
        {
            Console.SetIn(leitor);
            Console.SetOut(escritor);
            Program.Main();
        }
        finally
        {
            Console.SetIn(entradaOriginal);
            Console.SetOut(saidaOriginal);
        }

        Assert.Contains("Escolha a modalidade", escritor.ToString());
        Assert.DoesNotContain("Jogo", escritor.ToString());
    }

    private static void AdicionarSolicitacao(TestConsole console, string modalidade, int quantidade)
    {
        AdicionarModalidade(console, modalidade);
        console.Input.PushTextWithEnter(quantidade.ToString());
    }

    private static void AdicionarModalidade(TestConsole console, string modalidade)
    {
        if (modalidade == "2")
        {
            console.Input.PushKey(ConsoleKey.DownArrow);
        }

        console.Input.PushKey(ConsoleKey.Enter);
    }

    private static IEnumerable<int[]> JogosParciaisComFalha()
    {
        yield return new[] { 1, 2 };
        throw new InvalidOperationException("falha simulada");
    }
}
