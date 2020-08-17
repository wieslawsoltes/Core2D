#!/usr/bin/env bash

dotnet build -c Release -f netcoreapp3.1

zip -r Core2D.zip ./bin/Release/netcoreapp3.1/Core2D.dll ./bin/Release/netcoreapp3.1/Core2D.Modules.dll ./bin/Release/netcoreapp3.1/Core2D.UI.dll ./bin/Release/netcoreapp3.1/Core2D.Desktop.dll
