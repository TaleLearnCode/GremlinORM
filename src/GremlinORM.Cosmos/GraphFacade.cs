using Gremlin.Net.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaleLearnCode.GremlinORM.Cosmos
{
	public class GraphFacade : TaleLearnCode.GremlinORM.GraphFacade
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphFacade"/> class.
		/// </summary>
		/// <param name="endpoint">The Cosmos DB account endpoint.</param>
		/// <param name="authKey">The Cosmos DB account authentication key.</param>
		/// <param name="databaseName">Name of the database.</param>
		/// <param name="graphName">Name of the graph.</param>
		public GraphFacade(string endpoint, string authKey, string databaseName, string graphName) : base(endpoint, authKey, databaseName, graphName) { }

		/// <summary>
		/// Performs the specific query against the Gremlin server.
		/// </summary>
		/// <param name="gremlinQuery">The gremlin query to be executed.</param>
		/// <returns>A <see cref="ResultSet{dynamic}"/> containing the results of the query.</returns>
		public new async Task<TraversalResultset> QueryAsync(string gremlinQuery)
		{
			return new TraversalResultset(await GremlinClient.SubmitAsync<dynamic>(gremlinQuery).ConfigureAwait(true));
		}

	}
}
