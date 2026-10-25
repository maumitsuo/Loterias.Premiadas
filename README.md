# Loterias Premiadas

Aplicação de console em C# para gerar combinações de jogos da Mega-Sena e da Lotofácil. Este projeto também registra meu estudo e minha primeira experiência desenvolvendo um aplicativo com **Spec-Driven Development (SDD)**, utilizando o [GitHub Spec Kit](https://github.com/github/spec-kit).

> **Aviso:** os jogos são combinações aleatórias filtradas por critérios definidos neste projeto. Isso não prevê resultados, não aumenta a probabilidade de ganhar e não garante premiação.

## Funcionalidades

- Escolher entre Mega-Sena e Lotofácil.
- Solicitar de 1 a 1.000 jogos por execução.
- Gerar números distintos e em ordem crescente, substituindo combinações que não atendam aos critérios da modalidade.
- Receber mensagens de validação e tentar novamente quando a modalidade ou a quantidade informada for inválida.

### Critérios aplicados

**Mega-Sena**

- Seis números distintos entre 1 e 60.
- A combinação precisa atender a pelo menos duas destas regras: ter de 2 a 4 números pares; contemplar pelo menos 3 grupos numéricos; ou ter soma entre 150 e 220.
- São descartadas combinações com mais de dois números terminados no mesmo algarismo ou com uma sequência de três ou mais números consecutivos.

**Lotofácil**

- Quinze números distintos entre 1 e 25.
- De 7 a 9 números ímpares.
- De 9 a 11 números da moldura.
- Pelo menos um número de cada um dos cinco grupos sequenciais: 01–05, 06–10, 11–15, 16–20 e 21–25.

## Requisitos

- SDK do .NET 10.

## Executar

Na raiz do repositório, inicie a aplicação:

```powershell
dotnet run --project .\src\Loterias.Premiadas\Loterias.Premiadas.csproj
```

Escolha `1` para Mega-Sena ou `2` para Lotofácil e, em seguida, informe quantos jogos deseja gerar (de 1 a 1.000).

## Testes

Execute os testes automatizados da solution:

```powershell
dotnet test .\Loterias.Premiadas.sln
```

Os projetos de teste usam xUnit. Para gerar um relatório de cobertura com Coverlet:

```powershell
dotnet test .\tests\Loterias.Premiadas.Tests\Loterias.Premiadas.Tests.csproj --collect:"XPlat Code Coverage"
```

## Organização do repositório

```text
src/
  Loterias.Premiadas/       Aplicação de console e serviços de geração
tests/
  Loterias.Premiadas.Tests/ Testes automatizados
specs/
  001-gerar-jogos-megasena/
  002-gerar-jogos-lotofacil/
.specify/
  memory/constitution.md    Princípios e restrições do projeto
  templates/                Modelos dos artefatos SDD
.github/skills/             Skills Speckit integradas ao repositório
prompts/                    Prompts de apoio usados no estudo
```

## Estudo de SDD com Spec Kit

O desenvolvimento foi organizado em torno de especificações versionadas em `specs/`. Para cada funcionalidade, os artefatos registram os requisitos e cenários de aceitação (`spec.md`), decisões e estrutura técnica (`plan.md`), tarefas de implementação (`tasks.md`), além de pesquisa, modelo de dados, contratos de console, checklists e instruções de início rápido.

A integração do Spec Kit no repositório inclui a constituição do projeto, modelos, scripts e skills de fluxo, como especificação, planejamento, geração de tarefas, implementação e análise. Assim, o repositório serve também como referência prática do ciclo SDD: definir o comportamento esperado, planejar a solução, implementar e validar contra os critérios registrados.

## Tecnologias

- C# e .NET 10
- xUnit
- Coverlet
- GitHub Spec Kit

## Licença

Este projeto está sob a licença MIT. Consulte [LICENSE](./LICENSE).
