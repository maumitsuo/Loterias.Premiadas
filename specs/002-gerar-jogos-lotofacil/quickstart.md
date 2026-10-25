# Início Rápido: Gerar jogos da Lotofácil

## Pré-requisitos

- SDK .NET 10 instalado.
- Projetos de aplicação e testes existentes no repositório.

## Validar automaticamente

Na raiz do repositório, executar:

```powershell
dotnet test .\tests\Loterias.Premiadas.Tests\Loterias.Premiadas.Tests.csproj --collect:"XPlat Code Coverage"
```

Resultado esperado: todos os testes passam e o coletor produz relatório no diretório de resultados de teste (`TestResults`), demonstrando 100% de cobertura de linhas e ramificações do código executável da aplicação.

Os testes devem cobrir a seleção e validação de modalidade, os limites de quantidade, o comportamento de fim de entrada, os critérios de ambas as modalidades, a ordenação, a substituição de candidatas inválidas e a quantidade final. Usar fontes aleatórias controladas para tornar reproduzíveis os cenários de geração.

## Validar manualmente

Na raiz do repositório, iniciar a aplicação:

```powershell
dotnet run --project .\src\Loterias.Premiadas\Loterias.Premiadas.csproj
```

1. Informar uma modalidade inválida e depois uma modalidade válida; confirmar que a aplicação solicita novamente a escolha antes de pedir a quantidade.
2. Selecionar Lotofácil e informar `1`; confirmar que exatamente um jogo aparece, com 15 números ordenados e distintos, obedecendo paridade, moldura e grupos.
3. Selecionar Mega-Sena e informar `1`; confirmar que exatamente um jogo aparece com seis números ordenados e válidos segundo os critérios da Mega-Sena.
4. Informar uma quantidade inválida e depois `1`; confirmar que a aplicação explica o intervalo permitido e solicita novamente a quantidade.
5. Informar `1000` como quantidade válida; confirmar que são apresentados exatamente 1.000 jogos válidos.
6. Encerrar a entrada antes de concluir uma solicitação válida; confirmar que a aplicação termina sem apresentar jogos.

Consultar o [contrato de console](./contracts/console.md) para comportamento da interação e o [modelo de dados](./data-model.md) para regras de validação.
