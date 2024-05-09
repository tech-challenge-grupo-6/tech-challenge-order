using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using ControladorPedidos.Gateways.DependencyInjection;
using ControladorPedidos.Infrastructure.Configurations;
using ControladorPedidos.Infrastructure.Database.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ControladorPedidos", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            []
        }
    });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ControladorPedidos.xml"));
});

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddRepositories();

var cacheConfiguration = CacheConfiguration.FromConfiguration(builder.Configuration);
builder.Services.AddSingleton(cacheConfiguration);
builder.Services.AddStackExchangeRedisCache(options =>
{
    string redisConfiguration = cacheConfiguration.Configuration;
    options.Configuration = redisConfiguration;
    options.InstanceName = "ControladorPedidos";
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
    {
        // get JsonWebKeySet from AWS
#pragma warning disable SYSLIB0014 // Type or member is obsolete
        var json = new WebClient().DownloadString(parameters.ValidIssuer + "/.well-known/jwks.json");
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        // deserialize the result
        var keys = JsonSerializer.Deserialize<JsonWebKeySet>(json)!.Keys;
        foreach (var key in keys)
        {
            // Acessar as propriedades da chave
            Console.WriteLine(key); // Exemplo de acesso a uma propriedade, como o ID da chave
            // Faça o que for necessário com cada chave...
        }
        // cast the result to be the type expected by IssuerSigningKeyResolver
        return (IEnumerable<SecurityKey>)keys;
    },
        ValidIssuer = $"https://cognito-idp.{builder.Configuration["AWS:Region"]}.amazonaws.com/{builder.Configuration["AWS:UserPoolId"]}",
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["AWS:AppClientId"],
        ValidateAudience = true
    };
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();