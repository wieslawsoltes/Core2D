using Core2D.Containers;
using Core2D.Data;

namespace Core2D.Editor
{
    public interface IProjectEditorPlatform
    {
        void OnOpen(string path);

        void OnSave();

        void OnSaveAs();

        void OnImportJson(string path);

        void OnImportSvg(string path);

        void OnImportObject(string path);

        void OnExportJson(object item);

        void OnExportObject(object item);

        void OnExport(object item);

        void OnExecuteScriptFile(string path);

        void OnExit();

        void OnCopyAsXaml(object item);

        void OnCopyAsSvg(object item);

        void OnPasteSvg();

        void OnCopyAsEmf(object item);

        void OnCopyAsPathData(object item);

        void OnPastePathDataStroked();

        void OnPastePathDataFilled();

        void OnImportData(ProjectContainer project);

        void OnExportData(Database db);

        void OnUpdateData(Database db);

        void OnAboutDialog();

        void OnZoomReset();

        void OnZoomFill();

        void OnZoomUniform();

        void OnZoomUniformToFill();

        void OnZoomAutoFit();

        void OnZoomIn();

        void OnZoomOut();
    }
}
