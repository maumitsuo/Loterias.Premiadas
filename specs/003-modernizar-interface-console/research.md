# Pesquisa: Modernizar a interface do console

**Data**: 2026-10-27

## Spectre.Console para prompts interativos

- **Decisão**: Usar os prompts de seleção, texto validado e confirmação do Spectre.Console para escolha de modalidade, quantidade e decisão após os resultados.
- **Justificativa**: A biblioteca oferece os controles próprios para seleção e confirmação no terminal, com validação vinculada à solicitação; isso evita recriar interações com cursor e entrada manual.
- **Alternativas consideradas**: Continuar usando somente `Console.ReadLine()`; descartada para o caminho interativo por não oferecer seleção ou orientação visual.
- **Fontes**: [Prompts de entrada](https://spectreconsole.net/console/how-to/prompting-for-user-input/), [SelectionPrompt](https://spectreconsole.net/console/prompts/selection-prompt/).

## Apresentação dos resultados

- **Decisão**: Exibir identificação da modalidade e total, seguida por uma tabela com uma linha identificada por jogo e números em ordem crescente.
- **Justificativa**: Tabelas e painéis são renderizáveis pelo console da biblioteca, permitem alinhamento e delimitam cada jogo visualmente. Usar bordas ASCII ou formatação simples como alternativa quando apropriado ao ambiente.
- **Alternativas consideradas**: Linhas simples formatadas manualmente; descartadas como apresentação principal por exigirem controle próprio de largura e alinhamento.
- **Fontes**: [Widget Table](https://spectreconsole.net/console/widgets/table/), [Painéis e grades](https://spectreconsole.net/console/how-to/organizing-layout-with-panels-and-grids/).

## Terminais sem interação ou cor

- **Decisão**: Aproveitar detecção de capacidade do Spectre.Console e selecionar o fluxo textual quando a entrada ou saída for redirecionada; não emitir prompts interativos que dependam de cursor nesse modo.
- **Justificativa**: A configuração padrão detecta capacidades do terminal, e o detector de cores respeita `NO_COLOR`. A biblioteca identifica entrada, saída ou erro redirecionados como não interativos. Uma alternativa textual permite uso por pipes e automação sem depender de cor.
- **Alternativas consideradas**: Forçar sempre o modo interativo; descartado por não funcionar corretamente com entrada/saída redirecionada. Implementar detecção e remoção ANSI manualmente; descartado porque duplica capacidades da biblioteca.
- **Fontes**: [Código de configuração do console](https://github.com/spectreconsole/spectre.console/blob/main/src/Spectre.Console/AnsiConsoleSettings.cs), [Detector de cores](https://github.com/spectreconsole/spectre.console/blob/main/src/Spectre.Console.Ansi/ColorSystemDetector.cs), [Detector de interação](https://github.com/spectreconsole/spectre.console/blob/main/src/Spectre.Console/InteractionDetector.cs), [NO_COLOR](https://no-color.org/).

## Testes do conteúdo renderizado

- **Decisão**: Usar `Spectre.Console.Testing` e `TestConsole` para fornecer respostas de prompt e validar o texto renderizado sem sequências ANSI.
- **Justificativa**: `TestConsole` captura a saída renderizada em memória e expõe entrada de teste, evitando dependência de terminal real e verificações frágeis de códigos de controle.
- **Alternativas consideradas**: Capturar `Console.Out` e remover ANSI manualmente; descartada por duplicar a ferramenta de teste e depender de estado estático de console.
- **Fontes**: [Testando saída do console](https://spectreconsole.net/console/how-to/testing-console-output/), [TestConsole](https://github.com/spectreconsole/spectre.console/blob/main/src/Spectre.Console.Testing/TestConsole.cs).

## Simulação de falha na geração

- **Decisão**: O ponto de entrada testável recebe a função geradora como dependência junto com a instância de console; a inicialização de produção conecta essa função aos serviços de geração existentes. Os testes podem fornecer uma função que lança uma exceção para validar mensagem de erro, descarte de resultado parcial e retorno ao início da solicitação.
- **Justificativa**: Os serviços de geração permanecem inalterados e a falha pode ser reproduzida de forma determinística sem adicionar camadas ou alterar aleatoriedade, regras ou resultados.
- **Alternativas consideradas**: Modificar os serviços estáticos para introduzir falhas artificiais; rejeitada por contaminar o domínio de geração com um requisito exclusivo da interface. Confiar em falhas aleatórias de execução nos testes; rejeitada por ser não determinística.

## Versão e compatibilidade da dependência

- **Decisão**: Fixar inicialmente Spectre.Console e Spectre.Console.Testing na versão estável 0.57.2.
- **Justificativa**: A pesquisa de 2026-10-27 verificou 0.57.2 como versão estável mais recente na listagem NuGet; a compatibilidade publicada inclui .NET 10.
- **Alternativas consideradas**: Versões estáveis anteriores; não escolhidas por não oferecerem vantagem neste projeto. Versões pré-lançamento foram excluídas.
- **Fontes**: [Pacote Spectre.Console no NuGet](https://www.nuget.org/packages/Spectre.Console), [Pacote Spectre.Console.Testing no NuGet](https://www.nuget.org/packages/Spectre.Console.Testing), [Índice de versões do NuGet](https://api.nuget.org/v3-flatcontainer/spectre.console/index.json).
