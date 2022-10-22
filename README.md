![Logo of CloudTheWolf.DSharpPlus.Scaffolding](https://github.com/CloudTheWolf/CloudTheWolf.DSharpPlus.Scaffolding/raw/main/banner.png)

# CloudTheWolf.DSharpPlus.Scaffolding
A Simple, Unofficial, Scaffolding for [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus) Bots!

This project was created as a base for getting started, and has no affiliation with DShapPlus.


This Repo includes an example Worker, capable of running as either an console application or servive on both Windows and Linux environments, and an example plugin

This uses the fillowing libraries:

* [CloudTheWolf.DSharpPlus.Scaffolding.Shared](https://github.com/CloudTheWolf/CloudTheWolf.DSharpPlus.Scaffolding.Shared) - This impliments Either the `IBot` / `IPlugin` or `IShardBot` / `IShardPlugin` that act as the bridge between your Application, DSharp+ and Bot Plugins

* [CloudTheWolf.DSharpPlus.Scaffolding.Logging](https://github.com/CloudTheWolf/CloudTheWolf.DSharpPlus.Scaffolding.Logging) - This acts as your basic logger and handles logging for both the main application and plugins

* [CloudTheWolf.DSharpPlus.Scaffolding.Data](https://github.com/CloudTheWolf/CloudTheWolf.DSharpPlus.Scaffolding.Data) - This is the main connection between you and your Database. Currently it only supports MySql and Microsoft SQL Server.
