---
description: "Tarefas de implementação da remoção dos filtros de validação dos jogos"
---

# Tarefas: Remover filtros de validação dos jogos

**Entrada**: Documentos de design em `specs\004-remover-filtros-jogos\`

**Pré-requisitos**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts\geracao.md`, `quickstart.md`

**Testes**: Incluídos porque a constituição (princípio IV) exige testes automatizados com 100% de cobertura de linhas e ramificações do código executável da aplicação.

**Organização**: Tarefas agrupadas por história de usuário. US1 (Mega-Sena) e US2 (Lotofácil) mexem em arquivos diferentes e podem ser feitas em paralelo.

## Formato: `[ID] [P?] [História] Descrição`

- **[P]**: pode rodar em paralelo (arquivos diferentes, sem dependência de tarefa incompleta)
- **[História]**: história de usuário a que a tarefa pertence (US1, US2)

---

## Fase 1: Preparação (Infraestrutura Compartilhada)

**Objetivo**: Confirmar o ponto de partida antes de alterar o código.

- [X] T001 Executar `dotnet test --collect:"XPlat Code Coverage"` na raiz do repositório e confirmar que todos os testes atuais passam com `line-rate="1"` e `branch-rate="1"` no `coverage.cobertura.xml` gerado em `tests\Loterias.Premiadas.Tests\TestResults\`

---

## Fase 2: Fundação (Pré-requisitos Bloqueadores)

Nenhuma. A assinatura pública `IReadOnlyList<int[]> Gerar(int quantidade, Random aleatorio)` e as exceções de cada serviço não mudam (ver `contracts\geracao.md`), então não há infraestrutura compartilhada a preparar. `src\Loterias.Premiadas\Program.cs` e `tests\Loterias.Premiadas.Tests\ProgramaTests.cs` não são alterados.

---

## Fase 3: História de Usuário 1 - Gerar jogos da Mega-Sena sem filtros estatísticos (Prioridade: P1) 🎯 MVP

**Objetivo**: Cada jogo da Mega-Sena é aceito desde que tenha "6 números distintos entre 1 e 60", em ordem crescente, sem descarte por paridade, grupos, soma, algarismo final ou números consecutivos.

**Teste independente**: Com o `AleatorioControlado` existente, gerar 1 jogo a partir de exatamente 6 valores distintos que violam uma regra antiga e confirmar que o resultado é essa combinação ordenada, sem consumir valores adicionais; gerar 1.000 jogos com `new Random(42)` e confirmar quantidade, intervalo, distinção e ordem.

### Testes da História de Usuário 1

- [X] T002 [P] [US1] Em `tests\Loterias.Premiadas.Tests\Tests\MegasenaTests.cs`, simplificar `AssertJogoValido` para verificar apenas: `jogo.Length == 6`, ordem crescente, 6 números distintos e todos entre 1 e 60; remover as asserções de paridade, grupos, soma, algarismo final e consecutivos, e remover o auxiliar `ObterGrupo`
- [X] T003 [US1] Em `tests\Loterias.Premiadas.Tests\Tests\MegasenaTests.cs`, remover os testes das regras antigas: `Gerar_DeveAceitarQuandoParidadeEGruposSaoAtendidos`, `Gerar_DeveAceitarLimitesInclusivosDeParidade`, `Gerar_DeveAceitarQuandoParidadeESomaSaoAtendidas`, `Gerar_DeveAceitarQuandoGruposESomaSaoAtendidos`, `Gerar_DeveSubstituirCandidataComTresNumerosConsecutivos`, `Gerar_DeveSubstituirCandidataComTresFinaisIguais`, `Gerar_DevePermitirUmParConsecutivo`, `Gerar_DeveAceitarLimitesInclusivosDaSoma` e `Gerar_DeveSubstituirCandidataQueAtendeMenosDeDuasRegras`; remover também a sobrecarga `GerarUmaCandidata(params int[][] candidatas)` se não for mais usada
- [X] T004 [US1] Em `tests\Loterias.Premiadas.Tests\Tests\MegasenaTests.cs`, adicionar o teste `[Theory]` `Gerar_DeveAceitarCombinacaoAntesRejeitada`, com os casos `1,2,3,4,5,6` (consecutivos e soma baixa), `2,4,6,8,10,12` (só pares) e `2,12,22,31,44,55` (três finais iguais); usar `AleatorioControlado` com exatamente esses 6 valores, verificar que o jogo retornado é igual à combinação ordenada e que a fila do `AleatorioControlado` ficou vazia (expor a contagem restante na classe de teste)
- [X] T005 [US1] Em `tests\Loterias.Premiadas.Tests\Tests\MegasenaTests.cs`, manter `Gerar_DeveRetornarQuantidadeSolicitadaDeJogosValidos`, `Gerar_DeveDescartarNumerosRepetidosNaCandidata` (renomear para `Gerar_DeveIgnorarNumeroRepetidoNoJogo`), `Gerar_DeveRejeitarQuantidadeForaDoIntervalo` e `Gerar_DeveRejeitarFonteAleatoriaNula`, ajustando-os à nova `AssertJogoValido`; executar os testes da classe e confirmar que `Gerar_DeveAceitarCombinacaoAntesRejeitada` FALHA antes da T006

### Implementação da História de Usuário 1

- [X] T006 [US1] Em `src\Loterias.Premiadas\Services\MegasenaService.cs`, remover os métodos `EhValida` e `ObterGrupo`; renomear `GerarCandidata` para `GerarJogo`; em `Gerar`, substituir o laço com descarte por um laço que adiciona `GerarJogo(aleatorio)` até `jogos.Count == quantidade`; manter as validações de `aleatorio` nulo (`ArgumentNullException`) e de quantidade fora de 1 a 1000 (`ArgumentOutOfRangeException` com "A quantidade deve estar entre 1 e 1000."), o uso de `aleatorio.Next(1, 61)`, o descarte de números repetidos e a ordenação
- [X] T007 [US1] Executar `dotnet test --filter "FullyQualifiedName~MegasenaTests"` e confirmar que todos os testes da Mega-Sena passam

**Ponto de verificação**: a Mega-Sena gera jogos sem filtros estatísticos e pode ser demonstrada sozinha.

---

## Fase 4: História de Usuário 2 - Gerar jogos da Lotofácil sem filtros estatísticos (Prioridade: P1)

**Objetivo**: Cada jogo da Lotofácil é aceito desde que tenha "15 números distintos entre 1 e 25", em ordem crescente, sem descarte por paridade, moldura/miolo ou grupos sequenciais.

**Teste independente**: Com o `AleatorioControlado` existente, gerar 1 jogo a partir de exatamente 15 valores distintos que violam uma regra antiga e confirmar que o resultado é essa combinação ordenada, sem consumir valores adicionais; gerar 1.000 jogos com `new Random(42)` e confirmar quantidade, intervalo, distinção e ordem.

### Testes da História de Usuário 2

- [X] T008 [P] [US2] Em `tests\Loterias.Premiadas.Tests\Tests\LotofacilTests.cs`, simplificar `AssertJogoValido` para verificar apenas: `jogo.Length == 15`, 15 números distintos, ordem crescente e todos entre 1 e 25; remover as asserções de paridade, moldura e grupos sequenciais, e remover o auxiliar `EhDaMoldura`
- [X] T009 [US2] Em `tests\Loterias.Premiadas.Tests\Tests\LotofacilTests.cs`, remover os testes das regras antigas: `Gerar_DeveAceitarParidadesEContagensDeMolduraValidas`, `Gerar_DeveAceitarCadaUmaDasTresDistribuicoesDeParidade` e `Gerar_DeveSubstituirCandidataQueViolaUmaRegraObrigatoria`; remover também a sobrecarga `GerarUmaCandidata(params int[][] candidatas)` se não for mais usada
- [X] T010 [US2] Em `tests\Loterias.Premiadas.Tests\Tests\LotofacilTests.cs`, adicionar o teste `[Theory]` `Gerar_DeveAceitarCombinacaoAntesRejeitada`, com os casos `1,2,3,4,5,6,7,8,9,10,11,12,13,14,15` (grupos 16–20 e 21–25 ausentes), `1,3,5,7,9,11,13,15,17,19,2,4,6,8,10` (10 ímpares e 5 pares) e `1,4,6,7,10,11,12,13,14,17,18,20,21,22,24` (candidata antes rejeitada); usar `AleatorioControlado` com exatamente esses 15 valores, verificar que o jogo retornado é igual à combinação ordenada e que a fila do `AleatorioControlado` ficou vazia
- [X] T011 [US2] Em `tests\Loterias.Premiadas.Tests\Tests\LotofacilTests.cs`, manter `Gerar_DeveRetornarQuantidadeSolicitadaDeJogosValidos`, `Gerar_DeveIgnorarNumeroRepetidoNaCandidata` (renomear para `Gerar_DeveIgnorarNumeroRepetidoNoJogo` e verificar que o resultado é igual aos 15 números distintos ordenados), `Gerar_DeveRejeitarQuantidadeForaDoIntervalo` e `Gerar_DeveRejeitarFonteAleatoriaNula`; executar os testes da classe e confirmar que `Gerar_DeveAceitarCombinacaoAntesRejeitada` FALHA antes da T012

### Implementação da História de Usuário 2

- [X] T012 [US2] Em `src\Loterias.Premiadas\Services\LotofacilService.cs`, remover os métodos `EhValida`, `EhDaMoldura` e `ObterGrupoSequencial`; renomear `GerarCandidata` para `GerarJogo`; em `Gerar`, substituir o laço com descarte por um laço que adiciona `GerarJogo(aleatorio)` até `jogos.Count == quantidade`; manter as validações de `aleatorio` nulo (`ArgumentNullException`) e de quantidade fora de 1 a 1000 (`ArgumentOutOfRangeException` com "A quantidade deve estar entre 1 e 1000."), o uso de `aleatorio.Next(1, 26)`, o descarte de números repetidos e a ordenação
- [X] T013 [US2] Executar `dotnet test --filter "FullyQualifiedName~LotofacilTests"` e confirmar que todos os testes da Lotofácil passam

**Ponto de verificação**: as duas modalidades geram jogos sem filtros estatísticos.

---

## Fase 5: Acabamento e Questões Transversais

- [X] T014 [P] Atualizar `README.md`: na lista de funcionalidades, trocar "substituindo combinações que não atendam aos critérios da modalidade" por uma frase dizendo que cada jogo tem apenas números distintos; na seção "Critérios aplicados", deixar só "Seis números distintos entre 1 e 60" (Mega-Sena) e "Quinze números distintos entre 1 e 25" (Lotofácil) e remover as regras de paridade, grupos, soma, finais, consecutivos e moldura
- [X] T015 Executar `dotnet test --collect:"XPlat Code Coverage"` na raiz e confirmar que todos os testes passam, incluindo `tests\Loterias.Premiadas.Tests\ProgramaTests.cs` sem alterações, e que o `coverage.cobertura.xml` mais recente mostra `line-rate="1"` e `branch-rate="1"` para o pacote `Loterias.Premiadas` (princípio IV)
- [X] T016 Executar a verificação manual de `specs\004-remover-filtros-jogos\quickstart.md` (seção 3) com `dotnet run --project src\Loterias.Premiadas`: 20 jogos de cada modalidade, quantidade inválida `0` e 1.000 jogos sem espera perceptível (SC-004)

---

## Dependências e Ordem de Execução

### Dependências entre fases

- **Preparação (Fase 1)**: sem dependências; estabelece a linha de base.
- **Fundação (Fase 2)**: vazia.
- **US1 (Fase 3)** e **US2 (Fase 4)**: dependem só da Fase 1 e são independentes entre si (arquivos diferentes).
- **Acabamento (Fase 5)**: T014 pode ser feita a qualquer momento; T015 e T016 dependem de US1 e US2 concluídas.

### Dentro de cada história

- Testes primeiro (T002→T005, T008→T011), confirmando a falha do novo teste antes da implementação.
- Implementação (T006, T012) depois dos testes.
- Execução dos testes da história (T007, T013) ao final.
- As tarefas de teste de uma mesma história são sequenciais porque editam o mesmo arquivo.

### Oportunidades de paralelismo

- A Fase 3 inteira (`MegasenaService.cs`, `MegasenaTests.cs`) e a Fase 4 inteira (`LotofacilService.cs`, `LotofacilTests.cs`) podem correr ao mesmo tempo.
- T014 (`README.md`) pode correr em paralelo com qualquer tarefa das Fases 3 e 4.

## Exemplo de paralelismo

```text
# Depois da T001, em paralelo:
Fluxo A (US1): T002 → T003 → T004 → T005 → T006 → T007
Fluxo B (US2): T008 → T009 → T010 → T011 → T012 → T013
Fluxo C:       T014
# Depois dos três fluxos:
T015 → T016
```

## Estratégia de Implementação

### MVP primeiro (somente US1)

1. Concluir a Fase 1 (linha de base).
2. Concluir a Fase 3 (Mega-Sena).
3. **Parar e validar**: rodar `MegasenaTests` e gerar jogos da Mega-Sena pelo console.

### Entrega incremental

1. Linha de base → US1 (Mega-Sena sem filtros) → validar.
2. US2 (Lotofácil sem filtros) → validar.
3. README, cobertura integral e verificação manual.

A cobertura total só é exigida na T015, mas como cada serviço é independente, cada história já deve deixar seu serviço com 100% de linhas e ramificações cobertas.
