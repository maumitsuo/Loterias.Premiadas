---
name: "speckit-tasks"
description: "Gere um tasks.md acionável e ordenado por dependências para o recurso, com base nos artefatos de projeto disponíveis."
compatibility: "Requer estrutura de projeto spec-kit com diretório .specify/"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/tasks.md"
---


## Entrada do usuário

```text
$ARGUMENTS
```

Você **DEVE** considerar a entrada do usuário antes de prosseguir (se não estiver vazia).

## Verificações pré-execução

**Verifique os hooks de extensão (antes da geração das tarefas)**:
- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.before_tasks`
- Se o YAML não puder ser analisado ou for inválido, não ignore isso silenciosamente: informe ao usuário que não foi possível ler `.specify/extensions.yml` (inclua o erro do analisador) e que nenhum hook foi verificado, incluindo quaisquer hooks obrigatórios (`optional: false`) registrados nele; em seguida, continue normalmente
- Desconsidere hooks em que `enabled` seja explicitamente `false`. Considere habilitados por padrão os hooks sem o campo `enabled`.
- Para cada hook restante, **não** tente interpretar nem avaliar as expressões `condition` do hook:
  - Se o hook não tiver o campo `condition`, ou se ele for nulo/vazio, considere o hook executável
  - Se o hook definir uma `condition` não vazia, ignore o hook e deixe a avaliação da condição para a implementação de HookExecutor
- Ao construir invocações de comandos a partir dos nomes dos comandos dos hooks, substitua pontos (`.`) por hífens (`-`). Por exemplo, `speckit.git.commit` → `/speckit-git-commit`.
- Para cada hook executável, exiba o seguinte com base no sinalizador `optional`:
  - **Hook opcional** (`optional: true`):
    ```
    ## Hooks de extensão

    **Hook prévio opcional**: {extension}
    Comando: `/{command}`
    Descrição: {description}

    Instrução: {prompt}
    Para executar: `/{command}`
    ```
  - **Hook obrigatório** (`optional: false`):
    ```
    ## Hooks de extensão

    **Hook prévio automático**: {extension}
    Executando: `/{command}`
    EXECUTE_COMMAND: {command}

    Aguarde o resultado do comando do hook antes de prosseguir para o Roteiro.
    ```
    Depois de emitir o bloco acima, você DEVE realmente invocar o hook e aguardar sua conclusão antes de continuar. Execute-o da mesma forma que executaria o comando por conta própria neste agente/sessão (a invocação pode ser diferente do identificador literal `{command}` mostrado acima; por exemplo, um agente no modo de skills o executa como `/skill:speckit-...` ou `$speckit-...`). Emitir apenas o bloco não executa o hook.
- Se nenhum hook estiver registrado ou `.specify/extensions.yml` não existir, ignore silenciosamente

## Roteiro

1. **Configuração**: execute `.specify/scripts/powershell/setup-tasks.ps1 -Json` a partir da raiz do repositório e analise FEATURE_DIR, TASKS_TEMPLATE_CONTENT, TASKS_TEMPLATE e a lista AVAILABLE_DOCS. `FEATURE_DIR` e `TASKS_TEMPLATE` devem ser caminhos absolutos quando fornecidos. `AVAILABLE_DOCS` é uma lista de nomes de documentos/caminhos relativos disponíveis em FEATURE_DIR (por exemplo, `research.md` ou `contracts/`). Para aspas simples em argumentos como "I'm Groot", use a sintaxe de escape: por exemplo, 'I'\''m Groot' (ou aspas duplas, se possível: "I'm Groot").

2. **Carregue os documentos de projeto**: leia em FEATURE_DIR:
   - **Obrigatórios**: plan.md (pilha tecnológica, bibliotecas, estrutura), spec.md (histórias de usuário com prioridades)
   - **Opcionais**: data-model.md (entidades), contracts/ (contratos de interface), research.md (decisões), quickstart.md (cenários de teste)
   - **SE EXISTIR**: carregue `.specify/memory/constitution.md` para consultar os princípios do projeto e as restrições de governança
   - Observação: nem todos os projetos têm todos os documentos. Gere as tarefas com base no que estiver disponível.

3. **Execute o fluxo de geração de tarefas**:
   - Carregue plan.md e extraia a pilha tecnológica, as bibliotecas e a estrutura do projeto
   - Carregue spec.md e extraia as histórias de usuário com suas prioridades (P1, P2, P3 etc.)
   - Se data-model.md existir: extraia as entidades e associe-as às histórias de usuário
   - Se contracts/ existir: associe os contratos de interface às histórias de usuário
   - Se research.md existir: extraia as decisões para as tarefas de configuração
   - Gere tarefas organizadas por história de usuário (consulte as Regras de geração de tarefas abaixo)
   - Gere um grafo de dependências que mostre a ordem de conclusão das histórias de usuário
   - Crie exemplos de execução paralela para cada história de usuário
   - Valide a completude das tarefas (cada história de usuário deve conter todas as tarefas necessárias e poder ser testada de forma independente)

4. **Gere tasks.md**: use TASKS_TEMPLATE_CONTENT (da saída JSON acima) como estrutura. Para manter compatibilidade com scripts de configuração mais antigos que omitem TASKS_TEMPLATE_CONTENT, leia TASKS_TEMPLATE. Preencha com:
   - Nome correto do recurso de plan.md
   - Fase 1: tarefas de configuração (inicialização do projeto)
   - Fase 2: tarefas fundamentais (pré-requisitos bloqueadores para todas as histórias de usuário)
   - Fase 3+: uma fase por história de usuário (na ordem de prioridade de spec.md)
   - Cada fase inclui: objetivo da história, critérios de teste independentes, testes (se solicitados), tarefas de implementação
   - Fase final: refinamento e preocupações transversais
   - Todas as tarefas devem seguir o formato estrito de lista de verificação (consulte as Regras de geração de tarefas abaixo)
   - Caminhos claros para os arquivos de cada tarefa
   - Seção de dependências que mostra a ordem de conclusão das histórias
   - Exemplos de execução paralela por história
   - Seção de estratégia de implementação (MVP primeiro, entrega incremental)

## Hooks obrigatórios pós-execução

**Você DEVE concluir esta seção antes de informar ao usuário que terminou.**

Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se não existir ou se nenhum hook estiver registrado em `hooks.after_tasks`, avance para o Relatório de conclusão.
- Se existir, leia-o e procure entradas na chave `hooks.after_tasks`.
- Se o YAML não puder ser analisado ou for inválido, não ignore isso silenciosamente: informe ao usuário que não foi possível ler `.specify/extensions.yml` (inclua o erro do analisador) e que nenhum hook foi verificado, incluindo quaisquer hooks obrigatórios (`optional: false`) registrados nele; em seguida, avance para o Relatório de conclusão.
- Desconsidere hooks em que `enabled` seja explicitamente `false`. Considere habilitados por padrão os hooks sem o campo `enabled`.
- Para cada hook restante, **não** tente interpretar nem avaliar as expressões `condition` do hook:
  - Se o hook não tiver o campo `condition`, ou se ele for nulo/vazio, considere o hook executável
  - Se o hook definir uma `condition` não vazia, ignore o hook e deixe a avaliação da condição para a implementação de HookExecutor
- Ao construir invocações de comandos a partir dos nomes dos comandos dos hooks, substitua pontos (`.`) por hífens (`-`). Por exemplo, `speckit.git.commit` → `/speckit-git-commit`.
- Para cada hook executável, exiba o seguinte com base no sinalizador `optional`:
  - **Hook obrigatório** (`optional: false`) — **você DEVE emitir `EXECUTE_COMMAND:` para cada hook obrigatório**:
    ```
    ## Hooks de extensão

    **Hook automático**: {extension}
    Executando: `/{command}`
    EXECUTE_COMMAND: {command}
    ```
    Depois de emitir o bloco acima, você DEVE realmente invocar o hook e aguardar sua conclusão antes de continuar. Execute-o da mesma forma que executaria o comando por conta própria neste agente/sessão (a invocação pode ser diferente do identificador literal `{command}` mostrado acima; por exemplo, um agente no modo de skills o executa como `/skill:speckit-...` ou `$speckit-...`). Emitir apenas o bloco não executa o hook.
  - **Hook opcional** (`optional: true`):
    ```
    ## Hooks de extensão

    **Hook opcional**: {extension}
    Comando: `/{command}`
    Descrição: {description}

    Instrução: {prompt}
    Para executar: `/{command}`
    ```

## Relatório de conclusão

Informe o caminho do tasks.md gerado e um resumo com:
- Número total de tarefas
- Número de tarefas por história de usuário
- Oportunidades de paralelismo identificadas
- Critérios de teste independentes para cada história
- Escopo sugerido para o MVP (normalmente apenas a História de usuário 1)
- Validação do formato: confirme que TODAS as tarefas seguem o formato da lista de verificação (caixa de seleção, ID, rótulos, caminhos de arquivos)

Contexto para a geração das tarefas: $ARGUMENTS

O tasks.md deve estar pronto para execução imediata — cada tarefa deve ser específica o suficiente para que um LLM possa concluí-la sem contexto adicional.

## Regras de geração de tarefas

**CRÍTICO**: as tarefas DEVEM ser organizadas por história de usuário para permitir implementação e teste independentes.

**Testes são OPCIONAIS**: gere tarefas de teste somente se isso for solicitado explicitamente na especificação do recurso ou se o usuário solicitar a abordagem TDD.

### Formato da lista de verificação (OBRIGATÓRIO)

Todas as tarefas DEVEM seguir rigorosamente este formato:

```text
- [ ] [TaskID] [P?] [Story?] Descrição com caminho do arquivo
```

**Componentes do formato**:

1. **Caixa de seleção**: SEMPRE comece com `- [ ]` (caixa de seleção Markdown)
2. **ID da tarefa**: número sequencial (T001, T002, T003...) na ordem de execução
3. **Marcador [P]**: inclua SOMENTE se a tarefa puder ser paralelizada (arquivos diferentes, sem dependências de tarefas incompletas)
4. **Rótulo [Story]**: OBRIGATÓRIO somente para tarefas das fases de história de usuário
   - Formato: [US1], [US2], [US3] etc. (corresponde às histórias de usuário de spec.md)
   - Fase de configuração: SEM rótulo de história
   - Fase fundamental: SEM rótulo de história
   - Fases de história de usuário: DEVEM ter rótulo de história
   - Fase de refinamento: SEM rótulo de história
5. **Descrição**: ação clara com o caminho exato do arquivo

**Exemplos**:

- ✅ CORRETO: `- [ ] T001 Criar estrutura do projeto conforme o plano de implementação`
- ✅ CORRETO: `- [ ] T005 [P] Implementar middleware de autenticação em src/middleware/auth.py`
- ✅ CORRETO: `- [ ] T012 [P] [US1] Criar modelo User em src/models/user.py`
- ✅ CORRETO: `- [ ] T014 [US1] Implementar UserService em src/services/user_service.py`
- ❌ INCORRETO: `- [ ] Criar modelo User` (faltam o ID e o rótulo da história)
- ❌ INCORRETO: `T001 [US1] Criar modelo` (falta a caixa de seleção)
- ❌ INCORRETO: `- [ ] [US1] Criar modelo User` (falta o ID da tarefa)
- ❌ INCORRETO: `- [ ] T001 [US1] Criar modelo` (falta o caminho do arquivo)

### Organização das tarefas

1. **A partir das histórias de usuário (spec.md)** — ORGANIZAÇÃO PRINCIPAL:
   - Cada história de usuário (P1, P2, P3...) recebe sua própria fase
   - Associe todos os componentes relacionados à história correspondente:
     - Modelos necessários para essa história
     - Serviços necessários para essa história
     - Interfaces/UI necessárias para essa história
     - Se houver solicitação de testes: testes específicos para essa história
   - Marque as dependências entre histórias (a maioria deve ser independente)

2. **A partir dos contratos**:
   - Associe cada contrato de interface → à história de usuário atendida por ele
   - Se houver solicitação de testes: para cada contrato de interface → tarefa de teste de contrato [P] antes da implementação na fase da história correspondente

3. **A partir do modelo de dados**:
   - Associe cada entidade à(s) história(s) de usuário que precisa(m) dela
   - Se uma entidade atender a várias histórias: coloque-a na história mais antiga ou na fase de configuração
   - Relacionamentos → tarefas da camada de serviço na fase da história apropriada
   - Para cada campo com restrições em data-model.md (comprimento máximo, anulável/obrigatório, valores de enumeração, regras de validação), reproduza a restrição literalmente na descrição da tarefa para que ela não fique a critério de quem implementará

4. **A partir da configuração/infraestrutura**:
   - Infraestrutura compartilhada → fase de configuração (Fase 1)
   - Tarefas fundamentais/bloqueadoras → fase fundamental (Fase 2)
   - Configuração específica de uma história → dentro da fase dessa história

### Estrutura das fases

- **Fase 1**: Configuração (inicialização do projeto)
- **Fase 2**: Fundamental (pré-requisitos bloqueadores — DEVEM ser concluídos antes das histórias de usuário)
- **Fase 3+**: histórias de usuário na ordem de prioridade (P1, P2, P3...)
  - Dentro de cada história: testes (se solicitados) → modelos → serviços → endpoints → integração
  - Cada fase deve ser um incremento completo e testável de forma independente
- **Fase final**: refinamento e preocupações transversais

## Critérios de conclusão

- [ ] tasks.md gerado com todas as fases, IDs de tarefas e caminhos de arquivos
- [ ] Hooks de extensão acionados ou ignorados conforme as regras acima em Hooks obrigatórios pós-execução
- [ ] Conclusão informada ao usuário com a contagem de tarefas, a divisão por histórias e o escopo do MVP
