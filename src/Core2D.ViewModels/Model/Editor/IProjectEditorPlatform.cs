// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable

namespace Core2D.Model.Editor;

public interface IProjectEditorPlatform
{
    void OnOpen();

    void OnSave();

    void OnSaveAs();

    void OnClose();
    
    void OnImportJson(object? param);

    void OnImportSvg(object? param);

    void OnImportDwg(object? param);

    void OnImportPdf(object? param);
    void OnImportExcel(object? param);
    void OnImportWord(object? param);
    void OnImportPowerPoint(object? param);

    void OnExportJson(object? param);

    void OnExport(object? param);
    void OnOpenExportWizard(object? param);

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
