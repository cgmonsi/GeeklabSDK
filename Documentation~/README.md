
# Geeklab SDK

Geeklab SDK is a set of utilities for development in Unity. It includes tools for handling deep links, collecting metrics, managing network requests, and much more.

## Directory Structure
```
├── Handlers
│   ├── DeepLinkHandler.cs
│   ├── DeviceInfoHandler.cs
│   ├── ClipboardHandler.cs
│   ├── TokenHandler.cs
├── Metrics
│   ├── EngagementMetrics.cs
│   ├── PurchaseMetrics.cs
│   ├── AdMetrics.cs
├── Managers
│   ├── WebRequestManager.cs
│   ├── CoroutineService.cs
│   ├── ApiEndpoints.cs
├── Utils
│   ├── MetricToggle.cs
├── Models
│   ├── DeviceInfo.cs
│   ├── Metrics.cs
│   ├── TokenResponse.cs
```


## Component Descriptions

### Handlers

- `DeepLinkHandler.cs`: Handles deep linking in Unity. Supports handling URLs passed through deep links and passes them to the appropriate services.

- `DeviceInfoHandler.cs`: Gathers and stores information about the device. Used for device identification and provides important information that can affect app behavior.

- `ClipboardHandler.cs`: Handles the clipboard contents. Allows for reading and writing data to the clipboard.

- `TokenHandler.cs`: Handles authorization tokens. Sends requests to the server for token retrieval, stores retrieved tokens, and checks token lifetimes.

### Metrics

- `EngagementMetrics.cs`: Collects user engagement metrics. Can track parameters such as session time, user activity, UI interaction, etc.

- `PurchaseMetrics.cs`: Collects purchase metrics. Tracks user purchases within the app, including transaction details and currency used.

- `AdMetrics.cs`: Collects advertising metrics. Can track ad displays, clicks, and user interaction with advertisements.

### Managers

- `WebRequestManager.cs`: Manages network requests. Supports sending and receiving data, handling server responses, and managing request errors.

- `CoroutineService.cs`: Provides an interface for Unity coroutines. Allows for starting, pausing, and stopping coroutines.

- `ApiEndpoints.cs`: Manages API endpoints. Contains URL definitions and handles URL construction for API requests.

### Utils

- `MetricToggle.cs`: A utility script for enabling and disabling metrics collection.

### Models

- `DeviceInfo.cs`: A model for device information. Contains fields for device specifications and operating system information.

- `Metrics.cs`: A model for storing collected metrics. Contains fields for various types of metrics, such as engagement, purchase, and ad metrics.

- `TokenResponse.cs`: A model for token response data. Contains fields for token information, such as token value and expiry.


---

## Installation

To install the Geeklab SDK, simply import the package into your Unity project. The provided scripts can then be attached to game objects or used as needed in your project.

---

## Usage

Here's a brief description of how to use the main components:

### Handlers

To use a handler, simply attach the script to a game object and call the necessary methods from your other scripts.

For instance, if you wanted to handle a deep link, you would attach the `DeepLinkHandler.cs` script to a game object, then call the `HandleDeepLink(string url)` method with the URL you want to handle.

### Metrics

The metrics scripts work by collecting data from various sources and storing them in the `Metrics.cs` model. You can enable or disable the collection of specific types of metrics using the `MetricToggle.cs` utility script.

### Managers

The managers handle specific tasks such as network requests or coroutine management.

For instance, to make a network request, you would use the `WebRequestManager.cs` script. Simply attach it to a game object, then call the `SendRequest(string url, HttpMethod method, string body)` method with the necessary parameters.

### Models

The models are simple classes that store data. They contain fields for various types of data, such as device information or metrics.

To use a model, simply create an instance of it and fill in the fields as necessary. For instance, you could create a new `DeviceInfo.cs` object and fill in the device's specifications.

### CoroutineService

This script provides an interface for starting, pausing, and stopping Unity coroutines. This can be useful if you need to manage coroutines from scripts that don't inherit from MonoBehaviour.

To use it, you would call the `CoroutineService.Instance.StartCoroutine(IEnumerator routine)` method with the routine you want to start.

---

## Contributions

Contributions to the Geeklab SDK are welcome! Please open a pull request with your changes or improvements. We value the input of our community and are always looking for ways to improve our tools.

## License

The Geeklab SDK is licensed under the MIT license. See the `LICENSE` file for more information.


## Utils

The `Utils` folder contains utility scripts that provide additional functionality or assist in the handling of certain tasks.

- `MetricToggle.cs` : This script allows you to toggle specific types of metrics on or off. It's especially useful when you want to limit the types of metrics being collected or processed.

## Future Work

We're constantly improving the Geeklab SDK and adding new features based on community feedback. If you have any suggestions for features or improvements, please feel free to submit an issue or a pull request.

## Support

For any questions or issues regarding the Geeklab SDK, please create an issue on our Github page. Alternatively, you can reach us through our [official website](http://www.geeklab.com) or [community forum](http://www.geeklab.com/forum).

## Code of Conduct

We believe in fostering an open and welcoming environment in the Geeklab community. Please see our [Code of Conduct](http://www.geeklab.com/code-of-conduct) for more details.

## Acknowledgments

We would like to thank all the contributors and users of the Geeklab SDK. Without your feedback and support, this project would not be possible.



[API1](https://geeklab.app/posts/api-v1/) //
[API2](https://help.geeklab.app/en/article/geeklab-api-documentation-gcxquc) -
[DOC](https://docs.google.com/document/d/1u2-QWlseR1GwLYRejNVogqzFgQqDeOs0CsaPhkYO06Q/edit#heading=h.3znysh7)