# Controlador pedidos API - Microserviço de controle de pedidos

## Descrição
Este projeto contém a implementação do controlador de pedidos.

## Tecnologias
- ASP.NET Core
- Entity Framework Core
- MySQL
- Redis

## Grupo 6

- RM350836 (3SOAT): Marcio Lages Silva - marciolages@msn.com
- RM351061 (3SOAT): Renan Silva Xavier - renansx2013@hotmail.com
- RM351631 (3SOAT): Victor Sadao Higa Nagahara - viih.higa@gmail.com
- RM351041 (3SOAT): Vitor de Souza - vitordesolza@gmail.com

Swagger (localhost): [Link para Swagger](http://localhost:5003/swagger/index.html)

## Contexto do Negócio

A lanchonete de bairro, devido ao seu sucesso crescente, está expandindo suas operações. No entanto, sem um sistema de controle de pedidos eficiente, o atendimento aos clientes pode ser caótico e resultar em insatisfação. Para solucionar esse problema, a lanchonete está investindo em um sistema de autoatendimento de fast food, com as seguintes funcionalidades:

## Entidades e Agregados

### Cliente

- Nome
- CPF
- E-mail

### Pedido

- Cliente (identificado ou não)
- Status (em progresso, pronto, finalizado)
- Itens do pedido (produtos selecionados)
- Valor total
- Método de pagamento (Sempre QR Code)

### Produto

- Nome
- Categoria
- Preço
- Descrição
- Imagens

### Categoria de Produto

- Nome