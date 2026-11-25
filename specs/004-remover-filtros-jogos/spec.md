# Especificação da Funcionalidade: Remover filtros de validação dos jogos

**Branch da funcionalidade**: `004-remover-filtros-jogos`

**Criada em**: 2026-11-24

**Status**: Rascunho

**Entrada**: Descrição do usuário:
> Os jogos fazem algumas verificações para validar os jogos, gostaria de tirar todas as verificações somente não trazer numeros repetidos. Tanto para mega sena como para lotofacil

## Clarifications

### Session 2026-11-24

- Q: Quando a pessoa pede vários jogos de uma vez, dois desses jogos podem sair exatamente iguais? → A: Sim. Apenas os números dentro de cada jogo não se repetem; jogos iguais na mesma solicitação são permitidos.

## Cenários de Uso e Testes *(obrigatório)*

### História de Usuário 1 - Gerar jogos da Mega-Sena sem filtros estatísticos (Prioridade: P1)

Como pessoa que deseja montar jogos da Mega-Sena, quero receber combinações sorteadas livremente entre todas as possíveis, exigindo apenas que os números de cada jogo não se repitam, para não ter minhas opções limitadas por critérios de paridade, grupos, soma, finais ou sequências.

**Por que esta prioridade**: A Mega-Sena é a modalidade original da aplicação; retirar seus filtros entrega imediatamente o comportamento pedido.

**Teste independente**: Escolher Mega-Sena e solicitar uma quantidade válida; verificar que cada jogo tem seis números distintos entre 1 e 60, em ordem crescente, e que combinações antes rejeitadas (por exemplo, todos os números pares ou três números consecutivos) podem aparecer.

**Cenários de Aceitação**:

1. **Dado** que a pessoa escolhe Mega-Sena e informa uma quantidade válida, **Quando** solicita a geração, **Então** recebe exatamente essa quantidade de jogos, cada um com seis números distintos entre 1 e 60, em ordem crescente.
2. **Dado** uma combinação sorteada com seis números distintos que não atenderia às antigas regras de paridade, grupos, soma, algarismo final ou números consecutivos, **Quando** a geração ocorre, **Então** essa combinação é aceita e apresentada sem ser descartada.

---

### História de Usuário 2 - Gerar jogos da Lotofácil sem filtros estatísticos (Prioridade: P1)

Como pessoa que deseja montar jogos da Lotofácil, quero receber combinações sorteadas livremente entre todas as possíveis, exigindo apenas que os números de cada jogo não se repitam, para não ter minhas opções limitadas por critérios de paridade, moldura/miolo ou grupos sequenciais.

**Por que esta prioridade**: A mudança foi pedida para as duas modalidades com a mesma importância.

**Teste independente**: Escolher Lotofácil e solicitar uma quantidade válida; verificar que cada jogo tem 15 números distintos entre 1 e 25, em ordem crescente, e que combinações antes rejeitadas (por exemplo, 10 ímpares e 5 pares ou um grupo sequencial ausente) podem aparecer.

**Cenários de Aceitação**:

1. **Dado** que a pessoa escolhe Lotofácil e informa uma quantidade válida, **Quando** solicita a geração, **Então** recebe exatamente essa quantidade de jogos, cada um com 15 números distintos entre 1 e 25, em ordem crescente.
2. **Dado** uma combinação sorteada com 15 números distintos que não atenderia às antigas regras de paridade, moldura/miolo ou grupos sequenciais, **Quando** a geração ocorre, **Então** essa combinação é aceita e apresentada sem ser descartada.

### Casos Limítrofes

- Um número sorteado que já pertence ao jogo em formação é ignorado e substituído por outro, de modo que nenhum jogo contenha números repetidos.
- Combinações extremas e válidas pelo regulamento, como 1-2-3-4-5-6 na Mega-Sena ou 1 a 15 na Lotofácil, são aceitas.
- A validação da entrada da pessoa (modalidade reconhecida e quantidade inteira entre 1 e 1.000) permanece inalterada; ela não faz parte das verificações removidas.
- Combinações completas iguais entre jogos diferentes de uma mesma solicitação continuam permitidas, como já era antes.

## Requisitos *(obrigatório)*

### Requisitos Funcionais

- **FR-001**: Para Mega-Sena, o sistema DEVE gerar jogos com seis números distintos entre 1 e 60, inclusive.
- **FR-002**: Para Lotofácil, o sistema DEVE gerar jogos com 15 números distintos entre 1 e 25, inclusive.
- **FR-003**: O sistema NÃO DEVE descartar jogos da Mega-Sena com base em paridade, grupos numéricos, soma, repetição de algarismo final ou sequência de números consecutivos.
- **FR-004**: O sistema NÃO DEVE descartar jogos da Lotofácil com base em paridade, quantidade de números de moldura/miolo ou cobertura dos grupos sequenciais.
- **FR-005**: A única regra de aceitação de um jogo, em ambas as modalidades, DEVE ser a ausência de números repetidos dentro do próprio jogo, respeitando o intervalo e a quantidade de números da modalidade.
- **FR-006**: O sistema DEVE continuar apresentando os números de cada jogo em ordem crescente.
- **FR-007**: O sistema DEVE continuar aceitando a escolha entre Mega-Sena e Lotofácil e uma quantidade inteira entre 1 e 1.000 jogos, orientando a correção de entradas inválidas como antes.
- **FR-008**: O sistema DEVE entregar exatamente a quantidade de jogos solicitada.
- **FR-009**: O sistema NÃO DEVE descartar um jogo por ser igual a outro jogo da mesma solicitação.

### Entidades Principais *(inclua se a funcionalidade envolver dados)*

- **Jogo**: combinação ordenada de números distintos dentro do intervalo da modalidade (6 números de 1 a 60 na Mega-Sena; 15 números de 1 a 25 na Lotofácil), sem outros critérios de validade.
- **Modalidade**: Mega-Sena ou Lotofácil, definida apenas pelo intervalo de números e pela quantidade de números por jogo.

## Critérios de Sucesso *(obrigatório)*

### Resultados Mensuráveis

- **SC-001**: Em 100% das solicitações válidas, a quantidade de jogos apresentados é igual à quantidade solicitada.
- **SC-002**: Em 100% dos jogos apresentados, não há número repetido, todos os números estão no intervalo da modalidade e aparecem em ordem crescente.
- **SC-003**: Toda combinação com números distintos dentro do intervalo da modalidade pode ser apresentada como resultado; nenhuma é descartada por critério estatístico.
- **SC-004**: A geração de 1.000 jogos em qualquer modalidade termina sem espera perceptível para a pessoa, já que nenhuma combinação é rejeitada.

## Premissas

- "Não trazer números repetidos" refere-se aos números dentro de um mesmo jogo; a repetição de combinações completas entre jogos distintos de uma solicitação continua permitida, como nas especificações anteriores.
- Esta especificação substitui as regras de aceitação de jogos de `specs/001-gerar-jogos-megasena/spec.md` e `specs/002-gerar-jogos-lotofacil/spec.md`; os demais comportamentos dessas especificações e a interface de `specs/003-modernizar-interface-console/spec.md` permanecem.
- A quantidade de números por jogo e os intervalos de cada modalidade seguem o que já é adotado (Mega-Sena: 6 de 60; Lotofácil: 15 de 25); jogos com mais números ficam fora do escopo.
- Os testes automatizados que comprovavam as regras removidas serão substituídos por testes da nova regra, mantendo a cobertura integral exigida pela constituição.
