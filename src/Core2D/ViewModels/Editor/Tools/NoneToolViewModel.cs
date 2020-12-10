using System;
using System.Collections.Generic;
using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class NoneToolViewModel : ViewModelBase, IEditorTool
    {
        private readonly IServiceProvider _serviceProvider;

        public string Title => "None";

        public NoneToolViewModel(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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
