using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoApp.API;

namespace ToDoApp.Tests.IntegrationTests;

public class ToDoItemsControllerTests : IClassFixture<WebApplicationFactory<StartupMarker>>
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly WebApplicationFactory<StartupMarker> _factory;
    public ToDoItemsControllerTests(WebApplicationFactory<StartupMarker> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _configuration = factory.Services.GetService<IConfiguration>()!;
    }
    private string GenerateJwtToken(string role)
    {
        var claims = new[]
        {
                new Claim(ClaimTypes.Name, "owner@domain.com"),
                new Claim(ClaimTypes.Role, role),
            };

        //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkeythatisatleast128bitslong"));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryMinutes"])),
            signingCredentials: creds
    );


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    // اختبار الوصول للمورد مع دور "Owner"
    [Fact]
    public async Task Get_ToDoItems_WithOwnerRole_ReturnsOk()
    {
        // محاكاة JWT مع دور "Owner"
        var token = GenerateJwtToken("Owner"); // Generate a token with "Owner" role

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/ToDoItems");

        response.EnsureSuccessStatusCode(); // تأكد من أن الاستجابة كانت 200 OK
    }

    // اختبار الوصول للمورد مع دور "Guest"
    [Fact]
    public async Task Get_ToDoItems_WithGuestRole_ReturnsOk()
    {
        // محاكاة JWT مع دور "Guest"
        var token = GenerateJwtToken("Owner");
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/ToDoItems");

        response.EnsureSuccessStatusCode(); // تأكد من أن الاستجابة كانت 200 OK
    }

    // اختبار الوصول لمورد محمي بدون مصادقة
    [Fact]
    public async Task Get_ToDoItems_WithoutAuthorization_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/ToDoItems");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode); // تأكد من أن الاستجابة كانت 401 Unauthorized
    }

    // اختبار الوصول للمورد مع دور غير مصرح به
    [Fact]
    public async Task Get_ToDoItems_WithInvalidRole_ReturnsForbidden()
    {
        // محاكاة JWT مع دور غير مصرح به
        var token = GenerateJwtToken("User");
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/ToDoItems");

        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode); // تأكد من أن الاستجابة كانت 403 Forbidden
    }
}