using System;
using System.Globalization;
using NodaTime;
using NodaTime.Text;
using MadReflection.Osmotic;

namespace DBVariantDemo
{
	internal abstract class NodaTimeParserFormatter<TValue> : IParser<TValue>, IFormatter<TValue>
	{
		protected static IPattern<TValue> _defaultPattern;
		protected static Func<string, CultureInfo, IPattern<TValue>> _patternFactory;


		protected NodaTimeParserFormatter()
		{
		}


		object IParser.Parse(string s) => Parse(s);

		bool IParser.TryParse(string s, out object result)
		{
			bool success = TryParse(s, out TValue temp);
			result = temp;
			return success;
		}

		public TValue Parse(string s) => _defaultPattern.Parse(s).GetValueOrThrow();

		public bool TryParse(string s, out TValue result)
		{
			ParseResult<TValue> parseResult = _defaultPattern.Parse(s);
			if (!parseResult.Success)
			{
				result = default(TValue);
				return false;
			}

			result = parseResult.Value;
			return true;
		}


		string IFormatter.Format(object value) => Format(ValidateDataType(value));

		string IFormatter.Format(object value, string format) => Format(ValidateDataType(value), format);

		public string Format(TValue value) => _defaultPattern.Format(value);

		public string Format(TValue value, string format) => _patternFactory(format, CultureInfo.InvariantCulture).Format(value);


		private static TValue ValidateDataType(object value) => value is TValue typedValue ? typedValue : throw new ArgumentException("Invalid formatter selected.");
	}

	internal sealed class LocalDateParserFormatter : NodaTimeParserFormatter<LocalDate>
	{
		static LocalDateParserFormatter()
		{
			_defaultPattern = LocalDatePattern.Iso;
			_patternFactory = LocalDatePattern.Create;
		}


		public static readonly LocalDateParserFormatter Instance = new LocalDateParserFormatter();
	}

	internal sealed class LocalTimeParserFormatter : NodaTimeParserFormatter<LocalTime>
	{
		static LocalTimeParserFormatter()
		{
			_defaultPattern = LocalTimePattern.ExtendedIso;
			_patternFactory = LocalTimePattern.Create;
		}


		public static readonly LocalTimeParserFormatter Instance = new LocalTimeParserFormatter();
	}

	internal sealed class InstantParserFormatter : NodaTimeParserFormatter<Instant>
	{
		static InstantParserFormatter()
		{
			_defaultPattern = InstantPattern.ExtendedIso;
			_patternFactory = InstantPattern.Create;
		}


		public static readonly InstantParserFormatter Instance = new InstantParserFormatter();
	}
}
