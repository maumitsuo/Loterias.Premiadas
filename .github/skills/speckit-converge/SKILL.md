---
name: "speckit-converge"
description: "Avalia a base de código atual em relação à especificação, ao plano e às tarefas da funcionalidade e acrescenta a tasks.md novas tarefas para todo trabalho ainda não implementado, para que /speckit-implement possa concluí-lo."
compatibility: "Requer uma estrutura de projeto spec-kit com o diretório .specify/"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/converge.md"
---


## Entrada do usuário

```text
$ARGUMENTS
```

Você **DEVE** considerar a entrada do usuário antes de prosseguir (se não estiver vazia).

## Verificações pré-execução

**Verifique os hooks de extensão (antes da convergência)**:

- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.before_converge`.
- Se o YAML não puder ser analisado ou for inválido, não ignore isso silenciosamente: informe ao usuário que não foi possível ler `.specify/extensions.yml` (inclua o erro do analisador) e que nenhum hook foi verificado, inclusive os hooks obrigatórios (`optional: false`) registrados nele; em seguida, prossiga normalmente.
- Filtre os hooks em que `enabled` esteja explicitamente definido como `false`. Considere habilitados por padrão os hooks sem o campo `enabled`.
- Para cada hook restante, **não** tente interpretar nem avaliar expressões `condition`:
  - Se o hook não tiver o campo `condition`, ou se ele for nulo/vazio, considere-o executável.
  - Se o hook definir um `condition` não vazio, ignore-o e deixe a avaliação da condição para a implementação de HookExecutor.
- Ao construir invocações de comandos a partir dos nomes de hooks, substitua pontos (`.`) por hífens (`-`). Por exemplo, `speckit.git.commit` → `/speckit-git-commit`.
- Para cada hook executável, emita o seguinte conforme seu indicador `optional`:
  - **Hook opcional** (`optional: true`):

    ```text
    ## Hooks de extensão

    **Pré-hook opcional**: {extension}
    Comando: `/{command}`
    Descrição: {description}

    Solicitação: {prompt}
    Para executar: `/{command}`
    ```

  - **Hook obrigatório** (`optional: false`):

    ```text
    ## Hooks de extensão

    **Pré-hook automático**: {extension}
    Executando: `/{command}`
    EXECUTE_COMMAND: {command}

    Aguarde o resultado do comando do hook antes de prosseguir para a seção Objetivo.
    ```
    Depois de emitir o bloco acima, você DEVE invocar o hook de fato e aguardar sua conclusão antes de continuar. Execute-o da mesma forma que executaria o comando neste agente/sessão (a invocação pode diferir do identificador literal `{command}` mostrado acima; por exemplo, um agente no modo skills o executa como `/skill:speckit-...` ou `$speckit-...`). Apenas emitir o bloco não executa o hook.

- Se nenhum hook estiver registrado ou `.specify/extensions.yml` não existir, ignore silenciosamente.

## Objetivo

Feche a lacuna entre o que a especificação, o plano e as tarefas da funcionalidade exigem e o
que a base de código implementa atualmente. Leia `spec.md`, `plan.md` e `tasks.md` como a **única
fonte de intenção** (com a constituição como conjunto de restrições aplicáveis), avalie o estado
atual do código, determine quais requisitos, critérios de aceitação, decisões do plano e tarefas
existentes não foram atendidos, estão incompletos ou foram apenas parcialmente satisfeitos e
**acrescente cada trabalho restante como uma nova tarefa rastreável** ao final de `tasks.md` para
que `/speckit-implement` possa concluí-lo. Este comando DEVE ser executado somente depois que
`/speckit-implement` tiver sido executado no `tasks.md` atual e que `/speckit-tasks` tiver produzido um `tasks.md` completo.

Esta **não** é uma ferramenta de diff e **não** rastreia alterações. Ela avalia o estado atual
do código em relação aos artefatos da funcionalidade — sem git, comparação de branches ou histórico.

## Restrições de operação

**SOMENTE ACRÉSCIMO, NUNCA REESCRITA**: A **única** gravação do comando é acrescentar uma nova
seção `## Phase N: Convergence` a `tasks.md`. Ele NÃO DEVE:

