---
name: "speckit-specify"
description: "Crie ou atualize a especificação do recurso a partir de uma descrição em linguagem natural."
compatibility: "Requer estrutura de projeto spec-kit com diretório .specify/"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/specify.md"
---


## Entrada do usuário

```text
$ARGUMENTS
```

Você **DEVE** considerar a entrada do usuário antes de prosseguir (se não estiver vazia).

## Verificações pré-execução

**Verifique os hooks de extensão (antes da especificação)**:
- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.before_specify`
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

O texto digitado pelo usuário após `/speckit-specify` na mensagem que acionou o comando **é** a descrição do recurso. Considere que esse texto está sempre disponível nesta conversa, mesmo que `$ARGUMENTS` apareça literalmente abaixo. Não peça ao usuário que o repita, a menos que ele tenha fornecido um comando vazio.

Com base nessa descrição do recurso, faça o seguinte:

1. **Gere um nome curto e conciso** (2 a 4 palavras) para o recurso:
   - Analise a descrição do recurso e extraia as palavras-chave mais significativas
   - Crie um nome curto de 2 a 4 palavras que capture a essência do recurso
   - Sempre que possível, use o formato ação-substantivo (por exemplo, "adicionar-autenticacao-usuario", "corrigir-erro-pagamento")
   - Preserve termos técnicos e siglas (OAuth2, API, JWT etc.)
   - Seja conciso, mas suficientemente descritivo para entender o recurso rapidamente
   - Exemplos:
     - "Quero adicionar autenticação de usuário" → "autenticacao-usuario"
     - "Implementar integração OAuth2 para a API" → "integracao-oauth2-api"
     - "Criar um painel para análise de dados" → "painel-analise-dados"
     - "Corrigir erro de tempo limite no processamento de pagamentos" → "corrigir-tempo-limite-pagamento"

2. **Criação da branch** (opcional, por meio de hook):

   Se um hook `before_specify` tiver sido executado com sucesso nas Verificações pré-execução acima, ele terá criado ou alternado para uma branch git e gerado um JSON contendo `BRANCH_NAME` e `FEATURE_NUM`. Anote esses valores para referência, mas o nome da branch **não** determina o nome do diretório da especificação.

   Se o usuário tiver fornecido explicitamente `GIT_BRANCH_NAME`, passe esse valor ao hook para que o script da branch use exatamente esse nome (sem gerar prefixos ou sufixos).

3. **Crie o diretório do recurso da especificação**:

   As especificações ficam no diretório padrão `specs/`, a menos que o usuário forneça explicitamente `SPECIFY_FEATURE_DIRECTORY`.

   **Ordem de resolução de `SPECIFY_FEATURE_DIRECTORY`**:
   1. Se o usuário tiver fornecido explicitamente `SPECIFY_FEATURE_DIRECTORY` (por exemplo, por variável de ambiente, argumento ou configuração), use-o como está
   2. Caso contrário, gere-o automaticamente em `specs/`:
      - Verifique `.specify/init-options.json` para encontrar `feature_numbering` (preferencial) ou `branch_numbering` (obsoleto, somente para migração — será removido em uma versão futura)
      - Se for `"timestamp"`: o prefixo é `YYYYMMDD-HHMMSS` (data e hora atuais)
      - Se for `"sequential"` ou estiver ausente: o prefixo é `NNN` (próximo número de 3 dígitos disponível após examinar os diretórios existentes em `specs/`)
      - Construa o nome do diretório: `<prefix>-<short-name>` (por exemplo, `003-user-auth` ou `20260319-143022-user-auth`)
      - Defina `SPECIFY_FEATURE_DIRECTORY` como `specs/<directory-name>`
      - Se `branch_numbering` tiver sido usado (e `feature_numbering` estiver ausente), emita um aviso de uma linha: "⚠️ `branch_numbering` em init-options.json está obsoleto. Renomeie para `feature_numbering`."

   **Crie o diretório e o arquivo de especificação**:
   - `mkdir -p SPECIFY_FEATURE_DIRECTORY`
   - Resolva o `spec-template` ativo por meio da pilha de resolução de predefinições/modelos do Spec Kit (equivalente a `specify preset resolve spec-template`)
   - Copie o arquivo `spec-template` resolvido para `SPECIFY_FEATURE_DIRECTORY/spec.md` como ponto de partida
   - Defina `SPEC_FILE` como `SPECIFY_FEATURE_DIRECTORY/spec.md`
   - Persista o caminho resolvido em `.specify/feature.json`:
     ```json
     {
       "feature_directory": "<resolved feature dir>"
     }
     ```
     Grave o valor real do caminho do diretório resolvido (por exemplo, `specs/003-user-auth`), não a string literal `SPECIFY_FEATURE_DIRECTORY`.
     Isso permite que comandos subsequentes (`/speckit-plan`, `/speckit-tasks` etc.) localizem o diretório do recurso sem depender das convenções de nomes de branches git.

   **IMPORTANTE**:
   - Você deve criar apenas um recurso por invocação de `/speckit-specify`
   - O nome do diretório da especificação e o nome da branch git são independentes — podem ser iguais, mas essa é uma escolha do usuário
   - O diretório e o arquivo da especificação são sempre criados por este comando, nunca pelo hook

4. Carregue o arquivo `spec-template` ativo resolvido para entender as seções obrigatórias.

5. **SE EXISTIR**: carregue `.specify/memory/constitution.md` para consultar os princípios do projeto e as restrições de governança.

6. Siga este fluxo de execução:
    1. Analise a descrição do usuário nos argumentos
       Se estiver vazia: ERRO "Nenhuma descrição do recurso foi fornecida"
    2. Extraia os conceitos principais da descrição
       Identifique: atores, ações, dados e restrições
    3. Para aspectos pouco claros:
       - Faça suposições fundamentadas no contexto e nos padrões do setor
       - Use [NEEDS CLARIFICATION: specific question] somente se:
         - A escolha afetar significativamente o escopo do recurso ou a experiência do usuário
         - Houver várias interpretações razoáveis com implicações diferentes
         - Não houver uma opção padrão razoável
       - **LIMITE: no máximo 3 marcadores [NEEDS CLARIFICATION] no total**
       - Priorize os esclarecimentos por impacto: escopo > segurança/privacidade > experiência do usuário > detalhes técnicos
    4. Preencha a seção User Scenarios & Testing
       Se não houver um fluxo claro do usuário: ERRO "Não é possível determinar os cenários do usuário"
    5. Gere os requisitos funcionais
       Cada requisito deve ser testável
       Use padrões razoáveis para detalhes não especificados (documente as suposições na seção Assumptions)
    6. Defina os critérios de sucesso
       Crie resultados mensuráveis e independentes de tecnologia
       Inclua métricas quantitativas (tempo, desempenho, volume) e medidas qualitativas (satisfação do usuário, conclusão de tarefas)
       Cada critério deve ser verificável sem detalhes de implementação
    7. Identifique as entidades principais (se houver dados envolvidos)
    8. Retorne: SUCESSO (especificação pronta para planejamento)

7. Grave a especificação em SPEC_FILE usando a estrutura do modelo, substituindo os placeholders por detalhes concretos derivados da descrição do recurso (argumentos), preservando a ordem das seções e os títulos.

8. **Validação da qualidade da especificação**: após gravar a especificação inicial, valide-a com base nos critérios de qualidade:

   a. **Crie a lista de verificação da qualidade da especificação**: gere um arquivo de lista de verificação em `SPECIFY_FEATURE_DIRECTORY/checklists/requirements.md` usando a estrutura do modelo de lista com estes itens de validação:

      ```markdown
      # Lista de verificação de qualidade da especificação: [FEATURE NAME]

      **Objetivo**: validar a completude e a qualidade da especificação antes de prosseguir para o planejamento
      **Criada em**: [DATE]
      **Recurso**: [Link to spec.md]

      ## Qualidade do conteúdo

      - [ ] Sem detalhes de implementação (linguagens, frameworks, APIs)
      - [ ] Foco no valor para o usuário e nas necessidades de negócio
      - [ ] Texto voltado a partes interessadas não técnicas
      - [ ] Todas as seções obrigatórias preenchidas

      ## Completude dos requisitos

      - [ ] Não há marcadores [NEEDS CLARIFICATION] restantes
      - [ ] Requisitos testáveis e inequívocos
      - [ ] Critérios de sucesso mensuráveis
      - [ ] Critérios de sucesso independentes de tecnologia (sem detalhes de implementação)
      - [ ] Todos os cenários de aceitação definidos
      - [ ] Casos extremos identificados
      - [ ] Escopo claramente delimitado
      - [ ] Dependências e suposições identificadas

      ## Preparação do recurso

      - [ ] Todos os requisitos funcionais têm critérios de aceitação claros
      - [ ] Cenários do usuário abrangem os fluxos principais
      - [ ] O recurso atende aos resultados mensuráveis definidos nos critérios de sucesso
      - [ ] Nenhum detalhe de implementação aparece na especificação

      ## Observações

      - Itens marcados como incompletos exigem atualizações na especificação antes de `/speckit-clarify` ou `/speckit-plan`
      ```

   b. **Execute a validação**: revise a especificação com base em cada item da lista:
      - Determine se cada item foi aprovado ou reprovado
      - Documente os problemas específicos encontrados (cite as seções relevantes da especificação)

   c. **Trate os resultados da validação**:

      - **Se todos os itens forem aprovados**: marque a lista como concluída e prossiga para a seção Hooks obrigatórios pós-execução

      - **Se houver itens reprovados (exceto [NEEDS CLARIFICATION])**:
        1. Liste os itens reprovados e os problemas específicos
        2. Atualize a especificação para resolver cada problema
        3. Execute novamente a validação até que todos os itens sejam aprovados (no máximo 3 iterações)
        4. Se ainda houver reprovações após 3 iterações, documente os problemas restantes nas observações da lista e avise o usuário

      - **Se ainda houver marcadores [NEEDS CLARIFICATION]**:
        1. Extraia da especificação todos os marcadores [NEEDS CLARIFICATION: ...]
        2. **VERIFIQUE O LIMITE**: se houver mais de 3 marcadores, mantenha somente os 3 mais críticos (por impacto no escopo/segurança/UX) e faça suposições fundamentadas para os demais
        3. Para cada esclarecimento necessário (no máximo 3), apresente opções ao usuário neste formato:

           ```markdown
           ## Pergunta [N]: [Topic]

           **Contexto**: [Quote relevant spec section]

           **O que precisamos saber**: [Specific question from NEEDS CLARIFICATION marker]

           **Respostas sugeridas**:

           | Opção | Resposta | Implicações |
           |-------|----------|-------------|
           | A     | [First suggested answer] | [What this means for the feature] |
           | B     | [Second suggested answer] | [What this means for the feature] |
           | C     | [Third suggested answer] | [What this means for the feature] |
           | Personalizada | Forneça sua própria resposta | [Explain how to provide custom input] |

           **Sua escolha**: _[Wait for user response]_
           ```

        4. **CRÍTICO - Formatação da tabela**: garanta que as tabelas Markdown estejam formatadas corretamente:
           - Use espaçamento consistente e alinhe as barras verticais
           - Cada célula deve ter espaços ao redor do conteúdo: `| Conteúdo |`, não `|Conteúdo|`
           - O separador do cabeçalho deve ter pelo menos 3 hifens: `|--------|`
           - Teste se a tabela é renderizada corretamente na visualização Markdown
        5. Numere as perguntas sequencialmente (Q1, Q2, Q3 — no máximo 3 no total)
        6. Apresente todas as perguntas juntas antes de aguardar as respostas
        7. Aguarde o usuário responder com as escolhas para todas as perguntas (por exemplo, "Q1: A, Q2: Personalizada - [detalhes], Q3: B")
        8. Atualize a especificação substituindo cada marcador [NEEDS CLARIFICATION] pela resposta escolhida ou fornecida pelo usuário
        9. Execute novamente a validação após resolver todos os esclarecimentos

   d. **Atualize a lista de verificação**: após cada iteração de validação, atualize o arquivo da lista com o status atual de aprovação/reprovação

## Hooks obrigatórios pós-execução

**Você DEVE concluir esta seção antes de informar ao usuário que terminou.**

Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se não existir ou se nenhum hook estiver registrado em `hooks.after_specify`, avance para o Relatório de conclusão.
- Se existir, leia-o e procure entradas na chave `hooks.after_specify`.
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

Informe a conclusão ao usuário com:
- `SPECIFY_FEATURE_DIRECTORY` — o caminho do diretório do recurso
- `SPEC_FILE` — o caminho do arquivo de especificação
- Resumo dos resultados da lista de verificação
- Preparação para a próxima fase (`/speckit-clarify` ou `/speckit-plan`)

**OBSERVAÇÃO:** a criação da branch é tratada pelo hook `before_specify` (extensão git). A criação do diretório e do arquivo da especificação é sempre tratada por este comando principal.

## Diretrizes rápidas

- Foque no **QUE** os usuários precisam e no **POR QUÊ**.
- Evite explicar COMO implementar (sem pilha tecnológica, APIs ou estrutura de código).
- Escreva para as partes interessadas do negócio, não para desenvolvedores.
- NÃO crie listas de verificação incorporadas à especificação. Isso será feito por um comando separado.

### Requisitos das seções

- **Seções obrigatórias**: devem ser preenchidas para cada recurso
- **Seções opcionais**: inclua somente quando forem relevantes para o recurso
- Quando uma seção não se aplicar, remova-a por completo (não a deixe como "N/A")

### Para geração por IA

Ao criar esta especificação com base em uma solicitação do usuário:

1. **Faça suposições fundamentadas**: use o contexto, os padrões do setor e padrões comuns para preencher lacunas
2. **Documente as suposições**: registre os padrões razoáveis na seção Assumptions
3. **Limite os esclarecimentos**: no máximo 3 marcadores [NEEDS CLARIFICATION] — use-os somente para decisões críticas que:
   - Afetem significativamente o escopo do recurso ou a experiência do usuário
   - Tenham várias interpretações razoáveis com implicações diferentes
   - Não tenham nenhuma opção padrão razoável
4. **Priorize os esclarecimentos**: escopo > segurança/privacidade > experiência do usuário > detalhes técnicos
5. **Pense como uma pessoa testadora**: todo requisito vago deve reprovar no item da lista "testável e inequívoco"
6. **Áreas comuns que precisam de esclarecimento** (somente se não houver uma opção padrão razoável):
   - Escopo e limites do recurso (incluir/excluir casos de uso específicos)
   - Tipos de usuários e permissões (se houver interpretações conflitantes possíveis)
   - Requisitos de segurança/conformidade (quando forem significativos do ponto de vista legal/financeiro)

**Exemplos de padrões razoáveis** (não pergunte sobre eles):

- Retenção de dados: práticas padrão do setor para o domínio
- Metas de desempenho: expectativas padrão de aplicativos web/móveis, salvo indicação em contrário
- Tratamento de erros: mensagens fáceis de entender, com alternativas apropriadas
- Método de autenticação: autenticação padrão baseada em sessão ou OAuth2 para aplicativos web
- Padrões de integração: use padrões apropriados ao projeto (REST/GraphQL para serviços web, chamadas de função para bibliotecas, argumentos CLI para ferramentas etc.)

### Diretrizes para os critérios de sucesso

Os critérios de sucesso devem ser:

1. **Mensuráveis**: incluam métricas específicas (tempo, porcentagem, contagem, taxa)
2. **Independentes de tecnologia**: não mencionem frameworks, linguagens, bancos de dados ou ferramentas
3. **Focados no usuário**: descrevam resultados da perspectiva do usuário/negócio, não os componentes internos do sistema
4. **Verificáveis**: possam ser testados/validados sem conhecer detalhes da implementação

**Bons exemplos**:

- "Os usuários conseguem concluir a finalização da compra em menos de 3 minutos"
- "O sistema comporta 10.000 usuários simultâneos"
- "95% das pesquisas retornam resultados em menos de 1 segundo"
- "A taxa de conclusão de tarefas aumenta em 40%"

**Exemplos ruins** (focados na implementação):

- "O tempo de resposta da API é inferior a 200 ms" (técnico demais; use "Os usuários veem os resultados instantaneamente")
- "O banco de dados consegue processar 1000 TPS" (detalhe de implementação; use uma métrica voltada ao usuário)
- "Os componentes React são renderizados com eficiência" (específico de framework)
- "A taxa de acertos do cache Redis é superior a 80%" (específico de tecnologia)

## Critérios de conclusão

- [ ] Especificação gravada em `SPEC_FILE` e validada com base na lista de verificação de qualidade
- [ ] Hooks de extensão acionados ou ignorados conforme as regras acima em Hooks obrigatórios pós-execução
- [ ] Conclusão informada ao usuário com o diretório do recurso, o caminho do arquivo de especificação e os resultados da lista de verificação
