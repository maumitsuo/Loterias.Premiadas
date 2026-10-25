---

description: "Modelo de lista de tarefas para implementação de funcionalidades"
---

# Tarefas: [NOME DA FUNCIONALIDADE]

**Entrada**: Documentos de design em `/specs/[###-feature-name]/`

**Pré-requisitos**: plan.md (obrigatório), spec.md (obrigatório para histórias de usuário), research.md, data-model.md, contracts/

**Testes**: os exemplos abaixo incluem tarefas de teste. Testes são OPCIONAIS — inclua-os somente se forem explicitamente solicitados na especificação da funcionalidade.

**Organização**: as tarefas são agrupadas por história de usuário para permitir a implementação e o teste independentes de cada história.

## Formato: `[ID] [P?] [História] Descrição`

- **[P]**: pode ser executada em paralelo (arquivos diferentes, sem dependências)
- **[Story]**: a qual história de usuário esta tarefa pertence (por exemplo, US1, US2, US3)
- Inclua os caminhos exatos dos arquivos nas descrições

## Convenções de Caminhos

- **Projeto único**: `src/`, `tests/` na raiz do repositório
- **Aplicação web**: `backend/src/`, `frontend/src/`
- **Aplicativo móvel**: `api/src/`, `ios/src/` ou `android/src/`
- Os caminhos abaixo pressupõem um projeto único — ajuste-os conforme a estrutura definida em plan.md

<!--
  ============================================================================
  IMPORTANTE: as tarefas abaixo são apenas EXEMPLOS.

  O comando /speckit-tasks DEVE substituí-las por tarefas reais, com base em:
  - Histórias de usuário de spec.md (com prioridades P1, P2, P3...)
  - Requisitos da funcionalidade em plan.md
  - Entidades de data-model.md
  - Endpoints de contracts/

  As tarefas DEVEM ser organizadas por história de usuário para que cada história possa ser:
  - Implementada de forma independente
  - Testada de forma independente
  - Entregue como um incremento do MVP

  NÃO mantenha estas tarefas de exemplo no arquivo tasks.md gerado.
  ============================================================================
-->

## Fase 1: Preparação (Infraestrutura Compartilhada)

**Objetivo**: inicialização do projeto e estrutura básica

- [ ] T001 Criar a estrutura do projeto conforme o plano de implementação
- [ ] T002 Inicializar o projeto [linguagem] com as dependências de [framework]
- [ ] T003 [P] Configurar ferramentas de lint e formatação

---

## Fase 2: Fundação (Pré-requisitos Bloqueadores)

**Objetivo**: infraestrutura essencial que DEVE estar concluída antes da implementação de QUALQUER história de usuário

**⚠️ CRÍTICO**: o trabalho nas histórias de usuário não pode começar antes da conclusão desta fase

Exemplos de tarefas fundamentais (ajuste conforme o projeto):

- [ ] T004 Configurar o esquema do banco de dados e a estrutura de migrações
- [ ] T005 [P] Implementar a estrutura de autenticação/autorização
- [ ] T006 [P] Configurar o roteamento da API e a estrutura de middleware
- [ ] T007 Criar modelos/entidades base dos quais todas as histórias dependem
- [ ] T008 Configurar a infraestrutura de tratamento de erros e registro de logs
- [ ] T009 Configurar o gerenciamento das configurações de ambiente

**Ponto de verificação**: fundação pronta — a implementação das histórias de usuário pode começar em paralelo

---

## Fase 3: História de Usuário 1 - [Título] (Prioridade: P1) 🎯 MVP

**Objetivo**: [Breve descrição do que esta história entrega]

**Teste independente**: [Como verificar que esta história funciona por conta própria]

### Testes da História de Usuário 1 (OPCIONAIS — somente se solicitados) ⚠️

> **OBSERVAÇÃO: escreva estes testes PRIMEIRO e confirme que FALHAM antes da implementação**