- modificar `spec.md` ou `plan.md` de qualquer forma;
- reescrever, renumerar, reordenar ou excluir qualquer tarefa existente (inclusive tarefas de
  uma fase Convergence anterior);
- modificar, criar ou excluir código do aplicativo — concluir as tarefas acrescentadas é
  responsabilidade de `/speckit-implement`.

Quando a base de código já satisfizer tudo, o comando DEVE deixar `tasks.md`
**inalterado byte a byte** (sem cabeçalho Convergence vazio) e informar que não há pendências.

**Autoridade da constituição**: A constituição do projeto (`.specify/memory/constitution.md`) é
**inegociável**. Código que viole um princípio MUST constitui o achado de maior severidade e
gera uma tarefa de correção correspondente. Se a constituição for um modelo não preenchido,
ignore as verificações da constituição de forma adequada, em vez de falhar.

## Etapas de execução

### 1. Inicializar o contexto da convergência

Execute `.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireSpec -RequireTasks -IncludeTasks` uma vez a partir da raiz do repositório e analise o JSON para obter FEATURE_DIR e AVAILABLE_DOCS. Derive os caminhos absolutos:

- SPEC = FEATURE_DIR/spec.md
- PLAN = FEATURE_DIR/plan.md
- TASKS = FEATURE_DIR/tasks.md
- CONSTITUTION = `.specify/memory/constitution.md` (if present)
Se `spec.md`, `plan.md` ou `tasks.md` estiver ausente, PARE e emita uma mensagem clara e acionável que indique o comando de pré-requisito a executar (`/speckit-specify` se faltar a especificação, `/speckit-plan` se faltar o plano, `/speckit-tasks` se faltarem as tarefas). Não produza uma saída parcial.
Para apóstrofos em argumentos como "I'm Groot", use a sintaxe de escape, por exemplo: `I'\''m Groot` (ou aspas duplas, se possível: "I'm Groot").

### 2. Carregar os artefatos (divulgação progressiva)

Carregue apenas o contexto mínimo necessário de cada artefato:

**De spec.md:**

