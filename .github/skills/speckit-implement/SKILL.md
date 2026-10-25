---
name: "speckit-implement"
description: "Execute o plano de implementação processando e executando todas as tarefas definidas em tasks.md"
compatibility: "Requer estrutura de projeto spec-kit com diretório .specify/"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/implement.md"
---


## Entrada do usuário

```text
$ARGUMENTS
```

Você **DEVE** considerar a entrada do usuário antes de prosseguir (se não estiver vazia).

## Verificações pré-execução

**Verifique os hooks de extensão (antes da implementação)**:
- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.before_implement`
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

2. **Verifique o status das listas de verificação** (se FEATURE_DIR/checklists/ existir):
   - Trate os marcadores das listas como uma barreira somente de leitura: verifique o estado das caixas, informe o status e pergunte antes de prosseguir quando necessário; NÃO modifique arquivos nem marcadores das listas
   - `checklists/requirements.md` é a lista integrada de qualidade da especificação, mantida por `/speckit-specify` e `/speckit-clarify`; listas personalizadas geradas por `/speckit-checklist` são artefatos de revisão da qualidade dos requisitos pertencentes ao revisor
   - Nas listas personalizadas, `[x]` significa que o revisor determinou que o critério de qualidade dos requisitos foi atendido; isso NÃO significa que o trabalho de implementação foi concluído
   - Verifique todos os arquivos de lista de verificação no diretório checklists/
   - Para cada lista, conte:
     - Itens totais: todas as linhas correspondentes a `- [ ]`, `- [X]` ou `- [x]`
     - Itens marcados: linhas correspondentes a `- [X]` ou `- [x]`
     - Itens desmarcados: linhas correspondentes a `- [ ]`
   - Crie uma tabela de status:

     ```text
     | Lista de verificação | Total | Marcados | Desmarcados | Estado |
     |----------------------|-------|----------|-------------|--------|
     | ux.md     | 12    | 12      | 0         | ✓ APROVADO |
     | test.md   | 8     | 5       | 3         | ✗ REPROVADO |
     | security.md | 6   | 6       | 0         | ✓ APROVADO |
     ```

   - Calcule o status geral:
     - **APROVADO**: todas as listas têm 0 itens desmarcados
     - **REPROVADO**: uma ou mais listas têm itens desmarcados

   - **Se alguma lista tiver itens desmarcados**:
     - Exiba a tabela com as contagens de itens desmarcados
     - **PARE** e pergunte: "Algumas listas de verificação têm itens desmarcados. Deseja prosseguir com a implementação mesmo assim? (sim/não)"
     - Aguarde a resposta do usuário antes de continuar
     - Se o usuário disser "não", "aguarde" ou "pare", interrompa a execução
     - Se o usuário disser "sim", "prossiga" ou "continue", avance para a etapa 3

   - **Se todos os itens das listas estiverem marcados**:
     - Exiba a tabela indicando que todas as listas foram aprovadas
     - Avance automaticamente para a etapa 3

3. Carregue e analise o contexto da implementação:
   - **OBRIGATÓRIO**: leia tasks.md para obter a lista completa de tarefas e o plano de execução
   - **OBRIGATÓRIO**: leia plan.md para obter a pilha tecnológica, a arquitetura e a estrutura de arquivos
   - **SE EXISTIR**: leia data-model.md para conhecer as entidades e os relacionamentos
   - **SE EXISTIR**: leia contracts/ para consultar as especificações de API e os requisitos de teste
   - **SE EXISTIR**: leia research.md para conhecer as decisões técnicas e restrições
   - **SE EXISTIR**: leia .specify/memory/constitution.md para conhecer as restrições de governança
   - **SE EXISTIR**: leia quickstart.md para consultar os cenários de integração

4. **Verificação da configuração do projeto**:
   - **OBRIGATÓRIO**: crie/verifique arquivos ignore com base na configuração real do projeto:

   **Lógica de detecção e criação**:
   - Verifique se o comando a seguir é bem-sucedido para determinar se o repositório é um repositório git (crie/verifique .gitignore nesse caso):

     ```sh
     git rev-parse --git-dir 2>/dev/null
     ```

   - Verifique se Dockerfile* existe ou se Docker aparece em plan.md → crie/verifique .dockerignore
   - Verifique se .eslintrc* existe → crie/verifique .eslintignore
   - Verifique se eslint.config.* existe → garanta que as entradas `ignores` da configuração incluam os padrões necessários
   - Verifique se .prettierrc* existe → crie/verifique .prettierignore
   - Verifique se .npmrc ou package.json existe → crie/verifique .npmignore (se houver publicação)
   - Verifique se há arquivos do Terraform (*.tf) → crie/verifique .terraformignore
   - Verifique se .helmignore é necessário (há charts Helm) → crie/verifique .helmignore

   **Se o arquivo ignore já existir**: verifique se contém os padrões essenciais; acrescente somente os padrões críticos ausentes

   **Se o arquivo ignore estiver ausente**: crie-o com o conjunto completo de padrões para a tecnologia detectada

   **Padrões comuns por tecnologia** (da pilha tecnológica em plan.md):
   - **Node.js/JavaScript/TypeScript**: `node_modules/`, `dist/`, `build/`, `*.log`, `.env*`
   - **Python**: `__pycache__/`, `*.pyc`, `.venv/`, `venv/`, `dist/`, `*.egg-info/`
   - **Java**: `target/`, `*.class`, `*.jar`, `.gradle/`, `build/`
   - **C#/.NET**: `bin/`, `obj/`, `*.user`, `*.suo`, `packages/`
   - **Go**: `*.exe`, `*.test`, `vendor/`, `*.out`
   - **Ruby**: `.bundle/`, `log/`, `tmp/`, `*.gem`, `vendor/bundle/`
   - **PHP**: `vendor/`, `*.log`, `*.cache`, `*.env`
   - **Rust**: `target/`, `debug/`, `release/`, `*.rs.bk`, `*.rlib`, `*.prof*`, `.idea/`, `*.log`, `.env*`
   - **Kotlin**: `build/`, `out/`, `.gradle/`, `.idea/`, `*.class`, `*.jar`, `*.iml`, `*.log`, `.env*`
   - **C++**: `build/`, `bin/`, `obj/`, `out/`, `*.o`, `*.so`, `*.a`, `*.exe`, `*.dll`, `.idea/`, `*.log`, `.env*`
   - **C**: `build/`, `bin/`, `obj/`, `out/`, `*.o`, `*.a`, `*.so`, `*.exe`, `*.dll`, `autom4te.cache/`, `config.status`, `config.log`, `.idea/`, `*.log`, `.env*`
   - **Swift**: `.build/`, `DerivedData/`, `*.swiftpm/`, `Packages/`
   - **R**: `.Rproj.user/`, `.Rhistory`, `.RData`, `.Ruserdata`, `*.Rproj`, `packrat/`, `renv/`
   - **Universal**: `.DS_Store`, `Thumbs.db`, `*.tmp`, `*.swp`, `.vscode/`, `.idea/`

   **Padrões específicos de ferramentas**:
   - **Docker**: `node_modules/`, `.git/`, `Dockerfile*`, `.dockerignore`, `*.log*`, `.env*`, `coverage/`
   - **ESLint**: `node_modules/`, `dist/`, `build/`, `coverage/`, `*.min.js`
   - **Prettier**: `node_modules/`, `dist/`, `build/`, `coverage/`, `package-lock.json`, `yarn.lock`, `pnpm-lock.yaml`
   - **Terraform**: `.terraform/`, `*.tfstate*`, `*.tfvars`, `.terraform.lock.hcl`
   - **Kubernetes/k8s**: `*.secret.yaml`, `secrets/`, `.kube/`, `kubeconfig*`, `*.key`, `*.crt`

5. Analise a estrutura de tasks.md e extraia:
   - **Fases das tarefas**: Configuração, Testes, Núcleo, Integração, Refinamento
   - **Dependências entre tarefas**: regras de execução sequencial ou paralela
   - **Detalhes das tarefas**: ID, descrição, caminhos de arquivos, marcadores de paralelismo [P]
   - **Fluxo de execução**: requisitos de ordem e dependências

6. Execute a implementação seguindo o plano de tarefas:
   - **Execução fase por fase**: conclua cada fase antes de passar à seguinte
   - **Respeite as dependências**: execute tarefas sequenciais em ordem; tarefas paralelas [P] podem ser executadas juntas
   - **Siga a abordagem TDD**: execute tarefas de teste antes das tarefas de implementação correspondentes
   - **Coordenação por arquivos**: tarefas que afetam os mesmos arquivos devem ser executadas sequencialmente
   - **Pontos de verificação**: confirme a conclusão de cada fase antes de prosseguir

7. Regras de execução da implementação:
   - **Configuração primeiro**: inicialize a estrutura do projeto, as dependências e a configuração
   - **Testes antes do código**: escreva testes para contratos, entidades e cenários de integração, se necessário
   - **Desenvolvimento do núcleo**: implemente modelos, serviços, comandos CLI e endpoints
   - **Trabalho de integração**: conexões com bancos de dados, middleware, registros e serviços externos
   - **Refinamento e validação**: testes unitários, otimização de desempenho e documentação

8. Acompanhamento do progresso e tratamento de erros:
   - Informe o progresso após cada tarefa concluída
   - Interrompa a execução se qualquer tarefa não paralela falhar
   - Para tarefas paralelas [P], continue com as tarefas bem-sucedidas e informe as que falharam
   - Forneça mensagens de erro claras, com contexto para depuração
   - Sugira as próximas etapas se não for possível prosseguir com a implementação
   - **IMPORTANTE**: nas tarefas concluídas, marque a tarefa como [X] no arquivo de tarefas.

9. Validação da conclusão:
   - Verifique se todas as tarefas obrigatórias foram concluídas
   - Confira se os recursos implementados correspondem à especificação original
   - Valide se os testes passam e se a cobertura atende aos requisitos
   - Confirme se a implementação segue o plano técnico

Observação: este comando pressupõe que tasks.md contém uma divisão completa das tarefas. Se as tarefas estiverem incompletas ou ausentes, sugira executar `/speckit-tasks` primeiro para gerar novamente a lista de tarefas.

## Hooks obrigatórios pós-execução

**Você DEVE concluir esta seção antes de informar ao usuário que terminou.**

Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se não existir ou se nenhum hook estiver registrado em `hooks.after_implement`, avance para o Relatório de conclusão.
- Se existir, leia-o e procure entradas na chave `hooks.after_implement`.
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

Informe o status final com um resumo do trabalho concluído.

## Critérios de conclusão

- [ ] Todas as tarefas em tasks.md concluídas e marcadas como `[X]`
- [ ] Implementação validada com base na especificação, no plano e na cobertura de testes
- [ ] Hooks de extensão acionados ou ignorados conforme as regras acima em Hooks obrigatórios pós-execução
- [ ] Conclusão informada ao usuário com um resumo do trabalho concluído
