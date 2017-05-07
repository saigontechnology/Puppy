using System;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ElasticByte : ElasticNumber
	{
		public override string JsonString()
		{
			return JsonStringInternal("byte");
		}
	}
}