Feature: Pedido

    Realização de um novo pedido

    @pedido
    Scenario: Pedido com sucesso
        Given que estou logado no sistema
        And tenho um cliente cadastrado
        And categoria de produtos cadastrada
        And produtos cadastrados
        When eu realizar um pedido
        Then o pedido deve ser realizado com sucesso retornando um id