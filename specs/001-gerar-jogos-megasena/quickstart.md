# Início Rápido: Gerar jogos da Mega-Sena

## Pré-requisitos

- SDK .NET 10 instalado.
- Repositório com os projetos de aplicação e testes descritos no [plano](./plan.md).

## Validar automaticamente

Na raiz do repositório, executar:

```powershell
dotnet test .\tests\Loterias.Premiadas.Tests\Loterias.Premiadas.Tests.csproj --collect:"XPlat Code Coverage"
```

Resultado esperado: todos os testes passam e o coletor produz relatório Cobertura no diretório de resultados de teste (`TestResults`), com cobertura de 100% de linhas e ramificações do código executável da aplicação.

Os testes devem cobrir, no mínimo, o ponto de entrada e a interação por fluxos de console redirecionados, entradas válidas e inválidas, limites 1 e 1.000, critérios de composição e descarte, ordenação, substituição de candidatas rejeitadas e quantidade final. A instância aleatória com semente fixa deve tornar reproduzíveis os cenários que dependem de geração.

## Validar manualmente

Na raiz do repositório, iniciar a aplicação:

```powershell
dotnet run --project .\src\Loterias.Premiadas\Loterias.Premiadas.csproj
```

1. Informar `0`, texto ou `1001`; confirmar que a aplicação informa o intervalo permitido e solicita novamente a quantidade.
2. Informar `1`; confirmar que exatamente um jogo aparece em uma linha, com seis números ordenados e distintos.
3. Informar `1000`; confirmar que exatamente 1.000 jogos válidos são apresentados.
4. Encerrar a entrada antes de fornecer um valor válido; confirmar que a aplicação termina sem gerar jogos.

Consultar o [contrato de console](./contracts/console.md) para formato e comportamento da interação e o [modelo de dados](./data-model.md) para as regras de validação.
