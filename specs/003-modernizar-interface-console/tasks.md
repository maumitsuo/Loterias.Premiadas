---
description: "Tarefas de implementação da modernização da interface do console"
---

# Tarefas: Modernizar a interface do console

**Entrada**: Documentos de design em `specs\003-modernizar-interface-console\`

**Pré-requisitos**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts\console.md`

**Testes**: Incluídos porque os cenários da especificação e a constituição exigem testes automatizados e 100% de cobertura de linhas e ramificações do código executável da aplicação. O critério SC-002 (avaliação com pelo menos 10 pessoas) fica fora do escopo de implementação.

**Organização**: Tarefas agrupadas por história de usuário; cada história tem critérios para validação independente.

## Fase 1: Preparação (Infraestrutura Compartilhada)

**Objetivo**: Disponibilizar a biblioteca de interação e seu console de teste nos projetos existentes.

- [X] T001 [P] Adicionar `Spectre.Console` na versão `0.57.2` em `src\Loterias.Premiadas\Loterias.Premiadas.csproj`
- [X] T002 [P] Adicionar `Spectre.Console.Testing` na versão `0.57.2` em `tests\Loterias.Premiadas.Tests\Loterias.Premiadas.Tests.csproj`

---

## Fase 2: Fundação (Pré-requisitos Bloqueadores)

**Objetivo**: Permitir executar o fluxo com uma instância de console injetada sem alterar a inicialização de produção.

- [X] T003 Criar em `src\Loterias.Premiadas\Program.cs` um ponto de entrada testável que aceite `IAnsiConsole` e uma função geradora injetável; manter `Main()` usando o console padrão e conectando essa função aos serviços existentes

**Ponto de verificação**: projetos restauram as dependências e o fluxo pode ser exercitado com `TestConsole`.

---

## Fase 3: História de Usuário 1 - Solicitar jogos com orientação clara (Prioridade: P1)

**Objetivo**: Guiar a seleção entre Mega-Sena e Lotofácil e aceitar uma quantidade válida de 1 a 1.000, gerando a modalidade e a quantidade escolhidas.

**Teste independente**: Com `TestConsole`, percorrer cada modalidade com quantidade válida e confirmar que a opção escolhida determina a geração e a quantidade de jogos; injetar geradores falsos para verificar tanto uma exceção quanto um resultado parcial seguido por falha.

### Testes da História de Usuário 1

- [X] T004 [US1] Adicionar em `tests\Loterias.Premiadas.Tests\ProgramaTests.cs` testes com `TestConsole` para seleção das duas modalidades e quantidades válidas 1 e 1.000; injetar um gerador que lança exceção e outro que retorna jogos parciais antes da falha, verificando erro, ausência de exibição parcial e retorno ao início da solicitação

### Implementação da História de Usuário 1

- [X] T005 [US1] Implementar em `src\Loterias.Premiadas\Program.cs` a seleção interativa da modalidade e a solicitação orientada de quantidade inteira entre 1 e 1.000; gerar e validar a coleção completa de jogos antes de exibir qualquer resultado e, se a geração falhar, exibir uma mensagem explícita e retornar ao início da solicitação

**Ponto de verificação**: uma solicitação válida para cada modalidade produz exatamente a quantidade escolhida e preserva as regras dos serviços existentes; uma falha antes ou durante a produção da coleção apresenta erro e retorna ao início sem expor jogos parciais.

---

## Fase 4: História de Usuário 2 - Interpretar os jogos gerados (Prioridade: P1)

**Objetivo**: Identificar modalidade e totais, distinguir cada jogo, manter números crescentes e oferecer nova solicitação ou encerramento.

**Teste independente**: Gerar jogos nas duas modalidades e confirmar cabeçalho, total solicitado/apresentado, linhas identificadas e crescentes, legibilidade sem cores e escolha de continuar ou encerrar. A validação de usabilidade com 10 participantes do SC-002 não faz parte deste escopo de implementação.

### Testes da História de Usuário 2

- [X] T006 [US2] Adicionar em `tests\Loterias.Premiadas.Tests\ProgramaTests.cs` testes de saída para modalidade, quantidades solicitada e apresentada, identificadores sequenciais, números crescentes e decisões de repetir ou encerrar

### Implementação da História de Usuário 2

- [X] T007 [US2] Implementar em `src\Loterias.Premiadas\Program.cs` a apresentação dos resultados em tabela com modalidade, totais, índice por jogo e números crescentes, além da repetição da solicitação ou encerramento após confirmação
- [X] T008 [US2] Implementar em `src\Loterias.Premiadas\Program.cs` o fluxo textual sem cursor quando entrada ou saída estiver redirecionada, mantendo conteúdo legível sem cores e encerrando sem anunciar resultado parcial em fim de entrada

