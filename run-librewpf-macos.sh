#!/usr/bin/env bash
set -euo pipefail

script_directory="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
librewpf_feed="$script_directory/../wpf/artifacts/packages/Release/NonShipping"
librewpf_sdk="$librewpf_feed/LibreWPF.Sdk.0.1.0-preview.45.nupkg"
project="$script_directory/src/Core2D.UI.Wpf/Core2D.UI.Wpf.csproj"
application="$script_directory/src/Core2D.UI.Wpf/bin/AnyCPU/Debug/net10.0-windows/Core2D.UI.Wpf"

if [[ ! -f "$librewpf_sdk" ]]; then
    echo "LibreWPF 0.1.0-preview.45 was not found at: $librewpf_sdk" >&2
    echo "Build or restore the LibreWPF package bundle in the sibling wpf repository first." >&2
    exit 1
fi

dotnet build "$project" -c Debug -v:minimal
exec "$application" "$@"
