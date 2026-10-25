# Modelo de Dados: Gerar jogos da Mega-Sena

Todos os dados são transitórios e existem somente durante a execução do processo.

## Entidades

### Solicitação de jogos

Representa a quantidade que a pessoa deseja gerar.

| Campo | Tipo lógico | Regra |
|---|---|---|
| quantidade | Inteiro | Obrigatório; mínimo 1 e máximo 1.000, inclusive. Entradas vazias, não inteiras ou fora do intervalo são inválidas e devem permitir nova tentativa. |

### Jogo

Representa uma combinação candidata ou apresentada.

| Campo | Tipo lógico | Regra |
|---|---|---|
| números | Lista de inteiros | Exatamente seis números distintos entre 1 e 60, inclusive; apresentados em ordem crescente. |
| soma | Inteiro derivado | Soma dos seis números; a regra de soma é atendida entre 150 e 220, inclusive. |
| quantidade de pares | Inteiro derivado | A regra de paridade é atendida com 2, 3 ou 4 pares (e, consequentemente, 4, 3 ou 2 ímpares). |
| grupos representados | Conjunto derivado | A regra dos grupos é atendida quando aparecem pelo menos três grupos distintos. |
| finais repetidos | Contagem derivada | Inválido se qualquer algarismo final ocorrer em mais de dois números. |
| maior sequência consecutiva | Comprimento derivado | Inválido se houver uma sequência de pelo menos três inteiros consecutivos. |

Um jogo é aceito quando atende a pelo menos duas das três regras de composição (paridade, grupos e soma) e não viola nenhuma regra de descarte. Números consecutivos isolados em pares são permitidos.

### Grupo numérico

Classificação derivada de cada número, conforme os intervalos não contíguos definidos na especificação:

| Grupo | Intervalos |
|---|---|
| 1 | 1–5, 11–15, 21–25 |
| 2 | 6–10, 16–20, 26–30 |
| 3 | 31–35, 41–45, 51–55 |
| 4 | 36–40, 46–50, 56–60 |

Cada número de 1 a 60 pertence a exatamente um grupo. O jogo associa-se a um grupo por número; para a regra de composição conta-se o número de grupos distintos presentes.

## Fluxo e ciclo de vida

1. A entrada da pessoa é validada como quantidade inteira de 1 a 1.000.
2. Para cada posição solicitada, uma combinação candidata de seis números distintos é gerada, avaliada e descartada caso viole algum critério.
3. Cada candidata descartada é substituída até que a quantidade solicitada de jogos válidos esteja pronta.
4. Os jogos aceitos são apresentados ordenados; não há estado persistido após o encerramento.

Combinações iguais em jogos diferentes são permitidas. A unicidade é exigida somente dentro de cada jogo.