- [ ] T010 [P] [US1] Criar teste de contrato para [endpoint] em tests/contract/test_[name].py
- [ ] T011 [P] [US1] Criar teste de integração para [jornada do usuário] em tests/integration/test_[name].py

### Implementação da História de Usuário 1

- [ ] T012 [P] [US1] Criar o modelo [Entity1] em src/models/[entity1].py
- [ ] T013 [P] [US1] Criar o modelo [Entity2] em src/models/[entity2].py
- [ ] T014 [US1] Implementar [Service] em src/services/[service].py (depende de T012 e T013)
- [ ] T015 [US1] Implementar [endpoint/funcionalidade] em src/[location]/[file].py
- [ ] T016 [US1] Adicionar validação e tratamento de erros
- [ ] T017 [US1] Adicionar registro de logs para as operações da história de usuário 1

**Ponto de verificação**: neste ponto, a História de Usuário 1 deve estar totalmente funcional e ser testável de forma independente

---

## Fase 4: História de Usuário 2 - [Título] (Prioridade: P2)

**Objetivo**: [Breve descrição do que esta história entrega]

**Teste independente**: [Como verificar que esta história funciona por conta própria]

### Testes da História de Usuário 2 (OPCIONAIS — somente se solicitados) ⚠️

- [ ] T018 [P] [US2] Criar teste de contrato para [endpoint] em tests/contract/test_[name].py
- [ ] T019 [P] [US2] Criar teste de integração para [jornada do usuário] em tests/integration/test_[name].py

### Implementação da História de Usuário 2

- [ ] T020 [P] [US2] Criar o modelo [Entity] em src/models/[entity].py
- [ ] T021 [US2] Implementar [Service] em src/services/[service].py
- [ ] T022 [US2] Implementar [endpoint/funcionalidade] em src/[location]/[file].py
- [ ] T023 [US2] Integrar com os componentes da História de Usuário 1 (se necessário)

**Ponto de verificação**: neste ponto, as Histórias de Usuário 1 E 2 devem funcionar de forma independente

---

## Fase 5: História de Usuário 3 - [Título] (Prioridade: P3)

**Objetivo**: [Breve descrição do que esta história entrega]

**Teste independente**: [Como verificar que esta história funciona por conta própria]

### Testes da História de Usuário 3 (OPCIONAIS — somente se solicitados) ⚠️

- [ ] T024 [P] [US3] Criar teste de contrato para [endpoint] em tests/contract/test_[name].py
- [ ] T025 [P] [US3] Criar teste de integração para [jornada do usuário] em tests/integration/test_[name].py

### Implementação da História de Usuário 3

- [ ] T026 [P] [US3] Criar o modelo [Entity] em src/models/[entity].py
- [ ] T027 [US3] Implementar [Service] em src/services/[service].py
- [ ] T028 [US3] Implementar [endpoint/funcionalidade] em src/[location]/[file].py

**Ponto de verificação**: todas as histórias de usuário devem agora funcionar de forma independente

---

[Adicione outras fases de histórias de usuário conforme necessário, seguindo o mesmo padrão]

---

## Fase N: Acabamento e Aspectos Transversais

**Objetivo**: melhorias que afetam várias histórias de usuário

- [ ] TXXX [P] Atualizar a documentação em docs/
- [ ] TXXX Limpar e refatorar o código
- [ ] TXXX Otimizar o desempenho em todas as histórias
- [ ] TXXX [P] Adicionar testes unitários (se solicitados) em tests/unit/
- [ ] TXXX Reforçar a segurança
- [ ] TXXX Executar a validação de quickstart.md

---

## Dependências e Ordem de Execução

### Dependências entre Fases

- **Preparação (Fase 1)**: sem dependências — pode começar imediatamente
- **Fundação (Fase 2)**: depende da conclusão da Preparação — BLOQUEIA todas as histórias de usuário
- **Histórias de Usuário (Fase 3+)**: todas dependem da conclusão da fase Fundação
  - Depois disso, as histórias podem avançar em paralelo (se houver equipe disponível)
  - Ou em sequência, por ordem de prioridade (P1 → P2 → P3)
