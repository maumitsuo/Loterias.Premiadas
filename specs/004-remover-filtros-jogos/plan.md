# Plano de Implementação: Remover filtros de validação dos jogos

**Branch**: `004-remover-filtros-jogos` | **Data**: 2026-11-24 | **Especificação**: [spec.md](spec.md)

**Entrada**: Especificação da funcionalidade em `specs/004-remover-filtros-jogos/spec.md`

## Resumo

Retirar todos os critérios estatísticos de aceitação dos jogos da Mega-Sena (paridade, grupos, soma, algarismo final e números consecutivos) e da Lotofácil (paridade, moldura/miolo e grupos sequenciais). Cada jogo passa a ser apenas uma combinação de números distintos no intervalo da modalidade, em ordem crescente. Tecnicamente, os serviços deixam de sortear candidatas e descartá-las: cada jogo sorteado é aceito diretamente. A assinatura pública `Gerar(int quantidade, Random aleatorio)` é mantida, então `Program.cs` e a interface de console não mudam.

## Contexto Técnico

**Linguagem/Versão**: C# / .NET 10

**Dependências principais**: Nenhuma nova. Spectre.Console 0.57.2 (já existente, não afetado).

**Armazenamento**: N/A; os jogos existem apenas em memória durante a execução.

**Testes**: xUnit 2.9.3 com coverlet.collector 6.0.4; manter 100% de cobertura de linhas e ramificações do código executável da aplicação.

**Plataforma-alvo**: Aplicação de console .NET 10.

**Tipo de projeto**: Aplicação de console.

**Metas de desempenho**: Gerar 1.000 jogos sem espera perceptível; sem descartes, o custo passa a ser proporcional à quantidade pedida.

**Restrições**: Não alterar a validação de entrada (modalidade e quantidade de 1 a 1.000), a ordenação crescente nem a interface de console; jogos iguais na mesma solicitação continuam permitidos.

**Escala/Escopo**: Dois serviços (`MegasenaService`, `LotofacilService`) e seus testes.

## Verificação da Constituição

*GATE: deve passar antes da Fase 0. Reavaliada após a Fase 1.*

| Princípio | Estado | Justificativa |
|-----------|--------|---------------|
| I. Aplicação de console simples | Aprovado | A mudança remove código; nenhuma camada ou abstração é adicionada. |
| II. Dados transitórios | Aprovado | Nada passa a ser persistido. |
| III. Nomenclatura em português brasileiro | Aprovado | Nenhum identificador novo em inglês; os nomes existentes são mantidos. |
| IV. Cobertura integral de testes | Aprovado com critério | Os testes das regras removidas serão substituídos por testes que comprovem a aceitação de combinações antes rejeitadas e o descarte de números repetidos, mantendo 100% de linhas e ramificações. |

**Resultado inicial**: Aprovado. Nenhuma exceção necessária.

## Estrutura do Projeto

### Documentação (desta funcionalidade)

```text
specs/004-remover-filtros-jogos/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── geracao.md
├── checklists/
│   └── requirements.md
└── tasks.md             # gerado por /speckit-tasks
```

### Código-fonte (raiz do repositório)

```text
src/
└── Loterias.Premiadas/
    ├── Program.cs                     # sem alteração
    └── Services/
        ├── LotofacilService.cs        # remover EhValida, EhDaMoldura, ObterGrupoSequencial e o laço de descarte
        └── MegasenaService.cs         # remover EhValida, ObterGrupo e o laço de descarte

tests/
└── Loterias.Premiadas.Tests/
    ├── ProgramaTests.cs               # sem alteração esperada
    └── Tests/
        ├── LotofacilTests.cs          # trocar testes de regras por testes da nova regra
        └── MegasenaTests.cs           # trocar testes de regras por testes da nova regra
```

**Decisão de estrutura**: Manter os dois projetos e os arquivos existentes. A mudança fica restrita aos dois serviços de geração e aos testes deles.

## Projeto da Fase 1

- Cada serviço mantém a validação dos argumentos (`aleatorio` não nulo; `quantidade` entre 1 e 1.000) e as mesmas exceções.
- `Gerar` passa a adicionar ao resultado cada jogo retornado por `GerarCandidata` (que pode ser renomeado para `GerarJogo`), sem verificação adicional.
- O sorteio de cada jogo continua ignorando números já sorteados naquele jogo e ordena o resultado.
- Os testes usam o `AleatorioControlado` existente para provar, de forma determinística, que combinações antes rejeitadas agora são aceitas e que números repetidos são ignorados.
- Contrato de comportamento: [contracts/geracao.md](contracts/geracao.md). Modelo: [data-model.md](data-model.md).

## Verificação da Constituição após o projeto

| Princípio | Estado | Verificação |
|-----------|--------|-------------|
| I. Aplicação de console simples | Aprovado | Menos código e o mesmo número de componentes. |
| II. Dados transitórios | Aprovado | Sem persistência. |
| III. Nomenclatura em português brasileiro | Aprovado | Nomes próprios em português. |
| IV. Cobertura integral de testes | Aprovado com critério | Os ramos restantes (argumento nulo, quantidade fora do intervalo, número repetido ignorado, número novo aceito) são todos exercitados pelos testes planejados. |

**Resultado final**: Aprovado, sujeito à verificação do relatório de cobertura na implementação.

## Acompanhamento da Complexidade

Não há violações constitucionais a justificar.
