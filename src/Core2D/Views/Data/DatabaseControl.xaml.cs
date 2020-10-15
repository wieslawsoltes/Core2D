using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Core2D.Data;

namespace Core2D.UI.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="DatabaseControl"/> xaml.
    /// </summary>
    public class DatabaseControl : UserControl
    {
        private TextBox _filterRecordsText;
        private DataGrid _rowsDataGrid;
        private Database _database;
        private string _recordsFilter;
        private DataGridCollectionView _recordsView;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseControl"/> class.
        /// </summary>
        public DatabaseControl()
        {
            InitializeComponent();

            _filterRecordsText = this.FindControl<TextBox>("filterRecordsText");
            _filterRecordsText.GetObservable(TextBox.TextProperty).Subscribe(_ => OnFilterRecordsTextChanged());

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

        private void OnFilterRecordsTextChanged()
        {
            Dispatcher.UIThread.Post(() =>
            {
                _recordsFilter = _filterRecordsText?.Text;
                _recordsView?.Refresh();
            });
        }

        private bool FilterRecords(object arg)
        {
            if (!string.IsNullOrWhiteSpace(_recordsFilter) && arg is Record record)
            {
                foreach (var value in record.Values)
                {
                    if (!string.IsNullOrWhiteSpace(value.Content))
                    {
                        if (value.Content.IndexOf(_recordsFilter, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            return true;
        }

        private void RowsDataGrid_DataContextChanged(object sender, EventArgs e)
        {
            if (_database != null)
            {
                _database.PropertyChanged -= Database_PropertyChanged;
                _database = null;
            }

            ResetRecordsView();
            ResetColumns();

            if (DataContext is Database database)
            {

                _database = database;
                _database.PropertyChanged += Database_PropertyChanged;
                CreateColumns();
                CreateRecordsView();
            }
        }

        private void Database_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Database.Columns))
            {
                if (_database != null)
                {
                    ResetColumns();
                    CreateColumns();
                }
            }

            if (e.PropertyName == nameof(Database.Records))
            {
                if (_database != null)
                {
                    ResetRecordsView();
                    CreateRecordsView();
                }
            }
        }

        private void Column_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Column.Name)
                || e.PropertyName == nameof(Column.IsVisible))
            {
                if (_database != null)
                {
                    UpdateHeaders();
                }
            }
        }

        private void CreateRecordsView()
        {
            _recordsView = new DataGridCollectionView(_database.Records);
            _recordsView.Filter = FilterRecords;
            _rowsDataGrid.Items = _recordsView;

            _recordsFilter = _filterRecordsText?.Text;
            _recordsView?.Refresh();
        }

        private void ResetRecordsView()
        {
            _rowsDataGrid.Items = null;
            _recordsView = null;
        }

        private void CreateColumns()
        {
            for (int i = 0; i < _database.Columns.Length; i++)
            {
                var column = _database.Columns[i];
                var dataGridTextColumn = new DataGridTextColumn()
                {
                    Header = $"{column.Name}",
                    Width = DataGridLength.Auto,
                    IsVisible = column.IsVisible,
                    Binding = new Binding($"{nameof(Record.Values)}[{i}].{nameof(Value.Content)}"),
                    IsReadOnly = false
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
                _rowsDataGrid.Columns[i].IsVisible = column.IsVisible;
            }
        }
    }
}
