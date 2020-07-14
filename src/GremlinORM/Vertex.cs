using System;
using TaleLearnCode.GremlinORM.Attributes;

namespace TaleLearnCode.GremlinORM
{

	/// <summary>
	/// Abstract class to be used by vertex types.
	/// </summary>
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

		/// <summary>
		/// Gets the label of the vertex.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the vertex label.
		/// </value>
		[GraphProperty("label", IncludeInGraph = false)]
		public string Label { get; internal set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Vertex"/> class.
		/// </summary>
		/// <param name="label">The label of the vertex.</param>
		protected Vertex(string label)
		{
			Label = label;
		}

	}

}