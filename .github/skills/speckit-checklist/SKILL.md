---
name: "speckit-checklist"
description: "Gera uma lista de verificação personalizada para a funcionalidade atual com base nos requisitos do usuário."
compatibility: "Requer uma estrutura de projeto spec-kit com o diretório .specify/"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/checklist.md"
---


## Objetivo da lista de verificação: "Testes unitários para requisitos em linguagem natural"

**CONCEITO CRÍTICO**: Listas de verificação são **TESTES UNITÁRIOS DA REDAÇÃO DOS REQUISITOS** — validam a qualidade, clareza e completude dos requisitos em um determinado domínio.

**NÃO servem para verificação/testes de implementação**:

- ❌ NÃO "Verifique se o botão funciona ao ser clicado"
- ❌ NÃO "Teste se o tratamento de erros funciona"
- ❌ NÃO "Confirme se a API retorna 200"
- ❌ NÃO verifique se o código/implementação corresponde à especificação

**SERVEM para validar a qualidade dos requisitos**:

- ✅ "Os requisitos de hierarquia visual estão definidos para todos os tipos de cartão?" (completude)
- ✅ "'Exibição em destaque' está quantificada com dimensões/posicionamento específicos?" (clareza)
- ✅ "Os requisitos de estado ao passar o cursor são consistentes entre todos os elementos interativos?" (consistência)
- ✅ "Os requisitos de acessibilidade para navegação pelo teclado estão definidos?" (cobertura)
- ✅ "A especificação define o que acontece quando a imagem do logotipo não carrega?" (casos extremos)

**Metáfora**: Se sua especificação fosse código escrito em linguagem natural, a lista de verificação seria sua suíte de testes unitários. Você está testando se os requisitos estão bem redigidos, completos, inequívocos e prontos para implementação — NÃO se a implementação funciona.

**Responsabilidade e ciclo de vida das caixas de seleção**:

- As listas de verificação personalizadas geradas por este comando são artefatos de revisão da qualidade dos requisitos, sob responsabilidade do revisor.
- `[x]` significa que o revisor determinou que o critério de qualidade dos requisitos foi satisfeito.
- `[x]` NÃO significa que o trabalho de implementação foi concluído.
- Este comando gera ou acrescenta itens à lista; NÃO DEVE marcar os itens gerados com `[x]`.
- Um agente só pode ajudar a avaliar os itens quando solicitado explicitamente pelo revisor.
- `checklists/requirements.md` é uma lista de verificação de qualidade da especificação, integrada e separada, mantida por `/speckit-specify` e `/speckit-clarify`; não aplique essa exceção às listas personalizadas geradas aqui.

## Entrada do usuário

```text
$ARGUMENTS
```

Você **DEVE** considerar a entrada do usuário antes de prosseguir (se não estiver vazia).

## Verificações pré-execução

**Verifique os hooks de extensão (antes de gerar a lista)**:
- Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.before_checklist`.
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

    Aguarde o resultado do comando do hook antes de prosseguir para as Etapas de execução.
    ```
    Depois de emitir o bloco acima, você DEVE invocar o hook de fato e aguardar sua conclusão antes de continuar. Execute-o da mesma forma que executaria o comando neste agente/sessão (a invocação pode diferir do identificador literal `{command}` mostrado acima; por exemplo, um agente no modo skills o executa como `/skill:speckit-...` ou `$speckit-...`). Apenas emitir o bloco não executa o hook.
- Se nenhum hook estiver registrado ou `.specify/extensions.yml` não existir, ignore silenciosamente.

## Etapas de execução

1. **Configuração**: Execute `.specify/scripts/powershell/check-prerequisites.ps1 -Json -Template checklist-template` a partir da raiz do repositório e analise o JSON para obter FEATURE_DIR, a lista AVAILABLE_DOCS e TEMPLATE_CONTENT.
   - Todos os caminhos de arquivos devem ser absolutos.
   - Para apóstrofos em argumentos como "I'm Groot", use a sintaxe de escape, por exemplo: `I'\''m Groot` (ou aspas duplas, se possível: "I'm Groot").

