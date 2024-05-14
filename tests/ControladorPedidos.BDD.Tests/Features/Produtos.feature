Feature: Produtos
    Realização do cadastrado de Produtos

    @produto
    Scenario: Cadastro de produtos com sucesso
        Given que estou logado no sitema como administrador
        And informo a categoria do produto
        And informo os dados do produto
        When realizo o cadastro do produto
        Then o produto é cadastrado com sucesso
