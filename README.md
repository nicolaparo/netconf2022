# Frontend per servizi Backend con Blazor e .NET7
Netconf2022 1nn0va, 28 Jan 2023 - Pordenone (IT)

## Goal
The goal of this repository is to show the new features in Blazor implemented in .NET7.

This repository will focus on:
- **`<NavigationLock>`** component, to lock the navigation of the user. Prevents the user from accidentally leave our application with an external navigation or can prevent the user from navigating within our application in case of an internal navigation
- **`@bind:after`** attribute, to invoke a method after the parameter is bound.
- **Custom Elements** to integrate with Vue, React, Angular or plain HTML.
- **Blazor Empty Template** taken as a base for the App development
- **`<QuickGrid>`** component, to create a fully functioning table with pagination or virtualization support

## Scenario
Itemfactory S.R.L. is a fiction company specialized in the production of items.

Itemfactory developed a NotificationService to send notifications and infos to its customers, to inform them when their order is being produced or to share news.

The NotificationService is controllable only via REST or by querying the database manually. It is not a very user-friendly approach.

Sometimes the Helpdesk department needs to access the data of this service. It is required to quickly create a UI to allow the employees to interact with this software.

Blazor Server, in this scenario, can be an appropriate choice since we won't have that many users connected at the same time. Also, since we are already skilled with the development in .NET, we can create the UI using the language we used for the backend and we can share our models between Server and UI

## Contents of this repo
| | |
|-|-|
|NicolaParo.NetConf2022.NotificationSender.Api|MinimalApi project|
|NicolaParo.NetConf2022.NotificationSender.App|Full application. Includes the Api, the Swagger and the Blazor user interface|
|NicolaParo.NetConf2022.NotificationSender.Configuration|Models for loading the configuration|
|NicolaParo.NetConf2022.NotificationSender.DataInitializer|Utility application that fills the database with random data|
|NicolaParo.NetConf2022.NotificationSender.Models|Shared models|
|NicolaParo.NetConf2022.NotificationSender.Services|Data access objects and backend workers|

## Installation
This project requires a Telegram Bot in order to work properly.
(More info on how to create a bot here: https://core.telegram.org/bots).

After cloning the repository, create the `secrets.json` file as follows:
```json
{
    "telegramBotToken": "<your telegram token here>",
    "dataFilePath": "<where to store data>"
}
```
Modify the `Program.cs` to load the configurationModel from your secrets.json file:

```csharp
var configurationModel = await ConfigurationModel.LoadFromFileAsync(@"path/to/your/secrets.json");
```
