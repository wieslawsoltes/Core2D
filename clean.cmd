@echo off

rmdir /Q /S src\Dependencies\netDxf\netDxf\bin
rmdir /Q /S src\Dependencies\netDxf\netDxf\obj

rmdir /Q /S src\Dependencies\netDxf\TestDxfDocument\bin
rmdir /Q /S src\Dependencies\netDxf\TestDxfDocument\obj

rmdir /Q /S src\Dependencies\FileWriter.Dxf\bin
rmdir /Q /S src\Dependencies\FileWriter.Dxf\obj

rmdir /Q /S src\Dependencies\FileWriter.Emf\bin
rmdir /Q /S src\Dependencies\FileWriter.Emf\obj

rmdir /Q /S src\Dependencies\FileWriter.Pdf-core\bin
rmdir /Q /S src\Dependencies\FileWriter.Pdf-core\obj

rmdir /Q /S src\Dependencies\FileWriter.Pdf-wpf\bin
rmdir /Q /S src\Dependencies\FileWriter.Pdf-wpf\obj

rmdir /Q /S src\Dependencies\Log.Trace\bin
rmdir /Q /S src\Dependencies\Log.Trace\obj

rmdir /Q /S src\Dependencies\Renderer.Dxf\bin
rmdir /Q /S src\Dependencies\Renderer.Dxf\obj

rmdir /Q /S src\Dependencies\Renderer.PdfSharp-core\bin
rmdir /Q /S src\Dependencies\Renderer.PdfSharp-core\obj

rmdir /Q /S src\Dependencies\Renderer.PdfSharp-wpf\bin
rmdir /Q /S src\Dependencies\Renderer.PdfSharp-wpf\obj

rmdir /Q /S src\Dependencies\Renderer.Perspex\bin
rmdir /Q /S src\Dependencies\Renderer.Perspex\obj

rmdir /Q /S src\Dependencies\Renderer.WinForms\bin
rmdir /Q /S src\Dependencies\Renderer.WinForms\obj

rmdir /Q /S src\Dependencies\Renderer.Wpf\bin
rmdir /Q /S src\Dependencies\Renderer.Wpf\obj

rmdir /Q /S src\Dependencies\Serializer.Newtonsoft\bin
rmdir /Q /S src\Dependencies\Serializer.Newtonsoft\obj

rmdir /Q /S src\Dependencies\Serializer.ProtoBuf\bin
rmdir /Q /S src\Dependencies\Serializer.ProtoBuf\obj

rmdir /Q /S src\Dependencies\Serializer.ProtoBuf.Generate\bin
rmdir /Q /S src\Dependencies\Serializer.ProtoBuf.Generate\obj

rmdir /Q /S src\Dependencies\Serializer.Xaml\bin
rmdir /Q /S src\Dependencies\Serializer.Xaml\obj

rmdir /Q /S src\Dependencies\TextFieldReader.CsvHelper\bin
rmdir /Q /S src\Dependencies\TextFieldReader.CsvHelper\obj

rmdir /Q /S src\Dependencies\TextFieldWriter.CsvHelper\bin
rmdir /Q /S src\Dependencies\TextFieldWriter.CsvHelper\obj

rmdir /Q /S tests\Core2D.UnitTests\bin
rmdir /Q /S tests\Core2D.UnitTests\obj

rmdir /Q /S tests\Core2D.Perspex.UnitTests\bin
rmdir /Q /S tests\Core2D.Perspex.UnitTests\obj

rmdir /Q /S tests\Core2D.Wpf.UnitTests\bin
rmdir /Q /S tests\Core2D.Wpf.UnitTests\obj

rmdir /Q /S src\Core2D\bin
rmdir /Q /S src\Core2D\obj

rmdir /Q /S src\Core2D.Perspex\bin
rmdir /Q /S src\Core2D.Perspex\obj

rmdir /Q /S src\Core2D.Wpf\bin
rmdir /Q /S src\Core2D.Wpf\obj

rmdir /Q /S packages

del /Q src\Dependencies\Serializer.ProtoBuf.Generate\Serializer\*.dll
