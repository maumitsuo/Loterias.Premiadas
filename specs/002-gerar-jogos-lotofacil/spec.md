# Especificação da Funcionalidade: Gerar jogos da Lotofácil

**Branch da funcionalidade**: `002-gerar-jogos-lotofacil`

**Criada em**: 2026-10-25

**Status**: Rascunho

**Entrada**: Descrição do usuário:
> Gerador aleatório para jogos da Lotofácil. A pessoa escolhe Mega-Sena ou Lotofácil e informa a quantidade de jogos. A Lotofácil usa 15 números distintos de 1 a 25, respeitando as regras de paridade, moldura/miolo e cobertura dos cinco grupos sequenciais. A Mega-Sena segue as regras já definidas em `specs/001-gerar-jogos-megasena/spec.md`.
>
> Esclarecimento: incluir a seleção entre os dois jogos; aplicar à Mega-Sena as regras da especificação existente.

## Cenários de Uso e Testes *(obrigatório)*

### História de Usuário 1 - Gerar jogos do tipo escolhido (Prioridade: P1)

Como pessoa que deseja montar jogos de loteria, quero escolher Mega-Sena ou Lotofácil, informar quantos jogos desejo e receber essa quantidade de combinações válidas, para consultá-las e utilizá-las.

**Por que esta prioridade**: A geração na modalidade escolhida e na quantidade solicitada é o valor central da funcionalidade.

**Teste independente**: Escolher cada modalidade e solicitar uma quantidade válida; verificar que a resposta contém exatamente essa quantidade de jogos e que cada jogo atende às regras da modalidade.

**Cenários de Aceitação**:

1. **Dado** que a pessoa escolhe Lotofácil e informa uma quantidade válida, **Quando** solicita a geração, **Então** recebe exatamente a quantidade solicitada de jogos válidos da Lotofácil.
2. **Dado** que a pessoa escolhe Mega-Sena e informa uma quantidade válida, **Quando** solicita a geração, **Então** recebe exatamente a quantidade solicitada de jogos válidos da Mega-Sena, conforme os critérios já definidos para essa modalidade.
3. **Dado** um jogo exibido, **Quando** seus números são verificados, **Então** eles são apresentados em ordem crescente e atendem às regras de sua modalidade.
4. **Dado** uma combinação candidata que não atende a todos os critérios da modalidade escolhida, **Quando** a geração termina, **Então** essa combinação não aparece no resultado e é substituída por outra válida.

---

### História de Usuário 2 - Corrigir uma solicitação inválida (Prioridade: P2)

Como pessoa que solicita jogos, quero receber uma orientação clara quando escolher uma modalidade ou informar uma quantidade inválida, para corrigir a solicitação antes da geração.

**Por que esta prioridade**: Solicitações inválidas não devem produzir resultados incompletos ou que aparentem atender aos critérios.

**Teste independente**: Informar uma quantidade vazia, não inteira, inferior a 1 ou superior a 1.000, ou uma modalidade não reconhecida, e verificar a orientação para corrigir a entrada.

**Cenários de Aceitação**:

1. **Dado** que a quantidade não é um inteiro entre 1 e 1.000, inclusive, **Quando** a entrada é validada, **Então** a pessoa recebe uma indicação clara e pode informar uma quantidade válida.
2. **Dado** que a modalidade não corresponde a Mega-Sena ou Lotofácil, **Quando** a entrada é validada, **Então** a pessoa recebe uma indicação clara e pode escolher uma modalidade válida.

### Casos Limítrofes

- Quantidade vazia, zero, negativa, não inteira ou superior a 1.000: solicitar uma quantidade inteira entre 1 e 1.000.
- Modalidade vazia ou não reconhecida: solicitar a escolha entre Mega-Sena e Lotofácil.
- Na Lotofácil, são aceitas somente as distribuições de paridade 8 ímpares/7 pares, 7 ímpares/8 pares ou 9 ímpares/6 pares.
- Na Lotofácil, jogos com 8 ou menos, ou 12 ou mais, números de moldura são inválidos; as quantidades restantes devem pertencer ao miolo.
- Na Lotofácil, cada um dos cinco grupos sequenciais deve estar representado por pelo menos um número.
- Na Mega-Sena, aplicam-se integralmente os limites e critérios de aceitação e descarte registrados na especificação existente dessa modalidade.
- A ordem crescente é exigida dentro de cada jogo. Não foi solicitada a eliminação de combinações repetidas entre jogos diferentes da mesma solicitação.

