# Especificação da Funcionalidade: [NOME DA FUNCIONALIDADE]

**Branch da funcionalidade**: `[###-feature-name]`

**Criada em**: [DATE]

**Status**: Rascunho

**Entrada**: Descrição do usuário: "$ARGUMENTS"

## Cenários de Uso e Testes *(obrigatório)*

<!--
  IMPORTANTE: as histórias de usuário devem ser PRIORIZADAS como jornadas ordenadas
  por importância. Cada história/jornada deve ser TESTÁVEL DE FORMA INDEPENDENTE —
  isto é, implementar apenas UMA delas ainda deve resultar em um MVP (Produto Mínimo
  Viável) que entregue valor.

  Atribua prioridades (P1, P2, P3 etc.) a cada história, sendo P1 a mais crítica.
  Considere cada história uma parte independente da funcionalidade que pode ser:
  - Desenvolvida de forma independente
  - Testada de forma independente
  - Implantada de forma independente
  - Demonstrada aos usuários de forma independente
-->

### História de Usuário 1 - [Título breve] (Prioridade: P1)

[Descreva esta jornada do usuário em linguagem simples]

**Por que esta prioridade**: [Explique o valor e o motivo deste nível de prioridade]

**Teste independente**: [Descreva como testar esta história de forma independente — por exemplo, "Pode ser testada por completo por meio de [ação específica] e entrega [valor específico]"]

**Cenários de Aceitação**:

1. **Dado** [estado inicial], **Quando** [ação], **Então** [resultado esperado]
2. **Dado** [estado inicial], **Quando** [ação], **Então** [resultado esperado]

---

### História de Usuário 2 - [Título breve] (Prioridade: P2)

[Descreva esta jornada do usuário em linguagem simples]

**Por que esta prioridade**: [Explique o valor e o motivo deste nível de prioridade]

**Teste independente**: [Descreva como testar esta história de forma independente]

**Cenários de Aceitação**:

1. **Dado** [estado inicial], **Quando** [ação], **Então** [resultado esperado]

---

### História de Usuário 3 - [Título breve] (Prioridade: P3)

[Descreva esta jornada do usuário em linguagem simples]

**Por que esta prioridade**: [Explique o valor e o motivo deste nível de prioridade]

**Teste independente**: [Descreva como testar esta história de forma independente]

**Cenários de Aceitação**:

1. **Dado** [estado inicial], **Quando** [ação], **Então** [resultado esperado]

---

[Adicione outras histórias de usuário conforme necessário, cada uma com uma prioridade atribuída]

### Casos Limítrofes

<!--
  AÇÃO NECESSÁRIA: o conteúdo desta seção é ilustrativo.
  Preencha-a com os casos limítrofes apropriados.
-->

- O que acontece quando [condição de limite]?
- Como o sistema trata [cenário de erro]?

## Requisitos *(obrigatório)*

<!--
  AÇÃO NECESSÁRIA: o conteúdo desta seção é ilustrativo.
  Preencha-a com os requisitos funcionais apropriados.
-->

### Requisitos Funcionais

- **FR-001**: O sistema DEVE [capacidade específica, por exemplo, "permitir que usuários criem contas"]
- **FR-002**: O sistema DEVE [capacidade específica, por exemplo, "validar endereços de e-mail"]
- **FR-003**: Os usuários DEVEM poder [interação principal, por exemplo, "redefinir a senha"]
- **FR-004**: O sistema DEVE [requisito de dados, por exemplo, "persistir as preferências dos usuários"]
- **FR-005**: O sistema DEVE [comportamento, por exemplo, "registrar todos os eventos de segurança"]

*Exemplo de como marcar requisitos pouco claros:*

- **FR-006**: O sistema DEVE autenticar usuários por meio de [NEEDS CLARIFICATION: método de autenticação não especificado — e-mail/senha, SSO ou OAuth?]
- **FR-007**: O sistema DEVE reter os dados dos usuários por [NEEDS CLARIFICATION: período de retenção não especificado]

### Entidades Principais *(inclua se a funcionalidade envolver dados)*

- **[Entidade 1]**: [O que representa e seus principais atributos, sem detalhes de implementação]
- **[Entidade 2]**: [O que representa e suas relações com outras entidades]

## Critérios de Sucesso *(obrigatório)*

<!--
  AÇÃO NECESSÁRIA: defina critérios de sucesso mensuráveis.
  Eles devem ser independentes de tecnologia e mensuráveis.
-->

### Resultados Mensuráveis

- **SC-001**: [Métrica mensurável, por exemplo, "Usuários conseguem criar uma conta em menos de 2 minutos"]
- **SC-002**: [Métrica mensurável, por exemplo, "O sistema atende 1.000 usuários simultâneos sem degradação"]
- **SC-003**: [Métrica de satisfação, por exemplo, "90% dos usuários concluem a tarefa principal na primeira tentativa"]
- **SC-004**: [Métrica de negócio, por exemplo, "Reduzir em 50% os chamados de suporte relacionados a [X]"]

## Premissas

<!--
  AÇÃO NECESSÁRIA: o conteúdo desta seção é ilustrativo.
  Preencha-a com as premissas corretas, baseadas em padrões razoáveis
  adotados quando a descrição da funcionalidade não especificar certos detalhes.
-->

- [Premissa sobre os usuários-alvo, por exemplo, "Os usuários têm conexão estável com a internet"]
- [Premissa sobre os limites do escopo, por exemplo, "O suporte a dispositivos móveis está fora do escopo da versão 1"]
- [Premissa sobre dados/ambiente, por exemplo, "O sistema de autenticação existente será reutilizado"]
- [Dependência de sistema/serviço existente, por exemplo, "Requer acesso à API de perfil de usuário existente"]
