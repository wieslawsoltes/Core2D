﻿#nullable enable

namespace Core2D.Model.Editor;

public interface IProjectEditorPlatform
{
    void OnOpen();

    void OnSave();

    void OnSaveAs();

    void OnClose();
    
    void OnImportJson(object? param);

    void OnImportSvg(object? param);

    void OnExportJson(object? param);

    void OnExport(object? param);

    void OnExecuteScriptFile(object? param);

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