2. **SE EXISTIR**: Carregue `.specify/memory/constitution.md` para obter os princípios do projeto e as restrições de governança.

3. **Esclarecer a intenção (dinâmico)**: Elabore até TRÊS perguntas contextuais iniciais de esclarecimento (sem catálogo predefinido). Elas DEVEM:
   - Ser geradas a partir da formulação do usuário e dos sinais extraídos de spec/plan/tasks.
   - Perguntar somente sobre informações que alterem materialmente o conteúdo da lista.
   - Ser omitidas individualmente se já estiverem inequívocas em `$ARGUMENTS`.
   - Priorizar precisão em vez de abrangência.

   Algoritmo de geração:
   1. Extraia sinais: palavras-chave do domínio da funcionalidade (por exemplo, autenticação, latência, UX, API), indicadores de risco ("crítico", "deve", "conformidade"), pistas sobre as partes interessadas ("QA", "revisão", "equipe de segurança") e entregáveis explícitos ("a11y", "reversão", "contratos").
   2. Agrupe os sinais em áreas de foco candidatas (máximo de 4), classificadas por relevância.
   3. Identifique o público e o momento prováveis (autor, revisor, QA, lançamento), se não estiverem explícitos.
   4. Detecte dimensões ausentes: abrangência do escopo, profundidade/rigor, ênfase em riscos, limites de exclusão e critérios de aceitação mensuráveis.
   5. Formule perguntas escolhidas entre estes arquétipos:
      - Refinamento do escopo (por exemplo, "Isso deve incluir pontos de integração com X e Y ou ficar limitado à correção do módulo local?")
      - Priorização de riscos (por exemplo, "Quais dessas áreas de risco potenciais devem receber verificações obrigatórias?")
      - Calibração da profundidade (por exemplo, "Esta é uma lista leve de verificações antes do commit ou um gate formal de lançamento?")
      - Definição do público (por exemplo, "A lista será usada apenas pelo autor ou também por colegas durante a revisão do PR?")
      - Exclusão de limites (por exemplo, "Devemos excluir explicitamente itens de ajuste de desempenho nesta rodada?")
      - Lacuna de classe de cenário (por exemplo, "Não foram detectados fluxos de recuperação — caminhos de reversão/falha parcial fazem parte do escopo?")

   Regras de formatação das perguntas:
   - Se apresentar opções, gere uma tabela compacta com as colunas: Opção | Alternativa | Por que isso importa.
   - Limite a, no máximo, as opções A–E; omita a tabela se uma resposta livre for mais clara.
   - Nunca peça ao usuário que repita o que já disse.
   - Evite categorias especulativas (não alucine). Se houver dúvida, pergunte explicitamente: "Confirme se X faz parte do escopo."

   Padrões quando não for possível interagir:
   - Profundidade: padrão.
   - Público: revisor (PR), se relacionado a código; caso contrário, autor.
   - Foco: os 2 grupos mais relevantes.

   Emita as perguntas (identificadas como Q1/Q2/Q3). Após as respostas, se ≥2 classes de cenário (Alternativo / Exceção / Recuperação / domínio não funcional) permanecerem pouco claras, você PODE fazer até DUAS perguntas direcionadas adicionais (Q4/Q5), cada uma com uma justificativa de uma linha (por exemplo, "Risco de caminho de recuperação não resolvido"). Não exceda cinco perguntas no total. Não faça perguntas adicionais se o usuário recusar explicitamente.

4. **Entender a solicitação do usuário**: Combine `$ARGUMENTS` com as respostas de esclarecimento:
   - Determine o tema da lista (por exemplo, segurança, revisão, implantação, UX).
   - Consolide os itens obrigatórios explícitos mencionados pelo usuário.
   - Associe as seleções de foco à estrutura de categorias.
   - Infira qualquer contexto ausente a partir de spec/plan/tasks (NÃO alucine).

