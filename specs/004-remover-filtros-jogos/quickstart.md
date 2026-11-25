# Guia de validação: Remover filtros de validação dos jogos

## Pré-requisitos

- SDK do .NET 10 instalado.
- Executar os comandos a partir da raiz do repositório.

## 1. Testes automatizados e cobertura

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

**Esperado**:
- Todos os testes passam.
- O relatório `coverage.cobertura.xml` em `tests/Loterias.Premiadas.Tests/TestResults/` mostra `line-rate="1"` e `branch-rate="1"` para o pacote `Loterias.Premiadas` (princípio IV).

## 2. Cenários que os testes devem cobrir

| Cenário | Requisito |
|---------|-----------|
| 1.000 jogos Mega-Sena: 6 números distintos de 1 a 60, em ordem crescente | FR-001, FR-006, FR-008, SC-001, SC-002 |
| 1.000 jogos Lotofácil: 15 números distintos de 1 a 25, em ordem crescente | FR-002, FR-006, FR-008, SC-001, SC-002 |
| Mega-Sena aceita na primeira tentativa `1,2,3,4,5,6`, só pares (`2,4,6,8,10,12`) e três finais iguais (`2,12,22,31,44,55`) | FR-003, SC-003 |
| Lotofácil aceita na primeira tentativa `1..15`, 10 ímpares e 5 pares e um grupo sequencial ausente | FR-004, SC-003 |
| Número repetido sorteado é ignorado nas duas modalidades | FR-005 |
| Quantidade 0 ou 1001 e `Random` nulo continuam rejeitados | FR-007 |

Ver [contracts/geracao.md](contracts/geracao.md) para o comportamento esperado de cada serviço.

## 3. Verificação manual

```powershell
dotnet run --project src/Loterias.Premiadas
```

1. Escolha Mega-Sena e peça 20 jogos. Cada linha deve ter 6 números distintos, em ordem crescente.
2. Repita com Lotofácil. Cada linha deve ter 15 números distintos, em ordem crescente.
3. Informe a quantidade `0`; a interface deve pedir uma quantidade válida, como antes.
4. Peça 1.000 jogos em cada modalidade; o resultado deve aparecer sem espera perceptível (SC-004).
