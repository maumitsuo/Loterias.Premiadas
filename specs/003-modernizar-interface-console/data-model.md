# Modelo de dados: Modernizar a interface do console

O recurso não adiciona dados persistentes. As entidades abaixo existem apenas em memória durante uma solicitação e sua apresentação.

## Entidades

### Solicitação de jogos

- **Modalidade**: Mega-Sena ou Lotofácil; obrigatório.
- **Quantidade**: inteiro entre 1 e 1.000, inclusive; obrigatório.
- **Estado do fluxo**: aguardando modalidade, aguardando quantidade, gerando, exibindo resultado, aguardando decisão de continuidade ou encerrado.

### Jogo

- **Números**: coleção ordenada de números da modalidade; deve obedecer integralmente às regras existentes da modalidade.
- **Identificador de apresentação**: posição sequencial iniciada em 1 dentro do resultado; não altera nem identifica globalmente a combinação.

### Resultado

- **Modalidade**: modalidade da solicitação associada.
- **Quantidade solicitada**: valor informado e validado.
- **Jogos**: coleção de jogos concluídos e apresentados.
- **Condição de conclusão**: resultado só é completo quando o número de jogos corresponde à quantidade solicitada.

## Regras e transições

- A modalidade deve ser reconhecida como Mega-Sena ou Lotofácil antes da etapa de quantidade.
- Quantidade vazia, não inteira, menor que 1 ou maior que 1.000 mantém o fluxo na etapa de quantidade para nova tentativa.
- O estado de geração não deve ser apresentado como concluído antes de reunir a quantidade solicitada de jogos válidos.
- Se a geração falhar, descartar os jogos parciais, informar a falha e retornar ao início de uma nova solicitação.
- Após exibir um resultado concluído, a decisão de gerar novamente inicia uma nova solicitação; a decisão de encerrar termina o fluxo.
- Um fim de entrada durante a interação encerra sem apresentar um resultado parcial como completo.
- Nenhuma entidade é persistida após o fim do processo.
