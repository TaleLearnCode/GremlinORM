using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using System.Threading.Tasks;

namespace TaleLearnCode.GremlinORM
{

	/// <summary>
	/// Provides a facade to access the graph database.
	/// </summary>
	public class GraphFacade
	{

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
		/// <param name="hostname">The hostname of the Gremlin server to connect to.</param>
		/// <param name="password">The password to use for connecting to the Gremlin server.</param>
		/// <param name="databaseName">Name of the Gremlin database on the server to connect to.</param>
		/// <param name="graphName">Name of the graph to connect to.</param>
		/// <param name="port">The port on which the Gremlin Server can be reached.  Default is 443.</param>
		/// <param name="enableSSL">If set to <c>true</c> SSL shall be used for connecting to the server.  Default is true.</param>
		public GraphFacade(string hostname, string password, string databaseName, string graphName, int port = 443, bool enableSSL = true)
		{
			var gremlinServer = new GremlinServer(hostname, port, enableSsl: enableSSL, username: "/dbs/" + databaseName + "/colls/" + graphName, password: password);
			GremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType);
		}

		/// <summary>
		/// Performs the specific query against the Gremlin server.
		/// </summary>
		/// <param name="gremlinQuery">The gremlin query to be executed.</param>
		/// <returns>A <see cref="ResultSet{dynamic}"/> containing the results of the query.</returns>
		public async Task<TraversalResultset> QueryAsync(string gremlinQuery)
		{
			return new TraversalResultset(await GremlinClient.SubmitAsync<dynamic>(gremlinQuery).ConfigureAwait(true));
		}

	}

}