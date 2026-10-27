using System.Globalization;
using Spectre.Console;

namespace Loterias.Premiadas.Services;

public static class Program
{
    public static void Main()
    {
        IAnsiConsole console = AnsiConsole.Create(
            new AnsiConsoleSettings
            {
                Out = new AnsiConsoleOutput(Console.Out)
            });
        bool modoInterativo = console.Profile.Capabilities.Interactive;
        modoInterativo &= !Console.IsInputRedirected;
        modoInterativo &= !Console.IsOutputRedirected;

        Executar(console, Gerar, modoInterativo);
    }

    public static void Executar(
        IAnsiConsole console,
        Func<string, int, IEnumerable<int[]>> gerador,
        bool? interativo = null)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(gerador);

        bool modoInterativo = interativo ?? console.Profile.Capabilities.Interactive;
        while (true)
        {
            string? modalidade = SolicitarModalidade(console, modoInterativo);
            if (modalidade is null)
            {
                return;
            }

            int? quantidade = SolicitarQuantidade(console, modoInterativo);
            if (quantidade is null)
            {
                return;
            }

            IReadOnlyList<int[]> jogos;
            try
            {
                IEnumerable<int[]> jogosGerados = gerador(modalidade, quantidade.Value)
                    ?? throw new InvalidOperationException("A geração não retornou jogos.");
                jogos = jogosGerados.ToList();
                if (jogos.Count != quantidade.Value)
                {
                    throw new InvalidOperationException("A quantidade gerada não corresponde à solicitada.");
                }

                if (jogos.Any(jogo => jogo is null))
                {
                    throw new InvalidOperationException("A geração retornou um jogo inválido.");
                }
            }
            catch (Exception excecao)
            {
                string mensagem = string.IsNullOrWhiteSpace(excecao.Message)
                    ? "Falha ao gerar os jogos."
                    : $"Falha ao gerar os jogos: {excecao.Message}";
                console.WriteLine(mensagem);
                continue;
            }

            ApresentarResultado(console, modalidade, quantidade.Value, jogos);

            bool? continuar = SolicitarContinuacao(console, modoInterativo);
            if (continuar is null || !continuar.Value)
            {
                return;
            }
        }
    }

    private static string? SolicitarModalidade(IAnsiConsole console, bool interativo)
    {
        if (interativo)
        {
            return console.Prompt(
                new SelectionPrompt<string>()
                    .Title("Escolha Mega-Sena ou Lotofácil")
                    .AddChoices("Mega-Sena", "Lotofácil"));
        }

        while (true)
        {
            console.WriteLine("Escolha a modalidade (1 - Mega-Sena, 2 - Lotofácil):");
            string? entrada = Console.ReadLine();
            if (entrada is null)
            {
                return null;
            }

            if (entrada is "1" or "2")
            {
                return entrada == "1" ? "Mega-Sena" : "Lotofácil";
            }

            console.WriteLine("A modalidade deve ser Mega-Sena ou Lotofácil.");
        }
    }

    private static int? SolicitarQuantidade(IAnsiConsole console, bool interativo)
    {
        const string mensagemErro = "A quantidade deve ser um número inteiro entre 1 e 1.000.";
        if (interativo)
        {
            string entrada = console.Prompt(
                new TextPrompt<string>("Quantidade de jogos (1 a 1.000)")
                    .AllowEmpty()
                    .Validate(
                        valor => int.TryParse(valor, out int quantidade)
                            && quantidade is >= 1 and <= 1000,
                        mensagemErro));
            return int.Parse(entrada, CultureInfo.InvariantCulture);
        }

        while (true)
        {
            console.WriteLine("Informe a quantidade de jogos (1 a 1000):");
            string? entrada = Console.ReadLine();
            if (entrada is null)
            {
                return null;
            }

            if (int.TryParse(entrada, out int quantidade)
                && quantidade is >= 1 and <= 1000)
            {
                return quantidade;
            }

            console.WriteLine(mensagemErro);
        }
    }

    private static void ApresentarResultado(
        IAnsiConsole console,
        string modalidade,
        int quantidadeSolicitada,
        IReadOnlyList<int[]> jogos)
    {
        console.WriteLine(
            $"{modalidade}: {quantidadeSolicitada} jogo(s) solicitado(s) e {jogos.Count} apresentado(s).");

        var tabela = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Jogo")
            .AddColumn("Números");

        for (int indice = 0; indice < jogos.Count; indice++)
        {
            string numeros = string.Join(
                ", ",
                jogos[indice].Select(numero => numero.ToString("D2", CultureInfo.InvariantCulture)));
            tabela.AddRow($"Jogo {indice + 1}", numeros);
        }

        console.Write(tabela);
    }

    private static bool? SolicitarContinuacao(IAnsiConsole console, bool interativo)
    {
        if (interativo)
        {
            return console.Prompt(
                new ConfirmationPrompt("Deseja gerar outros jogos?")
                    .Yes('s')
                    .No('n'));
        }

        while (true)
        {
            console.WriteLine("Deseja gerar outros jogos? (s/n):");
            string? entrada = Console.ReadLine();
            if (entrada is null)
            {
                return null;
            }

            string resposta = entrada.Trim().ToLowerInvariant();
            if (resposta is "s" or "sim")
            {
                return true;
            }

            if (resposta is "n" or "nao" or "não")
            {
                return false;
            }

            console.WriteLine("Responda s para continuar ou n para encerrar.");
        }
    }

    private static IEnumerable<int[]> Gerar(string modalidade, int quantidade) =>
        modalidade == "Mega-Sena"
            ? MegasenaService.Gerar(quantidade, new Random())
            : LotofacilService.Gerar(quantidade, new Random());
}
