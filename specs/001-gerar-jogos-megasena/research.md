# Pesquisa: Gerar jogos da Mega-Sena

## Plataforma e estrutura da aplicação

- **Decisão**: criar uma aplicação de console em C# direcionada ao .NET 10, conforme a constituição, sem camadas adicionais.
- **Justificativa**: a interação solicitada é por console e o domínio consiste em validar uma quantidade e gerar combinações transitórias. Não há código preexistente no repositório que exija integração com outra arquitetura.
- **Alternativas consideradas**: aplicação web, serviço separado ou persistência de resultados; rejeitadas por excederem o escopo e contrariarem a constituição.

## Geração aleatória

- **Decisão**: usar a API `Random` da biblioteca base do .NET, fornecendo a instância ao gerador para que os testes usem uma semente fixa.
- **Justificativa**: o requisito é gerar combinações aleatórias, não proteger um segredo ou aumentar a chance de prêmio. A fonte padrão atende ao comportamento pedido; uma instância controlável permite testes reproduzíveis.
- **Alternativas consideradas**: gerador criptográfico, rejeitado por não haver requisito de segurança que o justifique; fonte aleatória global inacessível aos testes, rejeitada por dificultar a reprodução de cenários.

## Dependências de execução e dados

- **Decisão**: não adicionar dependências externas de execução nem armazenamento. Usar xUnit e Coverlet somente no projeto de testes para testes automatizados e medição de cobertura.
- **Justificativa**: console, geração, validação e apresentação podem ser atendidas pela biblioteca base do .NET. xUnit e Coverlet cobrem a exigência constitucional de testes e relatório de linhas/ramificações sem ampliar a aplicação de produção.
- **Alternativas consideradas**: bibliotecas de domínio, bancos de dados e serviços externos, rejeitados por serem desnecessários para os requisitos e incompatíveis com o escopo transitório.

## Metas de desempenho e escala

- **Decisão**: tratar 1.000 jogos por solicitação como o limite funcional; não definir meta de latência além disso.
- **Justificativa**: a especificação não fixa latência, throughput nem expectativa de concorrência. Introduzir valores de desempenho sem requisito de negócio produziria um critério arbitrário.
- **Alternativas consideradas**: estabelecer um prazo de resposta ou limite de concorrência, adiados porque não são necessários para validar este aplicativo local de console.

## Interfaces externas

- **Decisão**: documentar apenas o contrato de interação com a pessoa pelo console; não criar contrato de API ou integração máquina a máquina.
- **Justificativa**: a constituição exige interação por console e a especificação não declara integrações externas.
- **Alternativas consideradas**: API ou formato de arquivo exportável, rejeitados por não serem solicitados e por introduzirem persistência/interoperabilidade fora do escopo.
