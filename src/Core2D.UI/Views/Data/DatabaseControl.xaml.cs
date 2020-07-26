using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Core2D.Data;

namespace Core2D.UI.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="DatabaseControl"/> xaml.
    /// </summary>
    public class DatabaseControl : UserControl
    {
        private DataGrid _rowsDataGrid;
        private IDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseControl"/> class.
        /// </summary>
        public DatabaseControl()
        {
            InitializeComponent();
            _rowsDataGrid = this.FindControl<DataGrid>("rowsDataGrid");
            _rowsDataGrid.DataContextChanged += RowsDataGrid_DataContextChanged;
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void RowsDataGrid_DataContextChanged(object sender, EventArgs e)
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
            if (e.PropertyName == nameof(IDatabase.Columns))
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
            if (e.PropertyName == nameof(IColumn.Name)
                || e.PropertyName == nameof(IColumn.Width)
                || e.PropertyName == nameof(IColumn.IsVisible))
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
                    IsVisible = column.IsVisible,
                    Binding = new Binding($"{nameof(IRecord.Values)}[{i}].{nameof(IValue.Content)}"),
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
                _rowsDataGrid.Columns[i].IsVisible = column.IsVisible;
            }
        }
    }
}
