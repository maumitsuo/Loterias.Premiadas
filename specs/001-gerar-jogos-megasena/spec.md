# Especificação da Funcionalidade: Gerar jogos da Mega-Sena

**Branch da funcionalidade**: `001-gerar-jogos-megasena`

**Criada em**: 2026-10-25

**Status**: Rascunho

**Entrada**: Descrição do usuário:
> Gerador aleatório para jogos da Mega Sena.
>
> O usuario informa a quantidade de jogos que deseja gerar.
>
> Cada jogo é composto por 6 números aleatórios entre 1 e 60.
>
> Em cada jogo, os números não podem se repetir.
>
> Depois de gerado serão descartados os jogos que não atenderem a pelo menos 2 das seguintes regras.
>
> 1 - Relação entre números pares e impares. Devem respeitar a proporção de:
>     3 pares e 3 impares ou
>     2 pares e 4 impares ou
>     4 pares e 2 impares.
>
> 2 - Dividindo os números entre 1 e 60 nos grupos:
>     - 1 a 5, 11 a 15 e 21 a 25;
>     - 6 a 10, 16 a 20 e 26 a 30;
>     - 31 a 35, 41 a 45 e 51 a 55;
>     - 36 a 40, 46 a 50 e 56 a 60;
>         Os números gerados devem pertencer a pelo menos 3 grupos.
>
> 3 - A soma de todos os números deve estar entre 150 e 220
>
> Serão descartados todos os jogos que atenderem a qualquer uma das regras seguintes:
>
> 1 - Caso exita mais do que 2 numeros que o final se repita. Por exemplo 11, 21 e 31.
> 2 - Caso exista mais do que 2 números sequenciais.
>
> Os jogos descartados devem ser gerados novamente, para respeitar a quantidade de jogos solicitada.
>
> Cada jogo deve ser ordenado do menor para o maior para facilitar a leitura.

## Clarifications

### Session 2026-10-25

- Q: Qual deve ser o limite máximo de jogos em uma única solicitação? → A: No máximo 1.000 jogos.

## Cenários de Uso e Testes *(obrigatório)*

### História de Usuário 1 - Gerar a quantidade solicitada (Prioridade: P1)

Como pessoa que deseja montar jogos da Mega-Sena, quero informar quantos jogos desejo e receber essa quantidade de combinações que atendam aos critérios definidos, para poder consultá-las e utilizá-las.

**Por que esta prioridade**: Gerar jogos válidos na quantidade solicitada é o objetivo principal do recurso.

**Teste independente**: Informar uma quantidade entre 1 e 1.000 e verificar que o resultado contém exatamente essa quantidade de jogos, todos válidos e apresentados em ordem crescente.

**Cenários de Aceitação**:

1. **Dado** que a pessoa informa uma quantidade entre 1 e 1.000, inclusive, **Quando** solicita a geração, **Então** recebe exatamente essa quantidade de jogos.
2. **Dado** um jogo apresentado, **Quando** seus números são verificados, **Então** há seis números distintos entre 1 e 60, em ordem crescente, que atendem a pelo menos duas das três regras de composição.
3. **Dado** um jogo apresentado, **Quando** seus finais e números consecutivos são verificados, **Então** nenhum final aparece mais de duas vezes e não há sequência de três ou mais números consecutivos.
4. **Dado** que uma combinação candidata é descartada por qualquer critério, **Quando** a geração termina, **Então** ela foi substituída e não reduziu a quantidade final solicitada.

---

### História de Usuário 2 - Corrigir uma quantidade inválida (Prioridade: P2)

Como pessoa que solicita jogos, quero receber orientação quando informar uma quantidade inválida, para poder corrigir a entrada sem obter um resultado incompleto ou enganoso.

**Por que esta prioridade**: Uma entrada inválida não deve gerar resultados que contrariem a quantidade ou as regras do recurso.

**Teste independente**: Informar zero, um número negativo, um valor que não seja inteiro ou um valor acima de 1.000 e confirmar que a pessoa é orientada a informar uma quantidade válida.

**Cenários de Aceitação**:

1. **Dado** que a quantidade informada não é um inteiro entre 1 e 1.000, inclusive, **Quando** a entrada é validada, **Então** a pessoa recebe uma indicação clara do problema e pode informar novamente uma quantidade válida.

### Casos Limítrofes

