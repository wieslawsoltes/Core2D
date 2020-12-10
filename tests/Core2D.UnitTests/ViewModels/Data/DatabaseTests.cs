using Core2D.Model;
using Core2D.ViewModels;
using Xunit;

namespace Core2D.Data.UnitTests
{
    public class DatabaseTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateDatabase("db");
            Assert.True(target is ViewModelBase);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Columns_Not_Null()
        {
            var target = _factory.CreateDatabase("db");
            Assert.False(target.Columns.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Records_Not_Null()
        {
            var target = _factory.CreateDatabase("db");
            Assert.False(target.Records.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void FromFields_Existing_Record_Ids()
        {
            var fields = new string[][]
            {
                new string[] { "Id", "Column0", "Column1", "Column2" },
                new string[] { "75080d72-734d-4187-a38f-9860df375a2a", "Row0Value0", "Row0Value1", "Row0Value2" },
                new string[] { "547fe8cf-b3ab-4abb-843b-acb3df0f7ad1", "Row1Value0", "Row1Value1", "Row1Value2" }
            };

            var target = _factory.FromFields("Test", fields, "Id");

            Assert.Equal("Id", target.IdColumnName);

            Assert.Equal(4, target.Columns.Length);

            Assert.Equal("Id", target.Columns[0].Name);
            Assert.Equal("Column0", target.Columns[1].Name);
            Assert.Equal("Column1", target.Columns[2].Name);
            Assert.Equal("Column2", target.Columns[3].Name);

            Assert.Equal(2, target.Records.Length);

            Assert.Equal("75080d72-734d-4187-a38f-9860df375a2a", target.Records[0].Id.ToString());
            Assert.Equal("547fe8cf-b3ab-4abb-843b-acb3df0f7ad1", target.Records[1].Id.ToString());
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void FromFields_New_Record_Ids()
        {
            var fields = new string[][]
            {
                new string[] { "Column0", "Column1", "Column2" },
                new string[] { "Row0Value0", "Row0Value1", "Row0Value2" },
                new string[] { "Row1Value0", "Row1Value1", "Row1Value2" }
            };

            var target = _factory.FromFields("Test", fields);

            Assert.Equal(3, target.Columns.Length);

            Assert.Equal("Column0", target.Columns[0].Name);
            Assert.Equal("Column1", target.Columns[1].Name);
            Assert.Equal("Column2", target.Columns[2].Name);

            Assert.Equal(2, target.Records.Length);

            Assert.Equal("Row0Value0", target.Records[0].Values[0].Content);
            Assert.Equal("Row0Value1", target.Records[0].Values[1].Content);
            Assert.Equal("Row0Value2", target.Records[0].Values[2].Content);

            Assert.Equal("Row1Value0", target.Records[1].Values[0].Content);
            Assert.Equal("Row1Value1", target.Records[1].Values[1].Content);
            Assert.Equal("Row1Value2", target.Records[1].Values[2].Content);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Update_Destination()
        {
            var destinationFields = new string[][]
            {
                new string[] { "Column0", "Column1", "Column2" },
                new string[] { "Row0Value0", "Row0Value1", "Row0Value2" },
                new string[] { "Row1Value0", "Row1Value1", "Row1Value2" },
                new string[] { "Row2Value0", "Row2Value1", "Row2Value2" }
            };

            var destination = _factory.FromFields("Destination", destinationFields, "Id");

            var sourceFields = new string[][]
            {
                new string[] { "Id", "Column0", "Column1", "Column2" },
                // Existing record 0 and 1 values will be updated.
                new string[] { destination.Records[0].Id.ToString(), "Row0Value0Update", "Row0Value1Update", "Row0Value2Update" },
                new string[] { destination.Records[1].Id.ToString(), "Row1Value0", "Row1Value1", "Row1Value2" },
                // Existing record 2 will be removed if not present in source.
                // New record will be added.
                new string[] { "e450006a-cda2-46fc-b475-89335a87e2f8", "Row3Value0", "Row3Value1", "Row3Value2" },
                // New record will be added.
                new string[] { "410b0378-8ea5-4a21-8260-9aa929b2a57b", "Row4Value0", "Row4Value1", "Row4Value2" }
            };

            var source = _factory.FromFields("Source", sourceFields, "Id");

            bool isDirty = destination.Update(source, out var target);

            Assert.True(isDirty);
            Assert.NotNull(target);

            Assert.Equal(4, target.Count);

            Assert.Equal(destination.Records[0].Id.ToString(), target[0].Id.ToString());
            Assert.Equal("Row0Value0Update", target[0].Values[0].Content);
            Assert.Equal("Row0Value1Update", target[0].Values[1].Content);
            Assert.Equal("Row0Value2Update", target[0].Values[2].Content);

            Assert.Equal(destination.Records[1].Id.ToString(), target[1].Id.ToString());
            Assert.Equal("Row1Value0", target[1].Values[0].Content);
            Assert.Equal("Row1Value1", target[1].Values[1].Content);
            Assert.Equal("Row1Value2", target[1].Values[2].Content);

            Assert.Equal("e450006a-cda2-46fc-b475-89335a87e2f8", target[2].Id.ToString());
            Assert.Equal("Row3Value0", target[2].Values[0].Content);
            Assert.Equal("Row3Value1", target[2].Values[1].Content);
            Assert.Equal("Row3Value2", target[2].Values[2].Content);

            Assert.Equal("410b0378-8ea5-4a21-8260-9aa929b2a57b", target[3].Id.ToString());
            Assert.Equal("Row4Value0", target[3].Values[0].Content);
            Assert.Equal("Row4Value1", target[3].Values[1].Content);
            Assert.Equal("Row4Value2", target[3].Values[2].Content);
        }
    }
}