5. **Carregar o contexto da funcionalidade**: Leia em FEATURE_DIR:
   - spec.md: requisitos e escopo da funcionalidade.
   - plan.md (se existir): detalhes técnicos e dependências.
   - tasks.md (se existir): tarefas de implementação.

   **Estratégia de carregamento do contexto**:
   - Carregue somente as partes necessárias e relevantes às áreas de foco ativas (evite despejar arquivos inteiros).
   - Prefira resumir seções longas em tópicos concisos de cenários/requisitos.
   - Use divulgação progressiva: busque conteúdo adicional somente se detectar lacunas.
   - Se os documentos de origem forem extensos, gere itens de resumo intermediários em vez de incorporar o texto bruto.

6. **Gerar a lista de verificação** — Use TEMPLATE_CONTENT como modelo estrutural e crie "Testes unitários para requisitos":
   - Crie o diretório `FEATURE_DIR/checklists/` se ainda não existir.
   - Gere um nome de arquivo exclusivo para a lista:
     - Use um nome curto e descritivo baseado no domínio (por exemplo, `ux.md`, `api.md`, `security.md`).
     - Formato: `[domain].md`.
   - Comportamento de manipulação do arquivo:
     - Se o arquivo NÃO existir: crie-o e numere os itens a partir de CHK001.
     - Se o arquivo existir: acrescente os novos itens ao arquivo, continuando a partir do último ID CHK (por exemplo, se o último item for CHK015, comece os novos em CHK016).
   - Nunca exclua nem substitua conteúdo existente da lista; sempre preserve e acrescente.
   - Deixe todos os itens recém-gerados desmarcados (`[ ]`); o estado das caixas é responsabilidade do revisor.

   **PRINCÍPIO CENTRAL — Teste os requisitos, não a implementação**:
   Cada item da lista DEVE avaliar os PRÓPRIOS REQUISITOS quanto a:
   - **Completude**: Todos os requisitos necessários estão presentes?
   - **Clareza**: Os requisitos são inequívocos e específicos?
   - **Consistência**: Os requisitos estão alinhados entre si?
   - **Mensurabilidade**: Os requisitos podem ser verificados objetivamente?
   - **Cobertura**: Todos os cenários/casos extremos foram abordados?

   **Estrutura de categorias** — Agrupe os itens pelas dimensões de qualidade dos requisitos:
   - **Completude dos requisitos** (Todos os requisitos necessários estão documentados?)
   - **Clareza dos requisitos** (Os requisitos são específicos e inequívocos?)
   - **Consistência dos requisitos** (Os requisitos estão alinhados, sem conflitos?)
   - **Qualidade dos critérios de aceitação** (Os critérios de sucesso são mensuráveis?)
   - **Cobertura de cenários** (Todos os fluxos/casos foram abordados?)
   - **Cobertura de casos extremos** (As condições de limite estão definidas?)
   - **Requisitos não funcionais** (Desempenho, segurança, acessibilidade etc. estão especificados?)
   - **Dependências e premissas** (Estão documentadas e validadas?)
   - **Ambiguidades e conflitos** (O que precisa de esclarecimento?)

   **COMO REDIGIR ITENS DA LISTA — "Testes unitários para requisitos em linguagem natural"**:

   ❌ **INCORRETO** (testa a implementação):
   - "Verifique se a página inicial exibe 3 cartões de episódios"
   - "Teste se os estados ao passar o cursor funcionam no desktop"
   - "Confirme se clicar no logotipo leva à página inicial"

   ✅ **CORRETO** (testa a qualidade dos requisitos):
   - "A quantidade exata e o layout dos episódios em destaque estão especificados?" [Completeness]
   - "'Exibição em destaque' está quantificada com dimensões/posicionamento específicos?" [Clarity]
   - "Os requisitos de estado ao passar o cursor são consistentes entre todos os elementos interativos?" [Consistency]
   - "Os requisitos de navegação pelo teclado estão definidos para todos os elementos interativos da interface?" [Coverage]
   - "O comportamento alternativo está especificado para quando a imagem do logotipo não carregar?" [Edge Cases]
   - "Os estados de carregamento estão definidos para dados assíncronos de episódios?" [Completeness]
   - "A especificação define a hierarquia visual de elementos de interface concorrentes?" [Clarity]

   **ESTRUTURA DOS ITENS**:
   Cada item deve seguir este padrão:
   - Formato de pergunta sobre a qualidade dos requisitos.
   - Foco no que está ESCRITO (ou não está escrito) na especificação/no plano.
   - Inclua a dimensão de qualidade entre colchetes [Completeness/Clarity/Consistency/etc.].
   - Faça referência à seção da especificação `[Spec §X.Y]` ao verificar requisitos existentes.
   - Use o marcador `[Gap]` ao verificar requisitos ausentes.

   **EXEMPLOS POR DIMENSÃO DE QUALIDADE**:

   Completude:
   - "Os requisitos de tratamento de erros estão definidos para todos os modos de falha da API? [Gap]"
   - "Os requisitos de acessibilidade estão especificados para todos os elementos interativos? [Completeness]"
   - "Os requisitos de pontos de quebra para dispositivos móveis estão definidos para layouts responsivos? [Gap]"

   Clareza:
   - "'Carregamento rápido' está quantificado com limites de tempo específicos? [Clarity, Spec §NFR-2]"
   - "Os critérios de seleção de 'episódios relacionados' estão definidos explicitamente? [Clarity, Spec §FR-5]"
   - "'Em destaque' está definido com propriedades visuais mensuráveis? [Ambiguity, Spec §FR-4]"

   Consistência:
   - "Os requisitos de navegação estão alinhados em todas as páginas? [Consistency, Spec §FR-10]"
   - "Os requisitos dos componentes de cartão são consistentes entre as páginas inicial e de detalhes? [Consistency]"

   Cobertura:
   - "Há requisitos definidos para cenários vazios (sem episódios)? [Coverage, Edge Case]"
   - "Os cenários de interação simultânea entre usuários foram abordados? [Coverage, Gap]"
   - "Há requisitos especificados para falhas parciais no carregamento de dados? [Coverage, Exception Flow]"

   Mensurabilidade:
   - "Os requisitos de hierarquia visual são mensuráveis/testáveis? [Acceptance Criteria, Spec §FR-1]"
   - "'Peso visual equilibrado' pode ser verificado objetivamente? [Measurability, Spec §FR-2]"

   **Classificação e cobertura de cenários** (foco na qualidade dos requisitos):
   - Verifique se existem requisitos para cenários: primários, alternativos, de exceção/erro, de recuperação e não funcionais.
   - Para cada classe de cenário, pergunte: "Os requisitos de [tipo de cenário] são completos, claros e consistentes?"
   - Se uma classe de cenário estiver ausente: "Os requisitos de [tipo de cenário] foram excluídos intencionalmente ou estão faltando? [Gap]"
   - Inclua resiliência/reversão quando houver mutação de estado: "Os requisitos de reversão estão definidos para falhas de migração? [Gap]"

   **Requisitos de rastreabilidade**:
   - MÍNIMO: ≥80% dos itens DEVEM incluir pelo menos uma referência de rastreabilidade.
   - Cada item deve referenciar uma seção da especificação `[Spec §X.Y]` ou usar os marcadores: `[Gap]`, `[Ambiguity]`, `[Conflict]`, `[Assumption]`.
   - Se não houver um sistema de IDs: "Foi estabelecido um esquema de IDs para requisitos e critérios de aceitação? [Traceability]"

   **Identificar e resolver problemas** (problemas de qualidade dos requisitos):
   Faça perguntas sobre os próprios requisitos:
   - Ambiguidades: "O termo 'rápido' está quantificado com métricas específicas? [Ambiguity, Spec §NFR-1]"
   - Conflitos: "Há conflito entre os requisitos de navegação de §FR-10 e §FR-10a? [Conflict]"
   - Premissas: "A premissa de que a 'API de podcasts está sempre disponível' foi validada? [Assumption]"
   - Dependências: "Os requisitos para APIs externas de podcast estão documentados? [Dependency, Gap]"
   - Definições ausentes: "'Hierarquia visual' está definida com critérios mensuráveis? [Gap]"

   **Consolidação de conteúdo**:
   - Limite flexível: se houver >40 itens candidatos brutos, priorize-os por risco/impacto.
   - Una itens quase duplicados que verifiquem o mesmo aspecto do requisito.
   - Se houver >5 casos extremos de baixo impacto, crie um item: "Os casos extremos X, Y, Z foram abordados nos requisitos? [Coverage]"

   **🚫 ABSOLUTAMENTE PROIBIDO** — Isso transforma a lista em um teste de implementação, não de requisitos:
   - ❌ Qualquer item que comece com "Verifique", "Teste", "Confirme", "Confira" + comportamento da implementação.
   - ❌ Referências à execução de código, ações do usuário ou comportamento do sistema.
   - ❌ "É exibido corretamente", "funciona adequadamente", "funciona como esperado".
   - ❌ "Clique", "navegue", "renderize", "carregue", "execute".
   - ❌ Casos de teste, planos de teste, procedimentos de QA.
   - ❌ Detalhes de implementação (frameworks, APIs, algoritmos).

   **✅ PADRÕES OBRIGATÓRIOS** — Estes avaliam a qualidade dos requisitos:
   - ✅ "Os [tipo de requisito] estão definidos/especificados/documentados para [cenário]?"
   - ✅ "[Termo vago] está quantificado/esclarecido com critérios específicos?"
   - ✅ "Os requisitos são consistentes entre [seção A] e [seção B]?"
   - ✅ "[Requisito] pode ser medido/verificado objetivamente?"
   - ✅ "Os [casos extremos/cenários] foram abordados nos requisitos?"
   - ✅ "A especificação define [aspecto ausente]?"

