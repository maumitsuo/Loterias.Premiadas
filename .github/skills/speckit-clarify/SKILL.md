---
name: "speckit-clarify"
description: "Identifica áreas insuficientemente especificadas na especificação da funcionalidade atual, fazendo até 5 perguntas de esclarecimento direcionadas e registrando as respostas diretamente na especificação."
compatibility: "Requer uma estrutura de projeto spec-kit com o diretório .specify/"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/clarify.md"
---


## Entrada do usuário

```text
$ARGUMENTS
```

Você **DEVE** considerar a entrada do usuário antes de prosseguir (se não estiver vazia).

## Verificações pré-execução

**Verifique os hooks de extensão (antes do esclarecimento)**:
- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.before_clarify`.
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

Objetivo: Detectar e reduzir ambiguidades ou decisões ausentes na especificação ativa da funcionalidade e registrar os esclarecimentos diretamente no arquivo de especificação.

Observação: Espera-se que este fluxo de esclarecimento seja executado (e concluído) ANTES de invocar `/speckit-plan`. Se o usuário declarar explicitamente que está pulando o esclarecimento (por exemplo, em uma exploração técnica), você poderá prosseguir, mas deverá alertar que o risco de retrabalho posterior aumenta.

Etapas de execução:

1. Execute `.specify/scripts/powershell/check-prerequisites.ps1 -Json -PathsOnly` **uma vez** a partir da raiz do repositório (modo combinado `--json --paths-only` / `-Json -PathsOnly`). Analise os campos mínimos do conteúdo JSON:
   - `FEATURE_DIR`
   - `FEATURE_SPEC`
   - (Opcionalmente capture `IMPL_PLAN`, `TASKS` para fluxos encadeados futuros.)
   - Se a análise do JSON falhar, aborte e instrua o usuário a executar `/speckit-specify` novamente ou verificar o ambiente da branch da funcionalidade.
   - Para apóstrofos em argumentos como "I'm Groot", use a sintaxe de escape, por exemplo: `I'\''m Groot` (ou aspas duplas, se possível: "I'm Groot").

2. **SE EXISTIR**: Carregue `.specify/memory/constitution.md` para obter os princípios do projeto e as restrições de governança.

3. Carregue o arquivo de especificação atual. Faça uma verificação estruturada de ambiguidades e cobertura usando esta taxonomia. Para cada categoria, marque o status: Clear / Partial / Missing. Produza um mapa interno de cobertura para priorização (não emita o mapa bruto, exceto se nenhuma pergunta for feita).

   Escopo funcional e comportamento:
   - Objetivos centrais do usuário e critérios de sucesso.
   - Declarações explícitas do que está fora do escopo.
   - Diferenciação de papéis/personas de usuário.

   Domínio e modelo de dados:
   - Entidades, atributos e relacionamentos.
   - Regras de identidade e unicidade.
   - Ciclo de vida/transições de estado.
   - Premissas de volume/escala de dados.

   Fluxo de interação e UX:
   - Jornadas/sequências críticas do usuário.
   - Estados de erro/vazio/carregamento.
   - Observações de acessibilidade ou localização.

   Atributos de qualidade não funcionais:
   - Desempenho (metas de latência e throughput).
   - Escalabilidade (horizontal/vertical, limites).
   - Confiabilidade e disponibilidade (tempo de atividade, expectativas de recuperação).
   - Observabilidade (logs, métricas, sinais de rastreamento).
   - Segurança e privacidade (authN/Z, proteção de dados, premissas de ameaças).
   - Restrições regulatórias/de conformidade (se houver).

   Integrações e dependências externas:
   - Serviços/APIs externos e modos de falha.
   - Formatos de importação/exportação de dados.
   - Premissas de protocolo/versionamento.

   Casos extremos e tratamento de falhas:
   - Cenários negativos.
   - Limitação de taxa/controle de tráfego.
   - Resolução de conflitos (por exemplo, edições simultâneas).

   Restrições e compensações:
   - Restrições técnicas (linguagem, armazenamento, hospedagem).
   - Compensações explícitas ou alternativas rejeitadas.

   Terminologia e consistência:
   - Termos canônicos do glossário.
   - Sinônimos a evitar/termos obsoletos.

   Sinais de conclusão:
   - Testabilidade dos critérios de aceitação.
   - Indicadores mensuráveis no estilo de definição de pronto.

   Diversos/marcadores:
   - Marcadores TODO/decisões não resolvidas.
   - Adjetivos ambíguos ("robusto", "intuitivo") sem quantificação.

   Para cada categoria com status Partial ou Missing, considere uma pergunta candidata, exceto se:
   - O esclarecimento não alterar materialmente a implementação nem a estratégia de validação.
   - O item tratar especificamente de método de implementação, comparação de stack tecnológica ou decomposição de tarefas (anote internamente).

4. Gere (internamente) uma fila priorizada de perguntas candidatas de esclarecimento (máximo de 5). NÃO as apresente todas de uma vez. Aplique estas restrições:
    - No máximo 5 perguntas no total durante toda a sessão.
    - Cada pergunta deve poder ser respondida COM uma destas opções:
       - Uma seleção de múltipla escolha curta (2–5 opções distintas e mutuamente exclusivas); OU
       - Uma resposta de uma palavra/frase curta (limite explicitamente: "Responda em <=5 palavras").
    - Inclua somente perguntas cujas respostas afetem materialmente a arquitetura, o modelo de dados, a decomposição de tarefas, o projeto dos testes, o comportamento de UX, a prontidão operacional ou a validação de conformidade.
    - Equilibre a cobertura das categorias: tente abordar primeiro as categorias não resolvidas de maior impacto; evite fazer duas perguntas de baixo impacto quando uma área de alto impacto (por exemplo, postura de segurança) ainda estiver sem solução.
    - Exclua perguntas já respondidas, preferências estilísticas triviais ou detalhes de execução no nível do plano (a menos que impeçam a correção).
    - Prefira esclarecimentos que reduzam o risco de retrabalho futuro ou evitem critérios de aceitação desalinhados.
    - Se mais de 5 categorias permanecerem sem solução, selecione as 5 principais pela heurística (Impacto * Incerteza).

5. Ciclo sequencial de perguntas (interativo):
    - Apresente EXATAMENTE UMA pergunta por vez.
    - **Qualidade da redação das perguntas (aplica-se a todas, de múltipla escolha ou resposta curta):**
       - Comece com `**Question:**`, seguido de uma pergunta completa que termine com `?`. O texto da pergunta antes do `?` deve fazer sentido por si só.
       - NUNCA use um rótulo de tópico, título de seção ou ID de requisito como se fosse a pergunta. Por exemplo, `Matriz de dispositivo/runtime de aceitação (FR-023)` é INVÁLIDO — é um rótulo, não uma pergunta.
       - Depois do `?`, o único sufixo permitido é um ID opcional de requisito/pergunta entre parênteses. Formato exato: `**Question:** <interrogative>?` ou `**Question:** <interrogative>? (FR-023)`. Nunca coloque o ID antes do `?`, nem use o ID (sozinho ou com um rótulo de tópico) como pergunta completa.
       - Imediatamente após a linha da pergunta, adicione uma frase em linguagem simples explicando "Por que isso importa" (o impacto sobre a aceitação ou o lançamento), antes da recomendação/opções.
       - Use linguagem cotidiana; introduza jargões somente se definidos na mesma frase. Verificação: alguém que não conheça o Spec Kit deve conseguir responder apenas com base na linha da pergunta. Ser conciso é aceitável; rótulos enigmáticos, não.
    - Para perguntas de múltipla escolha:
       - **Analise todas as opções** e determine a **mais adequada** com base em:
          - Boas práticas para o tipo de projeto.
          - Padrões comuns em implementações semelhantes.
          - Redução de riscos (segurança, desempenho, manutenção).
          - Alinhamento com quaisquer objetivos ou restrições explícitos do projeto visíveis na especificação.
       - Apresente a **opção recomendada em destaque** no início, com justificativa clara (1–2 frases explicando por que é a melhor escolha).
       - Use o formato: `**Recommended:** Option [X] - <reasoning>`
       - Em seguida, apresente todas as opções em uma tabela Markdown:

       | Opção | Descrição |
       |--------|-------------|
       | A | <Option A description> |
       | B | <Option B description> |
       | C | <Option C description> (adicione D/E se necessário, até 5) |
       | Short | Forneça outra resposta curta (<=5 palavras) (inclua somente se uma alternativa livre for apropriada) |

       - Após a tabela, adicione: `Você pode responder com a letra da opção (por exemplo, "A"), aceitar a recomendação dizendo "yes" ou "recommended", ou fornecer sua própria resposta curta.`
    - Para respostas curtas (sem opções discretas significativas):
       - Forneça sua **resposta sugerida** com base nas boas práticas e no contexto.
       - Use o formato: `**Suggested:** <your proposed answer> - <brief reasoning>`
       - Em seguida, emita: `Formato: resposta curta (<=5 palavras). Você pode aceitar a sugestão dizendo "yes" ou "suggested", ou fornecer sua própria resposta.`
    - Após a resposta do usuário:
       - Se o usuário responder "yes", "recommended" ou "suggested", use como resposta a recomendação/sugestão indicada anteriormente.
       - Caso contrário, valide se a resposta corresponde a uma opção ou atende ao limite de <=5 palavras.
       - Se estiver ambígua, peça um esclarecimento rápido (a contagem continua sendo da mesma pergunta; não avance).
       - Quando a resposta for satisfatória, registre-a na memória de trabalho (ainda não a grave em disco) e passe à próxima pergunta da fila.
    - Pare de fazer perguntas quando:
       - Todas as ambiguidades críticas forem resolvidas antecipadamente (os itens restantes da fila se tornarem desnecessários); OU
       - O usuário indicar que terminou ("done", "good", "no more"); OU
       - Você atingir 5 perguntas feitas.
    - Nunca revele antecipadamente as perguntas futuras da fila.
    - Se não houver perguntas válidas desde o início, informe imediatamente que não há ambiguidades críticas.

6. Integração após CADA resposta aceita (abordagem de atualização incremental):
    - Mantenha uma representação da especificação em memória (carregada uma vez no início), além do conteúdo bruto do arquivo.
    - Na primeira resposta integrada nesta sessão:
       - Garanta que exista uma seção `## Clarifications` (crie-a logo após a seção contextual/de visão geral de nível mais alto, conforme o modelo da especificação, se estiver ausente).
       - Abaixo dela, crie (se ainda não existir) um subtítulo `### Session YYYY-MM-DD` para a data de hoje.
    - Acrescente imediatamente após a aceitação uma linha de tópico: `- Q: <question> → A: <final answer>`.
    - Em seguida, aplique o esclarecimento imediatamente à(s) seção(ões) mais apropriada(s):
       - Ambiguidade funcional → Atualize ou adicione um tópico em Requisitos funcionais.
       - Interação do usuário/diferenciação de ator → Atualize Histórias de usuário ou a subseção Atores (se houver), com o papel, a restrição ou o cenário esclarecido.
       - Formato dos dados/entidades → Atualize o Modelo de dados (adicione campos, tipos, relacionamentos) preservando a ordem; registre sucintamente as restrições adicionadas.
       - Restrição não funcional → Adicione/ajuste critérios mensuráveis em Critérios de sucesso > Resultados mensuráveis (converta adjetivos vagos em métricas ou metas explícitas).
       - Caso extremo/fluxo negativo → Adicione um tópico em Casos extremos/Tratamento de erros (ou crie essa subseção se o modelo fornecer um marcador para ela).
       - Conflito terminológico → Padronize o termo em toda a especificação; mantenha o original somente se necessário, adicionando `(anteriormente chamado de "X")` uma única vez.
    - Se o esclarecimento invalidar uma declaração ambígua anterior, substitua essa declaração em vez de duplicá-la; não deixe texto obsoleto ou contraditório.
    - Salve o arquivo de especificação APÓS cada integração para minimizar o risco de perda de contexto (sobrescrita atômica).
    - Preserve a formatação: não reordene seções não relacionadas; mantenha intacta a hierarquia dos títulos.
    - Mantenha cada esclarecimento inserido conciso e testável (evite desvio narrativo).

