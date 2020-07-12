using System;
using TaleLearnCode.GremlinORM.Attributes;

namespace TaleLearnCode.GremlinORM.Interfaces
{
	interface IGraphSet
	{
		public string Label { get; }
		public Type GetVertexType();
		internal object AddFromQuery(QueryResult queryResult);
		public VertexAttribute VertexAttribute { get; }
	}
}