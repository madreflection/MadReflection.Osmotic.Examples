using System;
using System.Collections.Generic;
using System.Text;
using MadReflection.Osmotic;

namespace DBVariantDemo
{
	internal class UriParser : IParser<Uri>
	{
		public static readonly UriParser Instance = new UriParser();


		public Uri Parse(string s) => new Uri(s);

		object IParser.Parse(string s) => Parse(s);

		public bool TryParse(string s, out object result)
		{
			bool x = TryParse(s, out Uri output);
			result = output;
			return x;
		}

		public bool TryParse(string s, out Uri result) => Uri.TryCreate(s, UriKind.Absolute, out result);
	}
}
