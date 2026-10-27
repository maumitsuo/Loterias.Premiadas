# Especificação da Funcionalidade: Modernizar a interface do console

**Branch da funcionalidade**: `003-modernizar-interface-console`

**Criada em**: 2026-10-27

**Status**: Rascunho

**Entrada**: Descrição do usuário: "Modernizar a experiência no console, melhorando o fluxo de utilização e a exibição dos resultados."

## Clarifications

### Session 2026-10-27

- Q: Depois de exibir os jogos, como o aplicativo deve continuar? → A: Perguntar se deseja gerar outros jogos ou encerrar.

## Cenários de Uso e Testes *(obrigatório)*

### História de Usuário 1 - Solicitar jogos com orientação clara (Prioridade: P1)

Como pessoa que deseja gerar jogos, quero entender cada etapa da solicitação e as opções disponíveis, para escolher uma modalidade e informar a quantidade desejada sem dúvidas.

**Por que esta prioridade**: Uma interação compreensível é necessária para iniciar corretamente a geração e constitui a entrada para consultar os resultados.

**Teste independente**: Percorrer o fluxo desde a escolha da modalidade até a solicitação de uma quantidade válida e verificar que cada etapa explica o que a pessoa deve informar.

**Cenários de Aceitação**:

1. **Dado** que a pessoa inicia uma solicitação, **Quando** o sistema pede a modalidade, **Então** apresenta as modalidades disponíveis e como selecioná-las.
2. **Dado** que a pessoa escolhe uma modalidade, **Quando** o sistema pede a quantidade, **Então** informa o intervalo aceito e permite continuar após uma resposta válida.
3. **Dado** que a pessoa informa modalidade e quantidade válidas, **Quando** a geração começa, **Então** a solicitação concluída corresponde às escolhas apresentadas.
4. **Dado** que ocorre uma falha durante a geração, **Quando** o sistema detecta a falha, **Então** exibe uma mensagem de erro, não apresenta resultado parcial e retorna ao início da solicitação.

---

### História de Usuário 2 - Interpretar os jogos gerados (Prioridade: P1)

Como pessoa que recebe jogos gerados, quero identificar rapidamente a modalidade, a quantidade e os números de cada jogo, para consultar os resultados sem confundi-los.

**Por que esta prioridade**: Os resultados são o principal valor da aplicação; uma apresentação organizada facilita sua leitura e conferência.

**Teste independente**: Solicitar jogos em cada modalidade e confirmar que o resultado identifica a modalidade e a quantidade, separa cada jogo e mantém os números legíveis e em ordem crescente.

**Cenários de Aceitação**:

1. **Dado** que uma solicitação válida foi concluída, **Quando** os resultados são exibidos, **Então** a pessoa consegue identificar a modalidade e a quantidade solicitada e apresentada.
2. **Dado** que há um ou mais jogos no resultado, **Quando** a pessoa os consulta, **Então** cada jogo tem um identificador visual próprio e seus números aparecem em ordem crescente.
3. **Dado** que o ambiente não exibe cores ou estilos visuais, **Quando** os resultados são apresentados, **Então** todas as informações continuam distinguíveis e legíveis.
4. **Dado** que os resultados foram exibidos, **Quando** a pessoa decide continuar, **Então** pode solicitar novos jogos sem reiniciar o aplicativo; se decidir encerrar, o aplicativo termina.

---

### História de Usuário 3 - Corrigir uma entrada inválida (Prioridade: P2)

Como pessoa que está preenchendo uma solicitação, quero saber o que está incorreto e como corrigir a entrada, para continuar sem reiniciar o fluxo nem receber resultados enganosos.

**Por que esta prioridade**: Orientação útil reduz erros e evita que uma solicitação incompleta pareça concluída.

**Teste independente**: Informar uma modalidade não disponível e quantidades vazias, não inteiras, menores que 1 ou maiores que 1.000; verificar a orientação e a possibilidade de tentar novamente.

**Cenários de Aceitação**:

1. **Dado** que a modalidade ou a quantidade informada é inválida, **Quando** o sistema valida a entrada, **Então** explica o problema e indica uma resposta aceita.
2. **Dado** que uma entrada inválida foi informada, **Quando** a orientação é exibida, **Então** a pessoa pode corrigir somente essa entrada e prosseguir sem reiniciar o aplicativo.
3. **Dado** que a solicitação permanece inválida, **Quando** a validação termina, **Então** nenhum resultado é apresentado como se a geração tivesse sido concluída.