7. Validação (realizada após CADA gravação e na verificação final):
   - A sessão de esclarecimentos contém exatamente um tópico por resposta aceita (sem duplicações).
   - O total de perguntas feitas (aceitas) é ≤ 5.
   - As seções atualizadas não contêm marcadores vagos que a nova resposta deveria resolver.
   - Não resta nenhuma declaração anterior contraditória (verifique se alternativas agora inválidas foram removidas).
   - A estrutura Markdown é válida; os únicos novos títulos permitidos são `## Clarifications` e `### Session YYYY-MM-DD`.
   - Consistência terminológica: use o mesmo termo canônico em todas as seções atualizadas.

8. Grave a especificação atualizada em `FEATURE_SPEC`.

9. **Revalidar a lista de verificação da qualidade da especificação** (se existir):
   - Verifique se `FEATURE_DIR/checklists/requirements.md` existe.
   - Se NÃO existir, ignore esta etapa silenciosamente.
   - Se existir:
     1. Leia o arquivo da lista.
     2. Identifique todas as linhas de caixas de seleção de listas de tarefas do GitHub — linhas que correspondam a `- [ ]`, `- [x]` ou `- [X]` (sem distinção entre maiúsculas/minúsculas e aceitando espaços iniciais para itens aninhados) fora de blocos de código. Ignore todo o restante do conteúdo (títulos, observações, tópicos sem caixas de seleção, metadados).
     3. Para cada linha com caixa de seleção, registre o estado atual do marcador (marcado ou desmarcado) e o texto do item em uma lista de captura anterior.
     4. Reavalie cada item da lista em relação à especificação **atualizada** (a versão que acabou de ser salva na etapa 7).
     5. Para cada item, atualize somente se o estado marcado/desmarcado realmente mudar:
        - Se o item agora for aprovado e estava desmarcado: altere `[ ]` para `[x]`.
        - Se o item agora falhar e estava marcado: altere `[x]`/`[X]` para `[ ]`.
        - Se o estado não mudar: mantenha o marcador como está (preserve a capitalização atual para evitar diferenças cosméticas).
     6. Salve o arquivo atualizado da lista. **Altere somente a parte do marcador `[ ]`/`[x]` das linhas cujo estado mudou.** Todo o restante do conteúdo — títulos, metadados, observações, ordem das linhas e espaços — deve permanecer inalterado para evitar diferenças desnecessárias.
     7. Compare a captura anterior com o estado atual para calcular três listas para o Relatório de conclusão:
        - **Aprovados agora**: itens que passaram de desmarcados a marcados.
        - **Regressões**: itens que passaram de marcados a desmarcados.
        - **Ainda desmarcados**: itens que continuam desmarcados.
     8. Registre as contagens de aprovação antes/depois como itens marcados/total (por exemplo, "12/16 → 15/16 itens aprovados").

