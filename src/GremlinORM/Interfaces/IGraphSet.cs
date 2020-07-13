using System;
using TaleLearnCode.GremlinORM.Attributes;

namespace TaleLearnCode.GremlinORM.Interfaces
{

	/// <summary>
	/// Interface used by the <see cref="GraphSet{TVertex}"/> type.
	/// </summary>
	interface IGraphSet
	{

		/// <summary>
		/// Gets the label of the vertex in the database.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the vertex label.
		/// </value>
		public string Label { get; }

		/// <summary>
		/// Gets the <see cref="VertexAttribute"/> added to the vertex type definition.
		/// </summary>
		/// <value>
		/// The <see cref="VertexAttribute"/> associated with the vertex type definition.
		/// </value>
		public VertexAttribute VertexAttribute { get; }

		/// <summary>
		/// Gets the type of the vertex.
		/// </summary>
		/// <returns>A <see cref="Type"/> representing the vertex type.</returns>
		public Type GetVertexType();

		/// <summary>
		/// Add an appropriately typed vertex to the change tracker from a traversal ran in <see cref="GraphContext"/>.
		/// </summary>
		/// <param name="queryResult">A <see cref="QueryResult"/> representing an individual result from a database traversal.</param>
		/// <returns>The serialized version of the <paramref name="queryResult"/>.</returns>
		internal object AddFromQuery(QueryResult queryResult);

	}
}