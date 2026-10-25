namespace Loterias.Premiadas;

public static class GeradorDeJogos
{
    public static IReadOnlyList<int[]> Gerar(int quantidade, Random aleatorio)
    {
        if (aleatorio is null)
        {
            throw new ArgumentNullException(nameof(aleatorio));
        }

        if (quantidade < 1 || quantidade > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade deve estar entre 1 e 1000.");
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
        var numeros = new List<int>(6);
        var numerosUnicos = new HashSet<int>();
        while (numeros.Count < 6)
        {
            int numero = aleatorio.Next(1, 61);
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
        int quantidadePares = numeros.Count(numero => numero % 2 == 0);
        bool paridadeValida = quantidadePares is 2 or 3 or 4;
        int quantidadeGrupos = numeros.Select(ObterGrupo).Distinct().Count();
        bool gruposValidos = quantidadeGrupos >= 3;
        int soma = numeros.Sum();
        bool somaValida = soma >= 150 && soma <= 220;
        int regrasAtendidas = (paridadeValida ? 1 : 0)
            + (gruposValidos ? 1 : 0)
            + (somaValida ? 1 : 0);

        if (regrasAtendidas < 2)
        {
            return false;
        }

        var quantidadePorFinal = new int[10];
        foreach (int numero in numeros)
        {
            int final = numero % 10;
            quantidadePorFinal[final]++;
            if (quantidadePorFinal[final] > 2)
            {
                return false;
            }
        }

        for (int indice = 2; indice < numeros.Length; indice++)
        {
            if (numeros[indice - 2] + 1 == numeros[indice - 1]
                && numeros[indice - 1] + 1 == numeros[indice])
            {
                return false;
            }
        }

        return true;
    }

    private static int ObterGrupo(int numero)
    {
        int posicaoNoBloco = (numero - 1) % 10;
        if (numero <= 30)
        {
            return posicaoNoBloco < 5 ? 1 : 2;
        }

        return (numero - 31) % 10 < 5 ? 3 : 4;
    }
}
