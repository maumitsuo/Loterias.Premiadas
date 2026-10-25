---
description: "Tarefas de implementação para gerar jogos da Lotofácil"
---

# Tarefas: Gerar jogos da Lotofácil

**Entrada**: Documentos de design em `specs/002-gerar-jogos-lotofacil/`

**Pré-requisitos**: `plan.md`, `spec.md`, `research.md`, `data-model.md` e `contracts/console.md`

**Testes**: tarefas de teste incluídas porque o plano e a constituição exigem testes automatizados com cobertura de 100% de linhas e ramificações do código executável da aplicação.

**Organização**: tarefas agrupadas por história de usuário para permitir implementação e validação incremental.

## Fase 1: Preparação

**Objetivo**: reutilizar a aplicação de console .NET 10, o projeto xUnit e o Coverlet já existentes, sem criar projetos ou dependências adicionais.

**Tarefas**: nenhuma alteração de preparação é necessária; os projetos e dependências existentes estão de acordo com `plan.md`.

---

## Fase 2: Fundação

**Objetivo**: identificar pré-requisitos compartilhados que bloqueiem as histórias de usuário.

**Tarefas**: não há novos pré-requisitos bloqueadores; a aplicação, o gerador Mega-Sena, o projeto de testes e o Coverlet já estão configurados.

**Ponto de verificação**: a implementação pode começar usando `src/Loterias.Premiadas/` e `tests/Loterias.Premiadas.Tests/`.

---

## Fase 3: História de Usuário 1 - Gerar jogos do tipo escolhido (Prioridade: P1) — MVP

**Objetivo**: permitir escolher Mega-Sena ou Lotofácil e apresentar exatamente a quantidade solicitada de jogos válidos e ordenados.

**Teste independente**: selecionar cada modalidade e solicitar uma quantidade válida; confirmar a quantidade exata, a formatação crescente e as regras completas de cada modalidade, inclusive a substituição de candidatas inválidas.

### Testes da História de Usuário 1

- [X] T001 [P] [US1] Criar testes determinísticos para 15 números distintos entre 1 e 25, as paridades 8/7, 7/8 e 9/6, 9 a 11 números de moldura, presença nos cinco grupos, ordenação, quantidade solicitada e substituição de candidatas inválidas em `tests/Loterias.Premiadas.Tests/GeradorDeJogosLotofacilTests.cs`.
- [X] T002 [US1] Estender os testes de console para selecionar Mega-Sena e Lotofácil, verificar a quantidade exata e a apresentação crescente com dois dígitos, e confirmar que a Mega-Sena continua respeitando suas regras em `tests/Loterias.Premiadas.Tests/ProgramaTests.cs`.

### Implementação da História de Usuário 1

- [X] T003 [US1] Implementar o gerador da Lotofácil em `src/Loterias.Premiadas/GeradorDeJogosLotofacil.cs`: sortear 15 valores únicos de 1 a 25 com `Random` injetável; aceitar somente paridades 8 ímpares/7 pares, 7 ímpares/8 pares ou 9 ímpares/6 pares; exigir 9 a 11 números de moldura (01–06, 10–11, 15–16, 20–25), 4 a 6 do miolo (07–09, 12–14, 17–19) e ao menos um número em cada grupo 01–05, 06–10, 11–15, 16–20 e 21–25; ordenar cada jogo e substituir candidatas inválidas até completar de 1 a 1.000 jogos.
- [X] T004 [US1] Adaptar `src/Loterias.Premiadas/Program.cs` para solicitar primeiro a modalidade, depois a quantidade, encaminhar Mega-Sena ao gerador existente e Lotofácil a `GeradorDeJogosLotofacil`, e apresentar todos os jogos no formato do contrato de console sem alterar os critérios da Mega-Sena.

**Ponto de verificação**: ambas as modalidades geram e apresentam a quantidade solicitada de jogos válidos, independentemente verificável pelos testes da história.

---

## Fase 4: História de Usuário 2 - Corrigir uma solicitação inválida (Prioridade: P2)

**Objetivo**: informar como corrigir uma modalidade ou quantidade inválida e permitir nova tentativa sem apresentar resultado incompleto.

**Teste independente**: informar modalidade vazia ou não reconhecida e quantidades vazias, não inteiras, menores que 1 ou maiores que 1.000; confirmar orientação e repetição da solicitação. Verificar também EOF antes de completar a solicitação.

### Testes da História de Usuário 2

- [X] T005 [P] [US2] Estender `tests/Loterias.Premiadas.Tests/ProgramaTests.cs` para verificar modalidade inválida seguida de válida, todas as classes de quantidade inválida seguidas de uma quantidade válida e EOF durante a seleção ou entrada da quantidade; confirmar que nenhum jogo é apresentado antes de uma solicitação válida.

