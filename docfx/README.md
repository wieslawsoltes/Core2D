# DocFX

### Mono

https://www.mono-project.com/download/stable/#download-lin-ubuntu

```
sudo apt install gnupg ca-certificates
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
sudo apt update
```

```
sudo apt install mono-devel
```

### DocFX

https://github.com/dotnet/docfx

```
wget https://github.com/dotnet/docfx/releases/download/v2.51/docfx.zip
unzip -q docfx.zip -d docfx
rm docfx.zip
```

### Docs

```
mono ./docfx/docfx.exe metadata
mono ./docfx/docfx.exe build -o ../docs
```

### Serve

```
mono ./docfx/docfx.exe serve _site
```