- Quantidade vazia, zero, negativa, não inteira ou superior a 1.000: informar que é necessário fornecer um inteiro entre 1 e 1.000 e permitir nova entrada.
- Uma combinação com exatamente dois números terminados no mesmo algarismo é permitida; três ou mais ocorrências desse algarismo tornam a combinação inválida.
- Um par de números consecutivos é permitido; uma sequência de três ou mais inteiros consecutivos torna a combinação inválida.
- A soma igual a 150 ou 220 atende à regra de soma.
- A divisão 3/3, 2/4 ou 4/2 entre pares e ímpares atende à regra de paridade; outras divisões não atendem a essa regra, mas ainda podem ser aceitas se outras duas regras de composição forem atendidas.
- Cada número pertence a exatamente um dos quatro grupos especificados; para atender à regra dos grupos, a combinação deve incluir números de pelo menos três grupos distintos.

## Requisitos *(obrigatório)*

### Requisitos Funcionais

- **FR-001**: O sistema DEVE permitir que a pessoa informe a quantidade de jogos desejada como um inteiro entre 1 e 1.000, inclusive, e orientar a correção de uma entrada inválida.
- **FR-002**: O sistema DEVE sortear seis números distintos entre 1 e 60, inclusive, para compor cada jogo.
- **FR-003**: O sistema DEVE considerar atendida a regra de paridade somente para jogos com 3 pares e 3 ímpares, 2 pares e 4 ímpares ou 4 pares e 2 ímpares.
- **FR-004**: O sistema DEVE considerar atendida a regra de grupos quando os seis números pertencerem a pelo menos três dos quatro grupos definidos: (1–5, 11–15, 21–25), (6–10, 16–20, 26–30), (31–35, 41–45, 51–55) e (36–40, 46–50, 56–60).
- **FR-005**: O sistema DEVE considerar atendida a regra de soma quando a soma dos seis números estiver entre 150 e 220, inclusive.
- **FR-006**: O sistema DEVE aceitar um jogo somente quando ele atender a pelo menos duas das regras de paridade, grupos e soma.
- **FR-007**: O sistema DEVE descartar qualquer jogo em que um mesmo algarismo final ocorra em mais de dois números.
- **FR-008**: O sistema DEVE descartar qualquer jogo que contenha três ou mais inteiros consecutivos em sequência.
- **FR-009**: O sistema DEVE substituir cada jogo descartado até apresentar a quantidade solicitada de jogos válidos.
- **FR-010**: O sistema DEVE apresentar os números de cada jogo em ordem crescente.

### Entidades Principais *(inclua se a funcionalidade envolver dados)*

- **Solicitação de jogos**: quantidade de jogos desejada pela pessoa, como um inteiro entre 1 e 1.000, inclusive.
- **Jogo**: combinação apresentada de seis números distintos no intervalo de 1 a 60, ordenados em ordem crescente e submetidos aos critérios de aceitação e descarte.
- **Grupo numérico**: uma das quatro categorias de intervalos especificadas para avaliar a distribuição dos números de um jogo.

## Critérios de Sucesso *(obrigatório)*

### Resultados Mensuráveis

- **SC-001**: Em 100% das solicitações válidas concluídas, o resultado contém exatamente a quantidade de jogos solicitada.
- **SC-002**: Em 100% dos jogos apresentados, os seis números são distintos e estão no intervalo de 1 a 60, em ordem crescente.
- **SC-003**: Em 100% dos jogos apresentados, pelo menos duas regras de composição são atendidas e nenhuma regra de descarte é violada.

## Premissas

- “Mais do que 2 números sequenciais” significa uma sequência de pelo menos três inteiros consecutivos; dois consecutivos isolados são permitidos.
- Os jogos resultantes não precisam ser distintos entre si, pois a descrição exige números sem repetição dentro de cada jogo, mas não proíbe combinações iguais em jogos diferentes.
- Uma quantidade válida é um inteiro entre 1 e 1.000, inclusive.
- Os resultados existem somente durante a execução do aplicativo e não são armazenados para uso posterior.
- A geração atende aos critérios descritos, mas não aumenta nem garante a probabilidade de premiação.
- A taxa de pessoas que conseguem informar uma quantidade válida e identificar os jogos na primeira tentativa (meta indicativa de 90%) é um indicador opcional de avaliação pós-lançamento, não um critério de implementação, aceitação ou bloqueio de lançamento.
