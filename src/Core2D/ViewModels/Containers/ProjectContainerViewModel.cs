#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Core2D.Model;
using Core2D.Model.History;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Containers
{
    public partial class ProjectContainerViewModel : BaseContainerViewModel, ISelection
    {
        [AutoNotify] private OptionsViewModel _options;
        [AutoNotify] private IHistory _history;
        [AutoNotify] private ImmutableArray<LibraryViewModel<ShapeStyleViewModel>> _styleLibraries;
        [AutoNotify] private ImmutableArray<LibraryViewModel<GroupShapeViewModel>> _groupLibraries;
        [AutoNotify] private ImmutableArray<DatabaseViewModel> _databases;
        [AutoNotify] private ImmutableArray<TemplateContainerViewModel> _templates;
        [AutoNotify] private ImmutableArray<ScriptViewModel> _scripts;
        [AutoNotify] private ImmutableArray<DocumentContainerViewModel> _documents;
        [AutoNotify] private LibraryViewModel<ShapeStyleViewModel> _currentStyleLibrary;
        [AutoNotify] private LibraryViewModel<GroupShapeViewModel> _currentGroupLibrary;
        [AutoNotify] private DatabaseViewModel _currentDatabase;
        [AutoNotify] private TemplateContainerViewModel _currentTemplate;
        [AutoNotify] private ScriptViewModel _currentScript;
        [AutoNotify] private DocumentContainerViewModel _currentDocument;
        [AutoNotify] private FrameContainerViewModel _currentContainer;
        [AutoNotify] private ViewModelBase _selected;
        [AutoNotify] private ISet<BaseShapeViewModel> _selectedShapes;

        public ProjectContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Selected))
                {
                    SetSelected(Selected);
                }
            };
        }

        public void SetSelected(ViewModelBase value)
        {
            Debug.WriteLine($"[SetSelected] {value?.Name} ({value?.GetType()})");
            if (value is BaseShapeViewModel shape)
            {
                var layer = _documents
                    .SelectMany(x => x.Pages)
                    .SelectMany(x => x.Layers)
                    .FirstOrDefault(l => l.Shapes.Contains(shape));

                var container = _documents
                    .SelectMany(x => x.Pages)
                    .FirstOrDefault(c => c.Layers.Contains(layer));

                if (container is { } && layer is { })
                {
                    if (container.CurrentLayer != layer)
                    {
                        Debug.WriteLine($"  [CurrentLayer] {layer.Name}");
                        container.CurrentLayer = layer;
                    }

                    if (CurrentContainer != container)
                    {
                        Debug.WriteLine($"  [CurrentContainer] {container.Name}");
                        CurrentContainer = container;
                        CurrentContainer.InvalidateLayer();
                    }
                }
            }
            else if (value is LayerContainerViewModel layer && _documents is { })
            {
                var container = _documents
                    .SelectMany(x => x.Pages)
                    .FirstOrDefault(c => c.Layers.Contains(layer));

                if (container is { })
                {
                    if (container.CurrentLayer != layer)
                    {
                        Debug.WriteLine($"  [CurrentLayer] {layer.Name}");
                        container.CurrentLayer = layer;
                    }

                    if (CurrentContainer != container)
                    {
                        Debug.WriteLine($"  [CurrentContainer] {container.Name}");
                        CurrentContainer = container;
                        CurrentContainer.InvalidateLayer();
                    }
                }
            }
            else if (value is FrameContainerViewModel container && _documents is { })
            {
                var document = _documents.FirstOrDefault(d => d.Pages.Contains(container));
                if (document is { })
                {
                    if (CurrentDocument != document)
                    {
                        Debug.WriteLine($"  [CurrentDocument] {document.Name}");
                        CurrentDocument = document;
                    }

                    if (CurrentContainer != container)
                    {
                        Debug.WriteLine($"  [CurrentContainer] {container.Name}");
                        CurrentContainer = container;
                        CurrentContainer.InvalidateLayer();
                    }
                }
            }
            else if (value is DocumentContainerViewModel document)
            {
                if (CurrentDocument != document)
                {
                    Debug.WriteLine($"  [CurrentDocument] {document.Name}");
                    CurrentDocument = document;
                    if (!CurrentDocument?.Pages.Contains(CurrentContainer) ?? false)
                    {
                        var current = CurrentDocument.Pages.FirstOrDefault();
                        if (CurrentContainer != current)
                        {
                            Debug.WriteLine($"  [CurrentContainer] {current?.Name}");
                            CurrentContainer = current;
                            CurrentContainer.InvalidateLayer();
                        }
                    }
                }
            }
        }

        public void SetCurrentDocument(DocumentContainerViewModel document)
        {
            CurrentDocument = document;
            Selected = document;
        }

        public void SetCurrentContainer(FrameContainerViewModel container)
        {
            CurrentContainer = container;
            Selected = container;
        }

        public void SetCurrentTemplate(TemplateContainerViewModel template) => CurrentTemplate = template;

        public void SetCurrentScript(ScriptViewModel script) => CurrentScript = script;

        public void SetCurrentDatabase(DatabaseViewModel db) => CurrentDatabase = db;

        public void SetCurrentGroupLibrary(LibraryViewModel<GroupShapeViewModel> libraryViewModel) => CurrentGroupLibrary = libraryViewModel;

        public void SetCurrentStyleLibrary(LibraryViewModel<ShapeStyleViewModel> libraryViewModel) => CurrentStyleLibrary = libraryViewModel;

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Options.IsDirty();

            foreach (var styleLibrary in StyleLibraries)
            {
                isDirty |= styleLibrary.IsDirty();
            }

            foreach (var groupLibrary in GroupLibraries)
            {
                isDirty |= groupLibrary.IsDirty();
            }

            foreach (var database in Databases)
            {
                isDirty |= database.IsDirty();
            }

            foreach (var template in Templates)
            {
                isDirty |= template.IsDirty();
            }

            foreach (var script in Scripts)
            {
                isDirty |= script.IsDirty();
            }

            foreach (var document in Documents)
            {
                isDirty |= document.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            Options.Invalidate();

            foreach (var styleLibrary in StyleLibraries)
            {
                styleLibrary.Invalidate();
            }

            foreach (var groupLibrary in GroupLibraries)
            {
                groupLibrary.Invalidate();
            }

            foreach (var database in Databases)
            {
                database.Invalidate();
            }

            foreach (var template in Templates)
            {
                template.Invalidate();
            }

            foreach (var script in Scripts)
            {
                script.Invalidate();
            }

            foreach (var document in Documents)
            {
                document.Invalidate();
            }
        }
    }
}
