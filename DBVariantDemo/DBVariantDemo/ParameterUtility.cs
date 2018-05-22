using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadReflection.Osmotic;

namespace DBVariantDemo
{
	public static class ParameterUtility
	{
		private static readonly ParserContainer _parser = ParserContainer.Create(config =>
		{
			config.ReferenceTypesParseNullToNull = true;
			config.NullableValueTypesParseNullToNull = true;
			config.NullableValueTypesParseEmptyStringToNull = true;

			config.UseParserObject(LocalDateParserFormatter.Instance);
			config.UseParserObject(LocalTimeParserFormatter.Instance);
			config.UseParserObject(InstantParserFormatter.Instance);

			config.UseParserObject(UriParser.Instance);
		});


		public static IParser<T> For<T>() => _parser.For<T>();

		public static IParser For(Type type) => _parser.For(type);
	}

	public static class ConsoleUtility
	{
		private static readonly FormatterContainer _formatter = FormatterContainer.Create(config =>
		{
			config.ReferenceTypesFormatNullToNull = true;
			config.NullableValueTypesFormatNull = NullFormatHandling.ReturnNull;

			config.UseFormatterObject(LocalDateParserFormatter.Instance);
			config.UseFormatterObject(LocalTimeParserFormatter.Instance);
			config.UseFormatterObject(InstantParserFormatter.Instance);
		});


		public static IFormatter<T> For<T>() => _formatter.For<T>();

		public static IFormatter For(Type type) => _formatter.For(type);
	}
}
