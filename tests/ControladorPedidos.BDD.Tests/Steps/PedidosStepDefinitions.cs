using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ControladorPedidos.Application.Clientes.Queries;
using ControladorPedidos.Application.Pedidos.Commands;
using ControladorPedidos.Application.Produtos.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TechTalk.SpecFlow;

namespace ControladorPedidos.BDD.Tests.Steps;

[Binding]
public class StepDefinitions(ScenarioContext scenarioContext, WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    private string _bearerToken = default!;
    private string _customerId = default!;
    private string _categoriaId = default!;
    private string _produtoId = default!;
    private CadastrarPedidoCommand _cadastrarPedidoCommand = default!;
    private string _pedidoId = default!;

    [Given(@"que estou logado no sistema")]
    public void Givenqueestoulogadonosistema()
    {

        _bearerToken = "Meu token";
        _bearerToken.Should().NotBeEmpty();
    }

    [Given(@"tenho um cliente cadastrado")]
    public async Task Giventenhoumclientecadastrado()
    {
        //Arrange 
        var httpClient = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/Cliente/35042084061");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);

        //Act
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var responseValues = JsonSerializer.Deserialize<PegarClientePorCpfQueryResponse>(content, _jsonSerializerOptions);
        _customerId = responseValues!.Id.ToString();

        //Assert
        _customerId.Should().NotBeEmpty();
    }

    [Given(@"categoria de produtos cadastrada")]
    public async Task Givencategoriadeprdutoscadastrada()
    {
        //Arrange
        var httpClient = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/categorias");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);

        //Act
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var responseValues = JsonSerializer.Deserialize<PegarCategoriasQueryResponse>(content, _jsonSerializerOptions);

        //Assert
        responseValues!.Categorias.Should().NotBeEmpty();
        _categoriaId = responseValues!.Categorias.First().Id.ToString();
    }

    [Given(@"produtos cadastrados")]
    public async Task Givenprodutoscadastrados()
    {
        //Arrange
        var httpClient = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/Produto/" + _categoriaId);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);

        //Act
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var responseValues = JsonSerializer.Deserialize<IEnumerable<PegarProdutoPorCategoriaQuery>>(content, _jsonSerializerOptions);

        //Assert
        responseValues!.Should().NotBeEmpty();
        _produtoId = responseValues!.First().Id.ToString();

    }

    [When(@"eu realizar um pedido")]
    public async Task Wheneurealizarumpedido()
    {
        //Arrange
        _cadastrarPedidoCommand = new CadastrarPedidoCommand(Guid.Parse(_customerId), new List<Guid> { Guid.Parse(_produtoId) });
        var httpClient = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "/Pedido");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        request.Content = new StringContent(JsonSerializer.Serialize(_cadastrarPedidoCommand), Encoding.UTF8, "application/json");

        //Act
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        content.Should().NotBeEmpty();
        _pedidoId = content;
    }

    [Then(@"o pedido deve ser realizado com sucesso retornando um id")]
    public void Thenopedidodeveserrealizadocomsucessoretornandoumid()
    {
        _pedidoId.Should().NotBeEmpty();
    }

}