using System.Net.Http.Headers;
using System.Text.Json;
using ControladorPedidos.Application.Produtos.Commands;
using ControladorPedidos.Application.Produtos.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TechTalk.SpecFlow;

namespace ControladorPedidos.BDD.Tests.Steps;

[Binding]
public class ProdutosStepDefinitions(ScenarioContext scenarioContext, WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    private string _bearerToken = default!;
    private string _categoriaId = default!;
    private CadastrarProdutoCommand _cadastrarProdutoCommand = default!;
    private bool _isSuccess = false;

    [Given(@"que estou logado no sitema como administrador")]
    public void Givenqueestoulogadonositemacomoadministrador()
    {
        _bearerToken = "Meu token";
        _bearerToken.Should().NotBeEmpty();
    }

    [Given(@"informo a categoria do produto")]
    public async Task Giveninformoacatgoriadoproduto()
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

    [Given(@"informo os dados do produto")]
    public void Giveninformoosdadosdoproduto()
    {

        _cadastrarProdutoCommand = new("teste", new Guid(_categoriaId), 10, "test", "test.png");
        _cadastrarProdutoCommand.Should().NotBeNull();
    }

    [When(@"realizo o cadastro do produto")]
    public async Task Whenrealizoocadastrodoproduto()
    {
        //Arrange
        var httpClient = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "/Produto");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        request.Content = new StringContent(JsonSerializer.Serialize(_cadastrarProdutoCommand), System.Text.Encoding.UTF8, "application/json");

        //Act
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        //Assert
        _isSuccess = response.IsSuccessStatusCode;
    }

    [Then(@"o produto é cadastrado com sucesso")]
    public void Thenoprodutocadastradocomsucesso()
    {
        _isSuccess.Should().BeTrue();
    }

}