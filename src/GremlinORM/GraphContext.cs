using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TaleLearnCode.GremlinORM.Interfaces;

namespace TaleLearnCode.GremlinORM
{

	public class GraphContext
	{

		/// <summary>
		/// Gets the tracked vertex types.
		/// </summary>
		/// <value>
		/// The tracked vertex types.
		/// Key = label; Value = Property Name
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
		/// <param name="endpoint">The Cosmos DB account endpoint.</param>
		/// <param name="authKey">The Cosmos DB account authentication key.</param>
		/// <param name="databaseName">Name of the database.</param>
		/// <param name="graphName">Name of the graph.</param>
		public GraphContext(string endpoint, string authKey, string databaseName, string graphName)
		{
			GraphFacade = new GraphFacade(endpoint, authKey, databaseName, graphName);

			foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
			{
				if (typeof(IGraphSet).IsAssignableFrom(propertyInfo.PropertyType))
				{

					IGraphSet graphSet = (IGraphSet)propertyInfo.GetValue(this);
					TrackedVertexTypes.Add(graphSet.VertexAttribute.Label, (propertyInfo.Name, graphSet.GetVertexType()));
				}

			}



		}

		public async Task<Dictionary<Type, List<object>>> ExecuteQueryAsync(string gremlinQuery)
		{
			Dictionary<Type, List<object>> returnValue = new Dictionary<Type, List<object>>();

			TraversalResultset traversalResultset = await GraphFacade.QueryAsync(gremlinQuery).ConfigureAwait(true);

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