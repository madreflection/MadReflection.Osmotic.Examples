using System;
using System.Collections.Generic;
using System.Linq;
using DBVariantDemo.Data;
using NodaTime;
using MadReflection.Osmotic;

namespace DBVariantDemo
{
	internal static partial class VariantUtility
	{
		private static List<SupportedType> _supportedTypes = new List<SupportedType>()
		{
			new SupportedType(0,  "string",   typeof(string)),
			new SupportedType(1,  "uuid",     typeof(Guid?)),
			new SupportedType(2,  "int",      typeof(int?)),
			new SupportedType(3,  "decimal",  typeof(decimal?)),
			new SupportedType(10, "uri",      typeof(Uri)),
			new SupportedType(20, "datetime", typeof(Instant?)),
			new SupportedType(21, "date",     typeof(LocalDate?)),
			new SupportedType(22, "time",     typeof(LocalTime?))
		};
		private static readonly Dictionary<int, Type> _supportedTypesIdToType = _supportedTypes.ToDictionary(x => x.ID, x => x.ClrType);
		private static readonly Dictionary<Type, int> _supportedTypesTypeToId = _supportedTypes.ToDictionary(x => x.ClrType, x => x.ID);
		private static readonly Dictionary<string, Type> _supportedTypesKeywordToType = _supportedTypes.ToDictionary(x => x.Keyword, x => x.ClrType);
		private static readonly Dictionary<Type, string> _supportedTypesTypeToKeyword = _supportedTypes.ToDictionary(x => x.ClrType, x => x.Keyword);
		// In another life, these lookups could also be useful:
		//private static readonly Dictionary<int, string> _supportedTypesIdToKeyword = _supportedTypes.ToDictionary(x => x.ID, x => x.Keyword);
		//private static readonly Dictionary<string, int> _supportedTypesKeywordToId = _supportedTypes.ToDictionary(x => x.Keyword, x => x.ID);

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
		private static readonly FormatterContainer _formatter = FormatterContainer.Create(config =>
		{
			config.ReferenceTypesFormatNullToNull = true;
			config.NullableValueTypesFormatNull = NullFormatHandling.ReturnNull;

			config.UseFormatterObject(LocalDateParserFormatter.Instance);
			config.UseFormatterObject(LocalTimeParserFormatter.Instance);
			config.UseFormatterObject(InstantParserFormatter.Instance);
		});


		public static IEnumerable<VariantItem> GetAllVariables()
		{
			using (DBVariantDemoDataContext context = new DBVariantDemoDataContext())
			{
				return (
					from v in context.Variants
					let type = _supportedTypesIdToType[v.TypeId]
					select new VariantItem()
					{
						Name = v.Name,
						Type = type,
						Value = _parser.For(type).Parse(v.Value)
					}).ToList();
			}
		}

		public static VariantItem GetVariable(string name)
		{
			using (DBVariantDemoDataContext context = new DBVariantDemoDataContext())
			{
				Variant variant = context.Variants.Where(v => v.Name == name).SingleOrDefault();
				if (variant == null)
					return null;

				Type type = _supportedTypesIdToType[variant.TypeId];

				return new VariantItem()
				{
					Name = variant.Name,
					Type = type,
					Value = _parser.For(type).Parse(variant.Value)
				};
			}
		}

		// Unused.
		public static T GetVariableValue<T>(string name)
		{
			VariantItem item = GetVariable(name);
			if (item.Value is T typedValue)
				return typedValue;

			throw new Exception("Value does not match the expected type.");
		}

		// Unused.
		public static object GetVariableValue(string name)
		{
			VariantItem item = GetVariable(name);
			return item?.Value;
		}

		public static void SetVariableValue<T>(string name, T value) => SetVariableValue(name, typeof(T), value);

		public static void SetVariableValue(string name, Type type, object value)
		{
			using (DBVariantDemoDataContext context = new DBVariantDemoDataContext())
			{
				Variant variant = context.Variants.Where(v => v.Name == name).SingleOrDefault();

				if (variant == null)
					context.Variants.Add(variant = new Variant() { Name = name });

				if (!_supportedTypesTypeToId.TryGetValue(type, out int typeId))
					throw new Exception("Unsupported type.");

				variant.TypeId = typeId;
				variant.Value = _formatter.For(type).Format(value);

				context.SaveChanges();
			}
		}

		public static void RemoveVariable(string name)
		{
			using (DBVariantDemoDataContext context = new DBVariantDemoDataContext())
			{
				Variant variant = context.Variants.Where(v => v.Name == name).SingleOrDefault();

				if (variant != null)
				{
					context.Variants.Remove(variant);
					context.SaveChanges();
				}
			}
		}


		public static Type FindTypeByTypeName(string typeName) => _supportedTypesKeywordToType.TryGetValue(typeName, out Type type) ? type : null;

		public static string FindTypeNameByType(Type type) => _supportedTypesTypeToKeyword.TryGetValue(type, out string typeName) ? typeName : null;
	}
}