**Ponto de verificação**: a saída interativa e a redirecionada distinguem os jogos e permitem continuar ou encerrar sem alterar os valores gerados.

---

## Fase 5: História de Usuário 3 - Corrigir uma entrada inválida (Prioridade: P2)

**Objetivo**: Explicar entradas inválidas, repetir somente a solicitação incorreta e nunca apresentar geração incompleta como concluída.

**Teste independente**: Fornecer modalidade vazia/desconhecida e quantidade vazia, não inteira, menor que 1 ou maior que 1.000; confirmar orientação, nova tentativa e ausência de resultado falso.

### Testes da História de Usuário 3

- [X] T009 [US3] Adicionar em `tests\Loterias.Premiadas.Tests\ProgramaTests.cs` testes de repetição para modalidade inválida e quantidades vazia, não inteira, abaixo de 1 e acima de 1.000, incluindo fim de entrada sem resultado

### Implementação da História de Usuário 3

- [X] T010 [US3] Implementar em `src\Loterias.Premiadas\Program.cs` mensagens de erro que indiquem a opção ou o intervalo aceito e permitam repetir apenas o campo inválido sem gerar resultado antes da conclusão

**Ponto de verificação**: toda entrada inválida permite correção no mesmo fluxo e nenhuma entrada incompleta é anunciada como geração concluída.

---

## Fase 6: Acabamento e Aspectos Transversais

**Objetivo**: Demonstrar cobertura integral e confirmar os cenários documentados de execução.

- [X] T011 Executar `dotnet test .\tests\Loterias.Premiadas.Tests\Loterias.Premiadas.Tests.csproj --collect:"XPlat Code Coverage"` e ajustar `tests\Loterias.Premiadas.Tests\ProgramaTests.cs` ou `src\Loterias.Premiadas\Program.cs` até atingir 100% das linhas e ramificações executáveis da aplicação; validar `specs\003-modernizar-interface-console\quickstart.md`

---

## Dependências e Ordem de Execução

### Dependências entre fases

- **Preparação (Fase 1)**: T001 e T002 não dependem uma da outra e podem ser feitas em paralelo.
- **Fundação (Fase 2)**: T003 depende das referências de pacote T001 e T002; bloqueia as histórias.
- **US1 (Fase 3)**: depende de T003; T004 deve ser escrito antes de T005 e cobre também o retorno ao início após falha na geração.
- **US2 (Fase 4)**: depende da geração da US1; T006 deve ser escrito antes de T007 e T008.
- **US3 (Fase 5)**: depende do fluxo de solicitação da US1 e da implementação do fluxo de console; T009 deve ser escrito antes de T010.
- **Acabamento (Fase 6)**: depende de todas as histórias desejadas.

### Grafo de dependências entre histórias

```text
Preparação (T001, T002)
        |
Fundação (T003)
        |
       US1 (T004 -> T005; erro de geração -> início da solicitação)
        |
       US2 (T006 -> T007 -> T008)
        |
       US3 (T009 -> T010)
        |
Acabamento (T011)
```

### Oportunidades de paralelismo

- T001 e T002 editam projetos diferentes e podem ser executadas em paralelo.
- Não há tarefas de implementação das histórias marcadas `[P]`: todas alteram o mesmo fluxo em `Program.cs` ou a mesma suíte `ProgramaTests.cs`; além disso, US2 consome os resultados gerados pela US1.
- Dentro de cada história, escreva primeiro os testes e então implemente o comportamento para que a história seja validada independentemente.

### Exemplos de execução paralela por história

**US1**: após T003, inicie T004; T005 começa após os testes estarem definidos. Inclua no teste uma falha de geração e confirme mensagem de erro, ausência de resultado parcial e retorno ao início da solicitação. Não há edição paralela segura dentro desta história porque testes e implementação integram o mesmo fluxo.

**US2**: após US1, inicie T006; em seguida, implemente a tabela em T007 e o modo redirecionado em T008, ambos em `Program.cs` e portanto sequenciais.

**US3**: após US2, inicie T009; implemente a correção de validação em T010 depois dos casos de teste. Os testes e a implementação compartilham os arquivos já utilizados pelas histórias anteriores.

---

## Estratégia de Implementação

### MVP primeiro (US1)

1. Concluir T001–T003 para configurar os pacotes e o ponto de entrada testável.
2. Concluir T004–T005 para solicitar modalidade/quantidade e gerar a solicitação válida.
3. Validar US1 de forma independente com `TestConsole`, incluindo as duas modalidades e os limites de quantidade.

### Entrega incremental

1. Preparação e fundação: referências de dependência e console injetável.
2. US1: solicitação válida e geração correspondente — MVP.
3. US2: resultados identificáveis, tabela, fallback textual e continuidade.
4. US3: mensagens e correções para entradas inválidas.
5. Acabamento: verificar 100% de cobertura de linhas e ramificações e executar o quickstart.
