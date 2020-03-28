# DocFX

### Mono

https://www.mono-project.com/download/stable/#download-lin-ubuntu

```bash
sudo apt install gnupg ca-certificates
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
sudo apt update
```

```bash
sudo apt install mono-devel
```

### DocFX

https://github.com/dotnet/docfx

```bash
wget https://github.com/dotnet/docfx/releases/download/v2.51/docfx.zip
unzip -q docfx.zip -d docfx
rm docfx.zip
```

```PowerShell
wget https://github.com/dotnet/docfx/releases/download/v2.51/docfx.zip -OutFile docfx.zip
```

### Docs

```bash
mono ./docfx/docfx.exe metadata
mono ./docfx/docfx.exe build -o ../docs
```

```PowerShell
./docfx/docfx.exe metadata
./docfx/docfx.exe build -o ../docs
```

### Serve

```bash
mono ./docfx/docfx.exe serve _site
```

```PowerShell
./docfx/docfx.exe serve _site
```
