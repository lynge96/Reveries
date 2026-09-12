using System.Text.Json;
using Reveries.Integration.Isbndb.Dtos;

namespace Reveries.Integration.Tests.Isbndb;

public class IsbndbDimensionsConverterTests
{
    private static readonly IsbndbDimensionsConverter Converter = new();

    [Fact]
    public void Read_EmptyArray_ReturnsNull()
    {
        var reader = new Utf8JsonReader("[]"u8);
        reader.Read();

        var result = Converter.Read(ref reader, typeof(IsbndbDimensionsDto), JsonSerializerOptions.Default);

        Assert.Null(result);
    }

    [Fact]
    public void Read_Object_DeserializesDimensions()
    {
        var reader = new Utf8JsonReader("""{"height":{"unit":"inches","value":9.3},"weight":{"unit":"pounds","value":1.2}}"""u8);
        reader.Read();

        var result = Converter.Read(ref reader, typeof(IsbndbDimensionsDto), JsonSerializerOptions.Default);

        Assert.NotNull(result);
        Assert.Equal("inches", result.Height?.Unit);
        Assert.Equal(9.3, result.Height?.Value);
        Assert.Equal(1.2, result.Weight?.Value);
    }
}