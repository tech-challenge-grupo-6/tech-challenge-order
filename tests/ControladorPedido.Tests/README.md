# Controlador Pedidos Tests
## Descrição
Este projeto contém testes de unidade para o controlador de pedidos.
## Tecnologias
- XUnit
- FluentAssertions
- NSubstitute

## Execução
Para executar os testes, basta executar o comando abaixo:
```bash
cd tests/ControladorPedidos.Tests
dotnet test --collect:"XPlat Code Coverage"
```
Para visualizar o relatório de cobertura de código, use o comando abaixo:
```bash
cd tests/ControladorPedidos.Tests
reportgenerator "-reports:"../TestsResults/<id-do-resultado>/coverage.cobertura.xml" -targetdir:../TestsResults/<id-do-resultado>/" -reporttypes:Html
```
Após isso, abra o arquivo `../TestsResults/<id-do-resultado>//index.html` em seu navegador.
