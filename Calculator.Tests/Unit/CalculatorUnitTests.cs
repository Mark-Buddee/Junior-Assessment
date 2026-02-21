public class MathsUnitTests
{
    private Maths maths = new Maths();

    // 3 + 4 = 7
    [Fact]
    public void addition()
    {
        Operation operation = new Addition() { Values = new List<double>() { 3, 4 } };
        Assert.Equal(7, maths.Evaluate(operation));
    }

    // 10 - 4 = 6
    [Fact]
    public void subtraction()
    {
        Operation operation = new Subtraction() { Values = new List<double>() { 10, 4 } };
        Assert.Equal(6, maths.Evaluate(operation));
    }

    // 4 * 5 = 20
    [Fact]
    public void multiplication()
    {
        Operation operation = new Multiplication() { Values = new List<double>() { 4, 5 } };
        Assert.Equal(20, maths.Evaluate(operation));
    }

    // 8 / 2 = 4
    [Fact]
    public void division()
    {
        Operation operation = new Division() { Values = new List<double>() { 8, 2 } };
        Assert.Equal(4, maths.Evaluate(operation));
    }

    // 2 + 3 + (4 * 5) = 25
    [Fact]
    public void complex_equation()
    {
        Operation inner = new Multiplication() { Values = new List<double>() { 4, 5 } };
        Operation outer = new Addition() { Values = new List<double>() { 2, 3 }, ChildOperations = new List<Operation>() { inner } };
        Assert.Equal(25, maths.Evaluate(outer));
    }

    // too few values
    [Fact]
    public void too_few_values()
    {
        Operation operation = new Addition() { Values = new List<double>() { 3 } };
        Assert.Throws<ArgumentException>(() => maths.Validate(operation));
    }

    // 4 / 0 = error
    [Fact]
    public void division_by_zero()
    {
        Operation operation = new Division() { Values = new List<double>() { 4, 0 } };
        Assert.Throws<DivideByZeroException>(() => maths.Evaluate(operation));
    }
}

public class XmlParserUnitTests
{
    private XmlParser parser = new XmlParser();

    // Parse returns correct operation type
    [Fact]
    public void parse_plus_returns_addition()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>3</Value><Value>4</Value></Operation></Maths>";
        Assert.IsType<Addition>(parser.Parse(xml));
    }

    // Parse returns correct values
    [Fact]
    public void parse_returns_correct_values()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>3</Value><Value>4</Value></Operation></Maths>";
        Operation result = parser.Parse(xml);
        Assert.Equal(new List<double>() { 3, 4 }, result.Values);
    }

    // Parse returns correct number of children
    [Fact]
    public void return_one_child()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>2</Value><Operation ID=\"Multiplication\"><Value>4</Value><Value>5</Value></Operation></Operation></Maths>";
        Operation result = parser.Parse(xml);
        Assert.Single(result.ChildOperations);
    }

    // Missing Operation element
    [Fact]
    public void missing_operation()
    {
        string xml = "<?xml version=\"1.0\"?><Maths></Maths>";
        Assert.Throws<ArgumentException>(() => parser.Parse(xml));
    }

    // Missing ID attribute
    [Fact]
    public void missing_id()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation><Value>3</Value><Value>4</Value></Operation></Maths>";
        Assert.Throws<ArgumentException>(() => parser.Parse(xml));
    }

    // Unknown operator
    [Fact]
    public void unknown_operator()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Modulus\"><Value>4</Value><Value>2</Value></Operation></Maths>";
        Assert.Throws<KeyNotFoundException>(() => parser.Parse(xml));
    }
}

public class JsonParserUnitTests
{
    private JsonParser parser = new JsonParser();

    // Parse returns correct operation type
    [Fact]
    public void parse_plus_returns_addition()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Plus\",\"Value\":[\"3\",\"4\"]}}}";
        Assert.IsType<Addition>(parser.Parse(json));
    }

    // Parse returns correct values
    [Fact]
    public void parse_returns_correct_values()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Plus\",\"Value\":[\"3\",\"4\"]}}}";
        Operation result = parser.Parse(json);
        Assert.Equal(new List<double>() { 3, 4 }, result.Values);
    }

    // Parse returns correct number of children
    [Fact]
    public void return_one_child()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Plus\",\"Value\":[\"2\"],\"Operation\":{\"@ID\":\"Multiplication\",\"Value\":[\"4\",\"5\"]}}}}";
        Operation result = parser.Parse(json);
        Assert.Single(result.ChildOperations);
    }

    // Missing Operation element
    [Fact]
    public void missing_operation()
    {
        string json = "{\"Maths\":{}}";
        Assert.Throws<ArgumentException>(() => parser.Parse(json));
    }

    // Missing ID attribute
    [Fact]
    public void missing_id_attribute()
    {
        string json = "{\"Maths\":{\"Operation\":{\"Value\":[\"3\",\"4\"]}}}";
        Assert.Throws<ArgumentException>(() => parser.Parse(json));
    }

    // Unknown operator
    [Fact]
    public void unknown_operator()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Modulus\",\"Value\":[\"4\",\"2\"]}}}";
        Assert.Throws<KeyNotFoundException>(() => parser.Parse(json));
    }
}