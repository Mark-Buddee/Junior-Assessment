using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;

public class HttpIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public HttpIntegrationTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    // Valid XML returns 200 and correct result
    [Fact]
    public async Task xml_post_returns_correct_result()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>3</Value><Value>4</Value></Operation></Maths>";
        HttpContent content = new StringContent(xml, Encoding.UTF8, "application/xml");

        HttpResponseMessage response = await client.PostAsync("/calculate", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string body = await response.Content.ReadAsStringAsync();
        Assert.Contains("7", body);
    }

    // Valid JSON returns 200 and correct result
    [Fact]
    public async Task json_post_returns_correct_result()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Plus\",\"Value\":[\"3\",\"4\"]}}}";
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync("/calculate", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string body = await response.Content.ReadAsStringAsync();
        Assert.Contains("7", body);
    }

    // Response content type is XML
    [Fact]
    public async Task response_content_type_is_xml()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>3</Value><Value>4</Value></Operation></Maths>";
        HttpContent content = new StringContent(xml, Encoding.UTF8, "application/xml");

        HttpResponseMessage response = await client.PostAsync("/calculate", content);

        Assert.Equal("application/xml", response.Content.Headers.ContentType?.MediaType);
    }

    // Nested operation returns correct result
    // 2 + 3 + (4 * 5) = 25
    [Fact]
    public async Task xml_nested_operation_returns_correct_result()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>2</Value><Value>3</Value><Operation ID=\"Multiplication\"><Value>4</Value><Value>5</Value></Operation></Operation></Maths>";
        HttpContent content = new StringContent(xml, Encoding.UTF8, "application/xml");

        HttpResponseMessage response = await client.PostAsync("/calculate", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string body = await response.Content.ReadAsStringAsync();
        Assert.Contains("25", body);
    }

    // Invalid XML returns 500
    [Fact]
    public async Task invalid_xml_returns_error()
    {
        string xml = "this is not xml";
        HttpContent content = new StringContent(xml, Encoding.UTF8, "application/xml");

        HttpResponseMessage response = await client.PostAsync("/calculate", content);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // Invalid JSON returns 500
    [Fact]
    public async Task invalid_json_returns_error()
    {
        string json = "this is not json";
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync("/calculate", content);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}