# Contrato: Serviços de geração de jogos

Vale para `MegasenaService.Gerar` e `LotofacilService.Gerar`. A assinatura não muda:

```text
IReadOnlyList<int[]> Gerar(int quantidade, Random aleatorio)
```

## Pré-condições (inalteradas)

| Condição | Resultado |
|----------|-----------|
| `aleatorio` nulo | `ArgumentNullException` |
| `quantidade` < 1 ou > 1000 | `ArgumentOutOfRangeException` com a mensagem "A quantidade deve estar entre 1 e 1000." |

## Pós-condições

- A lista tem exatamente `quantidade` jogos.
- Cada jogo segue as regras de [data-model.md](../data-model.md): quantidade e intervalo da modalidade, sem números repetidos, em ordem crescente.
- Nenhum jogo é descartado por critério estatístico: o primeiro jogo completo sorteado é sempre aceito.
- Jogos iguais na mesma lista são permitidos.

## Uso de `Random`

- Cada número é obtido com `aleatorio.Next(1, 61)` (Mega-Sena) ou `aleatorio.Next(1, 26)` (Lotofácil).
- Um número já presente no jogo em formação é ignorado e um novo valor é solicitado.
- Para `quantidade = 1`, uma sequência de valores com N números distintos (6 ou 15) consome exatamente esses valores mais os repetidos que aparecerem no caminho; nenhum valor adicional é consumido.

## Interface de console

Não muda. Ver `specs/003-modernizar-interface-console/contracts/console.md`.
