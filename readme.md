# ApiResultToolkit
 
ApiResultToolkit is a simple and flexible helper library for handling API responses in .NET. It provides a fluent interface for executing asynchronous operations, validating data, and encapsulating success or failure results. It's ideal for building clean and maintainable APIs.

# Usage

Here's an example of how you can use ApiResultToolkit:

```cs
var result = new ApiResult<string>();
```

Execute without error and success handlers
```cs
await result.ExecuteAsync(() => Task.FromResult("test"));
```

Execute without validations
```cs
(await result.ExecuteAsync(() => Task.FromResult("test")))
    .OnSucces(data => Console.WriteLine(data))
    .OnError((error, errorType) => Console.WriteLine(error));
```

Execute with multiple validations (only with ValidationResult type)
```cs
await result.ExecuteAsync(() => Task.FromResult("test"));

Func<string, ValidationResult> validation = data => new ValidationResult
{
    IsValid = data != null,
    ErrorMessage = "Validation 1: Data cannot be null."
};
Func<string, ValidationResult> validation2 = data => new ValidationResult
{
    IsValid = data != "test",
    ErrorMessage = "Validation 2: Data cannot be 'test'."
};

(await result.ExecuteAsync(func: () => Task.FromResult("test"), validation, validation2))
    .OnSucces(data => Console.WriteLine(data))
    .OnError((error, errorType) => Console.WriteLine(error));
```

Execute with boolean validators
```cs
Func<string, bool> validation3 = data => data != null;

(await result.ExecuteAsync(func: () => Task.FromResult("test"), validation3))
    .OnSucces(data => Console.WriteLine(data))
    .OnError((error, errorType) => Console.WriteLine(error));
```
