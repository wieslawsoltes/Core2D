#nullable enable
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;

namespace Core2D.Model.Editor;

public interface IProjectEditorPlatform
{
    void OnOpen();

    void OnSave();

    void OnSaveAs();

    void OnClose();
    
    void OnImportJson(object? param);

    void OnImportSvg(object? param);

    void OnImportObject(object? param);

    void OnExportJson(object? param);

    void OnExportObject(object? param);

    void OnExport(object? param);

    void OnExecuteScriptFile(object? param);

    void OnExecuteScriptFile();

    void OnExit();

    void OnCopyAsXaml(object? param);

    void OnCopyAsSvg(object? param);

    void OnPasteSvg();

    void OnCopyAsEmf(object? param);

    void OnCopyAsPathData(object? param);

    void OnPastePathDataStroked();

    void OnPastePathDataFilled();

    void OnImportData(object? param);

    void OnExportData(object? param);

    void OnUpdateData(object? param);

    void OnAboutDialog();

    void OnZoomReset();

    void OnZoomFill();

    void OnZoomUniform();

    void OnZoomUniformToFill();

    void OnZoomAutoFit();

    void OnZoomIn();

    void OnZoomOut();
}