## Requisitos *(obrigatório)*

### Requisitos Funcionais

- **FR-001**: O sistema DEVE permitir escolher entre Mega-Sena e Lotofácil antes de solicitar a geração.
- **FR-002**: O sistema DEVE aceitar uma quantidade inteira entre 1 e 1.000 jogos, inclusive, para qualquer modalidade, e orientar a correção de uma quantidade inválida.
- **FR-003**: Para Mega-Sena, o sistema DEVE gerar jogos com seis números distintos entre 1 e 60, aceitar somente jogos que atendam a pelo menos duas das regras de paridade, grupos numéricos e soma, e descartar jogos com mais de duas ocorrências de um mesmo algarismo final ou com uma sequência de três ou mais números consecutivos, conforme `specs/001-gerar-jogos-megasena/spec.md`.
- **FR-004**: Para Lotofácil, o sistema DEVE gerar jogos com 15 números distintos entre 1 e 25, inclusive.
- **FR-005**: Para Lotofácil, o sistema DEVE aceitar somente jogos com uma das seguintes relações de paridade: 8 ímpares e 7 pares; 7 ímpares e 8 pares; ou 9 ímpares e 6 pares.
- **FR-006**: Para Lotofácil, o sistema DEVE considerar moldura os números 01–06, 10–11, 15–16, 20–25 e miolo os números 07–09, 12–14 e 17–19; deve aceitar somente jogos com 9 a 11 números de moldura e, consequentemente, 4 a 6 números de miolo.
- **FR-007**: Para Lotofácil, o sistema DEVE considerar os cinco grupos sequenciais 01–05, 06–10, 11–15, 16–20 e 21–25 e aceitar somente jogos que contenham pelo menos um número de cada grupo.
- **FR-008**: O sistema DEVE descartar combinações que não atendam a todos os critérios obrigatórios da modalidade selecionada e gerar substituições até alcançar a quantidade válida solicitada.
- **FR-009**: O sistema DEVE apresentar os números de cada jogo em ordem crescente.
- **FR-010**: O sistema DEVE informar quando a modalidade ou quantidade fornecida for inválida, sem apresentar um resultado como se a solicitação tivesse sido concluída.

### Entidades Principais *(inclua se a funcionalidade envolver dados)*

- **Solicitação de jogos**: modalidade selecionada e quantidade inteira solicitada, entre 1 e 1.000.
- **Jogo**: combinação ordenada de números distintos que atende aos critérios da modalidade selecionada.
- **Modalidade**: Mega-Sena ou Lotofácil, cada qual com seu intervalo de números e critérios de validade.
- **Grupo numérico**: categoria usada para verificar a distribuição dos números; na Lotofácil, inclui os grupos de moldura/miolo e os cinco grupos sequenciais.

## Critérios de Sucesso *(obrigatório)*

### Resultados Mensuráveis

- **SC-001**: Em 100% das solicitações válidas concluídas, a quantidade de jogos apresentados é igual à quantidade solicitada.
- **SC-002**: Em 100% dos jogos Lotofácil apresentados, há 15 números distintos entre 1 e 25, a paridade está entre as três relações aceitas, a contagem de moldura está entre 9 e 11 e todos os cinco grupos sequenciais estão representados.
- **SC-003**: Em 100% dos jogos Mega-Sena apresentados, os números e critérios atendem à especificação existente da Mega-Sena.
## Premissas

- O limite de 1 a 1.000 jogos por solicitação segue o limite estabelecido para a funcionalidade existente da Mega-Sena e se aplica às duas modalidades.
- A opção Mega-Sena reutiliza os critérios funcionais já definidos em `specs/001-gerar-jogos-megasena/spec.md`; esta especificação não altera esses critérios.
- Repetições de uma combinação completa entre jogos distintos da mesma solicitação não são proibidas, pois a regra fornecida proíbe apenas números repetidos dentro de cada jogo.
- A geração deve substituir jogos rejeitados até completar a quantidade válida solicitada; a pessoa não precisa filtrar resultados manualmente.
- Os jogos são apresentados durante a interação e não há solicitação de persistência dos resultados.