7. **Referência de estrutura**: Gere a lista seguindo o modelo canônico em `.specify/templates/checklist-template.md` para o título, seção de metadados, cabeçalhos de categoria, nota de responsabilidade, seção de observações e formatação dos IDs. Se o modelo não estiver disponível, use: título H1, linhas de metadados de finalidade/criação, uma nota de responsabilidade explicando que `[x]` significa aprovação da qualidade dos requisitos pelo revisor, seções de categoria `##` contendo linhas `- [ ] CHK### <requirement item>` com IDs sequenciais globais a partir de CHK001 e observações de que `/speckit-implement` lê o estado da lista, mas não altera os marcadores.

8. **Relatório**: Informe o caminho completo do arquivo da lista, a quantidade de itens e se a execução criou um arquivo novo ou acrescentou conteúdo a um arquivo existente. Resuma:
   - Áreas de foco selecionadas.
   - Nível de profundidade.
   - Ator/momento.
   - Quaisquer itens obrigatórios explícitos do usuário que foram incorporados.

**Importante**: Cada invocação do comando `/speckit-checklist` usa um nome curto e descritivo para a lista e cria um arquivo novo ou acrescenta conteúdo a um existente. Isso permite:

- Várias listas de tipos diferentes (por exemplo, `ux.md`, `test.md`, `security.md`).
- Nomes de arquivo simples e fáceis de lembrar que indiquem a finalidade da lista.
- Identificação e navegação fáceis na pasta `checklists/`.

