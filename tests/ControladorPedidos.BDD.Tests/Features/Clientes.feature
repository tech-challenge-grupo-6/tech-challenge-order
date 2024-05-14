Feature: Clientes
    Realização do cadastro de cliente

    @cliente
    Scenario: Cadastro de cliente
        Given que estou logado no sistema como cliente
        And que o cliente informou os seguintes dados: nome, cpf, email
        When eu realizo o cadastro do cliente
        Then o cliente é cadastrado com sucesso