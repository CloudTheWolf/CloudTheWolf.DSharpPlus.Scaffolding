#!/bin/sh
set -eu

service_name="cloudthewolf-dsharpplus-scaffolding"
service_user="cloudthewolf-bot"
service_group="cloudthewolf-bot"
config_dir="/etc/${service_name}"
state_dir="/var/lib/${service_name}"

if ! getent group "$service_group" >/dev/null 2>&1; then
    if command -v groupadd >/dev/null 2>&1; then
        groupadd --system "$service_group"
    else
        addgroup -S "$service_group"
    fi
fi

if ! id "$service_user" >/dev/null 2>&1; then
    if command -v useradd >/dev/null 2>&1; then
        useradd --system --gid "$service_group" --home-dir "$state_dir" \
            --shell /usr/sbin/nologin "$service_user"
    else
        adduser -S -D -H -h "$state_dir" -s /sbin/nologin -G "$service_group" "$service_user"
    fi
fi

mkdir -p "$config_dir" "$state_dir"
chown "root:$service_group" "$config_dir"
chown "$service_user:$service_group" "$state_dir"
chmod 0750 "$config_dir" "$state_dir"

if [ ! -f "$config_dir/appsettings.json" ]; then
    cp "$config_dir/appsettings.json.example" "$config_dir/appsettings.json"
    chown "root:$service_group" "$config_dir/appsettings.json"
    chmod 0640 "$config_dir/appsettings.json"
fi

if command -v systemctl >/dev/null 2>&1; then
    systemctl daemon-reload || true
    systemctl enable "$service_name.service" >/dev/null 2>&1 || true
elif command -v rc-update >/dev/null 2>&1; then
    rc-update add "$service_name" default >/dev/null 2>&1 || true
fi

echo "Configure $config_dir/appsettings.json before starting $service_name."
