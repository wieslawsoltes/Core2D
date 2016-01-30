@echo off

rmdir /Q /S Dependencies\netDxf\netDxf\bin
rmdir /Q /S Dependencies\netDxf\netDxf\obj

rmdir /Q /S Dependencies\netDxf\TestDxfDocument\bin
rmdir /Q /S Dependencies\netDxf\TestDxfDocument\obj

rmdir /Q /S Dependencies\FileWriter.Dxf\bin
rmdir /Q /S Dependencies\FileWriter.Dxf\obj

rmdir /Q /S Dependencies\FileWriter.Emf\bin
rmdir /Q /S Dependencies\FileWriter.Emf\obj

rmdir /Q /S Dependencies\FileWriter.Pdf-core\bin
rmdir /Q /S Dependencies\FileWriter.Pdf-core\obj

rmdir /Q /S Dependencies\FileWriter.Pdf-wpf\bin
rmdir /Q /S Dependencies\FileWriter.Pdf-wpf\obj

rmdir /Q /S Dependencies\Log.Trace\bin
rmdir /Q /S Dependencies\Log.Trace\obj

rmdir /Q /S Dependencies\Renderer.Dxf\bin
rmdir /Q /S Dependencies\Renderer.Dxf\obj

rmdir /Q /S Dependencies\Renderer.PdfSharp-core\bin
rmdir /Q /S Dependencies\Renderer.PdfSharp-core\obj

rmdir /Q /S Dependencies\Renderer.PdfSharp-wpf\bin
rmdir /Q /S Dependencies\Renderer.PdfSharp-wpf\obj

rmdir /Q /S Dependencies\Renderer.Perspex\bin
rmdir /Q /S Dependencies\Renderer.Perspex\obj

rmdir /Q /S Dependencies\Renderer.WinForms\bin
rmdir /Q /S Dependencies\Renderer.WinForms\obj

rmdir /Q /S Dependencies\Renderer.Wpf\bin
rmdir /Q /S Dependencies\Renderer.Wpf\obj

rmdir /Q /S Dependencies\Serializer.Newtonsoft\bin
rmdir /Q /S Dependencies\Serializer.Newtonsoft\obj

rmdir /Q /S Dependencies\Serializer.ProtoBuf\bin
rmdir /Q /S Dependencies\Serializer.ProtoBuf\obj

rmdir /Q /S Dependencies\Serializer.ProtoBuf.Generate\bin
rmdir /Q /S Dependencies\Serializer.ProtoBuf.Generate\obj

rmdir /Q /S Dependencies\Serializer.Xaml\bin
rmdir /Q /S Dependencies\Serializer.Xaml\obj

rmdir /Q /S Dependencies\TextFieldReader.CsvHelper\bin
rmdir /Q /S Dependencies\TextFieldReader.CsvHelper\obj

rmdir /Q /S Dependencies\TextFieldWriter.CsvHelper\bin
rmdir /Q /S Dependencies\TextFieldWriter.CsvHelper\obj

rmdir /Q /S Core2D\bin
rmdir /Q /S Core2D\obj

rmdir /Q /S Core2D.Perspex\bin
rmdir /Q /S Core2D.Perspex\obj

rmdir /Q /S Core2D.Wpf\bin
rmdir /Q /S Core2D.Wpf\obj

rmdir /Q /S packages

del /Q Dependencies/Serializer.ProtoBuf.Generate/Serializer/*.dll

pause
