# Plano de Implementação: [FEATURE]

**Branch**: `[###-feature-name]` | **Data**: [DATE] | **Especificação**: [link]

**Entrada**: Especificação da funcionalidade em `/specs/[###-feature-name]/spec.md`

**Observação**: este modelo é preenchido pelo comando `/speckit-plan`; sua definição descreve o fluxo de execução.

## Resumo

[Extraia da especificação da funcionalidade: requisito principal e abordagem técnica da pesquisa]

## Contexto Técnico

<!--
  AÇÃO NECESSÁRIA: substitua o conteúdo desta seção pelos detalhes técnicos
  do projeto. A estrutura é apresentada como orientação para conduzir
  o processo de iteração.
-->

**Linguagem/Versão**: [por exemplo, Python 3.11, Swift 5.9, Rust 1.75 ou NEEDS CLARIFICATION]

**Dependências principais**: [por exemplo, FastAPI, UIKit, LLVM ou NEEDS CLARIFICATION]

**Armazenamento**: [se aplicável, por exemplo, PostgreSQL, CoreData, arquivos ou N/A]

**Testes**: [por exemplo, pytest, XCTest, cargo test ou NEEDS CLARIFICATION]

**Plataforma-alvo**: [por exemplo, servidor Linux, iOS 15+, WASM ou NEEDS CLARIFICATION]

**Tipo de projeto**: [por exemplo, biblioteca/cli/serviço web/aplicativo móvel/compilador/aplicativo desktop ou NEEDS CLARIFICATION]

**Metas de desempenho**: [específicas do domínio, por exemplo, 1000 req/s, 10 mil linhas/s, 60 fps ou NEEDS CLARIFICATION]

**Restrições**: [específicas do domínio, por exemplo, p95 < 200 ms, memória < 100 MB, funcionamento offline ou NEEDS CLARIFICATION]

**Escala/Escopo**: [específicos do domínio, por exemplo, 10 mil usuários, 1 milhão de linhas de código, 50 telas ou NEEDS CLARIFICATION]

## Verificação da Constituição

*CONTROLE: deve ser aprovado antes da pesquisa da Fase 0. Verifique novamente após o design da Fase 1.*

[Controles determinados com base no arquivo da constituição]

## Estrutura do Projeto

### Documentação (desta funcionalidade)

```text
specs/[###-feature]/
├── plan.md              # Este arquivo (saída do comando /speckit-plan)
├── research.md          # Saída da Fase 0 (comando /speckit-plan)
├── data-model.md        # Saída da Fase 1 (comando /speckit-plan)
├── quickstart.md        # Saída da Fase 1 (comando /speckit-plan)
├── contracts/           # Saída da Fase 1 (comando /speckit-plan)
└── tasks.md             # Saída da Fase 2 (comando /speckit-tasks — NÃO criado por /speckit-plan)
```

### Código-fonte (raiz do repositório)
<!--
  AÇÃO NECESSÁRIA: substitua a árvore ilustrativa abaixo pela estrutura concreta
  desta funcionalidade. Exclua opções não utilizadas e expanda a estrutura
  escolhida com caminhos reais (por exemplo, apps/admin, packages/something).
  O plano final não deve incluir os rótulos de opção.
-->

```text
# [REMOVER SE NÃO FOR USADA] Opção 1: Projeto único (PADRÃO)
src/
├── models/
├── services/
├── cli/
└── lib/

tests/
├── contract/
├── integration/
└── unit/

# [REMOVER SE NÃO FOR USADA] Opção 2: Aplicação web (quando "frontend" + "backend" forem detectados)
backend/
├── src/
│   ├── models/
│   ├── services/
│   └── api/
└── tests/

frontend/
├── src/
│   ├── components/
│   ├── pages/
│   └── services/
└── tests/

# [REMOVER SE NÃO FOR USADA] Opção 3: Aplicativo móvel + API (quando "iOS/Android" for detectado)
api/
└── [mesma estrutura de backend acima]

ios/ or android/
└── [estrutura específica da plataforma: módulos de funcionalidade, fluxos de interface e testes da plataforma]
```

**Decisão de estrutura**: [documente a estrutura selecionada e faça referência aos
diretórios reais listados acima]

## Acompanhamento da Complexidade

> **Preencha SOMENTE se a Verificação da Constituição apontar violações que precisem ser justificadas**

| Violação | Por que é necessária | Alternativa mais simples rejeitada porque |
|-----------|------------|-------------------------------------|
| [por exemplo, 4º projeto] | [necessidade atual] | [por que 3 projetos são insuficientes] |
| [por exemplo, padrão Repository] | [problema específico] | [por que o acesso direto ao banco não é suficiente] |
