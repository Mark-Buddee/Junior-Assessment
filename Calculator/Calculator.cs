using System.Text.Json;
using System.Xml.Linq;

public interface IInputParser
{
    Operation Parse(string input);
}

// BaseParser provides common functionality for both XML and JSON parsers
public abstract class BaseParser : IInputParser
{
    // Dictionary records which operator strings correspond to which Operation classes
    // To add a new operator, simply add it to this map and create the corresponding Operation class
    protected Dictionary<string, Func<Operation>> operationMap = new Dictionary<string, Func<Operation>>()
    {
        { "Plus", () => new Addition() },
        { "Multiplication", () => new Multiplication() },
        { "Subtraction", () => new Subtraction() },
        { "Division", () => new Division() }
    };

    public abstract Operation Parse(string input); // Derived parser classes must implement how to parse their specific input format into an Operation object
}

public class XmlParser : BaseParser
{
    private string rootElement;
    private string operationElement;
    private string idAttribute;
    private string valueElement;

    // Constructor allows customisation of XML structure
    public XmlParser(string rootElement = "Maths",
                     string operationElement = "Operation",
                     string idAttribute = "ID",
                     string valueElement = "Value")
    {
        this.rootElement = rootElement;
        this.operationElement = operationElement;
        this.idAttribute = idAttribute;
        this.valueElement = valueElement;
    }

    public override Operation Parse(string input)
    {
        XDocument doc = XDocument.Parse(input);
        XElement root = doc.Root ?? throw new ArgumentException("Invalid XML document");
        XElement firstOperation = root.Element(operationElement) ?? throw new ArgumentException("Operation element not found");
        return ParseOperation(firstOperation); // Recursively parse the XML into an Operation object
    }

    private Operation ParseOperation(XElement element)
    {
        // Operator ID
        string operatorID = element.Attribute(idAttribute)?.Value ?? throw new ArgumentException("Operation element is missing ID attribute");
        if (!operationMap.ContainsKey(operatorID))
        {
            throw new KeyNotFoundException($"Unknown operator: {operatorID}. Valid operators are: {string.Join(", ", operationMap.Keys)}");
        }

        // Values
        List<double> values = new List<double>();
        foreach (XElement val in element.Elements(valueElement))
        {
            values.Add(double.Parse(val.Value));
        }

        // Child operations
        List<Operation> children = new List<Operation>();
        foreach (XElement child in element.Elements(operationElement))
        {
            children.Add(ParseOperation(child));
        }

        Operation operation = operationMap[operatorID]();
        operation.Values = values;
        operation.ChildOperations = children;

        return operation;
    }
}

public class JsonParser : BaseParser
{
    private string rootProperty;
    private string operationProperty;
    private string idProperty;
    private string valueProperty;

    // Constructor allows customisation of JSON structure
    public JsonParser(string rootProperty = "Maths",
                      string operationProperty = "Operation",
                      string idProperty = "@ID",
                      string valueProperty = "Value")
    {
        this.rootProperty = rootProperty;
        this.operationProperty = operationProperty;
        this.idProperty = idProperty;
        this.valueProperty = valueProperty;
    }

    public override Operation Parse(string input)
    {
        JsonDocument doc = JsonDocument.Parse(input);
        JsonElement root = doc.RootElement;

        if (!root.TryGetProperty(rootProperty, out JsonElement maths))
            throw new ArgumentException($"Invalid JSON document - missing {rootProperty} element");

        if (!maths.TryGetProperty(operationProperty, out JsonElement firstOperation))
            throw new ArgumentException($"Operation element not found in {rootProperty}");

        return ParseOperation(firstOperation); // Recursively parse the JSON into an Operation object
    }

    private Operation ParseOperation(JsonElement element)
    {
        // Operator ID
        if (!element.TryGetProperty(idProperty, out JsonElement idElement))
            throw new ArgumentException("Operation element is missing ID attribute");
        string operatorID = idElement.GetString() ?? throw new ArgumentException("Operation ID cannot be null");
        if (!operationMap.ContainsKey(operatorID))
            throw new KeyNotFoundException($"Unknown operator: {operatorID}. Valid operators are: {string.Join(", ", operationMap.Keys)}");

        // Values
        List<double> values = new List<double>();
        if (element.TryGetProperty(valueProperty, out JsonElement valueArray))
        {
            foreach (JsonElement v in valueArray.EnumerateArray())
                values.Add(double.Parse(v.GetString() ?? throw new ArgumentException("Value cannot be null")));
        }

        // Child operations
        List<Operation> children = new List<Operation>();
        if (element.TryGetProperty(operationProperty, out JsonElement childElement))
        {
            if (childElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement child in childElement.EnumerateArray())
                    children.Add(ParseOperation(child));
            }
            else
            {
                Operation child = ParseOperation(childElement);
                children.Add(child);
            }
        }

        Operation operation = operationMap[operatorID]();
        operation.Values = values;
        operation.ChildOperations = children;

        return operation;
    }
}

// Base Operation class defines the structure for all operations, with a method to apply the operator to two values
public abstract class Operation
{
    public List<double> Values { get; set; } = new List<double>();
    public List<Operation> ChildOperations { get; set; } = new List<Operation>();
    public abstract double Apply(double a, double b); // derived operation classes must define how two values are combined
}

public class Addition : Operation
{
    public override double Apply(double a, double b) => a + b;
}

public class Subtraction : Operation
{
    public override double Apply(double a, double b) => a - b;
}

public class Multiplication : Operation
{
    public override double Apply(double a, double b) => a * b;
}

public class Division : Operation
{
    public override double Apply(double a, double b)
    {
        if (b == 0) throw new DivideByZeroException("Cannot divide by zero");
        return a / b;
    }
}

// Maths class evaluates an Operation by recursively evaluating any child operations and then applying the operator to all values
public class Maths
{
    public void Validate(Operation operation)
    {
        if (operation == null)
            throw new ArgumentNullException("Operation cannot be null");

        if (operation.Values.Count + operation.ChildOperations.Count < 2)
            throw new ArgumentException("Operation must have at least two values");
    }
    
    public double Evaluate(Operation operation)
    {
        Validate(operation);
        
        List<double> allValues = new List<double>(operation.Values);

        foreach (Operation child in operation.ChildOperations)
            allValues.Add(Evaluate(child));

        return allValues.Aggregate(operation.Apply);
    }
}
