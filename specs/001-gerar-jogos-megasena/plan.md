# Plano de Implementação: Gerar jogos da Mega-Sena

**Branch**: `001-gerar-jogos-megasena` | **Data**: 2026-10-25 | **Especificação**: [spec.md](./spec.md)

**Entrada**: Especificação da funcionalidade em `specs/001-gerar-jogos-megasena/spec.md`

## Resumo

Aplicação de console que solicita uma quantidade entre 1 e 1.000, gera combinações de seis números distintos de 1 a 60 e substitui candidatas que não atendam aos critérios da especificação. A solução será uma aplicação C#/.NET 10 pequena, sem persistência ou serviços externos. A lógica de geração receberá uma fonte `Random` para permitir testes reproduzíveis, e os testes automatizados demonstrarão 100% de cobertura de linhas e ramificações conforme a constituição.

## Contexto Técnico

**Linguagem/Versão**: C# / .NET 10

**Dependências principais**: .NET Base Class Library; xUnit e Coverlet para testes e relatório de cobertura

**Armazenamento**: N/A; solicitações e resultados permanecem em memória durante a execução

**Testes**: Testes automatizados com xUnit e relatório de cobertura de linhas e ramificações; meta de 100% para o código executável do aplicativo

**Plataforma-alvo**: Aplicação de console multiplataforma executada em .NET 10

**Tipo de projeto**: Aplicação CLI de console

**Metas de desempenho**: A especificação não define uma meta de latência; a aplicação deve gerar até 1.000 jogos em uma única solicitação, sem critério temporal adicional.

**Restrições**: Interação por console; quantidade de 1 a 1.000; sem persistência, integrações externas ou promessa de aumento da probabilidade de premiação; nomes declarados em português brasileiro, ressalvadas convenções exigidas pela plataforma.

**Escala/Escopo**: Uma solicitação por execução interativa, com até 1.000 jogos apresentados em memória.

## Verificação da Constituição

### Antes da pesquisa

- **Aplicação de console simples**: APROVADO. Uma aplicação e uma unidade de lógica de geração bastam para o escopo; não são necessárias camadas ou serviços.
- **Dados transitórios**: APROVADO. Os jogos e a quantidade solicitada existem somente em memória durante a execução.
- **Nomenclatura em português brasileiro**: APROVADO. Tipos, membros e mensagens da aplicação usarão português brasileiro, exceto nomes exigidos pela plataforma ou convenções técnicas estabelecidas.
- **Cobertura integral de testes**: APROVADO COMO CRITÉRIO DE PROJETO. O projeto de testes verificará 100% de linhas e ramificações do código executável do aplicativo e produzirá relatório de cobertura.
- **Critérios de aprovação**: nenhuma violação da constituição identificada; não é necessário acompanhamento de complexidade.

### Após o projeto

- **Aplicação de console simples**: APROVADO. A solução mantém a interação no console e limita o código de produção ao ponto de entrada e à lógica necessária para gerar e validar jogos.
- **Dados transitórios**: APROVADO. Não há armazenamento nem dependência de serviços externos.
- **Nomenclatura em português brasileiro**: APROVADO. Os nomes da aplicação e do domínio serão em português; nomes impostos pelo runtime e bibliotecas permanecem conforme suas APIs.
- **Cobertura integral de testes**: APROVADO COMO REQUISITO DE IMPLEMENTAÇÃO. Os testes cobrirão também a validação e os caminhos de repetição da interação, com relatório de linhas e ramificações.
- **Critérios de aprovação**: aprovado; nenhuma exceção ou justificativa de complexidade é necessária.

## Estrutura do Projeto

### Documentação desta funcionalidade

```text
specs/001-gerar-jogos-megasena/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    └── console.md
```

### Código-fonte proposto

```text
Loterias.Premiadas.sln
src/
└── Loterias.Premiadas/
    ├── Loterias.Premiadas.csproj
    ├── Program.cs
    └── GeradorDeJogos.cs
tests/
└── Loterias.Premiadas.Tests/
    ├── Loterias.Premiadas.Tests.csproj
    ├── ProgramaTests.cs
    └── GeradorDeJogosTests.cs
```

**Decisão de estrutura**: repositório sem projetos de código existentes; criar uma aplicação .NET 10 e um projeto de testes. Manter a interação no ponto de entrada e a geração/validação em uma classe de domínio pequena. Testar a interação pela entrada de console com fluxos redirecionados, inclusive o ponto de entrada, e fornecer um `Random` com semente fixa aos testes do gerador, evitando abstrações de aleatoriedade adicionais e garantindo reprodutibilidade.

## Acompanhamento da Complexidade

Não há violações da constituição que exijam justificativa. A injeção da instância `Random` é o único ponto de controle para testes determinísticos, sem introduzir serviço ou dependência externa.
