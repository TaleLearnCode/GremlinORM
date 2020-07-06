using System;
using TaleLearnCode.GremlinORM.Attributes;

namespace TaleLearnCode.GremlinORM
{
	public abstract class Vertex
	{

		/// <summary>
		/// Gets or sets the identifier of the vertex.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the vertex identifier within Cosmos.
		/// </value>
		[GraphProperty("id")]
		public string Id { get; set; } = Guid.NewGuid().ToString();

		[GraphProperty("label", IncludeInGraph = false)]
		public string Label { get; }

		protected Vertex(string label)
		{
			Label = label;
		}

		// TODO: Add PartitionKey for the Cosmos extension

	}

}