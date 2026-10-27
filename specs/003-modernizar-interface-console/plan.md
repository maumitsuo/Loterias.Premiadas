# Plano de Implementação: Modernizar a interface do console

**Branch**: `003-modernizar-interface-console` | **Data**: 2026-10-27 | **Especificação**: [spec.md](spec.md)

**Entrada**: Especificação da funcionalidade em `specs/003-modernizar-interface-console/spec.md`

## Resumo

Modernizar o fluxo de escolha de modalidade, quantidade e repetição da solicitação, e organizar os jogos gerados em uma tabela legível. A aplicação usará Spectre.Console 0.57.2 para prompts e apresentação formatada, mantendo um fluxo textual quando a entrada ou saída estiver redirecionada. A lógica de geração e suas regras permanecem inalteradas.

## Contexto Técnico

**Linguagem/Versão**: C# / .NET 10

**Dependências principais**: Spectre.Console 0.57.2 na aplicação; Spectre.Console.Testing 0.57.2 nos testes.

**Armazenamento**: N/A; solicitações e jogos permanecem apenas em memória durante a execução.

**Testes**: xUnit e Spectre.Console.Testing; manter 100% de cobertura de linhas e ramificações do código executável da aplicação.

**Plataforma-alvo**: Aplicação de console .NET 10; terminais interativos e entrada/saída redirecionada.

**Tipo de projeto**: Aplicação de console.

**Metas de desempenho**: Não há meta de latência específica; o fluxo deve exibir os jogos após a conclusão da geração.

**Restrições**: Preservar regras e resultados da geração existente; continuar legível sem cor; não persistir dados; nomes de código em português brasileiro, exceto nomenclatura exigida por bibliotecas/frameworks.

**Escala/Escopo**: Uma pessoa por processo e de 1 a 1.000 jogos por solicitação; duas modalidades existentes.

## Verificação da Constituição

| Princípio | Estado | Justificativa |
|-----------|--------|---------------|
| I. Aplicação de console simples | Aprovado | Spectre.Console enriquece a interação no console sem adicionar camadas ou serviços. O fluxo textual atende ambientes não interativos. |
| II. Dados transitórios | Aprovado | Solicitação e resultados são mantidos apenas durante o processo. |
| III. Nomenclatura em português brasileiro | Aprovado | Identificadores novos serão escritos em português; nomes de APIs e tipos externos permanecem conforme a biblioteca. |
| IV. Cobertura integral de testes | Aprovado com critério | Os testes devem atingir 100% de linhas e ramificações do código executável da aplicação; bibliotecas externas não fazem parte dessa medição. |

**Resultado inicial**: Aprovado. Nenhuma exceção constitucional é necessária.

## Estrutura do Projeto

### Documentação (desta funcionalidade)

```text
specs/003-modernizar-interface-console/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── console.md
└── tasks.md
```

### Código-fonte (raiz do repositório)

```text
src/
└── Loterias.Premiadas/
    ├── Loterias.Premiadas.csproj
    ├── Program.cs
    └── Services/
        ├── LotofacilService.cs
        └── MegasenaService.cs

tests/
└── Loterias.Premiadas.Tests/
    ├── Loterias.Premiadas.Tests.csproj
    ├── ProgramaTests.cs
    └── Tests/
        ├── LotofacilTests.cs
        └── MegasenaTests.cs
```

**Decisão de estrutura**: Manter os dois projetos existentes. O fluxo de interação e a apresentação continuam na aplicação de console; os testes de console permanecem no projeto xUnit existente. Não criar projetos ou camadas adicionais.

## Projeto da Fase 1

O fluxo de console usará prompts de seleção e confirmação nos terminais interativos, validação da quantidade entre 1 e 1.000 e apresentação dos jogos em tabela com identificação da modalidade e total. Saída e entrada redirecionadas usarão prompts de texto simples, sem sequências ANSI ou prompts interativos dependentes de cursor. Após os resultados, a pessoa poderá iniciar outra solicitação ou encerrar.

O fluxo testável deve aceitar uma instância de console e uma função geradora injetadas no ponto de entrada, permitindo que `TestConsole` valide prompts/saída e que os testes simulem uma falha de geração. A entrada de produção usará o console padrão e conectará a função aos serviços existentes. A função geradora será chamada em uma fronteira delimitada; se ela falhar, o aplicativo exibirá uma mensagem de erro, descartará qualquer resultado parcial e retornará ao início da solicitação. Não haverá mudança nas regras nem nos serviços de geração.

## Verificação da Constituição após o projeto

| Princípio | Estado | Verificação |
|-----------|--------|--------------|
| I. Aplicação de console simples | Aprovado | Apenas a apresentação e a interação do console são modernizadas; não se adicionam abstrações ou serviços desnecessários. |
| II. Dados transitórios | Aprovado | Sem persistência ou integração externa em tempo de execução. |
| III. Nomenclatura em português brasileiro | Aprovado | Aplicar aos identificadores próprios; preservar nomes externos exigidos pela dependência. |
| IV. Cobertura integral de testes | Aprovado com critério | Cobrir fluxos interativos e textuais, entradas inválidas, encerramento, repetição e apresentação, comprovando cobertura de linhas e ramificações do código da aplicação. |

**Resultado final**: Aprovado, sujeito à verificação integral de cobertura durante implementação.

## Acompanhamento da Complexidade

Não há violações constitucionais que precisem ser justificadas.