### Casos Limítrofes

- Quantidade vazia, não inteira, zero, negativa ou acima de 1.000: explicar que é necessário informar um inteiro entre 1 e 1.000, inclusive, e permitir nova tentativa.
- Modalidade vazia ou não reconhecida: reapresentar as opções disponíveis sem iniciar a geração.
- Solicitação de exatamente um jogo: identificar claramente o único jogo e a quantidade apresentada.
- Solicitação da quantidade máxima permitida: apresentar todos os jogos de forma distinguível, sem omitir a modalidade ou a quantidade total.
- Após a exibição dos resultados, oferecer a opção de gerar outros jogos ou encerrar o aplicativo.
- Terminal sem suporte a cores ou estilos: manter distinção e legibilidade por meio de texto e organização.
- Falha ou interrupção antes da conclusão: não apresentar uma saída parcial como resultado completo.
- Falha durante a geração: exibir uma mensagem de erro e retornar ao início da solicitação, sem apresentar resultado parcial.

## Requisitos *(obrigatório)*

### Requisitos Funcionais

- **FR-001**: O sistema DEVE conduzir a pessoa por etapas identificáveis para escolher uma modalidade disponível e informar a quantidade de jogos desejada.
- **FR-002**: O sistema DEVE indicar as opções de modalidade e o intervalo aceito para a quantidade no momento em que cada informação for solicitada.
- **FR-003**: O sistema DEVE validar a modalidade e aceitar uma quantidade inteira entre 1 e 1.000, inclusive.
- **FR-004**: Para cada entrada inválida, o sistema DEVE explicar o problema, indicar como corrigi-lo e permitir nova tentativa sem reiniciar o aplicativo.
- **FR-005**: O sistema DEVE apresentar resultados concluídos com identificação da modalidade e das quantidades solicitada e apresentada.
- **FR-006**: O sistema DEVE distinguir visualmente cada jogo e apresentar seus números em ordem crescente, sem alterar os valores gerados.
- **FR-007**: O sistema DEVE manter as informações essenciais distinguíveis e legíveis quando cores ou estilos visuais não estiverem disponíveis.
- **FR-008**: O sistema NÃO DEVE apresentar uma solicitação inválida, interrompida ou incompleta como uma geração concluída.
- **FR-009**: O sistema DEVE preservar as regras de geração e validação de jogos existentes para Mega-Sena e Lotofácil.
- **FR-010**: Após apresentar os resultados, o sistema DEVE permitir que a pessoa escolha entre iniciar uma nova solicitação e encerrar o aplicativo.

### Entidades Principais *(inclua se a funcionalidade envolver dados)*

- **Solicitação de jogos**: modalidade escolhida e quantidade inteira desejada, entre 1 e 1.000.
- **Resultado**: conjunto concluído de jogos associado à modalidade e à quantidade solicitada.
- **Jogo**: combinação de números gerada segundo as regras da modalidade, apresentada separadamente das demais.

## Critérios de Sucesso *(obrigatório)*

### Resultados Mensuráveis

- **SC-001**: Em 100% das solicitações válidas concluídas, a modalidade e as quantidades solicitada e apresentada podem ser identificadas no resultado.
- **SC-003**: Em 100% dos resultados concluídos, cada jogo é distinguível, seus números permanecem legíveis sem cores e a ordem crescente é preservada.
- **SC-004**: Em 100% das entradas inválidas testadas, a orientação indica o problema e uma correção possível, sem apresentar resultados como concluídos.
- **SC-005**: Em 100% das solicitações concluídas, a pessoa pode escolher entre iniciar outra solicitação e encerrar o aplicativo.

## Premissas

- A modernização abrange a interação de solicitação e a apresentação dos resultados; não altera a geração, as regras de validade ou as probabilidades dos jogos.
- As modalidades disponíveis permanecem Mega-Sena e Lotofácil, conforme as funcionalidades existentes.
- O limite de 1 a 1.000 jogos por solicitação permanece igual ao já estabelecido.
- Os resultados continuam disponíveis somente durante a execução do aplicativo e não são armazenados para uso posterior.
- O ambiente de uso pode não oferecer cores ou estilos visuais; o conteúdo deve continuar compreensível sem esses recursos.
- A avaliação de usabilidade com pelo menos 10 participantes, anteriormente identificada como SC-002, está fora do escopo desta funcionalidade.
