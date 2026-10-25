---
name: "speckit-plan"
description: "Execute o fluxo de planejamento da implementação usando o modelo de plano para gerar artefatos de projeto."
compatibility: "Requer estrutura de projeto spec-kit com diretório .specify/"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/plan.md"
---


## Entrada do usuário

```text
$ARGUMENTS
```

Você **DEVE** considerar a entrada do usuário antes de prosseguir (se não estiver vazia).

## Verificações pré-execução

**Verifique os hooks de extensão (antes do planejamento)**:
- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.before_plan`
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

1. **Configuração**: execute `.specify/scripts/powershell/setup-plan.ps1 -Json` a partir da raiz do repositório e analise o JSON para obter FEATURE_SPEC, IMPL_PLAN, FEATURE_DIR e BRANCH. Para aspas simples em argumentos como "I'm Groot", use a sintaxe de escape: por exemplo, 'I'\''m Groot' (ou aspas duplas, se possível: "I'm Groot").

2. **Carregue o contexto**: leia FEATURE_SPEC e `.specify/memory/constitution.md`. Carregue o modelo IMPL_PLAN (já copiado).

3. **Execute o fluxo do plano**: siga a estrutura do modelo IMPL_PLAN para:
   - Preencher o Contexto técnico (marque incógnitas como "NEEDS CLARIFICATION")
   - Preencher a seção `Constitution Check` com base na constituição
   - Avaliar os critérios de aprovação (ERRO se houver violações sem justificativa)
   - Fase 0: gerar research.md (resolver todas as ocorrências de NEEDS CLARIFICATION)
   - Fase 1: gerar data-model.md, contracts/ e quickstart.md
   - Reavaliar a seção `Constitution Check` após o projeto

## Hooks obrigatórios pós-execução

**Você DEVE concluir esta seção antes de informar ao usuário que terminou.**

Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se não existir ou se nenhum hook estiver registrado em `hooks.after_plan`, avance para o Relatório de conclusão.
- Se existir, leia-o e procure entradas na chave `hooks.after_plan`.
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

O comando termina após o projeto da Fase 1. Informe a branch, o caminho de IMPL_PLAN e os artefatos gerados.

## Fases

### Fase 0: Roteiro e pesquisa

1. **Extraia as incógnitas do Contexto técnico** acima:
   - Para cada NEEDS CLARIFICATION → tarefa de pesquisa
   - Para cada dependência → tarefa sobre práticas recomendadas
   - Para cada integração → tarefa sobre padrões

2. **Gere e acione agentes de pesquisa**:

   ```text
   Para cada incógnita no Contexto técnico:
     Tarefa: "Pesquise {unknown} para {feature context}"
   Para cada escolha tecnológica:
     Tarefa: "Encontre práticas recomendadas para {tech} em {domain}"
   ```

3. **Consolide os resultados** em `research.md` usando o formato:
   - Decisão: [o que foi escolhido]
   - Justificativa: [por que foi escolhido]
   - Alternativas consideradas: [o que mais foi avaliado]

**Saída**: research.md com todas as ocorrências de NEEDS CLARIFICATION resolvidas

### Fase 1: Projeto e contratos

**Pré-requisitos:** `research.md` concluído

1. **Extraia as entidades da especificação do recurso** → `data-model.md`:
   - Nome da entidade, campos e relacionamentos
   - Regras de validação dos requisitos
   - Transições de estado, se aplicável

2. **Defina contratos de interface** (se o projeto tiver interfaces externas) → `/contracts/`:
   - Identifique quais interfaces o projeto expõe a usuários ou a outros sistemas
   - Documente o formato de contrato apropriado ao tipo de projeto
   - Exemplos: APIs públicas para bibliotecas, esquemas de comandos para ferramentas CLI, endpoints para serviços web, gramáticas para analisadores, contratos de interface do usuário para aplicações
   - Ignore se o projeto for puramente interno (scripts de build, ferramentas pontuais etc.)

3. **Crie um guia de validação de início rápido** → `quickstart.md`:
   - Documente cenários de validação executáveis que comprovem o funcionamento completo do recurso
   - Inclua pré-requisitos, comandos de configuração, comandos de teste/execução e resultados esperados
   - Use links ou referências aos detalhes dos contratos e do modelo de dados em vez de duplicá-los
   - Não inclua o código completo da implementação, corpos de modelos/serviços/controladores, migrações nem suítes completas de testes
   - Mantenha este artefato como um guia de validação/execução; os detalhes de implementação pertencem a `tasks.md` e à fase de implementação

**Saída**: data-model.md, /contracts/*, quickstart.md

## Regras principais

- Use caminhos absolutos para operações no sistema de arquivos; use caminhos relativos ao projeto nas referências da documentação
- ERRO em caso de reprovação nos critérios ou de esclarecimentos não resolvidos

## Critérios de conclusão

- [ ] Fluxo de planejamento executado e artefatos de projeto gerados
- [ ] Hooks de extensão acionados ou ignorados conforme as regras acima em Hooks obrigatórios pós-execução
- [ ] Conclusão informada ao usuário com a branch, o caminho do plano e os artefatos gerados
