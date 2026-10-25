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
            int[] candidata = GerarCandidata(aleatorio);
            if (EhValida(candidata))
            {
                jogos.Add(candidata);
            }
        }

        return jogos;
    }

    private static int[] GerarCandidata(Random aleatorio)
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

    private static bool EhValida(int[] numeros)
    {
        int quantidadeImpares = numeros.Count(numero => numero % 2 != 0);
        bool paridadeValida = quantidadeImpares is 7 or 8 or 9;
        int quantidadeMoldura = numeros.Count(EhDaMoldura);
        bool molduraValida = quantidadeMoldura is >= 9 and <= 11;
        bool gruposValidos = numeros
            .Select(ObterGrupoSequencial)
            .Distinct()
            .Count() == 5;

        return paridadeValida && molduraValida && gruposValidos;
    }

    private static bool EhDaMoldura(int numero) =>
        numero is >= 1 and <= 6
            or 10 or 11
            or 15 or 16
            or >= 20 and <= 25;

    private static int ObterGrupoSequencial(int numero) => (numero - 1) / 5;
}
