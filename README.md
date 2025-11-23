<h2 align="center">Russkyc.Messaging - Independently packaged Messaging subset from CommunityToolkit.MVVM</h2>

<p align="center">
    <img src="https://img.shields.io/badge/-.NET%208.0-blueviolet?color=1f72de&label=NET" alt="">
    <img src="https://img.shields.io/badge/-.NET%209.0-blueviolet?color=1f72de&label=NET" alt="">
    <img src="https://img.shields.io/badge/-.NET%2010.0-blueviolet?color=1f72de&label=NET" alt="">
    <img src="https://img.shields.io/github/license/russkyc/minimalapi-framework">
    <img src="https://img.shields.io/github/issues/russkyc/minimalapi-framework">
    <img src="https://img.shields.io/nuget/v/Russkyc.Messaging?color=1f72de" alt="Nuget">
    <img src="https://img.shields.io/nuget/dt/Russkyc.Messaging">
</p>

The lightweight, high-performance messaging library extracted from the [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) project. This library provides a decoupled way for different parts of your application to communicate through messages, supporting both strong and weak reference implementations.

## Features

- **Decoupled Communication**: Send messages between different components without tight coupling
- **Multiple Messenger Types**: Choose between `StrongReferenceMessenger` and `WeakReferenceMessenger`
- **Channel Support**: Use tokens to create separate communication channels
- **Thread-Safe**: All operations are thread-safe
- **High Performance**: Optimized for minimal overhead
- **Observable Support**: Integration with reactive programming patterns
- **Request/Response Patterns**: Built-in support for request messages and responses

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package Russkyc.Messaging
```

Or via Package Manager Console:

```powershell
Install-Package Russkyc.Messaging
```

## Quick Start

### Basic Usage

```csharp
using Russkyc.Messaging;

// In your main method or somewhere else
var handler = new MessageHandler();

// Send a message
WeakReferenceMessenger.Default.Send(new UserLoggedInMessage("john_doe"));

// Define a message
public record UserLoggedInMessage(string Username);

// Create a recipient
public class MessageHandler
{
    public MessageHandler()
    {
        // Register to receive messages
        WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, OnUserLoggedIn);
    }

    private void OnUserLoggedIn(object recipient, UserLoggedInMessage message)
    {
        Console.WriteLine($"User {message.Username} logged in!");
    }
}
```

### Using Custom Messengers

```csharp
// Create a custom messenger instance
var messenger = new WeakReferenceMessenger();

// Create a handler instance
var handler = new MessageHandler();

// Register a recipient
messenger.Register<UserLoggedInMessage>(handler, (recipient, message) =>
{
    Console.WriteLine($"Custom messenger: User {message.Username} logged in!");
});

// Send a message
messenger.Send(new UserLoggedInMessage("jane_doe"));
```

### Using Channels (Tokens)

```csharp
// Create a handler for admin messages
var adminHandler = new object();

// Register on a specific channel
WeakReferenceMessenger.Default.Register<UserLoggedInMessage, AdminChannel>(
    adminHandler, 
    new AdminChannel(), 
    (recipient, message) => Console.WriteLine($"Admin: {message.Username} logged in!"));

// Send to a specific channel
WeakReferenceMessenger.Default.Send(
    new UserLoggedInMessage("admin"), 
    new AdminChannel());

// Define a token for a specific channel
public class AdminChannel { }

```

## Request/Response Patterns

The library supports both synchronous and asynchronous request/response messaging patterns using `RequestMessage<T>` and `AsyncRequestMessage<T>` respectively.

### Synchronous Requests

Synchronous requests allow you to send a message and immediately receive a response:

```csharp
using Russkyc.Messaging;

// Create a handler for requests
var requestHandler = new object();

// Handle the request
WeakReferenceMessenger.Default.Register<GetUserDataRequest>(requestHandler, (recipient, message) =>
{
    // Simulate data retrieval
    var userData = new UserData { Name = "John Doe", Email = "john@example.com" };
    message.Reply(userData);
});

// Send a request and get response
var request = new GetUserDataRequest { UserId = "123" };
var response = WeakReferenceMessenger.Default.Send(request);
Console.WriteLine($"User: {response.Name}, Email: {response.Email}");

// Define a request message
public class GetUserDataRequest : RequestMessage<UserData>
{
    public string UserId { get; init; }
}

// Define response data
public class UserData
{
    public string Name { get; set; }
    public string Email { get; set; }
}
```

### Asynchronous Requests

Asynchronous requests are useful when the response requires async operations like I/O:

```csharp
using Russkyc.Messaging;

// Create a handler for async requests
var asyncHandler = new object();

// Handle async request
WeakReferenceMessenger.Default.Register<AsyncDataRequest>(asyncHandler, async (recipient, message) =>
{
    // Simulate async data retrieval
    await Task.Delay(100); // Simulate async work
    var data = "Async data result";
    message.Reply(data);
});

// Send async request
var request = new AsyncDataRequest();
var response = await WeakReferenceMessenger.Default.Send(request);
Console.WriteLine($"Data: {response}");

// Define an async request message
public class AsyncDataRequest : AsyncRequestMessage<string> { }
```

## Messenger Types

### WeakReferenceMessenger (Recommended)

- Uses weak references to track recipients
- Recipients are automatically unregistered when garbage collected
- Prevents memory leaks
- Slightly higher overhead due to weak reference management

```csharp
var messenger = new WeakReferenceMessenger();
```

### StrongReferenceMessenger

- Uses strong references to track recipients
- Recipients must be manually unregistered
- Better performance
- Risk of memory leaks if not properly managed

```csharp
var messenger = new StrongReferenceMessenger();
```

## API Reference

### Core Interfaces

- `IMessenger`: Main interface for messaging functionality

### Message Types

The library provides several built-in message types for different communication patterns:

- **RequestMessage<T>**: Synchronous request/response messaging. Handlers call `message.Reply(T response)` to send a response.
- **AsyncRequestMessage<T>**: Asynchronous request/response messaging. Handlers call `message.Reply(T response)` after async operations.
- **CollectionRequestMessage<T>**: Request for collections of data. Handlers call `message.Reply(IEnumerable<T> response)`.
- **AsyncCollectionRequestMessage<T>**: Asynchronous request for collections. Handlers call `message.Reply(IEnumerable<T> response)`.
- **PropertyChangedMessage<T>**: Notifies about property changes. Contains `PropertyName` and `OldValue`/`NewValue`.
- **ValueChangedMessage<T>**: Notifies about value changes. Contains `OldValue` and `NewValue`.

### Key Methods

- `Register<TMessage>(recipient, handler)`: Register a message handler
- `Unregister<TMessage>(recipient)`: Unregister from a message type
- `Send<TMessage>(message)`: Send a message to all registered recipients
- `IsRegistered<TMessage>(recipient)`: Check if a recipient is registered

### Extension Methods

The `IMessengerExtensions` class provides convenient extension methods for common operations.

## Performance Considerations

- Use `WeakReferenceMessenger` for long-lived applications to prevent memory leaks
- Prefer `StrongReferenceMessenger` for short-lived scenarios where performance is critical
- Use channels (tokens) to reduce message broadcasting overhead
- Consider the frequency of message sending in performance-critical paths

## Building from Source

1. Clone the repository
2. Ensure you have .NET 8.0 SDK installed
3. Run `dotnet build` in the project directory

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## Credits

This library is a subset of the messaging functionality from [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet), extracted and independently packaged.

The original code is licensed under the MIT License by the .NET Foundation and Contributors.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
