#nullable disable
using System;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor.Tools
{
    public partial class NoneToolViewModel : ViewModelBase, IEditorTool
    {        public string Title => "None";

        public NoneToolViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void BeginDown(InputArgs args)
        {
        }

        public void BeginUp(InputArgs args)
        {
        }

        public void EndDown(InputArgs args)
        {
        }

        public void EndUp(InputArgs args)
        {
        }

        public void Move(InputArgs args)
        {
        }

        public void Move(BaseShapeViewModel shape)
        {
        }

        public void Finalize(BaseShapeViewModel shape)
        {
        }

        public void Reset()
        {
        }
    }
}
