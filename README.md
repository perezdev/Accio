# Accio

http://accio.cards

![](https://i.imgur.com/djcsDeR.png)

# Overview

Accio is an opensource card browsing website built on ASP.NET Core.

# Contributors

[perezdev](https://github.com/perezdev), [Tressley](https://github.com/Tressley), [nicolaspfernandes](https://github.com/nicolaspfernandes)

# Technology Overview

ASP.NET Core/Web API, SQL Server, HTML/JS/AJAX, Entity Framework

# Requirements

DotNet Core 3.1, SQL Server

# Project Overview

The Accio **solution** consists of 4 projects:

* Business - Contains all of the "business logic" for the app and API.
* Data - The database layer, strictly a container for EF classes.
* SetUpload - Not to be used on it's own. It's a console app that is used to periodically upload data from https://github.com/Tressley/hpjson.
* **Web** - Main app.
* Web.API - API

# Setup

Once you have the code downloaded and the database server running, you need to make the following changes to run the website:

Add two application JSON settings files to the root of **Accio.Web**:

appsettings.json:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "AccioConnection": "Server=<SERVER_NAME>;Initial Catalog=Accio;Persist Security Info=False;User ID=<SQL_USER_NAME>;Password=<SQL_USER_PASSWORD>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

appsettings.Development.json:

````json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "AccioConnection": "Server=<SERVER_NAME>;Initial Catalog=Accio;Persist Security Info=False;User ID=<SQL_USER_NAME>;Password=<SQL_USER_PASSWORD>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
````

You will need to replace **<SERVER_NAME>**, **<SQL_USER_NAME>**, and **<SQL_USER_PASSWORD>** in each file.

After the config changes are in place, you can run the data scripts to create the database, create the schema, and upload the data. The scripts are located in the Scripts folder under the Accio.Data project:

* Accio_CreateDatabase.sql
* Accio_CreateSchema.sql
* Accio_ImportData.sql
