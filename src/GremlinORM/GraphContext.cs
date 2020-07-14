using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TaleLearnCode.GremlinORM.Interfaces;

namespace TaleLearnCode.GremlinORM
{

	/// <summary>
	/// A GraphSet instance represents a session with a Gremlin database and can be
	/// used to query and save instances o f your entities.
	/// </summary>
	/// <remaks>
	/// <para>
	/// Typically you create a class that derives from DBContext and contains <see cref="GraphSet{TVertex}"/>
	/// properties for each entity in the model. If the <see cref="GraphSet{TVertex}"/> properties have a public setter
	/// and implements the <see cref="GraphPropertyAttribute"/>, they are automatically initialized when the instance
	/// of the derived context is created.
	/// </para>
	/// </remaks>
	public class GraphContext
	{

		// Key = label; Value = Property Name & Vertex Type		
		/// <summary>
		/// Gets the list of vertices being tracked by the context.
		/// </summary>
		/// <value>
		/// A <see cref="Dictionary{string, (string, Type)}"/> representing the list of
		/// vertices being tracked by the context. The value is a tuple containing the
		/// name of the property within the context instance (PropertyName) and the type
		/// of the of the vertices within the <see cref="GraphSet{TVertex}"/> property (VertexType).
		/// </value>
		private Dictionary<string, (string PropertyName, Type VertexType)> TrackedVertexTypes { get; } = new Dictionary<string, (string PropertyName, Type VertexType)>();

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
		/// <param name="hostname">The hostname of the Gremlin server to connect to.</param>
		/// <param name="password">The password to use for connecting to the Gremlin server.</param>
		/// <param name="databaseName">Name of the Gremlin database on the server to connect to.</param>
		/// <param name="graphName">Name of the graph to connect to.</param>
		/// <param name="port">The port on which the Gremlin Server can be reached.  Default is 443.</param>
		/// <param name="enableSSL">If set to <c>true</c> SSL shall be used for connecting to the server.  Default is true.</param>
		public GraphContext(string hostname, string password, string databaseName, string graphName, int port = 443, bool enableSSL = true)
		{

			GraphFacade = new GraphFacade(hostname, password, databaseName, graphName, port, enableSSL);

			foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
				if (typeof(IGraphSet).IsAssignableFrom(propertyInfo.PropertyType))
				{
					IGraphSet graphSet = (IGraphSet)propertyInfo.GetValue(this);
					TrackedVertexTypes.Add(graphSet.VertexAttribute.Label, (propertyInfo.Name, graphSet.GetVertexType()));
				}

		}

		/// <summary>
		/// Executes the <paramref name="gremlin"/> traversal against the Gremlin server.
		/// </summary>
		/// <param name="gremlin">The gremlin traversal to be executed against the Gremlin server.</param>
		/// <returns>A <see cref="Dictionary{Type, List{object}}"/> representing the results of the traversal. The key is the type of the entity and the value is a list of objects casted to the keyed type.</returns>
		public async Task<Dictionary<Type, List<object>>> ExecuteQueryAsync(string gremlin)
		{

			Dictionary<Type, List<object>> returnValue = new Dictionary<Type, List<object>>();

			TraversalResultset traversalResultset = await GraphFacade.QueryAsync(gremlin).ConfigureAwait(true);

			foreach (QueryResult queryResult in traversalResultset.Results)
			{
				if (TrackedVertexTypes.TryGetValue(queryResult.Label, out (string PropertyName, Type VertexType) graphSet))
				{
					if (!returnValue.ContainsKey(((IGraphSet)this.GetType().GetProperty(graphSet.PropertyName).GetValue(this)).GetVertexType()))
						returnValue.Add(((IGraphSet)this.GetType().GetProperty(graphSet.PropertyName).GetValue(this)).GetVertexType(), new List<object>());
					returnValue[((IGraphSet)this.GetType().GetProperty(graphSet.PropertyName).GetValue(this)).GetVertexType()].Add(((IGraphSet)this.GetType().GetProperty(graphSet.PropertyName).GetValue(this)).AddFromQuery(queryResult));
				}
			}

			return returnValue;

		}

	}

}