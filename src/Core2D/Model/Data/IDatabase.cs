using System.Collections.Immutable;

namespace Core2D.Data
{
    /// <summary>
    /// Defines database contract.
    /// </summary>
    public interface IDatabase : IObservableObject
    {
        /// <summary>
        /// Gets or sets Id column name.
        /// </summary>
        string IdColumnName { get; set; }

        /// <summary>
        /// Gets or sets database columns.
        /// </summary>
        ImmutableArray<IColumn> Columns { get; set; }

        /// <summary>
        /// Gets or sets database records.
        /// </summary>
        ImmutableArray<IRecord> Records { get; set; }

        /// <summary>
        /// Gets or sets database current record.
        /// </summary>
        IRecord CurrentRecord { get; set; }
    }
}
