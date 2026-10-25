namespace Loterias.Premiadas;

public static class Program
{
    public static void Main()
    {
        while (true)
        {
            Console.WriteLine("Informe a quantidade de jogos (1 a 1000):");
            string? entrada = Console.ReadLine();
            if (entrada is null)
            {
                return;
            }

            if (!int.TryParse(entrada, out int quantidade) || quantidade < 1 || quantidade > 1000)
            {
                Console.WriteLine("A quantidade deve ser um número inteiro entre 1 e 1000.");
                continue;
            }

            IReadOnlyList<int[]> jogos = GeradorDeJogos.Gerar(quantidade, new Random());
            for (int indice = 0; indice < jogos.Count; indice++)
            {
                string numeros = string.Join(", ", jogos[indice].Select(numero => numero.ToString("D2")));
                Console.WriteLine($"Jogo {indice + 1}: {numeros}");
            }

            return;
        }
    }
}
