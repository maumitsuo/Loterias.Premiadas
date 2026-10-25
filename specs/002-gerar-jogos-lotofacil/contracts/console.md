# Contrato de Console: Gerar jogos da Lotofácil

## Interação

- A aplicação apresenta as opções Mega-Sena e Lotofácil e solicita que a pessoa escolha uma delas.
- Uma modalidade vazia ou não reconhecida gera uma mensagem clara e nova solicitação de escolha; nenhum jogo é gerado enquanto a escolha for inválida.
- Depois da escolha, a aplicação solicita a quantidade de jogos como inteiro entre 1 e 1.000, inclusive.
- Para quantidade vazia, não inteira, menor que 1 ou maior que 1.000, a aplicação informa o intervalo permitido e solicita nova entrada.
- Ao receber fim da entrada (EOF) antes de uma modalidade ou quantidade válida, a aplicação encerra sem gerar jogos.

## Resultado

- Para uma solicitação válida, a aplicação apresenta exatamente a quantidade solicitada de jogos da modalidade escolhida.
- Cada jogo aparece identificado e com seus números distintos em ordem crescente, formatados com dois dígitos e separados por vírgula.
- Os jogos da Lotofácil contêm 15 números entre 1 e 25 e satisfazem todas as regras de paridade, moldura/miolo e grupos sequenciais.
- Os jogos da Mega-Sena satisfazem os critérios da [especificação Mega-Sena](../../001-gerar-jogos-megasena/spec.md).
- Combinações inválidas não são apresentadas; a aplicação gera substituições até alcançar a quantidade solicitada.
- A aplicação não promete aumento ou garantia de probabilidade de premiação e não persiste entradas nem resultados.
