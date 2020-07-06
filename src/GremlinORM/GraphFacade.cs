using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using System.Threading.Tasks;

namespace GremlinORM
{
	/// <summary>
	/// Provides access to graph related information.
	/// </summary>
	public class GraphFacade
	{

		/// <summary>
		/// Gets the endpoint of the Cosmos DB account.
		/// </summary>
		/// <value>
		/// A <c>string</c> that represents the Cosmos DB account endpoint.
		/// </value>
		public string Endpoint { get; }

		/// <summary>
		/// Gets the authentication key for the Cosmos DB account.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the Cosmos DB account authentication key.
		/// </value>
		public string AuthKey { get; }

		/// <summary>
		/// Gets the name of the Gremlin database.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the Gremlin database name.
		/// </value>
		public string DatabaseName { get; }

		/// <summary>
		/// Gets the name of the graph.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the graph name.
		/// </value>
		public string GraphName { get; }

		/// <summary>
		/// Gets the <see cref="GremlinClient"/> being used in the background for the Gremlin operations.
		/// </summary>
		/// <value>
		/// A <see cref="GremlinClient"/> reference which is being used for Gremlin database operations.
		/// </value>
		public GremlinClient GremlinClient { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphFacade"/> class.
		/// </summary>
		/// <param name="endpoint">The Cosmos DB account endpoint.</param>
		/// <param name="authKey">The Cosmos DB account authentication key.</param>
		/// <param name="databaseName">Name of the database.</param>
		/// <param name="graphName">Name of the graph.</param>
		public GraphFacade(string endpoint, string authKey, string databaseName, string graphName)
		{
			Endpoint = endpoint;
			AuthKey = authKey;
			DatabaseName = databaseName;
			GraphName = graphName;
			var gremlinServer = new GremlinServer(endpoint, 443, enableSsl: true, username: "/dbs/" + databaseName + "/colls/" + graphName, password: authKey);
			GremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType);
		}

		/// <summary>
		/// Performs the specific query against the Gremlin server.
		/// </summary>
		/// <param name="gremlinQuery">The gremlin query to be executed.</param>
		/// <returns>A <see cref="ResultSet{dynamic}"/> containing the results of the query.</returns>
		public async Task<ResultSet<dynamic>> QueryAsync(string gremlinQuery)
		{
			return await GremlinClient.SubmitAsync<dynamic>(gremlinQuery).ConfigureAwait(true);
		}

	}

}