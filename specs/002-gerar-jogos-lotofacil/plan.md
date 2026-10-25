# Plano de Implementação: Gerar jogos da Lotofácil

**Branch**: `002-gerar-jogos-lotofacil` | **Data**: 2026-10-25 | **Especificação**: [spec.md](./spec.md)

**Entrada**: Especificação da funcionalidade em `specs/002-gerar-jogos-lotofacil/spec.md`

## Resumo

Estender a aplicação de console existente para permitir escolher Mega-Sena ou Lotofácil e solicitar de 1 a 1.000 jogos. A geração da Mega-Sena permanece conforme os critérios já implementados; a Lotofácil produz 15 números distintos de 1 a 25 e rejeita combinações que não atendam às regras de paridade, moldura/miolo e cobertura dos cinco grupos sequenciais. Os dados permanecem em memória. A solução reutiliza C#/.NET 10, a biblioteca base e a estrutura de testes xUnit/Coverlet existentes.

## Contexto Técnico

**Linguagem/Versão**: C# / .NET 10

**Dependências principais**: .NET Base Class Library; xUnit e Coverlet no projeto de testes existente

**Armazenamento**: N/A; solicitações e jogos permanecem em memória durante a execução

**Testes**: Testes automatizados com xUnit e relatório de cobertura de linhas e ramificações; meta constitucional de 100% do código executável da aplicação

**Plataforma-alvo**: Aplicação de console multiplataforma executada em .NET 10

**Tipo de projeto**: Aplicação CLI de console existente

**Metas de desempenho**: A especificação limita cada solicitação a 1.000 jogos e não define prazo de resposta ou throughput.

**Restrições**: Interação pelo console; modalidades Mega-Sena e Lotofácil; quantidade de 1 a 1.000; sem persistência ou serviços externos; nomes declarados em português brasileiro, ressalvadas convenções obrigatórias da plataforma.

**Escala/Escopo**: Uma solicitação interativa por execução, com até 1.000 jogos gerados e apresentados em memória.

## Verificação da Constituição

### Antes da pesquisa

- **Aplicação de console simples**: APROVADO. A alteração amplia o fluxo existente e adiciona apenas a geração e validação necessárias para a Lotofácil.
- **Dados transitórios**: APROVADO. A modalidade, quantidade, candidatas e jogos são mantidos somente durante a execução.
- **Nomenclatura em português brasileiro**: APROVADO. Tipos, membros e mensagens novos usarão português brasileiro, exceto identificadores exigidos pelas APIs ou convenções da plataforma.
- **Cobertura integral de testes**: APROVADO COMO CRITÉRIO DE PROJETO. Os testes existentes serão preservados e ampliados para cobrir seleção, validação, filtros da Lotofácil, repetição de candidatas e saída; o relatório deve demonstrar 100% de linhas e ramificações do código executável da aplicação.
- **Critérios de aprovação**: nenhuma violação identificada; não é necessária exceção ou justificativa de complexidade.

### Após o projeto

- **Aplicação de console simples**: APROVADO. A solução reutiliza o ponto de entrada e o gerador existentes e adiciona apenas a lógica de domínio e testes específicos da nova modalidade.
- **Dados transitórios**: APROVADO. Nenhuma entidade é persistida e não há integrações ou armazenamento externos.
- **Nomenclatura em português brasileiro**: APROVADO. Os símbolos e mensagens da funcionalidade usam português; nomes impostos pela plataforma e bibliotecas permanecem conforme suas APIs.
- **Cobertura integral de testes**: APROVADO COMO REQUISITO DE IMPLEMENTAÇÃO. Os critérios de cada modalidade, os limites de entrada e os caminhos da interação devem ser cobertos, mantendo relatório completo de linhas e ramificações.
- **Critérios de aprovação**: aprovado; nenhuma exceção é necessária.

## Estrutura do Projeto

### Documentação desta funcionalidade

```text
specs/002-gerar-jogos-lotofacil/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    └── console.md
```

### Código-fonte

```text
src/Loterias.Premiadas/
├── Loterias.Premiadas.csproj
├── Program.cs
├── GeradorDeJogos.cs
└── GeradorDeJogosLotofacil.cs

tests/Loterias.Premiadas.Tests/
├── Loterias.Premiadas.Tests.csproj
├── ProgramaTests.cs
├── GeradorDeJogosTests.cs
└── GeradorDeJogosLotofacilTests.cs
```

**Decisão de estrutura**: o repositório já contém uma aplicação .NET 10 e um projeto xUnit/Coverlet com testes do gerador da Mega-Sena. Preservar o comportamento da Mega-Sena, adaptar o ponto de entrada para solicitar a modalidade antes da quantidade e acrescentar uma unidade de geração da Lotofácil com testes próprios. Evitar projetos adicionais, camadas, persistência e abstrações sem necessidade demonstrável. Fornecer uma fonte `Random` controlável aos testes para reproduzir a geração e a substituição de candidatas.

## Acompanhamento da Complexidade

Não há violações da constituição que exijam justificativa. A separação dos testes por modalidade mantém a lógica existente da Mega-Sena isolada da nova validação Lotofácil, sem adicionar serviços ou dependências de produção.
