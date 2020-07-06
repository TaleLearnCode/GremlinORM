using System.Collections.Generic;

namespace GremlinORMSample
{
	public class QueryResult
	{
		public string Label { get; set; }

		public string Id { get; set; }

		public GremlinType GremlinType { get; set; }

		public Dictionary<string, List<string>> Properties { get; set; } = new Dictionary<string, List<string>>();

	}

}