Para evitar desorganização, use tipos descritivos e remova listas obsoletas quando terminar.

## Tipos de listas e exemplos de itens

**Qualidade dos requisitos de UX:** `ux.md`

Exemplos de itens (testam os requisitos, NÃO a implementação):

- "Os requisitos de hierarquia visual estão definidos com critérios mensuráveis? [Clarity, Spec §FR-1]"
- "A quantidade e o posicionamento dos elementos da interface estão especificados explicitamente? [Completeness, Spec §FR-1]"
- "Os requisitos de estado de interação (ao passar o cursor, foco, ativo) estão definidos de forma consistente? [Consistency]"
- "Os requisitos de acessibilidade estão especificados para todos os elementos interativos? [Coverage, Gap]"
- "O comportamento alternativo está definido para quando as imagens não carregarem? [Edge Case, Gap]"
- "'Exibição em destaque' pode ser medida objetivamente? [Measurability, Spec §FR-4]"

**Qualidade dos requisitos de API:** `api.md`

Exemplos de itens:

- "Os formatos de resposta de erro estão especificados para todos os cenários de falha? [Completeness]"
- "Os requisitos de limitação de taxa estão quantificados com limites específicos? [Clarity]"
- "Os requisitos de autenticação são consistentes em todos os endpoints? [Consistency]"
- "Os requisitos de nova tentativa/tempo limite estão definidos para dependências externas? [Coverage, Gap]"
- "A estratégia de versionamento está documentada nos requisitos? [Gap]"

