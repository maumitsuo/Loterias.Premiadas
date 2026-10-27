# Contrato de interação do console

## Solicitação

1. Apresentar as opções Mega-Sena e Lotofácil e permitir seleção.
2. Solicitar a quantidade de jogos e informar que o intervalo aceito é de 1 a 1.000, inclusive.
3. Para modalidade ou quantidade inválida, explicar como corrigir e repetir somente a solicitação inválida.
4. Em terminal interativo, usar prompts que indiquem a etapa atual e aceitem a interação correspondente.
5. Se entrada ou saída estiver redirecionada, usar interação textual compatível com entrada e saída sequenciais; não depender de cursor, teclas especiais ou cor.
6. Se a geração falhar, exibir uma mensagem de erro, descartar qualquer resultado parcial e retornar ao início da solicitação.

## Resultado

- Identificar modalidade e quantidade solicitada e apresentada.
- Apresentar cada jogo em uma linha ou agrupamento visual distinto, com índice e números em ordem crescente.
- Preservar as regras de geração e a quantidade solicitada.
- Permanecer compreensível sem cores, respeitando a convenção `NO_COLOR` e a detecção de capacidades do terminal.
- Não apresentar resultado parcial como concluído.
- Uma falha da função geradora não pode ser apresentada como conclusão; a próxima etapa é iniciar uma nova solicitação.

## Continuidade

- Depois de exibir todos os jogos, perguntar se a pessoa deseja gerar outros jogos ou encerrar.
- Se a pessoa continuar, iniciar uma nova solicitação; caso contrário, encerrar normalmente.
- Fim de entrada durante qualquer etapa encerra sem aguardar indefinidamente e sem indicar conclusão inexistente.