- **Acabamento (fase final)**: depende da conclusão de todas as histórias de usuário desejadas

### Dependências entre Histórias de Usuário

- **História de Usuário 1 (P1)**: pode começar após Fundação (Fase 2) — não depende de outras histórias
- **História de Usuário 2 (P2)**: pode começar após Fundação (Fase 2) — pode integrar-se à US1, mas deve ser testável de forma independente
- **História de Usuário 3 (P3)**: pode começar após Fundação (Fase 2) — pode integrar-se à US1/US2, mas deve ser testável de forma independente

### Dentro de Cada História de Usuário

- Os testes (se incluídos) DEVEM ser escritos e FALHAR antes da implementação
- Modelos antes de serviços
- Serviços antes de endpoints
- Implementação principal antes da integração
- Conclua uma história antes de passar para a próxima prioridade

### Oportunidades de Paralelismo

- Todas as tarefas de Preparação marcadas com [P] podem ser executadas em paralelo
- Todas as tarefas Fundamentais marcadas com [P] podem ser executadas em paralelo (dentro da Fase 2)
- Após a conclusão da fase Fundação, todas as histórias de usuário podem começar em paralelo (se a capacidade da equipe permitir)
- Todos os testes de uma história de usuário marcados com [P] podem ser executados em paralelo
- Os modelos de uma história marcados com [P] podem ser criados em paralelo
- Diferentes integrantes da equipe podem trabalhar em paralelo em histórias de usuário distintas

---

## Exemplo de Paralelismo: História de Usuário 1

```bash
# Inicie juntos todos os testes da História de Usuário 1 (se os testes forem solicitados):
Task: "Criar teste de contrato para [endpoint] em tests/contract/test_[name].py"
Task: "Criar teste de integração para [jornada do usuário] em tests/integration/test_[name].py"

# Crie em paralelo todos os modelos da História de Usuário 1:
Task: "Criar o modelo [Entity1] em src/models/[entity1].py"
Task: "Criar o modelo [Entity2] em src/models/[entity2].py"
```

---

## Estratégia de Implementação

### MVP Primeiro (Somente História de Usuário 1)

1. Conclua a Fase 1: Preparação
2. Conclua a Fase 2: Fundação (CRÍTICO — bloqueia todas as histórias)
3. Conclua a Fase 3: História de Usuário 1
4. **PARE e VALIDE**: teste a História de Usuário 1 de forma independente
5. Implante/faça uma demonstração, se estiver pronta

### Entrega Incremental

1. Conclua Preparação + Fundação → base pronta
2. Adicione a História de Usuário 1 → teste de forma independente → implante/faça uma demonstração (MVP!)
3. Adicione a História de Usuário 2 → teste de forma independente → implante/faça uma demonstração
4. Adicione a História de Usuário 3 → teste de forma independente → implante/faça uma demonstração
5. Cada história agrega valor sem prejudicar as anteriores

### Estratégia para Equipe em Paralelo

Com vários desenvolvedores:

1. A equipe conclui Preparação + Fundação em conjunto
2. Após concluir Fundação:
   - Desenvolvedor A: História de Usuário 1
   - Desenvolvedor B: História de Usuário 2
   - Desenvolvedor C: História de Usuário 3
3. As histórias são concluídas e integradas de forma independente

---

## Observações

- Tarefas [P] = arquivos diferentes, sem dependências
- O rótulo [Story] vincula a tarefa a uma história de usuário específica para fins de rastreabilidade
- Cada história de usuário deve poder ser concluída e testada de forma independente
- Confirme que os testes falham antes de implementar
- Faça commit após cada tarefa ou grupo lógico
- Pare em qualquer ponto de verificação para validar a história de forma independente
- Evite: tarefas vagas, conflitos no mesmo arquivo e dependências entre histórias que prejudiquem a independência
