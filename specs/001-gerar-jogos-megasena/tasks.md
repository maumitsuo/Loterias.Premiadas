# Tarefas: Gerar jogos da Mega-Sena

**Entrada**: Documentos de design em `specs/001-gerar-jogos-megasena/`

**Pré-requisitos**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/console.md](./contracts/console.md), [quickstart.md](./quickstart.md)

**Testes**: A especificação não solicita TDD, mas a constituição exige testes automatizados para todo o código executável e relatório com 100% de cobertura de linhas e ramificações. Por isso, tarefas de teste são obrigatórias neste plano.

**Organização**: tarefas agrupadas por história para que cada entrega tenha critérios próprios de aceitação. Os caminhos são relativos à raiz do repositório.

## Fase 1: Preparação

**Objetivo**: criar os projetos .NET 10 e a configuração compartilhada mínima.

- [ ] T001 Criar a aplicação de console .NET 10 e seu ponto de entrada em `src/Loterias.Premiadas/Loterias.Premiadas.csproj` e `src/Loterias.Premiadas/Program.cs`.
- [ ] T002 [P] Criar o projeto de testes .NET 10 com xUnit e Coverlet como dependências somente de teste em `tests/Loterias.Premiadas.Tests/Loterias.Premiadas.Tests.csproj`.
- [ ] T003 Criar `Loterias.Premiadas.sln` na raiz do repositório e adicionar os projetos `src/Loterias.Premiadas/Loterias.Premiadas.csproj` e `tests/Loterias.Premiadas.Tests/Loterias.Premiadas.Tests.csproj`, configurando o projeto de testes para referenciar o projeto da aplicação.
- [ ] T004 [P] Atualizar `.gitignore` para excluir artefatos gerados `**/bin/`, `**/obj/` e `**/TestResults/`, preservando as regras existentes.

## Fase 2: Fundação

**Objetivo**: confirmar que a estrutura compartilhada permite compilar a aplicação e executar o projeto de testes antes de implementar as histórias.

- [ ] T005 Restaurar e compilar `Loterias.Premiadas.sln` com o SDK .NET 10 e corrigir problemas na configuração dos arquivos `src/Loterias.Premiadas/Loterias.Premiadas.csproj`, `tests/Loterias.Premiadas.Tests/Loterias.Premiadas.Tests.csproj` e `Loterias.Premiadas.sln`.

**Ponto de verificação**: os dois projetos compilam e a suíte de testes pode ser executada.

## Fase 3: História de Usuário 1 - Gerar a quantidade solicitada (Prioridade: P1) 🎯 MVP

**Objetivo**: gerar e apresentar exatamente a quantidade solicitada de jogos válidos, com seis números distintos de 1 a 60, ordenados e em conformidade com as regras de composição e descarte.

**Teste independente**: fornecer uma quantidade entre 1 e 1.000 e verificar que são apresentados exatamente tantos jogos; cada jogo tem seis números distintos no intervalo de 1 a 60, em ordem crescente, atende a pelo menos duas regras de composição e não viola nenhum critério de descarte.

### Testes da História de Usuário 1

- [ ] T006 [P] [US1] Criar testes automatizados em `tests/Loterias.Premiadas.Tests/GeradorDeJogosTests.cs` para o contrato `GeradorDeJogos.Gerar(int quantidade, Random aleatorio)`, que retorna `IReadOnlyList<int[]>`; verificar seis números únicos no intervalo de 1 a 60, ordenação crescente, regra de paridade, pertencimento a pelo menos três grupos, soma inclusiva de 150 a 220, aceitação por pelo menos duas regras, rejeição de três números com o mesmo algarismo final, rejeição de sequências de três consecutivos e substituição de candidatas inválidas até atingir a quantidade solicitada.

### Implementação da História de Usuário 1

