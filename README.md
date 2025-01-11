# IceCreamCSharp 

## Version: 0.0.1

---

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
  - [Prerequisites](#prerequisites)
  - [Installing IceCreamCSharp](#installing-icecreamcsharp)
- [Getting Started](#getting-started)
  - [Basic Setup](#basic-setup)
- [Configuration](#configuration)
  - [Setting Log Level](#setting-log-level)
  - [Setting Log File Path](#setting-log-file-path)
  - [Setting Log Prefix](#setting-log-prefix)
  - [Enabling or Disabling Logging](#enabling-or-disabling-logging)
  - [Including Context Information](#including-context-information)
  - [Using Absolute or Relative File Paths](#using-absolute-or-relative-file-paths)
  - [Custom Argument-to-String Conversion](#custom-argument-to-string-conversion)
- [Usage](#usage)
  - [Logging Single Values](#logging-single-values)
  - [Logging Multiple Values](#logging-multiple-values)
  - [Logging Expressions](#logging-expressions)
  - [Detailed Logging](#detailed-logging)
  - [Pretty Logging](#pretty-logging)
  - [Asynchronous Logging](#asynchronous-logging)
- [API Reference](#api-reference)
  - [Enums](#enums)
  - [Delegates](#delegates)
  - [Methods](#methods)
- [Customization](#customization)
  - [Color Configuration](#color-configuration)
  - [Custom Argument Conversion](#custom-argument-conversion)
- [Examples](#examples)
- [Troubleshooting](#troubleshooting)
  - [Logs Are Not Being Created](#logs-are-not-being-created)
  - [Color Output Not Displaying Correctly](#color-output-not-displaying-correctly)
  - [Asynchronous Logs Not Appearing](#asynchronous-logs-not-appearing)
  - [Custom Argument Conversion Not Working](#custom-argument-conversion-not-working)
- [FAQ](#faq)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## Introduction
IceCreamCSharp is a powerful and flexible logging library for C# developers, inspired by the original IceCream library in Python. It simplifies debugging by providing easy-to-use methods for logging variables, expressions, and complex objects with contextual information and color-coded output. Additionally, it supports asynchronous logging to ensure that logging operations do not block the main execution thread.

---

## Features
- **Easy Logging:** Quickly log single values, multiple values, and expressions.
- **Contextual Information:** Automatically includes file name, line number, and method name in logs.
- **Color-Coded Output:** Differentiates log levels (INFO, WARN, ERROR) with distinct colors for enhanced readability.
- **Pretty Logging:** Formats complex objects using JSON serialization for clear and organized output.
- **Asynchronous Logging:** Perform logging operations asynchronously to avoid blocking the main thread.
- **Customization:** Easily customize log prefixes, output functions, and argument-to-string conversions.
- **Flexible Configuration:** Enable or disable logging, set log levels, and specify log file paths.

---

#### Add the Library to Your Project

Later

## Getting Started

### Basic Setup

**Import the Namespace:**
```csharp
using IceCreamCSharp;
```

**Configure the Logger (Optional):**
```csharp
IceCream.SetLogLevel(LogLevel.INFO);
IceCream.SetLogFile("logs/application_logs.txt");
IceCream.SetPrefix("ic| ");
IceCream.SetIncludeContext(true);
IceCream.SetContextAbsPath(false);
```

**Log a Simple Value:**
```csharp
int number = 42;
IceCream.ic(number);
```

---

## Configuration

### Setting Log Level
Control which log levels are recorded:
```csharp
IceCream.SetLogLevel(LogLevel.INFO); // Logs INFO, WARN, ERROR
IceCream.SetLogLevel(LogLevel.WARN); // Logs WARN, ERROR
IceCream.SetLogLevel(LogLevel.ERROR); // Logs only ERROR
```

### Setting Log File Path
Specify where the log file should be created:
```csharp
IceCream.SetLogFile("logs/application_logs.txt");
```

**Absolute Path Example:**
```csharp
IceCream.SetLogFile(@"C:\Logs\app_logs.txt");
```

### Setting Log Prefix
Customize the prefix that appears before each log message:
```csharp
IceCream.SetPrefix("DEBUG: ");
```

### Enabling or Disabling Logging
Toggle logging on or off globally:
```csharp
IceCream.EnableLogging(true); // Enable logging
IceCream.EnableLogging(false); // Disable logging
```

### Including Context Information
Control whether to include contextual information (file name, line number, method name) in logs:
```csharp
IceCream.SetIncludeContext(true); // Include context
IceCream.SetIncludeContext(false); // Exclude context
```

### Using Absolute or Relative File Paths
Specify whether to display absolute or relative paths in the context:
```csharp
IceCream.SetContextAbsPath(true); // Use absolute paths
IceCream.SetContextAbsPath(false); // Use relative paths
```

### Custom Argument-to-String Conversion
Define how objects are converted to strings in logs:
```csharp
IceCream.SetArgToStringFunction(obj =>
{
    if (obj is string s)
        return $"\"{s}\"";
    return obj?.ToString() ?? "null";
});
```

---

## Usage

### Logging Single Values
Log a single value with optional log level:
```csharp
int number = 42;
IceCream.ic(number); // Defaults to LogLevel.INFO
```

**With Specified Log Level:**
```csharp
string message = "An important message.";
IceCream.ic(message, LogLevel.WARN);
```

### Logging Multiple Values
Log multiple values simultaneously:
```csharp
int a = 10;
string b = "Test";
var c = new { Name = "Alice", Age = 30 };

IceCream.ic(LogLevel.INFO, values: new object[] { a, b, c });
```

### Logging Expressions
Log an expression and its result:
```csharp
int x = 5;
int y = 10;
IceCream.icExpression("x + y", x + y); // Logs: x + y = 15
```

### Detailed Logging
Log detailed information about an object, including its type:
```csharp
var user = new { Name = "Bob", Age = 25 };
IceCream.PrintDetailed(user, LogLevel.INFO); // Logs type and value
```

### Pretty Logging
Log objects in a formatted, readable JSON style:
```csharp
var complexObj = new
{
    Title = "Complex Object",
    Items = new[] { 1, 2, 3, 4, 5 },
    Nested = new { Key = "Value", Flag = true }
};

IceCream.icPretty(complexObj, LogLevel.INFO);
```

### Asynchronous Logging
Perform logging operations asynchronously to prevent blocking the main thread:
```csharp
await IceCream.PrintAsync("Asynchronous log message", LogLevel.INFO);
```

---

## API Reference

### Enums
#### LogLevel
Defines the severity level of log messages:
```csharp
public enum LogLevel
{
    INFO,
    WARN,
    ERROR
}
```
- **INFO:** Informational messages that highlight the progress of the application.
- **WARN:** Potentially harmful situations.
- **ERROR:** Error events that might still allow the application to continue running.

### Delegates
#### ArgToStringDelegate
A delegate that defines a method to convert an object to its string representation:
```csharp
public delegate string ArgToStringDelegate(object obj);
```

---

## Troubleshooting

### Logs Are Not Being Created
Possible causes and solutions are provided for common issues such as incorrect file paths, insufficient permissions, and more.

### Color Output Not Displaying Correctly
Verify terminal support and configure fallback options if needed.

---

## FAQ
**Can I Change the Log File Encoding?**
Modify the `LogToFile` method to specify a different encoding.

**Is It Possible to Log to Multiple Destinations?**
Extend the `PrintMessage` method for additional logging mechanisms.

---

## License
This project is licensed under the MIT License.

---


