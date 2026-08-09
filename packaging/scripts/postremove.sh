#!/bin/sh
set -eu

service_name="cloudthewolf-dsharpplus-scaffolding"
executable="/opt/${service_name}/CloudTheWolf.DSharpPlus.Scaffolding.Worker"

# During an upgrade the replacement executable is already present. Only disable
# the service when the package was actually removed.
if [ ! -e "$executable" ]; then
    if command -v systemctl >/dev/null 2>&1; then
        systemctl disable --now "$service_name.service" >/dev/null 2>&1 || true
        systemctl daemon-reload || true
    elif command -v rc-update >/dev/null 2>&1; then
        rc-service "$service_name" stop >/dev/null 2>&1 || true
        rc-update del "$service_name" default >/dev/null 2>&1 || true
    fi
fi
