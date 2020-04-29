using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Core2D.Data;
using Dock.Avalonia;
using System;
using System.ComponentModel;

namespace Core2D.UI.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="RecordsControl"/> xaml.
    /// </summary>
    public class RecordsControl : UserControl
    {
        private DataGrid _rowsDataGrid;
        private IDatabase _database;
        private Point _dragStartPoint;
        private PointerEventArgs _triggerEvent;
        private IRecord _record;
        private bool _lock = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordsControl"/> class.
        /// </summary>
        public RecordsControl()
        {
            InitializeComponent();

            _rowsDataGrid = this.FindControl<DataGrid>("rowsDataGrid");
            //_rowsDataGrid.PointerPressed += RowsDataGrid_PointerPressed;
            //_rowsDataGrid.PointerMoved += RowsDataGrid_PointerMoved;

            _rowsDataGrid.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            _rowsDataGrid.AddHandler(InputElement.PointerMovedEvent, Moved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

            DataContextChanged += RecordsControl_DataContextChanged;
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Pressed(object sender, PointerPressedEventArgs e)
        {
            var properties = e.GetCurrentPoint(_rowsDataGrid).Properties;
            if (properties.IsLeftButtonPressed)
            {
                if (e.Source is IControl control && control.DataContext is IRecord record)
                {
                    _dragStartPoint = e.GetPosition(null);
                    _triggerEvent = e;
                    _record = record;
                    _lock = true;
                }
            }
        }

        private async void Moved(object sender, PointerEventArgs e)
        {
            var properties = e.GetCurrentPoint(_rowsDataGrid).Properties;
            if (properties.IsLeftButtonPressed && _triggerEvent != null)
            {
                var point = e.GetPosition(null);
                var diff = _dragStartPoint - point;
                if (Math.Abs(diff.X) > 5 || Math.Abs(diff.Y) > 3)
                {
                    if (_lock == true)
                    {
                        _lock = false;
                    }
                    else
                    {
                        return;
                    }

                    var data = new DataObject();
                    data.Set(DragDataFormats.Context, _record);
                    var effect = DragDropEffects.None;
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Alt))
                    {
                        effect |= DragDropEffects.Link;
                    }
                    else if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    {
                        effect |= DragDropEffects.Move;
                    }
                    else if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                    {
                        effect |= DragDropEffects.Copy;
                    }
                    else
                    {
                        effect |= DragDropEffects.Move;
                    }
                    var result = await DragDrop.DoDragDrop(_triggerEvent, data, effect);

                    _triggerEvent = null;
                    _record = null;
                }
            }
        }

        private void RowsDataGrid_PointerPressed(object sender, PointerPressedEventArgs e)
        {

        }

        private void RowsDataGrid_PointerMoved(object sender, PointerEventArgs e)
        {
        }

        private void RecordsControl_DataContextChanged(object sender, System.EventArgs e)
        {
            if (_database != null)
            {
                _database.PropertyChanged -= Database_PropertyChanged;
                _database = null;
            }

            ResetColumns();

            if (DataContext is IDatabase database)
            {
                _database = database;
                _database.PropertyChanged += Database_PropertyChanged;
                CreateColumns();
            }
        }

        private void Database_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Columns")
            {
                if (_database != null)
                {
                    ResetColumns();
                    CreateColumns();
                }
            }
        }

        private void Column_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" || e.PropertyName == "Width")
            {
                if (_database != null)
                {
                    UpdateHeaders();
                }
            }
        }

        private void CreateColumns()
        {
            for (int i = 0; i < _database.Columns.Length; i++)
            {
                var column = _database.Columns[i];
                var dataGridTextColumn = new DataGridTextColumn()
                {
                    Header = $"{column.Name}",
                    Width = double.IsNaN(column.Width) ? DataGridLength.Auto : new DataGridLength(column.Width),
                    Binding = new Binding($"Values[{i}].Content"),
                    IsReadOnly = true
                };
                column.PropertyChanged += Column_PropertyChanged;
                _rowsDataGrid.Columns.Add(dataGridTextColumn);
            }
        }

        private void ResetColumns()
        {
            _rowsDataGrid.Columns.Clear();
        }

        private void UpdateHeaders()
        {
            for (int i = 0; i < _database.Columns.Length; i++)
            {
                var column = _database.Columns[i];
                _rowsDataGrid.Columns[i].Header = column.Name;
                _rowsDataGrid.Columns[i].Width = double.IsNaN(column.Width) ? DataGridLength.Auto : new DataGridLength(column.Width);
            }
        }
    }
}
