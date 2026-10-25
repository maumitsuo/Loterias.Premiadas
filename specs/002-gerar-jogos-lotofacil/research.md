# Pesquisa: Gerar jogos da Lotofácil

## Plataforma e estrutura da aplicação

- **Decisão**: estender a aplicação de console existente em C#/.NET 10 e reutilizar os projetos de aplicação e testes presentes no repositório.
- **Justificativa**: a constituição exige console em .NET 10, e a base atual já contém o fluxo Mega-Sena, testes xUnit e coleta de cobertura. A funcionalidade não requer outra plataforma ou serviço.
- **Alternativas consideradas**: criar uma aplicação ou API separada; rejeitadas por duplicarem o produto existente e excederem o escopo.

## Seleção da modalidade e interação

- **Decisão**: solicitar a modalidade antes da quantidade, oferecer Mega-Sena e Lotofácil como opções explícitas e repetir a solicitação quando a entrada não for reconhecida.
- **Justificativa**: a especificação define essa ordem e pede correção de uma modalidade inválida. A experiência de console existente já permite repetir entradas inválidas e encerrar sem gerar jogos quando a entrada termina.
- **Alternativas consideradas**: escolher modalidade por argumento de linha de comando ou inferir a partir da quantidade; rejeitadas porque não correspondem ao fluxo solicitado.

## Geração e validação dos jogos

- **Decisão**: manter o gerador da Mega-Sena e seus critérios inalterados; implementar a Lotofácil selecionando 15 números distintos do intervalo 1–25, ordenando-os e aceitando somente candidatas que satisfaçam simultaneamente paridade, contagem de moldura/miolo e cobertura dos cinco grupos sequenciais. Substituir candidatas inválidas até completar a quantidade solicitada.
- **Justificativa**: este é o comportamento declarado nos requisitos; a seleção aleatória sem repetição, seguida de validação, é consistente com a geração já utilizada no projeto e permite testar filtros e substituições.
- **Alternativas consideradas**: relaxar filtros, retornar menos jogos ou apresentar candidatas inválidas; rejeitadas por violarem os requisitos de saída.

## Dependências e reprodutibilidade

- **Decisão**: usar somente `Random` da biblioteca base na aplicação e fornecer uma instância controlável ao gerador para testes reproduzíveis. Manter xUnit e Coverlet como dependências do projeto de testes.
- **Justificativa**: a aleatoriedade não é usada para proteger segredos; uma fonte com sequência controlada torna determinísticos os testes dos filtros e não adiciona dependência de produção.
- **Alternativas consideradas**: gerador criptográfico ou pacote externo de aleatoriedade; não justificados pelos requisitos.

## Dados, integrações e desempenho

- **Decisão**: manter as solicitações e os resultados apenas em memória, sem integrações externas, formatos de exportação ou meta de latência adicional. Tratar 1.000 jogos como limite funcional por solicitação.
- **Justificativa**: a constituição proíbe persistência, e não há necessidade de serviços ou metas de desempenho além do limite funcional especificado.
- **Alternativas consideradas**: persistir resultados, expor API ou fixar uma meta temporal; não solicitadas e sem justificativa de negócio.

## Incertezas

Não há ocorrências de `NEEDS CLARIFICATION` no contexto técnico. A especificação contém decisões suficientes para o desenho inicial; detalhes de organização interna podem ser decididos durante a implementação sem alterar critérios de aceitação.
