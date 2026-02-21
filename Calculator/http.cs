using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/calculate", (HttpRequest request) =>
{
    using StreamReader reader = new StreamReader(request.Body);
    string input = reader.ReadToEndAsync().Result;

    // Default element/property names
    IInputParser parser = input.TrimStart().StartsWith("<") ? new XmlParser() : new JsonParser();
    
    // // Customised element/property names
    // IInputParser parser = input.TrimStart().StartsWith("<") ? new XmlParser("MyMaths", "MyOperation", "ID", "Value") : new JsonParser("MyMaths", "MyOperation", "@ID", "Value");

    Operation expression = parser.Parse(input);
    Maths maths = new Maths();
    double result = maths.Evaluate(expression);

    XElement response = new XElement("Result", result);
    return Results.Content(response.ToString(), "application/xml");
});

app.Run();

public partial class Program { }