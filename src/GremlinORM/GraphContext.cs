using System;
using System.Collections.Generic;

namespace TaleLearnCode.GremlinORM
{

	public class GraphContext
	{

		private Dictionary<string, Type> ModelVertices { get; } = new Dictionary<string, Type>();

		/// <summary>
		/// Gets the a facade of the connection to the Gremlin graph.
		/// </summary>
		/// <value>
		/// The <see cref="GraphFacade"/> used by the GraphContext.
		/// </value>
		public GraphFacade GraphFacade { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphContext"/> class.
		/// </summary>
		/// <param name="endpoint">The Cosmos DB account endpoint.</param>
		/// <param name="authKey">The Cosmos DB account authentication key.</param>
		/// <param name="databaseName">Name of the database.</param>
		/// <param name="graphName">Name of the graph.</param>
		public GraphContext(string endpoint, string authKey, string databaseName, string graphName)
		{
			GraphFacade = new GraphFacade(endpoint, authKey, databaseName, graphName);
		}

		/// <summary>
		/// Adds the specified vertex to the graph model maintained by the context.
		/// </summary>
		/// <param name="type">The type of the vertex.</param>
		/// <param name="label">The label of the vertex.</param>
		public void AddVertexToModel(Type type, string label)
		{
			if (!ModelVertices.ContainsKey(label))
				ModelVertices.Add(label, type);
		}

		/// <summary>
		/// Removes the specified vertex from the graph model maintained by the context.
		/// </summary>
		/// <param name="label">The label of the vertex to be removed.</param>
		public void RemoveVertexFromModel(string label)
		{
			if (ModelVertices.ContainsKey(label))
				ModelVertices.Remove(label);
		}



	}

}