# Modelo de Dados: Gerar jogos da Lotofácil

Todos os dados são transitórios e existem somente durante a execução do processo.

## Entidades

### Solicitação de jogos

Representa uma solicitação de geração.

| Campo | Tipo lógico | Regra |
|---|---|---|
| modalidade | Modalidade | Obrigatória; Mega-Sena ou Lotofácil. |
| quantidade | Inteiro | Obrigatória; mínimo 1 e máximo 1.000, inclusive. Entradas vazias, não inteiras ou fora do intervalo são inválidas e permitem nova tentativa. |

### Modalidade

Determina o intervalo e os critérios de validade aplicáveis a cada jogo.

| Modalidade | Números por jogo | Intervalo | Regras |
|---|---:|---|---|
| Mega-Sena | 6 | 1–60 | Conforme [especificação Mega-Sena](../../001-gerar-jogos-megasena/spec.md): ao menos duas regras de composição (paridade, grupos e soma), sem violar as regras de descarte. |
| Lotofácil | 15 | 1–25 | Paridade aceita, 9–11 números de moldura e presença em todos os cinco grupos sequenciais. |

### Jogo

Representa uma combinação candidata ou apresentada, associada à modalidade da solicitação.

| Campo | Tipo lógico | Regra |
|---|---|---|
| modalidade | Modalidade | Determina os critérios usados para validar o jogo. |
| números | Lista de inteiros | Quantidade e intervalo conforme a modalidade; números distintos e apresentados em ordem crescente. |
| pares/ímpares | Contagens derivadas | Lotofácil: 8 ímpares/7 pares, 7 ímpares/8 pares ou 9 ímpares/6 pares. Mega-Sena: regras definidas na especificação específica. |
| moldura/miolo | Contagens derivadas | Lotofácil: moldura de 9 a 11 números e miolo de 4 a 6. |
| grupos sequenciais | Conjunto derivado | Lotofácil: deve conter ao menos um número de cada grupo de 01–05, 06–10, 11–15, 16–20 e 21–25. |

Um jogo da Lotofácil é aceito somente quando satisfaz simultaneamente as regras de paridade, moldura/miolo e grupos sequenciais. A validade de um jogo da Mega-Sena segue as regras de sua especificação.

### Grupos numéricos da Lotofácil

#### Moldura e miolo

| Grupo | Números |
|---|---|
| Moldura | 01–06, 10–11, 15–16, 20–25 |
| Miolo | 07–09, 12–14, 17–19 |

Os grupos são disjuntos e, juntos, abrangem os números de 1 a 25.

#### Grupos sequenciais

| Grupo | Números |
|---|---|
| 1 | 01–05 |
| 2 | 06–10 |
| 3 | 11–15 |
| 4 | 16–20 |
| 5 | 21–25 |

Cada número de 1 a 25 pertence a exatamente um grupo sequencial.

## Fluxo e ciclo de vida

1. A pessoa escolhe uma modalidade; entrada vazia ou não reconhecida permite nova tentativa.
2. A pessoa informa uma quantidade inteira entre 1 e 1.000; entrada inválida permite nova tentativa.
3. Para cada jogo solicitado, uma candidata é sorteada, ordenada e validada segundo a modalidade escolhida.
4. Candidatas inválidas são descartadas e substituídas até completar a quantidade solicitada.
5. Os jogos são apresentados; não há persistência após o encerramento.

Números repetidos dentro de um jogo são proibidos. A especificação não proíbe a repetição de uma combinação completa entre jogos diferentes da mesma solicitação.
