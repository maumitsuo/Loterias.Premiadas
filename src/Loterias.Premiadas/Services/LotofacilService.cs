namespace Loterias.Premiadas.Services;

public static class LotofacilService
{
    public static IReadOnlyList<int[]> Gerar(int quantidade, Random aleatorio)
    {
        if (aleatorio is null)
        {
            throw new ArgumentNullException(nameof(aleatorio));
        }

        if (quantidade < 1 || quantidade > 1000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantidade),
                "A quantidade deve estar entre 1 e 1000.");
        }

        var jogos = new List<int[]>(quantidade);
        while (jogos.Count < quantidade)
        {
            jogos.Add(GerarJogo(aleatorio));
        }

        return jogos;
    }

    private static int[] GerarJogo(Random aleatorio)
    {
        var numeros = new List<int>(15);
        var numerosUnicos = new HashSet<int>();
        while (numeros.Count < 15)
        {
            int numero = aleatorio.Next(1, 26);
            if (numerosUnicos.Add(numero))
            {
                numeros.Add(numero);
            }
        }

        numeros.Sort();
        return numeros.ToArray();
    }
}
