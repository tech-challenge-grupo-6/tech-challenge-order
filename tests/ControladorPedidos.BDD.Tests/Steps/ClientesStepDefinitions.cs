
using System.Net.Http.Headers;
using System.Text.Json;
using ControladorPedidos.Application.Clientes.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TechTalk.SpecFlow;

namespace ControladorPedidos.BDD.Tests.Steps;

[Binding]
public class ClientesStepDefinitions(ScenarioContext scenarioContext, WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    private string _bearerToken = default!;
    private CadastroClienteCommand _cadastroClienteCommand = default!;
    private string _clienteId = default!;

    [Given(@"que estou logado no sistema como cliente")]
    public void Givenqueestoulogadonosistema()
    {
        _bearerToken = "Meu token";
        _bearerToken.Should().NotBeEmpty();
    }

    [Given(@"que o cliente informou os seguintes dados: nome, cpf, email")]
    public void Givenqueoclienteinformouosseguintesdados()
    {
        //Arrange
        string nome = "Fulano";
        string cpf = "40405341660";
        string email = "test@test.com";

        //Act
        _cadastroClienteCommand = new CadastroClienteCommand(nome, cpf, email);

        //Assert
        _cadastroClienteCommand.Should().NotBeNull();
    }

    [When(@"eu realizo o cadastro do cliente")]
    public async Task Wheneurealizoocadastrodocliente()
    {
        //Arrange
        var httpClient = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "/Cliente");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        request.Content = new StringContent(JsonSerializer.Serialize(_cadastroClienteCommand), System.Text.Encoding.UTF8, "application/json");

        //Act
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        _clienteId = content;
        _clienteId.Should().NotBeEmpty();
    }

    [Then(@"o cliente é cadastrado com sucesso")]
    public void Thenoclientecadastradocomsucesso()
    {
        _clienteId.Should().NotBeEmpty();
    }

}