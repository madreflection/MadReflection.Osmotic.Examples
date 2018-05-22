using System;

namespace DBVariantDemo
{
	internal class SupportedType
	{
		public SupportedType(int id, string keyword, Type clrType)
		{
			ID = id;
			Keyword = keyword;
			ClrType = clrType;
		}


		public int ID { get; }
		public string Keyword { get; }
		public Type ClrType { get; }
	}
}