Regras de comportamento:

- Se não forem encontradas ambiguidades significativas (ou se todas as perguntas potenciais forem de baixo impacto), responda: "Nenhuma ambiguidade crítica que justifique esclarecimento formal foi detectada." e sugira prosseguir.
- Se o arquivo de especificação estiver ausente, instrua o usuário a executar `/speckit-specify` primeiro (não crie uma especificação aqui).
- Nunca ultrapasse 5 perguntas no total (novas tentativas de esclarecimento de uma pergunta não contam como perguntas novas).
- Evite perguntas especulativas sobre stack tecnológica, a menos que sua ausência impeça a clareza funcional.
- Respeite sinais de encerramento antecipado do usuário ("stop", "done", "proceed").
- Se nenhuma pergunta for feita por haver cobertura completa, emita um resumo compacto da cobertura (todas as categorias Clear) e sugira avançar.
- Se o limite for atingido e ainda houver categorias não resolvidas de alto impacto, sinalize-as explicitamente em Adiadas, com justificativa.

Contexto para priorização: $ARGUMENTS

## Hooks pós-execução obrigatórios

**Você DEVE concluir esta seção antes de informar o usuário sobre a conclusão.**

- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se não existir, ou se nenhum hook estiver registrado em `hooks.after_clarify`, passe ao Relatório de conclusão.
- Se existir, leia-o e procure entradas na chave `hooks.after_clarify`.
- Se o YAML não puder ser analisado ou for inválido, não ignore isso silenciosamente: informe ao usuário que não foi possível ler `.specify/extensions.yml` (inclua o erro do analisador) e que nenhum hook foi verificado, inclusive os hooks obrigatórios (`optional: false`) registrados nele; em seguida, prossiga para o Relatório de conclusão.
- Filtre os hooks em que `enabled` esteja explicitamente definido como `false`. Considere habilitados por padrão os hooks sem o campo `enabled`.
- Para cada hook restante, **não** tente interpretar nem avaliar expressões `condition`:
  - Se o hook não tiver o campo `condition`, ou se ele for nulo/vazio, considere-o executável.
  - Se o hook definir um `condition` não vazio, ignore-o e deixe a avaliação da condição para a implementação de HookExecutor.
