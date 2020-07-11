using System;

namespace TaleLearnCode.GremlinORM.Interfaces
{
	interface IGraphSet
	{
		public string Label { get; }
		public Type GetVertexType();
		internal void AddFromQuery(object vertex, VertexState vertexState);
	}
}