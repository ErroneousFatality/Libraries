# DotNet
[![latest version](https://img.shields.io/nuget/v/AndrejKrizan.DotNet)](https://www.nuget.org/packages/AndrejKrizan.DotNet)

## Utils
### OrAsync

Useful when you want to concurrently check if any of given condition sources, which are either asynchronous or long-running, will return `true`.
- Finishes as soon as any of them return `true` or after all of them return `false`.
- When one condition task resolves truthfully, all other unresolved tasks are cancelled.
- Accepts a cancellation token which cancells all unresolved tasks upon its cancellation.

The method has a declaration category for every of the following condition types:
- `Func<bool>` (function)
- `Func<Task<bool>>` (asynchronous function)
- `Func<CancellationToken, Task<bool>>` (cancellable asynchronous function)
- `Task<bool>` (an already started task)

Every declaration category accepts its types as either an `IEnumerable<T>` parameter, or two required parameters with as many additional parameters as needed. 
In both versions it can also accept a `CancellationToken`.

The method also has a special declaration category which accepts any combination of those types along with the `bool` type, when cast to the wrapper class `Condition`. 

#### Examples
Ideally you want to use the cancellable asynchronous function type for your condition arguments, so they can be all gracefully stopped when the first of them resolves to `true`, or when the method's `CancellationToken` parameter is canceled.

##### Internal cancellation example:

```cs
Stopwatch stopwatch = Stopwatch.StartNew();
bool result = await Utils.OrAsync(
    async cancellationToken => await PrintAndReturnAfterDelayAsync(100, false, cancellationToken),
    async cancellationToken => await PrintAndReturnAfterDelayAsync(200, false, cancellationToken),
    async cancellationToken => await PrintAndReturnAfterDelayAsync(300, true, cancellationToken),
    async cancellationToken => await PrintAndReturnAfterDelayAsync(400, false, cancellationToken),
    async cancellationToken => await PrintAndReturnAfterDelayAsync(500, true, cancellationToken)
);
stopwatch.Stop();
Console.WriteLine($"Final result after {stopwatch.ElapsedMilliseconds}ms: " + result);
```

Prints:
```
Condition after 100ms: False
Condition after 200ms: False
Condition after 300ms: True
Final result after 319ms: True
```

##### External cancellation example:
```cs
using CancellationTokenSource cancellationTokenSource = new();
Stopwatch stopwatch = Stopwatch.StartNew();

Task<bool> resultTask = Utils.OrAsync(cancellationTokenSource.Token,
    async cancellationToken => await PrintAndReturnAfterDelayAsync(100, false, cancellationToken),
    async cancellationToken => await PrintAndReturnAfterDelayAsync(200, false, cancellationToken),
    async cancellationToken => await PrintAndReturnAfterDelayAsync(300, true, cancellationToken),
    async cancellationToken => await PrintAndReturnAfterDelayAsync(400, false, cancellationToken),
    async cancellationToken => await PrintAndReturnAfterDelayAsync(500, true, cancellationToken)
);

_ = Task.Run(async () => { 
    await Task.Delay(200); 
    cancellationTokenSource.Cancel(); 
});

string result;
try
{
    result = (await resultTask).ToString();
}
catch (OperationCanceledException)
{
    result = "Canceled";
}

stopwatch.Stop();
Console.WriteLine($"Final result after {stopwatch.ElapsedMilliseconds}ms: " + result);
```

Prints:
```
Condition after 100ms: False
Condition after 200ms: False
Final result after 261ms: Cancelled
```

##### Multi-type support example:
```cs
bool value = await PrintAndReturnAfterDelayAsync(0, false);
Func<bool> function = () 
	=> PrintAndReturnAfterDelayAsync(100, false).Result;
Func<Task<bool>> asyncFunction = async () 
	=> await PrintAndReturnAfterDelayAsync(200, true);
Func<CancellationToken, Task<bool>> cancellableAsyncFunction = async (CancellationToken cancellationToken) 
	=> await PrintAndReturnAfterDelayAsync(300, false, cancellationToken);
Task<bool> task = PrintAndReturnAfterDelayAsync(400, true);

Stopwatch stopwatch = Stopwatch.StartNew();
bool result = await Utils.OrAsync(
    (Condition)value,
    (Condition)function,
    (Condition)asyncFunction,
    (Condition)cancellableAsyncFunction,
    (Condition)task
);
stopwatch.Stop();
Console.WriteLine($"Result after {stopwatch.ElapsedMilliseconds}ms: " + result);
```

Prints:
```
Condition after 0ms: False
Condition after 100ms: False
Condition after 200ms: True
Result after 211ms: True
Condition after 400ms: True
```
