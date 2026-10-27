# Validação rápida: Modernizar a interface do console

## Pré-requisitos

- .NET 10 SDK instalado.
- Execução a partir da raiz do repositório.

## Executar os testes

```powershell
dotnet test .\tests\Loterias.Premiadas.Tests\Loterias.Premiadas.Tests.csproj
```

**Resultado esperado**: testes passam, incluindo cobertura dos fluxos interativos e textuais e relatório de cobertura integral do código executável da aplicação.

**Falha simulada de geração**: a suíte deve injetar uma função geradora que lance uma exceção e verificar que o fluxo mostra uma mensagem de erro, não exibe jogos parciais e retorna à seleção de uma nova solicitação.

## Executar a aplicação em terminal interativo

```powershell
dotnet run --project .\src\Loterias.Premiadas\Loterias.Premiadas.csproj
```

**Resultado esperado**:

1. O terminal apresenta seleção da modalidade e solicitação validada da quantidade.
2. Entrada inválida recebe orientação e pode ser corrigida sem reiniciar.
3. Os jogos aparecem identificados e organizados, com modalidade e quantidade visíveis.
4. Após o resultado, é possível gerar novamente ou encerrar.

## Verificar fluxo textual com entrada redirecionada

Forneça modalidade, quantidade e opção de saída pelo fluxo padrão de entrada, sem usar prompts de cursor. O resultado deve permanecer legível e não conter códigos ANSI obrigatórios.

**Resultado esperado**: a entrada sequencial é aceita, os resultados são exibidos sem cor ou interação de cursor e não há espera por seleção interativa.

## Verificar casos essenciais

- Quantidade `1` e `1000` são aceitas; `0`, `1001`, texto e entrada vazia são rejeitadas com orientação.
- As duas modalidades continuam apresentando jogos segundo suas regras atuais.
- Uma nova solicitação ocorre somente após a opção de continuar.
- Entrada encerrada antes de completar uma solicitação não produz mensagem de sucesso nem resultado parcial anunciado como completo.
- Falha da geração exibe erro, descarta qualquer resultado parcial e retorna ao início da solicitação.

Consulte [contrato do console](contracts/console.md) e [modelo de dados](data-model.md) para os detalhes dos estados e resultados esperados.
