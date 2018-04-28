# dotnet-epo-ops-client

dotnet-epo-ops-client is an [Apache2 Licensed][Apache license] client library for accessing the [European Patent Office][EPO]'s ("EPO") [Open Patent Services][OPS] ("OPS") v.3.2 (based on [v 1.3.4 of the reference guide][OPS guide]).

This library contains logic that I've used for several years to query and synchronize patent data. Only recently did I think to post it to my GitHub account. To my surprise, there already exists a similar and well-designed solution on GitHub written in Python. Please check out [55minutes python-epo-ops-client][python client] if dotnet isn't your preferred framework.

```csharp
using EpoOpsClient;

var srv = new Service(
	LogManager.GetLogger<Service>(), 
	Configuration.GetNamedSection<Service>(), 
	() => { return srv.CreateAccessToken().Value; });
var output = srv.GetPatent("EP1883031").Result;
```

---

`dotnet-epo-ops-client` is a simple HTTP client wrapper for accessing the EPO OPS service. I've tried to minimize the extension points and just focus on getting the job done. The library does make a few assumptions:

* Assumes that you're using constructor injection
* Assumes that you're keeping track of quota details
* Assumes that you're keeping track of throttling details
* Assumes that you might have custom logic for generating and caching tokens
* Assumes that you might have custom logic for deserializing response streams

The library essentially only contains one class, Service, which is a simple facade for the 'register' endpoint. I've also included the `EpoOpsClient.Serialization` code only as an example of what could be done to handle the data that is returned from EPO OPS. If XSLT isn't your preferred tool for parsing and deserializing data, please feel free to disregard this additional library.

### Service

The `Service` class contains all the methods necessary to generate tokens, call EPO OPS 'register' endpoint, and return results.

Constructor injection is a common pattern for most of my applications so the `Service` class is no different. I prefer to pass loggers and configuration settings rather than instantiate these objects within the class constructor. I'm using [Common.Logging .NET][common-logging] so the `Service` class expects an `ILog` implementation. Additionally, I prefer key-value configuration settings so the `Service` expects a list of settings with keys that match the list of constants within the class. An example of the settings can be found in the `EpoOpsClient.Testing.App.config` file. 

I've also included a simple configuration file mapper in the `Configuration` class. This does nothing but provide a mechanism to map classes to named sections in a traditional .NET XML configuration file.

For example, the configuration file can declare an `EpoOpsClient` section and include several settings:

```xml
<EpoOpsClient>
	<EpoOpsClient.Service>
		<add key="authEndPointAddress" value="https://ops.epo.org/3.2/auth/accesstoken" />
		<add key="applicationServiceEndPointAddress" value="https://ops.epo.org/3.2/rest-services/register/application/epodoc/{0}/biblio,procedural-steps" />
		<add key="patentServiceEndPointAddress" value="https://ops.epo.org/3.2/rest-services/register/publication/epodoc/{0}/biblio,procedural-steps" />
		<add key="serviceConsumerKey" value="aaabbbccc" />
		<add key="serviceConsumerSecret" value="aaabbbccc" />
		<add key="userAgentName" value="EpoOpsClient-Service/0.1" />
	</EpoOpsClient.Service>
</EpoOpsClient>
```

The `Configuration` class simply maps a class to the named section in the configuration file:

```csharp
public class Configuration
{
	public const string DefaultGroupSectionName = "EpoOpsClient";

	public static NameValueCollection GetNamedSection<T>()
	{
		var type = typeof(T);
		var nm = type.FullName;
		var settings = ConfigurationManager.GetSection(string.Concat(DefaultGroupSectionName, "/", nm)) as NameValueCollection;
		return settings;
	}
}
```

Of course, you can also just dynamically declare these settings at runtime or generate them through some other means (e.g. ZooKeeper):