- [ ] T007 [P] [US1] Implementar em `src/Loterias.Premiadas/GeradorDeJogos.cs` o contrato `GeradorDeJogos.Gerar(int quantidade, Random aleatorio)`, que retorna `IReadOnlyList<int[]>`; cada jogo deve conter exatamente seis inteiros distintos de 1 a 60, ser ordenado crescentemente e ser aceito somente se cumprir pelo menos duas das regras de paridade (3/3, 2/4 ou 4/2), grupos (pelo menos três dos quatro grupos especificados) e soma (150 a 220, inclusive), sem qualquer algarismo final aparecer mais de duas vezes nem conter três inteiros consecutivos; substituir candidatas rejeitadas até obter a quantidade pedida e permitir jogos iguais em solicitações diferentes.
- [ ] T008 [US1] Implementar o fluxo de console de caminho válido em `src/Loterias.Premiadas/Program.cs`: solicitar uma quantidade de jogos, gerar os jogos chamando `GeradorDeJogos.Gerar` e apresentar cada um em linha identificada, com seis números formatados em ordem crescente, seguindo `specs/001-gerar-jogos-megasena/contracts/console.md`.
- [ ] T009 [US1] Criar testes automatizados com entrada e saída de console redirecionadas em `tests/Loterias.Premiadas.Tests/ProgramaTests.cs` para confirmar que uma solicitação válida de 1 jogo apresenta exatamente 1 jogo no formato do contrato, com seis números distintos e em ordem crescente.

**Ponto de verificação**: a História 1 funciona para entradas válidas e pode ser testada sem implementar a repetição de entradas inválidas da História 2.

## Fase 4: História de Usuário 2 - Corrigir uma quantidade inválida (Prioridade: P2)

**Objetivo**: orientar a pessoa quando a entrada estiver vazia, não for inteira ou estiver fora do intervalo de 1 a 1.000, permitindo nova tentativa sem gerar resultados enganosos.

**Teste independente**: fornecer uma entrada inválida seguida de uma quantidade válida e confirmar que a aplicação informa o intervalo permitido, repete a solicitação e então apresenta exatamente a quantidade de jogos solicitada.

### Testes da História de Usuário 2

- [ ] T010 [US2] Ampliar `tests/Loterias.Premiadas.Tests/ProgramaTests.cs` com casos para entrada vazia, texto, valor não inteiro, zero, número negativo, valor acima de 1.000 e fim de entrada; verificar que entradas inválidas recebem orientação clara e nova solicitação, que uma entrada válida posterior prossegue sem contabilizar tentativas inválidas como jogos e que EOF termina sem gerar jogos.

### Implementação da História de Usuário 2

- [ ] T011 [US2] Atualizar a leitura de quantidade em `src/Loterias.Premiadas/Program.cs` para aceitar somente inteiros de 1 a 1.000, inclusive; em entrada vazia, não inteira, menor que 1 ou maior que 1.000, exibir orientação clara e solicitar novamente; em EOF, encerrar sem gerar jogos; manter o fluxo de geração e apresentação da História 1 para uma entrada válida.

**Ponto de verificação**: ambas as histórias funcionam em conjunto; entradas inválidas não geram jogos nem alteram a quantidade final solicitada.

## Fase 5: Refinamento e validação transversal

**Objetivo**: demonstrar a cobertura integral exigida pela constituição e confirmar os cenários de validação documentados.

- [ ] T012 Executar `dotnet test .\tests\Loterias.Premiadas.Tests\Loterias.Premiadas.Tests.csproj --collect:"XPlat Code Coverage"` conforme `specs/001-gerar-jogos-megasena/quickstart.md`, inspecionar o relatório em `TestResults` e confirmar 100% de cobertura de linhas e ramificações para todo o código executável da aplicação; corrigir lacunas em `src/Loterias.Premiadas/Program.cs`, `src/Loterias.Premiadas/GeradorDeJogos.cs` ou `tests/Loterias.Premiadas.Tests/ProgramaTests.cs` e `tests/Loterias.Premiadas.Tests/GeradorDeJogosTests.cs`.
- [ ] T013 Executar os cenários manuais de `specs/001-gerar-jogos-megasena/quickstart.md` e confirmar a mensagem de nova tentativa para entradas inválidas, exatamente 1 jogo para a quantidade `1`, exatamente 1.000 jogos para `1000` e encerramento sem jogos em EOF; corrigir divergências em `src/Loterias.Premiadas/Program.cs` e atualizar o guia `specs/001-gerar-jogos-megasena/quickstart.md` somente se o comportamento aprovado ou os comandos diferirem.

