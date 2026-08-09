Plugin deployment
=================

Use one directory per plugin. The directory and entry DLL must have the same
name, for example:

Plugins/ExamplePlugin/ExamplePlugin.dll

Hot loading and unloading
=========================

Enable the watcher in appsettings.json:

"Plugins": {
  "hotReload": true,
  "reloadDelayMilliseconds": 1500
}

- Load or update a plugin by copying its complete output into its directory.
  Copy dependencies first and the entry DLL last.
- Unload a plugin by moving or deleting its directory. Renaming the directory
  to end in ".disabled" also prevents its entry DLL from being discovered.
- File changes are debounced so one deployment produces one reload.

DSharpPlus finalizes commands and event handlers when a client is built. A hot
reload therefore performs a short Discord gateway disconnect/reconnect while
the Worker process and service remain running. Plugins receive ShutdownAsync
before their collectible assembly context is released.
