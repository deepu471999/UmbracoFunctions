using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace UmbracoFunction;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<FormData>(requestBody, options);

        if (data == null || string.IsNullOrEmpty(data.Email))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid request payload.");
            return badResponse;
        }

        _logger.LogInformation("Form submission received:");
        _logger.LogInformation($"Name: {data.Name}");
        _logger.LogInformation($"Email: {data.Email}");
        _logger.LogInformation($"Description: {data.Description}");

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Data logged successfully.");
        return response;
    }

    public class FormData
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
    }
}