- Ao construir invocações de comandos a partir dos nomes de hooks, substitua pontos (`.`) por hífens (`-`). Por exemplo, `speckit.git.commit` → `/speckit-git-commit`.
- Para cada hook executável, emita o seguinte conforme seu indicador `optional`:
  - **Hook obrigatório** (`optional: false`) — **você DEVE emitir `EXECUTE_COMMAND:` para cada hook obrigatório**:
    ```
    ## Hooks de extensão

    **Hook automático**: {extension}
    Executando: `/{command}`
    EXECUTE_COMMAND: {command}
    ```
    Depois de emitir o bloco acima, você DEVE invocar o hook de fato e aguardar sua conclusão antes de continuar. Execute-o da mesma forma que executaria o comando neste agente/sessão (a invocação pode diferir do identificador literal `{command}` mostrado acima; por exemplo, um agente no modo skills o executa como `/skill:speckit-...` ou `$speckit-...`). Apenas emitir o bloco não executa o hook.
  - **Hook opcional** (`optional: true`):
    ```
    ## Hooks de extensão

    **Hook opcional**: {extension}
    Comando: `/{command}`
    Descrição: {description}

    Solicitação: {prompt}
    Para executar: `/{command}`
    ```

## Relatório de conclusão

Informe a conclusão (após o fim do ciclo de perguntas ou o encerramento antecipado):
- Número de perguntas feitas e respondidas.
- Caminho da especificação atualizada.
- Seções alteradas (liste os nomes).
- Estado da lista de verificação da qualidade da especificação (se `FEATURE_DIR/checklists/requirements.md` foi revalidada): mostre as contagens de aprovação antes/depois (por exemplo, "Lista de qualidade da especificação: 12/16 → 15/16 itens aprovados") e liste os itens cujo estado mudou — tanto os recém-marcados (desmarcado → marcado) quanto as regressões (marcado → desmarcado). Se algum item continuar desmarcado, liste-o como área que precisa de atenção.
- Tabela de resumo da cobertura listando cada categoria da taxonomia com o status: Resolved (estava Partial/Missing e foi abordada), Deferred (excede o limite de perguntas ou o item ainda se refere especificamente ao método de implementação, à comparação de stack tecnológica ou à decomposição de tarefas), Clear (já era suficiente), Outstanding (ainda Partial/Missing, mas de baixo impacto).
- Se houver itens Pendentes ou Adiados, recomende se deve prosseguir para `/speckit-plan` ou executar `/speckit-clarify` novamente mais tarde, após o planejamento.
- Próximo comando sugerido.

## Critérios de conclusão

- [ ] Ambiguidades da especificação identificadas e esclarecimentos integrados ao arquivo de especificação.
- [ ] Lista de verificação da qualidade da especificação revalidada em relação à especificação atualizada (se `FEATURE_DIR/checklists/requirements.md` existir).
- [ ] Hooks de extensão executados ou ignorados conforme as regras da seção Hooks pós-execução obrigatórios acima.
- [ ] Conclusão informada ao usuário com as perguntas respondidas, seções alteradas, estado da lista de verificação e resumo da cobertura.
