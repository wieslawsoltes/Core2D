using System.Collections.Immutable;
using System.Linq;

namespace Core2D.Data
{
    /// <summary>
    /// Database extensions.
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Update the destination database using data from source database using Id column as identification.
        /// </summary>
        /// <param name="destination">The destination database.</param>
        /// <param name="source">The source database.</param>
        /// <param name="records">The updated records from destination database.</param>
        /// <returns>True if destination database was updated.</returns>
        public static bool Update(this IDatabase destination, IDatabase source, out ImmutableArray<IRecord>.Builder records)
        {
            bool isDirty = false;
            records = null;

            if (source == null || destination == null)
            {
                return isDirty;
            }

            // Check the number of source database columns.
            if (source.Columns.Length <= 1)
            {
                return isDirty;
            }

            // Check for presence of the Id column in the source database.
            if (source.Columns[0].Name != destination.IdColumnName)
            {
                return isDirty;
            }

            // Check for matching columns length.
            if (source.Columns.Length - 1 != destination.Columns.Length)
            {
                return isDirty;
            }

            // Check for matching column names.
            for (int i = 1; i < source.Columns.Length; i++)
            {
                if (source.Columns[i].Name != destination.Columns[i - 1].Name)
                {
                    return isDirty;
                }
            }

            // Create updated records builder.
            records = destination.Records.ToBuilder();

            // Update or remove existing records.
            for (int i = 0; i < destination.Records.Length; i++)
            {
                var record = destination.Records[i];
                var result = source.Records.FirstOrDefault(r => r.Id == record.Id);
                if (result != null)
                {
                    // Update existing record.
                    for (int j = 1; j < result.Values.Length; j++)
                    {
                        var valuesBuilder = record.Values.ToBuilder();
                        valuesBuilder[j - 1] = result.Values[j];
                        record.Values = valuesBuilder.ToImmutable();
                    }
                    isDirty = true;
                }
                else
                {
                    // Remove existing record.
                    records.Remove(record);
                    isDirty = true;
                }
            }

            // Add new records.
            for (int i = 0; i < source.Records.Length; i++)
            {
                var record = source.Records[i];
                var result = destination.Records.FirstOrDefault(r => r.Id == record.Id);
                if (result == null)
                {
                    var r = source.Records[i];

                    // Skip Id column.
                    r.Values = r.Values.Skip(1).ToImmutableArray();

                    // Add new record.
                    records.Add(r);
                    isDirty = true;
                }
            }

            return isDirty;
        }
    }
}
