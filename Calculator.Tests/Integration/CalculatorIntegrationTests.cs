using System.Collections.Generic;

public class CalculatorIntegrationTests
{
    // Simple operations
    // 3 + 4 = 7
    [Fact]
    public void xml_simple_addition()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>3</Value><Value>4</Value></Operation></Maths>";
        IInputParser parser = new XmlParser();
        Operation expression = parser.Parse(xml);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(7, result);
    }

    // 3 + 4 = 7
    [Fact]
    public void json_simple_addition()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Plus\",\"Value\":[\"3\",\"4\"]}}}";
        IInputParser parser = new JsonParser();
        Operation expression = parser.Parse(json);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(7, result);
    }

    // Nested operations
    // 2 + 3 + (4 * 5) = 25
    [Fact]
    public void xml_nested_operation1()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>2</Value><Value>3</Value><Operation ID=\"Multiplication\"><Value>4</Value><Value>5</Value></Operation></Operation></Maths>";
        IInputParser parser = new XmlParser();
        Operation expression = parser.Parse(xml);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(25, result);
    }

    // 2 + 3 + (4 * 5) = 25
    [Fact]
    public void json_nested_operation1()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Plus\",\"Value\":[\"2\",\"3\"],\"Operation\":{\"@ID\":\"Multiplication\",\"Value\":[\"4\",\"5\"]}}}}";
        IInputParser parser = new JsonParser();
        Operation expression = parser.Parse(json);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(25, result);
    }

    // (10 - 4) * (8 / 2) = 24
    [Fact]
    public void xml_nested_operation2()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Multiplication\"><Operation ID=\"Subtraction\"><Value>10</Value><Value>4</Value></Operation><Operation ID=\"Division\"><Value>8</Value><Value>2</Value></Operation></Operation></Maths>";
        IInputParser parser = new XmlParser();
        Operation expression = parser.Parse(xml);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(24, result);
    }

    // (10 - 4) * (8 / 2) = 24
    [Fact]
    public void json_nested_operation2()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Multiplication\",\"Operation\":[{\"@ID\":\"Subtraction\",\"Value\":[\"10\",\"4\"]},{\"@ID\":\"Division\",\"Value\":[\"8\",\"2\"]}]}}}";
        IInputParser parser = new JsonParser();
        Operation expression = parser.Parse(json);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(24, result);
    }

    // (3 * 4) - (10 / 2) + 1 = 8
    [Fact]
    public void xml_nested_operation3()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>1</Value><Operation ID=\"Subtraction\"><Operation ID=\"Multiplication\"><Value>3</Value><Value>4</Value></Operation><Operation ID=\"Division\"><Value>10</Value><Value>2</Value></Operation></Operation></Operation></Maths>";
        IInputParser parser = new XmlParser();
        Operation expression = parser.Parse(xml);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(8, result);
    }

    // (3 * 4) - (10 / 2) + 1 = 8
    [Fact]
    public void json_nested_operation3()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Plus\",\"Value\":[\"1\"],\"Operation\":{\"@ID\":\"Subtraction\",\"Operation\":[{\"@ID\":\"Multiplication\",\"Value\":[\"3\",\"4\"]},{\"@ID\":\"Division\",\"Value\":[\"10\",\"2\"]}]}}}}";
        IInputParser parser = new JsonParser();
        Operation expression = parser.Parse(json);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(8, result);
    }

    // Without correct nesting, 2 + 3 * 4 is evaluated left to right as (2 + 3) * 4 = 20, not 14
    [Fact]
    public void xml_order_of_operations()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Multiplication\"><Operation ID=\"Plus\"><Value>2</Value><Value>3</Value></Operation><Value>4</Value></Operation></Maths>";
        IInputParser parser = new XmlParser();
        Operation expression = parser.Parse(xml);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(14, result);
    }

    [Fact]
    public void json_order_of_operations()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Multiplication\",\"Value\":[\"4\"],\"Operation\":{\"@ID\":\"Plus\",\"Value\":[\"2\",\"3\"]}}}}";
        IInputParser parser = new JsonParser();
        Operation expression = parser.Parse(json);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(14, result);
    }

    // Fractional values
    // 1.5 + 2.5 = 4.0
    [Fact]
    public void xml_fractional_values()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>1.5</Value><Value>2.5</Value></Operation></Maths>";
        IInputParser parser = new XmlParser();
        Operation expression = parser.Parse(xml);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(4.0, result);
    }

    // 1.5 + 2.5 = 4.0
    [Fact]
    public void json_fractional_values()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Plus\",\"Value\":[\"1.5\",\"2.5\"]}}}";
        IInputParser parser = new JsonParser();
        Operation expression = parser.Parse(json);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(4.0, result);
    }

    // Alternative XML structure (MyMaths/MyOperation)
    // 3 + 4 = 7
    [Fact]
    public void xml_alternative_structure()
    {
        string xml = "<?xml version=\"1.0\"?><MyMaths><MyOperation ID=\"Plus\"><Value>3</Value><Value>4</Value></MyOperation></MyMaths>";
        IInputParser parser = new XmlParser("MyMaths", "MyOperation");
        Operation expression = parser.Parse(xml);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(7, result);
    }

    // Alternative JSON structure (MyMaths/MyOperation)
    // 3 + 4 = 7
    [Fact]
    public void json_alternative_structure()
    {
        string json = "{\"MyMaths\":{\"MyOperation\":{\"@ID\":\"Plus\",\"Value\":[\"3\",\"4\"]}}}";
        IInputParser parser = new JsonParser("MyMaths", "MyOperation");
        Operation expression = parser.Parse(json);
        Maths maths = new Maths();
        double result = maths.Evaluate(expression);
        Assert.Equal(7, result);
    }

    // Division by zero
    // 4 / 0 = error
    [Fact]
    public void xml_division_by_zero()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Division\"><Value>4</Value><Value>0</Value></Operation></Maths>";
        IInputParser parser = new XmlParser();
        Operation expression = parser.Parse(xml);
        Maths maths = new Maths();
        Assert.Throws<DivideByZeroException>(() => maths.Evaluate(expression));
    }

    // 4 / 0 = error
    [Fact]
    public void json_division_by_zero()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Division\",\"Value\":[\"4\",\"0\"]}}}";
        IInputParser parser = new JsonParser();
        Operation expression = parser.Parse(json);
        Maths maths = new Maths();
        Assert.Throws<DivideByZeroException>(() => maths.Evaluate(expression));
    }

    // Missing Operation element - ArgumentException thrown by XmlParser.Parse
    [Fact]
    public void xml_missing_operation()
    {
        string xml = "<?xml version=\"1.0\"?><Maths></Maths>";
        IInputParser parser = new XmlParser();
        Assert.Throws<ArgumentException>(() => parser.Parse(xml));
    }

    [Fact]
    public void json_missing_operation()
    {
        string json = "{\"Maths\":{}}";
        IInputParser parser = new JsonParser();
        Assert.Throws<ArgumentException>(() => parser.Parse(json));
    }

    // Missing ID attribute - ArgumentException thrown by ParseOperation
    [Fact]
    public void xml_missing_id_attribute()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation><Value>3</Value><Value>4</Value></Operation></Maths>";
        IInputParser parser = new XmlParser();
        Assert.Throws<ArgumentException>(() => parser.Parse(xml));
    }

    [Fact]
    public void json_missing_id_attribute()
    {
        string json = "{\"Maths\":{\"Operation\":{\"Value\":[\"3\",\"4\"]}}}";
        IInputParser parser = new JsonParser();
        Assert.Throws<ArgumentException>(() => parser.Parse(json));
    }

    // Unknown operator - KeyNotFoundException thrown by ParseOperation
    [Fact]
    public void xml_invalid_operator()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Modulus\"><Value>4</Value><Value>2</Value></Operation></Maths>";
        IInputParser parser = new XmlParser();
        Assert.Throws<KeyNotFoundException>(() => parser.Parse(xml));
    }

    [Fact]
    public void json_invalid_operator()
    {
        string json = "{\"Maths\":{\"Operation\":{\"@ID\":\"Modulus\",\"Value\":[\"4\",\"2\"]}}}";
        IInputParser parser = new JsonParser();
        Assert.Throws<KeyNotFoundException>(() => parser.Parse(json));
    }

    // Too few values - ArgumentException thrown by Maths.Validate
    [Fact]
    public void xml_too_few_values()
    {
        string xml = "<?xml version=\"1.0\"?><Maths><Operation ID=\"Plus\"><Value>3</Value></Operation></Maths>";
        IInputParser parser = new XmlParser();
        Operation expression = parser.Parse(xml);
        Maths maths = new Maths();
        Assert.Throws<ArgumentException>(() => maths.Evaluate(expression));
    }
}