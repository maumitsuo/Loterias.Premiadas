---
name: "speckit-constitution"
description: "Cria ou atualiza a constituição do projeto a partir de princípios fornecidos ou definidos interativamente."
compatibility: "Requer uma estrutura de projeto spec-kit com o diretório .specify/"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/constitution.md"
---


## Entrada do usuário

```text
$ARGUMENTS
```

Você **DEVE** considerar a entrada do usuário antes de prosseguir (se não estiver vazia).

## Limites de escopo

O trabalho deste comando limita-se à atualização da própria constituição do projeto. Modelos
dependentes e comandos leem a constituição em tempo de execução e não são modificados aqui.

- Classifique cada parte da entrada do usuário como conteúdo da constituição ou como uma intenção
  separada, não relacionada à governança.
- Se a entrada incluir solicitações de implementação de funcionalidades, geração de código,
  refatoração, compilação ou implantação, você **NÃO DEVE** executá-las. Extraia-as como intenções
  adiadas.
- Você **NÃO DEVE** criar, modificar ou excluir arquivos de código-fonte do aplicativo, rotas de
  funcionalidades, componentes, testes, arquivos de implantação ou outros artefatos não
  relacionados ao fluxo de trabalho da constituição.
- Se não estiver claro se uma instrução faz parte do conteúdo da constituição, peça esclarecimentos
  antes de fazer alterações.
- Depois de concluir a atualização da constituição, inclua uma seção `Próximas ações` para cada
  intenção adiada. Liste a intenção original e sugira o comando Spec Kit apropriado para dar
  continuidade, como `/speckit-specify`, sem invocá-lo.
- Se não houver intenções não relacionadas à governança, omita a seção `Próximas ações`.

## Verificações pré-execução

**Verifique os hooks de extensão (antes da atualização da constituição)**:
- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.before_constitution`.
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

    Aguarde o resultado do comando do hook antes de prosseguir para a seção Esboço.
    ```
    Depois de emitir o bloco acima, você DEVE invocar o hook de fato e aguardar sua conclusão antes de continuar. Execute-o da mesma forma que executaria o comando neste agente/sessão (a invocação pode diferir do identificador literal `{command}` mostrado acima; por exemplo, um agente no modo skills o executa como `/skill:speckit-...` ou `$speckit-...`). Apenas emitir o bloco não executa o hook.
- Se nenhum hook estiver registrado ou `.specify/extensions.yml` não existir, ignore silenciosamente.

## Esboço

Você está atualizando a constituição do projeto em `.specify/memory/constitution.md`. O modelo
ativo da constituição é resolvido no momento da execução do comando a partir de
`constitution-template`, por meio da pilha de resolução de predefinições/modelos do Spec Kit.

Siga este fluxo de execução:

1. Execute `.specify/scripts/powershell/resolve-template.ps1 constitution-template -Json` a partir da raiz do repositório e analise `TEMPLATE_CONTENT` como o modelo ativo.
   - O resolvedor compartilhado aplica substituições do projeto, combina camadas de predefinição
     e camadas de extensão antes de recorrer ao modelo principal. Ele DEVE ter êxito antes de
     continuar.
   - Se falhar, pare e informe o erro de resolução; não continue usando apenas uma das camadas de
     modelo contribuintes.
   - Se `.specify/memory/constitution.md` existir, carregue-o como fonte dos valores e alterações
     atuais específicos do projeto. Preserve as informações ainda aplicáveis ao usar a nova
     estrutura resolvida.
   - Se não existir, use o modelo resolvido como documento inicial.
   - Não grave em nenhuma camada de modelo versionada.
   - Identifique todos os tokens de marcador no formato `[ALL_CAPS_IDENTIFIER]`.
   **IMPORTANTE**: O usuário pode exigir menos ou mais princípios do que os presentes no modelo.
   Se um número for especificado, respeite-o — siga o modelo geral e atualize o documento de acordo.

