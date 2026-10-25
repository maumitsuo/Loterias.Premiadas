---
name: "speckit-analyze"
description: "Realiza uma análise não destrutiva de consistência e qualidade entre spec.md, plan.md e tasks.md após a geração das tarefas."
compatibility: "Requer uma estrutura de projeto spec-kit com o diretório .specify/"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/analyze.md"
---


## Entrada do usuário

```text
$ARGUMENTS
```

Você **DEVE** considerar a entrada do usuário antes de prosseguir (se não estiver vazia).

## Verificações pré-execução

**Verifique os hooks de extensão (antes da análise)**:
- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.before_analyze`.
- Se o YAML não puder ser analisado ou for inválido, não ignore isso silenciosamente: informe ao usuário que não foi possível ler `.specify/extensions.yml` (inclua o erro do analisador) e que nenhum hook foi verificado, inclusive os hooks obrigatórios (`optional: false`) registrados nele; em seguida, prossiga normalmente.
- Filtre os hooks em que `enabled` esteja explicitamente definido como `false`. Considere habilitados por padrão os hooks sem o campo `enabled`.
- Para cada hook restante, **não** tente interpretar nem avaliar expressões `condition`:
  - Se o hook não tiver o campo `condition`, ou se ele for nulo/vazio, considere-o executável.
  - Se o hook definir um `condition` não vazio, ignore-o e deixe a avaliação da condição para a implementação de HookExecutor.
- Ao construir invocações de comandos a partir dos nomes de hooks, substitua pontos (`.`) por hífens (`-`). Por exemplo, `speckit.git.commit` → `/speckit-git-commit`.
- Para cada hook executável, emita o seguinte conforme seu indicador `optional`:
  - **Hook opcional** (`optional: true`):
    ```
    ## Hooks de extensão

    **Pré-hook opcional**: {extension}
    Comando: `/{command}`
    Descrição: {description}

    Solicitação: {prompt}
    Para executar: `/{command}`
    ```
  - **Hook obrigatório** (`optional: false`):
    ```
    ## Hooks de extensão

    **Pré-hook automático**: {extension}
    Executando: `/{command}`
    EXECUTE_COMMAND: {command}

    Aguarde o resultado do comando do hook antes de prosseguir para a seção Objetivo.
    ```
    Depois de emitir o bloco acima, você DEVE invocar o hook de fato e aguardar sua conclusão antes de continuar. Execute-o da mesma forma que executaria o comando neste agente/sessão (a invocação pode diferir do identificador literal `{command}` mostrado acima; por exemplo, um agente no modo skills o executa como `/skill:speckit-...` ou `$speckit-...`). Apenas emitir o bloco não executa o hook.
- Se nenhum hook estiver registrado ou `.specify/extensions.yml` não existir, ignore silenciosamente.

## Objetivo

Identifique inconsistências, duplicações, ambiguidades e itens insuficientemente especificados nos três artefatos principais (`spec.md`, `plan.md`, `tasks.md`) antes da implementação. Este comando DEVE ser executado somente depois que `/speckit-tasks` tiver produzido com sucesso um `tasks.md` completo.

## Restrições de operação

**ESTRITAMENTE SOMENTE LEITURA**: **Não** modifique nenhum arquivo. Emita um relatório de análise estruturado. Ofereça um plano opcional de correção (o usuário deve aprová-lo explicitamente antes que quaisquer comandos de edição subsequentes sejam invocados manualmente).

**Autoridade da constituição**: A constituição do projeto (`.specify/memory/constitution.md`) é **inegociável** no escopo desta análise. Conflitos com a constituição são automaticamente CRÍTICOS e exigem ajustes na especificação, no plano ou nas tarefas — não a diluição, reinterpretação nem o descumprimento silencioso do princípio. Se um princípio precisar ser alterado, isso deverá ocorrer em uma atualização separada e explícita da constituição, fora de `/speckit-analyze`.

## Etapas de execução

### 1. Inicializar o contexto da análise

Execute `.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireSpec -RequireTasks -IncludeTasks` uma vez a partir da raiz do repositório e analise o JSON para obter FEATURE_DIR e AVAILABLE_DOCS. Derive os caminhos absolutos:

- SPEC = FEATURE_DIR/spec.md
- PLAN = FEATURE_DIR/plan.md
- TASKS = FEATURE_DIR/tasks.md

Aborte com uma mensagem de erro se algum arquivo obrigatório estiver ausente (instrua o usuário a executar o comando de pré-requisito que falta).
Para apóstrofos em argumentos como "I'm Groot", use a sintaxe de escape, por exemplo: `I'\''m Groot` (ou aspas duplas, se possível: "I'm Groot").

### 2. Carregar os artefatos (divulgação progressiva)

Carregue apenas o contexto mínimo necessário de cada artefato:

**De spec.md:**

- Visão geral/Contexto
- Requisitos funcionais
- Critérios de sucesso (resultados mensuráveis — por exemplo, desempenho, segurança, disponibilidade, sucesso do usuário, impacto nos negócios)
- Histórias de usuário
- Casos extremos (se houver)

**De plan.md:**

- Arquitetura/escolhas de stack
- Referências ao modelo de dados
- Fases
- Restrições técnicas

**De tasks.md:**

- IDs das tarefas
- Descrições
- Agrupamento por fase
- Marcadores de paralelismo [P]
- Caminhos de arquivos referenciados

**Da constituição:**

- Carregue `.specify/memory/constitution.md` para validar os princípios.

### 3. Construir modelos semânticos

Crie representações internas (não inclua os artefatos brutos na saída):

- **Inventário de requisitos**: Para cada requisito funcional (FR-###) e critério de sucesso (SC-###), registre uma chave estável. Use o identificador explícito FR-/SC- como chave primária quando presente e, opcionalmente, derive também um slug de frase imperativa para facilitar a leitura (por exemplo, "O usuário pode enviar um arquivo" → `user-can-upload-file`). Inclua somente os critérios de sucesso que exigem trabalho implementável (por exemplo, infraestrutura de teste de carga, ferramentas de auditoria de segurança) e exclua métricas de resultado pós-lançamento e KPIs de negócios (por exemplo, "Reduzir os chamados de suporte em 50%").
- **Inventário de histórias de usuário/ações**: Ações discretas do usuário com critérios de aceitação.
- **Mapeamento da cobertura das tarefas**: Associe cada tarefa a um ou mais requisitos ou histórias (inferindo por palavras-chave/referências explícitas, como IDs ou frases-chave).
- **Conjunto de regras da constituição**: Extraia os nomes dos princípios e as declarações normativas MUST/SHOULD.

### 4. Etapas de detecção (análise eficiente em tokens)

Concentre-se em achados de alto sinal. Limite o total a 50 achados; agregue os demais no resumo de excedentes.

#### A. Detecção de duplicações

- Identifique requisitos quase duplicados.
- Marque formulações de qualidade inferior para consolidação.

#### B. Detecção de ambiguidades

- Sinalize adjetivos vagos (rápido, escalável, seguro, intuitivo, robusto) sem critérios mensuráveis.
- Sinalize marcadores não resolvidos (TODO, TKTK, ???, `<placeholder>` etc.).

#### C. Especificação insuficiente

- Requisitos com verbos, mas sem objeto ou resultado mensurável.
- Histórias de usuário sem alinhamento com critérios de aceitação.
- Tarefas que referenciam arquivos ou componentes não definidos na especificação/no plano.

#### D. Alinhamento com a constituição

- Qualquer requisito ou elemento do plano em conflito com um princípio MUST.
- Seções ou gates de qualidade exigidos pela constituição que estejam ausentes.

#### E. Lacunas de cobertura

- Requisitos sem tarefas associadas.
- Tarefas sem requisito/história correspondente.
- Critérios de sucesso que exigem trabalho implementável (desempenho, segurança, disponibilidade) e não estão refletidos nas tarefas.

#### F. Inconsistências

- Variações terminológicas (mesmo conceito com nomes diferentes nos arquivos).
- Entidades de dados referenciadas no plano, mas ausentes da especificação (ou vice-versa).
- Contradições na ordem das tarefas (por exemplo, tarefas de integração antes das tarefas de configuração fundamental, sem nota de dependência).
- Requisitos conflitantes (por exemplo, um exige Next.js e outro especifica Vue).

### 5. Atribuir severidade

Use esta heurística para priorizar os achados:

- **CRITICAL**: Viola um MUST da constituição, falta um artefato central da especificação ou há requisito sem cobertura que bloqueia a funcionalidade básica.
- **HIGH**: Requisito duplicado ou conflitante, atributo de segurança/desempenho ambíguo ou critério de aceitação não testável.
- **MEDIUM**: Variação terminológica, falta de cobertura de tarefas não funcionais ou caso extremo insuficientemente especificado.
- **LOW**: Melhorias de estilo/formulação ou redundância menor que não afeta a ordem de execução.

### 6. Produzir um relatório de análise compacto

Emita um relatório Markdown (sem gravar arquivos) com a seguinte estrutura:

## Relatório de análise da especificação

| ID | Categoria | Severidade | Local(is) | Resumo | Recomendação |
|----|----------|----------|-------------|---------|----------------|
| A1 | Duplicação | HIGH | spec.md:L120-134 | Dois requisitos semelhantes ... | Una as formulações; mantenha a versão mais clara |

(Adicione uma linha por achado; gere IDs estáveis prefixados pela inicial da categoria.)

**Tabela de resumo da cobertura:**

| Chave do requisito | Tem tarefa? | IDs das tarefas | Observações |
|-----------------|-----------|----------|-------|

**Problemas de alinhamento com a constituição:** (se houver)

**Tarefas sem mapeamento:** (se houver)

**Métricas:**

- Total de requisitos
- Total de tarefas
- Cobertura % (requisitos com >=1 tarefa)
- Quantidade de ambiguidades
- Quantidade de duplicações
- Quantidade de problemas críticos

### 7. Informar as próximas ações

Ao final do relatório, emita um bloco conciso de Próximas ações:

- Se houver problemas CRÍTICOS: Recomende resolvê-los antes de `/speckit-implement`.
- Se houver somente problemas BAIXOS/MÉDIOS: O usuário pode prosseguir, mas forneça sugestões de melhoria.
- Forneça sugestões explícitas de comandos, por exemplo: "Execute /speckit-specify com refinamentos", "Execute /speckit-plan para ajustar a arquitetura", "Edite tasks.md manualmente para adicionar cobertura de 'performance-metrics'".

### 8. Oferecer correções

Pergunte ao usuário: "Você gostaria que eu sugerisse edições concretas para corrigir os N principais problemas?" (NÃO as aplique automaticamente.)

### 9. Verificar os hooks de extensão

Após o relatório, verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.after_analyze`.
- Se o YAML não puder ser analisado ou for inválido, não ignore isso silenciosamente: informe ao usuário que não foi possível ler `.specify/extensions.yml` (inclua o erro do analisador) e que nenhum hook foi verificado, inclusive os hooks obrigatórios (`optional: false`) registrados nele; em seguida, prossiga normalmente.
- Filtre os hooks em que `enabled` esteja explicitamente definido como `false`. Considere habilitados por padrão os hooks sem o campo `enabled`.
- Para cada hook restante, **não** tente interpretar nem avaliar expressões `condition`:
  - Se o hook não tiver o campo `condition`, ou se ele for nulo/vazio, considere-o executável.
  - Se o hook definir um `condition` não vazio, ignore-o e deixe a avaliação da condição para a implementação de HookExecutor.