- Requisitos funcionais (FR-###).
- Critérios de sucesso (SC-###) — inclua somente itens que exijam trabalho implementável; exclua
  métricas de resultado pós-lançamento e KPIs de negócios.
- Histórias de usuário e seus cenários de aceitação.
- Casos extremos (se houver).

**De plan.md:**

- Escolhas de arquitetura/stack e decisões técnicas.
- Referências ao modelo de dados.
- Fases e pontos de contato nomeados (arquivos/componentes que o plano indica que serão criados ou editados).
- Restrições técnicas.

**De tasks.md:**

- IDs das tarefas (para calcular o próximo ID e o próximo número de fase).
- Descrições, agrupamento por fase e caminhos de arquivos referenciados.

**Da constituição (se não for um modelo não preenchido):**

- Nomes dos princípios e declarações normativas MUST/SHOULD.

### 3. Criar o inventário de intenções

Crie um modelo interno (não repita os artefatos brutos):

- **Inventário de requisitos**: uma chave estável por FR-### / SC-### / cenário de aceitação de
  história de usuário (por exemplo, `US1/AC2`), além das decisões do plano e dos princípios da
  constituição que imponham obrigações implementáveis.
- **Mapa do escopo do código**: a partir dos caminhos de arquivos nomeados em `plan.md` e
  `tasks.md`, além de uma busca por palavras-chave dos conceitos descritos por cada requisito,
  derive o conjunto de arquivos-fonte e componentes dentro do escopo da avaliação. Limite a
  avaliação a esse conjunto — **não** infira um escopo além do definido pelos artefatos.

### 4. Avaliar a base de código e classificar os achados

Inclua todas as tarefas existentes no inventário de intenções, independentemente do estado das
caixas de seleção ou da fase Convergence: alegações de conclusão não são evidências. Verifique o
comportamento atual em relação à especificação, ao plano, às tarefas e à constituição; em cadeias
de tarefas corretivas, avalie o comportamento resultante, não detalhes de implementação
substituídos. Verifique tanto as obrigações não atendidas quanto as implementações que contradigam,
excedam ou estejam fora da intenção declarada.

Para cada item do inventário de intenções, inspecione o código atual dentro do escopo e produza um
`Finding` somente quando houver uma lacuna. Classifique cada achado por **tipo de lacuna**:

- **`missing`**: o trabalho exigido está totalmente ausente do código.
- **`partial`**: o trabalho existe, mas ainda não satisfaz plenamente o requisito/o critério de
  aceitação/a decisão do plano.
- **`contradicts`**: o código faz algo que entra em conflito com a intenção declarada ou com um
  princípio MUST da constituição.
- **`unrequested`**: o código contém trabalho não solicitado pela especificação, pelo plano ou
  pelas tarefas (sinalizado para conhecimento — converge **não** exclui código; apenas acrescenta
  uma tarefa para revisá-lo/justificá-lo ou removê-lo).

Cada `Finding` registra: um ID estável, a `source-ref` à qual está associado, o `gap-type`, uma
severidade e uma descrição curta em linguagem natural acompanhada de evidências (arquivo/área
observada).

**Casos extremos:**

- **Pouco ou nenhum código ainda**: considere todo o escopo especificado como trabalho restante
  `missing`, em vez de falhar.
- **Nada resta**: não produza achados e siga o caminho converged na Etapa 7.

### 5. Atribuir severidade

- **CRITICAL**: viola um princípio MUST da constituição ou uma lacuna `missing`/`contradicts`
  que bloqueia a funcionalidade básica de uma história de usuário P1.
- **HIGH**: lacuna `missing` ou `partial` em um requisito funcional central ou critério de
  aceitação.
- **MEDIUM**: lacuna `partial` em requisito secundário ou adição `unrequested` com justificativa
  pouco clara.
- **LOW**: lacunas parciais menores, polimento ou adições `unrequested` de baixo risco.

### 6. Apresentar o resumo dos achados na sessão

Antes de acrescentar qualquer conteúdo, emita um resumo compacto, classificado por severidade (ainda sem gravar arquivos):

## Achados da convergência

| ID | Tipo de lacuna | Severidade | Origem | Evidência | Trabalho restante |
|----|----------|----------|--------|----------|----------------|
| F1 | missing  | HIGH     | FR-008 | Exemplo: nenhuma proteção de somente acréscimo detectada em path/to/module.py ao gravar tasks.md | Adicione a garantia de somente acréscimo |

**Métricas do resumo:**

- Requisitos/critérios de aceitação verificados.
- Decisões do plano verificadas.
- Princípios da constituição verificados (ou "ignorados — modelo").
- Achados por tipo de lacuna (missing / partial / contradicts / unrequested).
- Achados por severidade.

### 7. Acrescentar tarefas de convergência (ou informar converged)

**Se houver um ou mais achados acionáveis** (resultado `tasks_appended`):

Acrescente ao **final** de `tasks.md`, conforme o contrato de acréscimo:

1. Examine todos os IDs de tarefas existentes; considere `M` como o maior. Determine o próximo
   número de fase `N` (maior fase existente + 1).
2. Escreva um único cabeçalho de seção novo `## Phase N: Convergence`.
3. Emita um item da lista por achado acionável, primeiro os de severidade CRITICAL/HIGH,
   atribuindo IDs com zeros à esquerda `T{M+1:03d}, T{M+2:03d}, …`:

   ```markdown
   - [ ] T042 <imperative description> per <source-ref> (<gap-type>)
   ```

   `<source-ref>` associa a tarefa à sua origem: por exemplo, `FR-003`, `SC-002`,
   `US1/AC2`, `plan: storage decision`, `Constitution II`.

   `<gap-type>` é um dos seguintes: `missing`, `partial`, `contradicts`, `unrequested`.

   Tarefas de violação da constituição DEVEM ser emitidas primeiro e descritas como
   `CRITICAL`.
4. Nunca reutilize nem renumere IDs existentes. Se já houver uma fase Convergence, acrescente
   abaixo dela uma nova fase, com numeração separada — não altere a fase anterior.

**Se não houver achados acionáveis** (resultado `converged`):

- **Não** modifique `tasks.md` — não acrescente um cabeçalho de fase vazio.
- Informe: **"✅ Convergido — a implementação satisfaz a especificação, o plano e as tarefas."**
- Inclua as contagens resumidas do que foi verificado.

### 8. Informar as próximas ações (transferência)

- Em `tasks_appended`: informe quantas tarefas foram acrescentadas e em qual fase; recomende
  executar `/speckit-implement` para concluí-las e observe que uma execução posterior de converge
  encontrará menos itens pendentes ou nenhum.
- Em `converged`: recomende prosseguir para revisão/abertura de um PR. Não é necessária outra
  etapa de implementação para o escopo especificado desta funcionalidade.

### 9. Verificar os hooks de extensão

Após produzir o resultado, verifique se `.specify/extensions.yml` existe na raiz do projeto.

- Se existir, leia-o e procure entradas na chave `hooks.after_converge`.
- Se o YAML não puder ser analisado ou for inválido, não ignore isso silenciosamente: informe ao usuário que não foi possível ler `.specify/extensions.yml` (inclua o erro do analisador) e que nenhum hook foi verificado, inclusive os hooks obrigatórios (`optional: false`) registrados nele; em seguida, prossiga normalmente.
- Filtre os hooks em que `enabled` esteja explicitamente definido como `false`. Considere habilitados por padrão os hooks sem o campo `enabled`.
- Para cada hook restante, **não** tente interpretar nem avaliar expressões `condition`:
  - Se o hook não tiver o campo `condition`, ou se ele for nulo/vazio, considere-o executável.
  - Se o hook definir um `condition` não vazio, ignore-o e deixe a avaliação da condição para a implementação de HookExecutor.
- Informe o resultado da convergência (`converged` ou `tasks_appended`) na sessão antes de listar
  quaisquer hooks, para que os usuários decidam se querem executar comandos opcionais subsequentes.
- Ao construir invocações de comandos a partir dos nomes de hooks, substitua pontos (`.`) por hífens (`-`). Por exemplo, `speckit.git.commit` → `/speckit-git-commit`.
- Para cada hook executável, emita o seguinte conforme seu indicador `optional`:
  - **Hook opcional** (`optional: true`):

    ```text
    ## Hooks de extensão

    **Hook opcional**: {extension}
    Comando: `/{command}`
    Descrição: {description}

    Solicitação: {prompt}
    Para executar: `/{command}`
    ```

  - **Hook obrigatório** (`optional: false`):

    ```text
    ## Hooks de extensão

    **Hook automático**: {extension}
    Executando: `/{command}`
    EXECUTE_COMMAND: {command}
    ```
    Depois de emitir o bloco acima, você DEVE invocar o hook de fato e aguardar sua conclusão antes de continuar. Execute-o da mesma forma que executaria o comando neste agente/sessão (a invocação pode diferir do identificador literal `{command}` mostrado acima; por exemplo, um agente no modo skills o executa como `/skill:speckit-...` ou `$speckit-...`). Apenas emitir o bloco não executa o hook.

- Se nenhum hook estiver registrado ou `.specify/extensions.yml` não existir, ignore silenciosamente.
