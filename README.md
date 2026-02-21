# Calculator

A C# calculator application service that receives XML and JSON via HTTP POST, performs mathematical operations, and sends the answer back over HTTP/XML.

## Design Overview

### Parsing
Input is received as a raw string and is parsed into an object tree. `IInputParser` class ensures derived classes implement a `Parse()` method, `BaseParser` defines common functionality, and `XmlParser`/`JsonParser` implement format-specific parsing.

Element/property names can be customised via construction arguments, allowing for various XML/JSON structures with minimal code changes.

```csharp
// Default structure
IInputParser parser = new XmlParser();

// Customised structure
IInputParser parser = new XmlParser("MyMaths", "MyOperation", "OperationID", "Val");
```

### Operations

The `Operation` base class holds `Values` and `ChildOperations`. Each derived class also implements `Apply(double a, double b)` to define how two values should be combined.

Nested operations are represented as child `Operation` objects, mirroring the nested structure of the input XML/JSON.

To add a new supported operator, add a single entry to the `operationMap` in `BaseParser` and create the corresponding `Operation` subclass.

### Evaluation

`Maths.Evaluate` recursively resolves child operations first, then applies the parent operation to all values.

## Input Formats

### XML
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Maths>
  <Operation ID="Plus">
    <Value>2</Value>
    <Value>3</Value>
    <Operation ID="Multiplication">
      <Value>4</Value>
      <Value>5</Value>
    </Operation>
  </Operation>
</Maths>
```

### JSON
```json
{
  "Maths": {
    "Operation": {
      "@ID": "Plus",
      "Value": ["2", "3"],
      "Operation": {
        "@ID": "Multiplication",
        "Value": ["4", "5"]
      }
    }
  }
}
```

Both inputs represent: `2 + 3 + (4 × 5) = 25`

## Running the Application

```bash
cd Calculator
dotnet run
```

The server starts on `http://localhost:5000`.

## Sending Requests

### XML
```powershell
curl -Method POST http://localhost:5000/calculate -Body '<?xml version="1.0"?><Maths><Operation ID="Plus"><Value>3</Value><Value>4</Value></Operation></Maths>'
```

### JSON
```powershell
curl -Method POST http://localhost:5000/calculate -Body '{"Maths":{"Operation":{"@ID":"Plus","Value":["3","4"]}}}' 
```

### Response
```xml
<Result>7</Result>
```

## Running Tests
Tests are writting using the xUnit framework.
```bash
cd Calculator.Tests
dotnet test
```

## Exception Handling

| Exception | Cause |
|---|---|
| `ArgumentException` | Missing root element, missing `Operation`, missing `ID` attribute |
| `KeyNotFoundException` | Unrecognised operator |
| `DivideByZeroException` | Division by zero |
| `XmlException` | Bad XML input |
| `JsonException` | Bad JSON input |
| `ArgumentNullException` | Null operation passed to `Maths` |

## Limitations
Order of operations (BODMAS) is not automatically enforced. Instead, the correct evaluation order must be expressed explicitly through the nesting structure of the XML/JSON input.

Supporting BODMAS automatically would require an additional function (Shunting Yard Algorithm) to reorder the operation tree before evaluation.