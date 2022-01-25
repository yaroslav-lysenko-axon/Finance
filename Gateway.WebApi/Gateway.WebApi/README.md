## Implement API Gateways with Ocelot

1. [What is API gateway?](#apigateway)
2. [Implementing your API Gateways with Ocelot](#implementingwithocelot)
3. [How to up and run our project with gateway](#up&run)

# What is API gateway?<a name="apigateway"> </a>

An API gateway is a software pattern that sits in front of an application programming interface (API) or group of
microservices, to facilitate requests and delivery of data and services. Its primary role is to act as a single entry
point and standardized process for interactions between an organization's apps, data and services and internal and
external customers.

# Implementing your API Gateways with Ocelot<a name="implementingwithocelot"> </a>

Ocelot is basically a set of middleware that you can apply in a specific order. You install Ocelot and its dependencies
in your ASP.NET Core project with Ocelot's NuGet package.

Our ASP.NET Core Gateway.WebApi project is built with two simple files: Program.cs and Startup.cs. The Program.cs just
needs to create and configure the typical ASP.NET Core BuildWebHost.

The important point here for Ocelot is the configuration.json file that you must provide to the builder through the
AddJsonFile() method. That configuration.json is where you specify all the API Gateway ReRoutes, meaning the external
endpoints with specific ports and the correlated internal endpoints, usually using different ports.

```bash
{
    "ReRoutes": [],
    "GlobalConfiguration": {}
}
```

The main functionality of an Ocelot API Gateway is to take incoming HTTP requests and forward them on to a downstream
service, currently as another HTTP request. Ocelot's describes the routing of one request to another as a ReRoute.

So, enable Ocelot in Startup.cs

```bash
public static void ConfigureServices(IServiceCollection services)
{
    services.AddOcelot();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
  //..
  app.UseOcelot();
}
```

# How to up and run our project with gateway<a name="up&run"> </a>

In all of our microservices, we must specify a unique port in the url. And set this to appsettings.json.

```bash
{
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://localhost:XXXX"
      }
    }
  }
}
```

After that, create a composite configuration in Run / Debug Configuration where you will include all your microservices.
And run or debug your project thanks to this compound. 


