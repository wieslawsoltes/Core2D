﻿using System;
using System.Collections.Generic;
using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class ToolNone : ObservableObject, IEditorTool
    {
        private readonly IServiceProvider _serviceProvider;

        public string Title => "None";

        public ToolNone(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public void LeftDown(InputArgs args)
        {
        }

        public void LeftUp(InputArgs args)
        {
        }

        public void RightDown(InputArgs args)
        {
        }

        public void RightUp(InputArgs args)
        {
        }

        public void Move(InputArgs args)
        {
        }

        public void Move(BaseShape shape)
        {
        }

        public void Finalize(BaseShape shape)
        {
        }

        public void Reset()
        {
        }
    }
}