- Ao construir invocações de comandos a partir dos nomes de hooks, substitua pontos (`.`) por hífens (`-`). Por exemplo, `speckit.git.commit` → `/speckit-git-commit`.
- Para cada hook executável, emita o seguinte conforme seu indicador `optional`:
  - **Hook opcional** (`optional: true`):
    ```
    ## Hooks de extensão

    **Hook opcional**: {extension}
    Comando: `/{command}`
    Descrição: {description}

    Solicitação: {prompt}
    Para executar: `/{command}`
    ```
  - **Hook obrigatório** (`optional: false`):
    ```
    ## Hooks de extensão

    **Hook automático**: {extension}
    Executando: `/{command}`
    EXECUTE_COMMAND: {command}
    ```
    Depois de emitir o bloco acima, você DEVE invocar o hook de fato e aguardar sua conclusão antes de continuar. Execute-o da mesma forma que executaria o comando neste agente/sessão (a invocação pode diferir do identificador literal `{command}` mostrado acima; por exemplo, um agente no modo skills o executa como `/skill:speckit-...` ou `$speckit-...`). Apenas emitir o bloco não executa o hook.
- Se nenhum hook estiver registrado ou `.specify/extensions.yml` não existir, ignore silenciosamente.

## Princípios de operação

### Eficiência de contexto

- **Mínimo de tokens de alto sinal**: Concentre-se em achados acionáveis, não em documentação exaustiva.
- **Divulgação progressiva**: Carregue os artefatos incrementalmente; não despeje todo o conteúdo na análise.
- **Saída eficiente em tokens**: Limite a tabela de achados a 50 linhas; resuma o excedente.
- **Resultados determinísticos**: Reexecuções sem alterações devem produzir IDs e contagens consistentes.

### Diretrizes de análise

- **NUNCA modifique arquivos** (esta análise é somente leitura).
- **NUNCA invente seções ausentes** (se não existirem, informe isso com precisão).
- **Priorize violações da constituição** (elas são sempre CRITICAL).
- **Prefira exemplos a regras exaustivas** (cite casos específicos, não padrões genéricos).
- **Relate a ausência de problemas de forma clara** (emita um relatório de sucesso com estatísticas de cobertura).

## Contexto

$ARGUMENTS
