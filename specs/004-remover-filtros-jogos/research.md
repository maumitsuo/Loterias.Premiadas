# Pesquisa: Remover filtros de validação dos jogos

Não havia itens "NEEDS CLARIFICATION" no Contexto Técnico. As decisões abaixo registram as escolhas de projeto.

## 1. Remover o laço de descarte em vez de deixar a validação sempre verdadeira

- **Decisão**: Apagar `EhValida` e as funções auxiliares, e aceitar diretamente cada jogo sorteado.
- **Justificativa**: Atende ao princípio I (complexidade proporcional) e ao princípio IV: um `EhValida` que sempre retorna `true` deixaria um ramo `false` impossível de cobrir.
- **Alternativas consideradas**: Manter as regras configuráveis ou desligáveis; rejeitado por adicionar complexidade sem pedido.

## 2. Manter a assinatura pública dos serviços

- **Decisão**: Manter `IReadOnlyList<int[]> Gerar(int quantidade, Random aleatorio)` nos dois serviços, com as mesmas exceções.
- **Justificativa**: `Program.cs` e `ProgramaTests.cs` continuam funcionando sem alteração; o escopo fica restrito aos serviços.
- **Alternativas consideradas**: Unificar os dois serviços em um gerador parametrizado por modalidade; rejeitado por estar fora do escopo pedido.

## 3. Estratégia de sorteio sem repetição

- **Decisão**: Manter o sorteio atual: sortear números do intervalo e ignorar os já presentes no jogo até completar a quantidade, depois ordenar.
- **Justificativa**: Já cumpre a única regra restante e é compatível com o `AleatorioControlado` dos testes, que fornece valores por `Next(min, max)`.
- **Alternativas consideradas**: Embaralhar o intervalo inteiro (Fisher–Yates) e pegar os primeiros números; rejeitado porque mudaria a forma de consumir `Random` e exigiria reescrever os testes determinísticos sem ganho para a pessoa.

## 4. Jogos iguais na mesma solicitação

- **Decisão**: Não verificar se um jogo é igual a outro da mesma lista (esclarecimento de 2026-11-24, FR-009).
- **Justificativa**: Decisão explícita da pessoa; preserva o comportamento anterior.

## 5. Estratégia de testes

- **Decisão**: Substituir os testes que comprovavam as regras removidas por testes que, com `AleatorioControlado`, mostram que combinações antes rejeitadas (por exemplo, Mega-Sena `1,2,3,4,5,6`, só pares, três finais iguais; Lotofácil `1..15`, 10 ímpares, grupo sequencial ausente) agora são aceitas na primeira tentativa, sem consumir valores de uma segunda candidata.
- **Justificativa**: Prova os FR-003/FR-004 de forma determinística e mantém 100% de cobertura.
- **Alternativas consideradas**: Testes estatísticos com `Random` com semente; mantidos apenas no teste de 1.000 jogos (SC-001/SC-002), por não provarem sozinhos a ausência de filtros.