```csharp
var config = new System.Collections.Specialized.NameValueCollection
{
	{ Service.ServiceConsumerKeyKey, "bbbcccddd" },
	{ Service.ServiceConsumerSecretKey, "bbbcccddd" }
};
sut = new Service(
	LogManager.GetLogger<Service>(), 
	config, 
	() => { return sut.CreateAccessToken().Value; });
```

## ResponseStream

While the `Service` class handles the logic to prepare the GET request, the `ResponseStream` class handles the logic to interrogate the response from EPO OPS. The `ResponseStream` constructor accepts a `HttpWebResponse` object and it will transform the EPO OPS headers for quota and throttling into simple data structures. The `ResponseStream` class will not handle the content so I have included the `EpoOpsClient.Serialization.XmlSerialization` class as an example of what can be done to read, parse, and transform the raw response stream into a simple data structure.

See the [OPS guide][] or use the [Developer's Area][] for more information on how to use each service.

## Getting Started

Check out the `EpoOpsClient.Testing` project for code examples. The dotnet-epo-ops-client library is little more than a shim so there isn't much to dig through. The descriptions below might help to clarify if you have doubts.

The `Service` class expects a small number of configuration settings will be provided to the constructor. These include:

* `authEndPointAddress` - EPO OPS endpoint for creating an auth token
* `applicationServiceEndPointAddress` - EPO OPS endpoint for querying application-related data
* `patentServiceEndPointAddress` - EPO OPS endpoint for querying patent-related data
* `serviceConsumerKey` - Your EPO OPS consumer key
* `serviceConsumerSecret` - Your EPO OPS consumer secret
* `userAgentName` - Agent name attached to the HTTP requests

With these settings, the `Service` is ready to handle calls to either the `GetApplication()` method or the `GetPatent()` methods. Both methods behave the same way and will return a `Task<ResponseStream>` instance, as is typical when working with HTTP services. Both methods will also capture any exceptions (i.e. `WebException`) that might occur, depending on the response. Since the dotnet-epo-ops-client library is using the (somewhat outdated) `System.Net.WebRequest` class to handle request and response activities, it is possible that some normal operations will actually throw an exception. For example, a 404 Not Found response will trigger an exception so the `Service` class catches these exceptions and "normalizes" the response.

The `System.Net.HttpWebResponse` objects that are received from the HTTP service will be wrapped by the `ResponseStream` class. This class does very little besides capturing a few EPO OPS-specific header values from the response and providing access to the bare response `System.IO.Stream` object.

With the `Stream` object from the HTTP response, it is possible to follow your common patterns for parsing and transforming result data. For example, it is possible to convert the data into structured objects or use regular expressions to extract specific bits of text that are important. The `EpoOpsClient.Serialization` library has an example that transforms the EPO OPS XML response data into a POCO, `Patent`, which has a handful of meaningful properties. Not all off the `Patent` properties are populated.

## TODO Items
- [] Swap out the WebRequest logic for HttpClient
- [] Create method for EPO OPS 'family' endpoint
- [] Create method for EPO OPS 'published-data/images' endpoint
- [] Create method for EPO OPS 'number-service' endpoint
- [] Create method for EPO OPS 'published-data' endpoint
- [] Create method for EPO OPS 'published-data/search' endpoint
- [] Create method for EPO OPS 'register/search' endpoint

[common-logging]: https://github.com/net-commons/common-logging
[python client]: https://github.com/55minutes/python-epo-ops-client
[Apache license]: http://www.apache.org/licenses/LICENSE-2.0
[Developer's Area]: https://developers.epo.org/ops-v3-2/apis
[EPO]: http://epo.org
[OPS guide]: http://documents.epo.org/projects/babylon/eponet.nsf/0/F3ECDCC915C9BCD8C1258060003AA712/$FILE/ops_v3.2_documentation%20_version_1.3.4_en.pdf
[OPS registration]: https://developers.epo.org/user/register
[OPS]: http://www.epo.org/searching/free/ops.html
