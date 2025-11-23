# Russkyc.Messaging

[![NuGet](https://img.shields.io/nuget/v/Russkyc.Messaging.svg)](https://www.nuget.org/packages/Russkyc.Messaging/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

A lightweight, high-performance messaging library extracted from the [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) project. This library provides a decoupled way for different parts of your application to communicate through messages, supporting both strong and weak reference implementations.

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

// In your main method or somewhere else
var handler = new MessageHandler();

// Send a message
WeakReferenceMessenger.Default.Send(new UserLoggedInMessage("john_doe"));
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
// Define a token for a specific channel
public class AdminChannel { }

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
```

### Request/Response Pattern

```csharp
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
Console.WriteLine($"User: {response.Name}");
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

## Advanced Usage

### Async Messages

```csharp
// Define an async request message
public class AsyncDataRequest : AsyncRequestMessage<string> { }

// Create a handler for async requests
var asyncHandler = new object();

// Handle async request
WeakReferenceMessenger.Default.Register<AsyncDataRequest>(asyncHandler, async (recipient, message) =>
{
    var data = await FetchDataAsync();
    message.Reply(data);
});

// Send async request
var request = new AsyncDataRequest();
var response = await WeakReferenceMessenger.Default.Send(request);
Console.WriteLine($"Data: {response}");
```

## API Reference

### Core Interfaces

- `IMessenger`: Main interface for messaging functionality
- `IRecipient<TMessage>`: Interface for recipients that handle specific message types

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
