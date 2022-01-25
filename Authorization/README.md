## Table of Contents 
1. [Reference Authorization Microservice](#refauthmicroservice)
2. [Generate OpenSSL RSA Key Pair from the Command Line](#generateprivate&publickeys)

# Reference Authorization Microservice <a name="refauthmicroservice"> </a>
[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com) [![forthebadge](https://forthebadge.com/images/badges/built-with-love.svg)](https://forthebadge.com)  
This microservice is an example of **OAuth2 authorizatoin** microservice.

## Prerequisites
For a **"docker-way"**, you need only [Docker](https://www.docker.com/).

For a **"non-docker-way"**, you should have [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) installed

## How to run it?
#### Docker-way:
1. Setup a containerized *PostgreSQL*, following instructions in [db provisioning](provisioning/postgres/README.md)
3. Setup an *application*, following instructions in [app provisioning](provisioning/app/README.md)

#### Non-docker-way:
1. Setup a [PostgreSQL](https://lmgtfy.com/?q=how+to+setup+postgresql+on+linux)
3. Launch *migrations* by running this command:
    ```
    dotnet run --project Authorization.Migrations
    ```
4. Launch an *application* by running this command:
    ```
    dotnet run --project Authorization.Host
    ```

## Supported OAuth2 flows
As for now it supports only one flow - *Resource Owner Password Credential Grant Type Flow*.

![image](https://docs.oracle.com/cd/E39820_01/doc.11121/gateway_docs/content/images/oauth/oauth_username_password_flow.png)

# Generate OpenSSL RSA Key Pair from the Command Line <a name="generateprivate&publickeys"> </a>
1) Generate a 2048 bit RSA Key
You can generate a public and private RSA key pair like this:
```bash
openssl genrsa -out private-key.pem 2048
```
That generates a 2048-bit RSA key pair, encrypts them with a password you provide and writes them to a file. You need to next extract the public key file. You will use this, for instance, on your web server to encrypt content so that it can only be read with the private key.
2) Export the RSA Public Key to a File
This is a command that is:
```bash
openssl rsa -in private-key.pem -outform PEM -pubout -out public-key.pem
```
The -pubout flag is really important. Be sure to include it.

Next open the public-key.pem and ensure that it starts with -----BEGIN PUBLIC KEY-----. This is how you know that this file is the public key of the pair and not a private key.
To check the file from the command line you can use the less command, like this:
```bash
less public-key.pem
```
3) Visually Inspect Your Key Files
It is important to visually inspect you private and public key files to make sure that they are what you expect. OpenSSL will clearly explain the nature of the key block with a -----BEGIN RSA PRIVATE KEY----- or -----BEGIN PUBLIC KEY-----.

You can use less to inspect each of your two files in turn:

    less private-key.pem to verify that it starts with a -----BEGIN RSA PRIVATE KEY-----
    less public-key.pem to verify that it starts with a -----BEGIN PUBLIC KEY-----


