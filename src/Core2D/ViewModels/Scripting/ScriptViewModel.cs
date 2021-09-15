#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Input;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels.Scripting
{
    public partial class ScriptViewModel : ViewModelBase
    {
        [AutoNotify] private string? _code;

        public ScriptViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
        {
            ResetRepl = new Command(() => GetProject()?.OnResetRepl());

            ExecuteRepl = new Command<string?>(x => GetProject()?.OnExecuteRepl(x));

            ExecuteCode = new Command<string?>(x => GetProject()?.OnExecuteCode(x));

            AddScript = new Command(() => GetProject()?.OnAddScript());

            RemoveScript = new Command<ScriptViewModel?>(x => GetProject()?.OnRemoveScript(x));

            ExportScript = new Command<ScriptViewModel?>(x => GetProject()?.OnExportScript(x));

            ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
        }

        [IgnoreDataMember]
        public ICommand ResetRepl { get; }

        [IgnoreDataMember]
        public ICommand ExecuteRepl { get; }

        [IgnoreDataMember]
        public ICommand ExecuteCode { get; }

        [IgnoreDataMember]
        public ICommand AddScript { get; }

        [IgnoreDataMember]
        public ICommand RemoveScript { get; }

        [IgnoreDataMember]
        public ICommand ExportScript { get; }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var copy = new ScriptViewModel(ServiceProvider)
            {
                Code = _code
            };

            return copy;
        }
    }
}
