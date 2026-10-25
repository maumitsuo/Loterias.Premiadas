<!--
Relatório de impacto de sincronização
- Alteração de versão: 2.1.0 → 2.1.1.
- Princípios modificados: nenhum.
- Princípios adicionados: nenhum.
- Seções adicionadas: nenhuma.
- Seções removidas: nenhuma.
- TODOs de acompanhamento: nenhum.
-->
# Constituição de Loterias Premiadas

## Princípios Fundamentais

### I. Aplicação de console simples
A solução MUST permanecer uma aplicação de console em C# sobre .NET 10. A implementação
MUST manter a complexidade proporcional aos requisitos e MUST NOT introduzir camadas,
serviços ou abstrações sem necessidade demonstrável. A interação com o usuário MUST ocorrer
por meio do console.

### II. Dados transitórios
Os dados manipulados pelo aplicativo MUST existir somente durante a execução do processo.
O aplicativo MUST NOT persistir esses dados em arquivos, bancos de dados, caches duráveis,
serviços externos ou outros meios de armazenamento. Ao encerrar o processo, os dados em
memória deixam de estar disponíveis.

### III. Nomenclatura em português brasileiro
Nomes declarados no código MUST usar português brasileiro. Nomenclaturas técnicas
amplamente estabelecidas, identificadores exigidos por frameworks ou protocolos e
convenções idiomáticas obrigatórias da plataforma podem permanecer em inglês para preservar
interoperabilidade e clareza técnica.

### IV. Cobertura integral de testes
Todo o código executável do aplicativo MUST ser exercitado por testes automatizados. A
execução dos testes MUST produzir um relatório que demonstre 100% de cobertura de linhas e
ramificações do código do aplicativo. Esta exigência torna verificável a qualidade em um
projeto cujo escopo reduzido permite cobertura completa.

## Restrições técnicas

A solução MUST ser implementada em C# e direcionada ao .NET 10. O produto MUST ser uma
aplicação de console e MUST NOT incluir mecanismos de persistência de dados.

## Desenvolvimento e conformidade

Cada mudança MUST preservar o escopo de aplicação de console e a regra de dados transitórios.
Os testes automatizados MUST manter 100% de cobertura de linhas e ramificações do código
executável do aplicativo, conforme demonstrado por relatório de cobertura. Revisões MUST
verificar o uso de C# e .NET 10, a ausência de persistência de dados, a cobertura total e a
conformidade dos nomes com o português brasileiro.

## Governança

Esta constituição prevalece sobre orientações conflitantes do projeto. Uma alteração exige
documentar a justificativa e o impacto nos princípios, atualizar este arquivo e revisar a
conformidade das áreas afetadas. Mudanças incompatíveis nas regras de governança exigem
justificativa explícita e plano para adequar o código e a documentação afetados.

A versão segue versionamento semântico: MAJOR para remoções ou redefinições incompatíveis de
regras; MINOR para novos princípios ou expansões materiais; PATCH para esclarecimentos e
ajustes sem mudança de significado. Revisões de código e validações de alterações MUST
verificar a conformidade com esta constituição; qualquer exceção MUST ser justificada e
aprovada na revisão.

**Versão**: 2.1.1 | **Ratificada em**: 2026-10-25 | **Última alteração**: 2026-10-25