2. Colete/derive valores para os marcadores:
   - Se a entrada do usuário (conversa) fornecer um valor, use-o.
   - Caso contrário, infira-o do contexto existente do repositório (README, documentação,
     versões anteriores da constituição, se incluídas).
   - Para datas de governança: `RATIFICATION_DATE` é a data original de adoção (se desconhecida,
     pergunte ou marque TODO); `LAST_AMENDED_DATE` é a data de hoje se houver alterações; caso
     contrário, mantenha a data anterior.
   - `CONSTITUTION_VERSION` deve ser incrementada conforme as regras de versionamento semântico:
     - MAJOR: Remoções ou redefinições incompatíveis de regras/princípios de governança.
     - MINOR: Novo princípio/seção ou expansão material das orientações.
     - PATCH: Esclarecimentos, ajustes de redação, correções de erros e refinamentos não semânticos.
   - Se o tipo de incremento de versão for ambíguo, apresente a justificativa antes de finalizar.

3. Redija o conteúdo atualizado da constituição usando o modelo resolvido como estrutura obrigatória:
   - Substitua todos os marcadores por texto concreto (não deixe tokens entre colchetes, exceto
     espaços reservados do modelo mantidos intencionalmente e ainda não definidos pelo projeto —
     justifique explicitamente cada um que permanecer).
   - Preserve a hierarquia dos títulos. Comentários podem ser removidos depois de substituídos,
     salvo quando ainda fornecerem orientações esclarecedoras.
   - Garanta que cada seção de Princípio tenha: uma linha com nome conciso, um parágrafo (ou lista
     de tópicos) com regras inegociáveis e justificativa explícita se não for óbvia.
   - Garanta que a seção Governança liste o procedimento de alteração, a política de versionamento
     e as expectativas de revisão de conformidade.

4. Produza um Relatório de impacto de sincronização como comentário HTML no início do arquivo da
   constituição após a atualização.
   Este relatório é material temporário para revisão humana da alteração, não conteúdo de
   governança; espera-se que seja removido antes do commit do arquivo alterado da constituição.
   - Alteração de versão: anterior → nova.
   - Lista de princípios modificados (título anterior → novo título, se renomeado).
   - Seções adicionadas.
   - Seções removidas.
   - TODOs de acompanhamento, caso algum marcador tenha sido adiado intencionalmente.

5. Valide antes da saída final:
   - Não restam tokens entre colchetes sem explicação.
   - A linha de versão corresponde ao relatório.
   - As datas estão no formato ISO YYYY-MM-DD.
   - Os princípios são declarativos, testáveis e não contêm linguagem vaga ("should" → substitua
     por MUST/SHOULD, com justificativa quando apropriado).

6. Grave a constituição concluída em `.specify/memory/constitution.md` (sobrescreva o arquivo).

7. Emita um resumo final para o usuário com:
   - A nova versão e a justificativa do incremento.
   - Quaisquer marcadores TODO ou itens adiados que exijam acompanhamento manual.
   - Uma sugestão de mensagem de commit (por exemplo, `docs: atualizar constituição para vX.Y.Z (adição de princípios + atualização de governança)`).
   - Uma seção `Próximas ações` para quaisquer intenções adiadas não relacionadas à governança.

Requisitos de formatação e estilo:

- Use os títulos Markdown exatamente como no modelo (não altere seus níveis).
- Quebre linhas longas de justificativa para manter a legibilidade (idealmente <100 caracteres), mas
  não imponha quebras artificiais.
- Mantenha uma única linha em branco entre seções.
- Evite espaços em branco no fim das linhas.

Se o usuário fornecer atualizações parciais (por exemplo, revisão de apenas um princípio), ainda
assim realize as etapas de validação e decisão de versão.

Se faltar informação essencial (por exemplo, se a data de ratificação for realmente desconhecida),
insira `TODO(<FIELD_NAME>): explanation` e inclua o item no Relatório de impacto de sincronização,
na seção de itens adiados.

Grave somente em `.specify/memory/constitution.md`; não crie nem modifique arquivos-fonte de modelos.

## Verificações pós-execução

**Verifique os hooks de extensão (após atualizar a constituição)**:
Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.after_constitution`.
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