## Dependências e Ordem de Execução

### Dependências entre fases

- A Fase 2 depende da conclusão da Fase 1.
- A Fase 3 depende da Fase 2.
- A Fase 4 depende da Fase 3, pois a correção de entrada inválida complementa o fluxo de solicitação implementado na História 1.
- A Fase 5 depende das Fases 3 e 4.

### Dependências entre histórias de usuário

```text
Preparação (T001–T004)
  └── Fundação (T005)
      └── US1 - Gerar quantidade solicitada (T006–T009)
          └── US2 - Corrigir quantidade inválida (T010–T011)
              └── Refinamento e validação (T012–T013)
```

### Dependências dentro das histórias

- US1: T006 e T007 alteram arquivos diferentes e podem ser iniciadas em paralelo após a fundação; ambos compartilham o contrato explícito `GeradorDeJogos.Gerar(int quantidade, Random aleatorio)` → `IReadOnlyList<int[]>`. T008 depende de T007; T009 depende de T008 para testar a interação completa.
- US2: T010 define os casos de teste; T011 altera o mesmo fluxo de console após T010, evitando alterações simultâneas em `ProgramaTests.cs` e `Program.cs`.
- A validação final requer todas as tarefas das duas histórias concluídas.

### Oportunidades de paralelismo

- Preparação: T002 e T004 podem ser executadas em paralelo com T001; T003 requer T001 e T002; T005 requer T003.
- US1: T006 (`GeradorDeJogosTests.cs`) e T007 (`GeradorDeJogos.cs`) podem ser desenvolvidas em paralelo, pois usam arquivos diferentes. Executar T008 após T007 e T009 após T008.
- US2: concluir T010 e então T011; não há tarefas paralelas nessa história porque os testes e a implementação compartilham o fluxo de entrada e saída da aplicação.

## Exemplo de Execução Paralela

### História de Usuário 1

```text
# Após a Fase 2, trabalhar em arquivos separados:
T006  tests/Loterias.Premiadas.Tests/GeradorDeJogosTests.cs
T007  src/Loterias.Premiadas/GeradorDeJogos.cs

# Depois:
T008  src/Loterias.Premiadas/Program.cs
T009  tests/Loterias.Premiadas.Tests/ProgramaTests.cs
```

### História de Usuário 2

```text
# Executar em ordem, pois os testes e a implementação alteram o mesmo fluxo:
T010  tests/Loterias.Premiadas.Tests/ProgramaTests.cs
T011  src/Loterias.Premiadas/Program.cs
```

## Estratégia de Implementação

### MVP Primeiro (Somente História de Usuário 1)

Concluir as Fases 1 e 2 e, em seguida, a História 1. O MVP solicita uma quantidade válida de 1 a 1.000, gera e apresenta exatamente essa quantidade de jogos válidos e ordenados. A História 2 adiciona a correção interativa de entradas inválidas.

### Entrega Incremental

1. Preparar e validar a solução e os projetos de teste.
2. Entregar a geração e apresentação para entradas válidas; verificar os critérios do gerador e a quantidade exata.
3. Entregar o tratamento de entradas inválidas e EOF sem regressões no fluxo válido.
4. Executar os testes e validar 100% da cobertura de linhas e ramificações, além dos cenários manuais do quickstart.

### Estratégia para Equipe em Paralelo

Delegar T006 e T007 em paralelo após a fundação, mantendo cada tarefa restrita a seu próprio arquivo. Serializar a interação de console e o fluxo da História 2 para evitar conflitos em `Program.cs` e `ProgramaTests.cs`.
