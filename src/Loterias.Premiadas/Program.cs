namespace Loterias.Premiadas;

public static class Program
{
    public static void Main()
    {
        string modalidade;
        while (true)
        {
            Console.WriteLine("Escolha a modalidade (1 - Mega-Sena, 2 - Lotofácil):");
            string? entradaModalidade = Console.ReadLine();
            if (entradaModalidade is null)
            {
                return;
            }

            if (entradaModalidade is "1" or "2")
            {
                modalidade = entradaModalidade;
                break;
            }

            Console.WriteLine("A modalidade deve ser Mega-Sena ou Lotofácil.");
        }

        int quantidade;
        while (true)
        {
            Console.WriteLine("Informe a quantidade de jogos (1 a 1000):");
            string? entradaQuantidade = Console.ReadLine();
            if (entradaQuantidade is null)
            {
                return;
            }

            if (int.TryParse(entradaQuantidade, out quantidade)
                && quantidade is >= 1 and <= 1000)
            {
                break;
            }

            Console.WriteLine("A quantidade deve ser um número inteiro entre 1 e 1000.");
        }

        IReadOnlyList<int[]> jogos = modalidade == "1"
            ? GeradorDeJogos.Gerar(quantidade, new Random())
            : GeradorDeJogosLotofacil.Gerar(quantidade, new Random());

        for (int indice = 0; indice < jogos.Count; indice++)
        {
            string numeros = string.Join(", ", jogos[indice].Select(numero => numero.ToString("D2")));
            Console.WriteLine($"Jogo {indice + 1}: {numeros}");
        }
    }
}