### Implementação da História de Usuário 2

- [X] T006 [P] [US2] Atualizar `src/Loterias.Premiadas/Program.cs` para explicar a escolha válida entre Mega-Sena e Lotofácil e o intervalo inteiro de 1 a 1.000, repetir cada solicitação inválida e encerrar sem gerar jogos quando ocorrer EOF antes da solicitação estar completa.

**Ponto de verificação**: entradas inválidas são corrigíveis, EOF encerra sem resultado enganoso e solicitações válidas continuam funcionando.

---

## Fase 5: Refinamento e aspectos transversais

**Objetivo**: verificar a cobertura exigida e os cenários de execução documentados para a aplicação completa.

- [X] T007 Executar `dotnet test .\tests\Loterias.Premiadas.Tests\Loterias.Premiadas.Tests.csproj --collect:"XPlat Code Coverage"` e ajustar os testes em `tests/Loterias.Premiadas.Tests/GeradorDeJogosLotofacilTests.cs` e `tests/Loterias.Premiadas.Tests/ProgramaTests.cs` até o relatório comprovar 100% de cobertura de linhas e ramificações do código executável em `src/Loterias.Premiadas/`.
- [X] T008 Executar os cenários manuais documentados em `specs/002-gerar-jogos-lotofacil/quickstart.md` usando `src/Loterias.Premiadas/Loterias.Premiadas.csproj`, incluindo ambas as modalidades, entrada inválida, 1.000 jogos e EOF; corrigir discrepâncias no comportamento da aplicação ou no documento.

---

## Dependências e ordem de execução

### Dependências entre fases

- **Preparação (Fase 1)** e **Fundação (Fase 2)**: nenhuma alteração necessária; a estrutura existente atende ao plano.
- **US1 (Fase 3)**: pode começar imediatamente. T001 e T002 podem ser executadas em paralelo; T003 depende de T001 e T004 depende de T002 e T003.
- **US2 (Fase 4)**: os testes T005 e a implementação T006 podem avançar em paralelo em arquivos distintos após a conclusão da interação básica de US1; a integração e validação devem ocorrer depois de ambos.
- **Refinamento (Fase 5)**: T007 e T008 dependem da conclusão das duas histórias de usuário.

### Dependências entre histórias de usuário

- **US1 (P1)**: sem dependência de outra história; entrega o MVP.
- **US2 (P2)**: estende os caminhos de entrada do mesmo fluxo de console; executar após US1 para evitar alterações concorrentes em `Program.cs` e `ProgramaTests.cs`. Não há dependência das regras de domínio ou dos resultados válidos da US1 para testar a correção de entradas inválidas.

### Oportunidades de paralelismo

- Na US1, T001 e T002 atuam em arquivos de teste distintos e podem começar em paralelo. A implementação T003 pode começar após T001; a integração em T004 aguarda T002 e T003.
- Na US2, T005 e T006 atuam em arquivos diferentes e podem ser desenvolvidas em paralelo após a US1; validar a história somente quando ambas estiverem concluídas.
- Não há tarefas independentes de preparação ou fundação.

### Exemplo de execução paralela: História de Usuário 1

```text
Em paralelo:
- T001: testes do gerador Lotofácil em tests/Loterias.Premiadas.Tests/GeradorDeJogosLotofacilTests.cs
- T002: testes do fluxo de console em tests/Loterias.Premiadas.Tests/ProgramaTests.cs

Depois:
- T003: implementar src/Loterias.Premiadas/GeradorDeJogosLotofacil.cs
- T004: integrar os dois geradores em src/Loterias.Premiadas/Program.cs
```

### Exemplo de execução paralela: História de Usuário 2

```text
Após a US1, em paralelo:
- T005: testes de entradas inválidas em tests/Loterias.Premiadas.Tests/ProgramaTests.cs
- T006: correção e repetição de solicitações em src/Loterias.Premiadas/Program.cs

Depois, validar os caminhos corrigidos e executar a Fase 5.
```

## Estratégia de implementação

### MVP primeiro

1. Usar os projetos .NET 10 e xUnit/Coverlet já existentes; nenhuma configuração adicional é necessária.
2. Concluir a US1 e verificar independentemente a geração válida das duas modalidades e a saída no console.
3. Usar a US1 como MVP demonstrável antes de incluir a correção de solicitações inválidas da US2.

### Entrega incremental

1. Entregar a US1 com geração e seleção de modalidade preservando os critérios existentes da Mega-Sena.
2. Acrescentar a US2 para orientar e repetir entradas inválidas sem emitir resultados.
3. Concluir a validação transversal e comprovar a cobertura integral exigida pela constituição.
