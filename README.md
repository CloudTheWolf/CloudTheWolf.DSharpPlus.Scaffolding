> [!WARNING]
> **Version 5 Introduces Breaking Changes:**
> 
> See https://github.com/CloudTheWolf/CloudTheWolf.DSharpPlus.Scaffolding/wiki/Migration-from-4.x-to-5.x

![Logo of CloudTheWolf.DSharpPlus.Scaffolding](https://github.com/CloudTheWolf/CloudTheWolf.DSharpPlus.Scaffolding/raw/main/banner.png)

# CloudTheWolf.DSharpPlus.Scaffolding
A Simple, Unofficial, Scaffolding for [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus) Bots!

This project was created as a base for getting started, and has no affiliation with DShapPlus.


This Repo includes an example Worker, capable of running as either an console application or servive on both Windows and Linux environments, and an example plugin

This uses the fillowing libraries:

* [CloudTheWolf.DSharpPlus.Scaffolding.Shared](https://github.com/CloudTheWolf/CloudTheWolf.DSharpPlus.Scaffolding.Shared) - This impliments Either the `IBot` / `IPlugin` or `IShardBot` / `IShardPlugin` that act as the bridge between your Application, DSharp+ and Bot Plugins

* [CloudTheWolf.DSharpPlus.Scaffolding.Logging](https://github.com/CloudTheWolf/CloudTheWolf.DSharpPlus.Scaffolding.Logging) - This acts as your basic logger and handles logging for both the main application and plugins

* [CloudTheWolf.DSharpPlus.Scaffolding.Data](https://github.com/CloudTheWolf/CloudTheWolf.DSharpPlus.Scaffolding.Data) - This is the main connection between you and your Database. Currently it only supports MySql and Microsoft SQL Server.

# Help, my plugins don't work in Version 4.0
First of all, as with any update to the Scaffolding, please make sure your plugins are using the correct version of [CloudTheWolf.DSharpPlus.Scaffolding.Shared](https://github.com/CloudTheWolf/CloudTheWolf.DSharpPlus.Scaffolding.Shared)

Next, you will now need to put each individual plugin in it's own directory within the pluging folder.

Eg for a Plugin called Example you would now put it in `/Plugins/Example` instead of just adding it to `/Plugins`

## Why the change?
This change is a bit of a big one, and is sort of a double edge sword. 

First of this means that if you have multiple plugins that share the same dependancies, putting them in seperate folders will basically double the space requirements. 
You are free to put all your plugins in a single "Master" directory, however this also means the benifits of this change are lost.

Now, what are these benifits of this new design?
First if all, you can now easilly add/remove plugins to help diagnose issue with your bot.
This also means you can fully remove a plugin and all of its dependancies without the risk of removing a shared depandancy that another plugin may use.