**Qualidade dos requisitos de desempenho:** `performance.md`

Exemplos de itens:

- "Os requisitos de desempenho estão quantificados com métricas específicas? [Clarity]"
- "As metas de desempenho estão definidas para todas as jornadas críticas do usuário? [Coverage]"
- "Os requisitos de desempenho estão especificados para diferentes condições de carga? [Completeness]"
- "Os requisitos de desempenho podem ser medidos objetivamente? [Measurability]"
- "Os requisitos de degradação estão definidos para cenários de carga alta? [Edge Case, Gap]"

**Qualidade dos requisitos de segurança:** `security.md`

Exemplos de itens:

- "Os requisitos de autenticação estão especificados para todos os recursos protegidos? [Coverage]"
- "Os requisitos de proteção de dados estão definidos para informações confidenciais? [Completeness]"
- "O modelo de ameaças está documentado e os requisitos estão alinhados a ele? [Traceability]"
- "Os requisitos de segurança são consistentes com as obrigações de conformidade? [Consistency]"
- "Os requisitos de resposta a falhas/incidentes de segurança estão definidos? [Gap, Exception Flow]"

## Exemplos incorretos: o que NÃO fazer

**❌ INCORRETO — Estes itens testam a implementação, não os requisitos:**

```markdown
- [ ] CHK001 - Verifique se a página inicial exibe 3 cartões de episódios [Spec §FR-001]
- [ ] CHK002 - Teste se os estados ao passar o cursor funcionam corretamente no desktop [Spec §FR-003]
- [ ] CHK003 - Confirme se clicar no logotipo leva à página inicial [Spec §FR-010]
- [ ] CHK004 - Confira se a seção de episódios relacionados mostra de 3 a 5 itens [Spec §FR-005]
```

**✅ CORRETO — Estes itens testam a qualidade dos requisitos:**

```markdown
- [ ] CHK001 - A quantidade e o layout dos episódios em destaque estão especificados explicitamente? [Completeness, Spec §FR-001]
- [ ] CHK002 - Os requisitos de estado ao passar o cursor estão definidos de forma consistente para todos os elementos interativos? [Consistency, Spec §FR-003]
- [ ] CHK003 - Os requisitos de navegação estão claros para todos os elementos clicáveis da marca? [Clarity, Spec §FR-010]
- [ ] CHK004 - Os critérios de seleção de episódios relacionados estão documentados? [Gap, Spec §FR-005]
- [ ] CHK005 - Os requisitos de estado de carregamento estão definidos para dados assíncronos de episódios? [Gap]
- [ ] CHK006 - Os requisitos de "hierarquia visual" podem ser medidos objetivamente? [Measurability, Spec §FR-001]
```

**Principais diferenças:**

- Incorreto: testa se o sistema funciona corretamente.
- Correto: testa se os requisitos foram redigidos corretamente.
- Incorreto: verifica o comportamento.
- Correto: valida a qualidade dos requisitos.
- Incorreto: "Ele faz X?"
- Correto: "X está especificado claramente?"

## Verificações pós-execução

**Verifique os hooks de extensão (após gerar a lista)**:
Verifique se `.specify/extensions.yml` existe na raiz do projeto.
- Se existir, leia-o e procure entradas na chave `hooks.after_checklist`.
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
