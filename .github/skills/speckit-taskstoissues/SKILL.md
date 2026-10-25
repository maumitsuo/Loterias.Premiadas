---
name: "speckit-taskstoissues"
description: "Obsoleto: converta tarefas existentes em issues do GitHub acionáveis e ordenadas por dependências para o recurso, com base nos artefatos de projeto disponíveis."
compatibility: "Requer estrutura de projeto spec-kit com diretório .specify/"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/taskstoissues.md"
---


## Aviso de descontinuação

Você **DEVE exibir primeiro o seguinte aviso conciso**, como primeira etapa deste comando e antes das seções Entrada do usuário e Verificações pré-execução abaixo:

> ⚠️ `/speckit-taskstoissues` está obsoleto e será removido em uma versão secundária futura. Em vez disso, use a extensão `github` incluída: execute `specify extension add github` e, em seguida, invoque `/speckit-github-taskstoissues`.

Continue o fluxo existente sem alterações. Não instale nem habilite a extensão automaticamente, não pare e não execute o comando substituto.

## Entrada do usuário

```text
$ARGUMENTS
```

Você **DEVE** considerar a entrada do usuário antes de prosseguir (se não estiver vazia).

## Verificações pré-execução

**Verifique os hooks de extensão (antes da conversão de tarefas em issues)**:
- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.before_taskstoissues`
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

1. Execute `.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireTasks -IncludeTasks` a partir da raiz do repositório e analise FEATURE_DIR e a lista AVAILABLE_DOCS. Todos os caminhos devem ser absolutos. Para aspas simples em argumentos como "I'm Groot", use a sintaxe de escape: por exemplo, 'I'\''m Groot' (ou aspas duplas, se possível: "I'm Groot").
1. **SE EXISTIR**: carregue `.specify/memory/constitution.md` para consultar os princípios do projeto e as restrições de governança.
1. No script executado, extraia o caminho de **tasks**.
1. Obtenha o remoto Git executando:

```bash
git config --get remote.origin.url
```

> [!CAUTION]
> PROSSIGA PARA AS PRÓXIMAS ETAPAS SOMENTE SE O REMOTO FOR UMA URL DO GITHUB

1. **Busque issues existentes para evitar duplicatas**: antes de criar qualquer item, monte o conjunto de IDs das tarefas que serão processadas a partir de `tasks.md` (cada ID é um `T` seguido de **pelo menos** três dígitos, por exemplo, `T001` — `/speckit-converge` atribui novos IDs com `T{M+1:03d}`, que estabelece um mínimo, não um limite; portanto, quando um arquivo tiver mais de 999 tarefas, os IDs terão quatro dígitos ou mais). Em seguida, use a ferramenta `list_issues` do servidor MCP do GitHub para procurar issues que já cubram esses IDs. Não passe um valor para `state`, pois omiti-lo faz com que a ferramenta retorne issues abertas e fechadas. Solicite `perPage: 100` para reduzir o número de chamadas e, como a ferramenta usa paginação baseada em cursor, solicite as páginas com o parâmetro `after` (usando o `endCursor` da resposta anterior). Para cada título de issue, compare-o com o padrão de ID de tarefa `\bT\d{3,}\b` (o `{3,}` aceita IDs com quatro ou mais dígitos — com `\d{3}`, um título que contenha `T1000` não corresponderia, pois o `\b` final não pode ocorrer entre dois dígitos; assim, essa tarefa não seria silenciosamente deduplicada nem criada. Os limites de palavra ainda impedem que um token como `ST001` corresponda e garantem que toda a sequência numérica seja consumida, para que `T100` nunca corresponda dentro de `T1000`. Isso também reconhece títulos escritos como `T001 ...`, `T001: ...` ou `[T001] ...`) e, quando corresponder a um dos IDs de tarefa, marque esse ID como já associado a uma issue. Pare de paginar assim que todos os IDs das tarefas forem encontrados ou quando não houver mais páginas, para não continuar buscando todo o histórico de issues do repositório depois que todas as tarefas estiverem contabilizadas. Isso limita o número de chamadas em repositórios com históricos extensos de issues e ainda evita duplicatas quando o comando é executado novamente após a regeneração de `tasks.md` ou a reinvocação da skill.
1. Para cada tarefa da lista, use o servidor MCP do GitHub para criar uma issue no repositório correspondente ao remoto Git. As linhas de tarefa em `tasks.md` começam com uma caixa de seleção Markdown; portanto, primeiro remova o prefixo `- [ ]` (e quaisquer marcadores `[P]` / `[US#]`) para recuperar o ID da tarefa e sua descrição. Crie a issue com um único título canônico no formato `T001: <description>`, com o ID escrito uma vez, seguido da descrição da tarefa (por exemplo, a linha `- [ ] T001 Criar estrutura do projeto` torna-se o título `T001: Criar estrutura do projeto`).
   - **Ignore** qualquer tarefa cujo ID já esteja no conjunto de issues existentes da etapa anterior e informe isso (por exemplo, `T001 já tem uma issue; ignorando`).
   - Crie issues somente para tarefas que ainda não tenham uma issue correspondente.

> [!CAUTION]
> EM NENHUMA CIRCUNSTÂNCIA CRIE ISSUES EM REPOSITÓRIOS QUE NÃO CORRESPONDAM À URL DO REMOTO

## Verificações pós-execução

**Verifique os hooks de extensão (após a conversão de tarefas em issues)**:
Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.after_taskstoissues`
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

    **Hook opcional**: {extension}
    Comando: `/{command}`
    Descrição: {description}

    Instrução: {prompt}
    Para executar: `/{command}`
    ```
  - **Hook obrigatório** (`optional: false`):
    ```
    ## Hooks de extensão

    **Hook automático**: {extension}
    Executando: `/{command}`
    EXECUTE_COMMAND: {command}
    ```
    Depois de emitir o bloco acima, você DEVE realmente invocar o hook e aguardar sua conclusão antes de continuar. Execute-o da mesma forma que executaria o comando por conta própria neste agente/sessão (a invocação pode ser diferente do identificador literal `{command}` mostrado acima; por exemplo, um agente no modo de skills o executa como `/skill:speckit-...` ou `$speckit-...`). Emitir apenas o bloco não executa o hook.
- Se nenhum hook estiver registrado ou `.specify/extensions.yml` não existir, ignore silenciosamente
