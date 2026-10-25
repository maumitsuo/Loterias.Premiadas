# Contrato de Console: Gerar jogos da Mega-Sena

## Interação

- A aplicação solicita a quantidade de jogos em português brasileiro.
- A entrada válida é um inteiro de 1 a 1.000, inclusive.
- Para entrada vazia, não inteira, menor que 1 ou maior que 1.000, a aplicação informa que a quantidade deve estar entre 1 e 1.000 e solicita uma nova entrada. Não gera resultado enquanto a entrada for inválida.
- Ao receber fim da entrada (EOF) antes de uma quantidade válida, encerra sem gerar jogos.

## Resultado

- Para uma entrada válida, apresenta exatamente a quantidade solicitada de jogos.
- Cada linha identifica o jogo e apresenta seis números distintos em ordem crescente, em português brasileiro, por exemplo: `Jogo 1: 03, 08, 15, 22, 34, 49`.
- Cada jogo atende a pelo menos duas regras de composição e não viola nenhuma regra de descarte da especificação.
- A aplicação não promete aumento ou garantia de probabilidade de premiação e não persiste entradas nem resultados.
