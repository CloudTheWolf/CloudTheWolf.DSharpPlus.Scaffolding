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

Additionally, the folder name much match the plugin DLL name. Eg if you plugin as `MyProject.Games.dll` then you much put the plugin in `/Plugins/MyProject.Games` 

## Why the change?
This change is a bit of a big one, and is sort of a double edge sword. 

First of this means that if you have multiple plugins that share the same dependancies, putting them in seperate folders will basically double the space requirements. 

Now, what are these benifits of this new design?
First if all, you can now easilly add/remove plugins to help diagnose issue with your bot.
This also means you can fully remove a plugin and all of its dependancies without the risk of removing a shared depandancy that another plugin may use.

## Hot loading and unloading plugins

Worker 5.2 can monitor the `Plugins` directory and load, replace, or unload
plugins without restarting the Worker process or service:

```json
{
  "Plugins": {
    "hotReload": true,
    "reloadDelayMilliseconds": 1500
  }
}
```

Deploy plugins to `Plugins/<PluginName>/<PluginName>.dll`. Copy dependencies
first and the entry DLL last. Move or delete the plugin directory to unload it;
renaming it to `<PluginName>.disabled` also disables discovery.

DSharpPlus finalizes commands and event handlers when its client is built, so a
hot reload includes a short Discord gateway disconnect and reconnect. The Worker
process remains alive, each plugin receives `ShutdownAsync`, and its collectible
assembly context is released before the replacement is loaded. Changes are
debounced, and a failed replacement reconnects the core bot without plugins so a
subsequent corrected deployment can recover automatically.

See the [ExamplePlugin migration guide](ExamplePlugin/README.md) for a complete
`InitPlugin` to `PluginContext` conversion, shutdown guidance, and deployment
layout.

## Linux packages and releases

The repository includes two GitHub Actions workflows:

- **Build Linux packages** can be run manually for a branch, tag, or commit. It
  builds self-contained `deb`, `rpm`, and `apk` packages for AMD64 and ARM64.
- **Release Linux packages** runs for version tags such as `v5.3.0`. It builds
  all six packages, creates a `SHA256SUMS` file, and creates or updates the
  matching GitHub Release. Tags containing a hyphen, such as `v5.3.0-beta.1`,
  are published as prereleases.

Release tags must contain a numeric package version with two to four numeric
parts and may have a prerelease suffix. You can also run the release workflow
manually against an existing tag.

Install a downloaded package with the package manager for your distribution:

```bash
# Debian or Ubuntu
sudo apt install ./cloudthewolf-dsharpplus-scaffolding_5.3.0_amd64.deb

# Fedora, RHEL, or a compatible distribution
sudo dnf install ./cloudthewolf-dsharpplus-scaffolding_5.3.0_x86_64.rpm

# Alpine Linux
sudo apk add --allow-untrusted ./cloudthewolf-dsharpplus-scaffolding_5.3.0_x86_64.apk
```

The package installs the Worker under
`/opt/cloudthewolf-dsharpplus-scaffolding`, creates the unprivileged
`cloudthewolf-bot` service account, and enables a systemd or OpenRC service. It
does not start the service automatically. Edit
`/etc/cloudthewolf-dsharpplus-scaffolding/appsettings.json`, then start it with
`sudo systemctl start cloudthewolf-dsharpplus-scaffolding` or
`sudo rc-service cloudthewolf-dsharpplus-scaffolding start`.
