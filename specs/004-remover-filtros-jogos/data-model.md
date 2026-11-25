# Modelo de Dados: Remover filtros de validação dos jogos

Todos os dados existem apenas em memória durante a execução (princípio II).

## Modalidade

| Modalidade | Números por jogo | Intervalo |
|------------|------------------|-----------|
| Mega-Sena | 6 | 1 a 60 |
| Lotofácil | 15 | 1 a 25 |

A modalidade é definida **apenas** pelo intervalo e pela quantidade de números por jogo. Os critérios anteriores (paridade, grupos, soma, algarismo final, consecutivos, moldura/miolo, grupos sequenciais) deixam de existir.

## Jogo

- **Representação**: vetor de inteiros (`int[]`).
- **Regras de validade** (únicas):
  1. Tem exatamente a quantidade de números da modalidade.
  2. Todos os números estão no intervalo da modalidade.
  3. Não há números repetidos.
  4. Os números estão em ordem crescente.
- **Sem estados ou transições**: o jogo é criado já válido e não muda depois.

## Solicitação de jogos (inalterada)

- Modalidade escolhida e quantidade inteira entre 1 e 1.000.
- O resultado é uma lista com exatamente a quantidade pedida; jogos iguais na mesma lista são permitidos.
