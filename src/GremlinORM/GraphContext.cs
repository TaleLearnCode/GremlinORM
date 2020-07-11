using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace TaleLearnCode.GremlinORM
{

	public class GraphContext
	{

		private Dictionary<string, Type> _trackedVertexTypes = new Dictionary<string, Type>();

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

		public void TrackVertexType(Type type, string label)
		{
			_trackedVertexTypes[label] = type;
		}

		//public async Task<List<TVertex>> ExecuteQueryAsync(string gremlinQuery, GraphFacade graphFacade)
		//{

		//	if (graphFacade is null) throw new ArgumentNullException(nameof(graphFacade));

		//	List<TVertex> reutrnValue = new List<TVertex>();

		//	TraversalResultset queryResultset = await graphFacade.QueryAsync(gremlinQuery).ConfigureAwait(true);


		//	return reutrnValue;
		//}


		public async Task<List<TVertex>> ExecuteQueryAsync<TVertex>(string gremlinQuery)
		{
			List<TVertex> returnValue = new List<TVertex>();
			TraversalResultset traversalResultset = await GraphFacade.QueryAsync(gremlinQuery).ConfigureAwait(true);

			foreach (QueryResult queryResult in traversalResultset.Results)
			{

				Type vertexType = _trackedVertexTypes[queryResult.Label];


				PropertyInfo myPropInfo = this.GetType().GetProperty("corina");



				foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
				{
					if (propertyInfo.PropertyType == typeof(QueryResult))
					{

					}







				}
			}




			return returnValue;
		}



	}

}