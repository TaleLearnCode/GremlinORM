using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleLearnCode.GremlinORM.Exceptions;
using TaleLearnCode.GremlinORM.Interfaces;

namespace TaleLearnCode.GremlinORM
{

	public class GraphSet<TVertex> : IGraphSet
		where TVertex : class
	{

		Dictionary<string, TrackedVertex<TVertex>> _changeTracker = new Dictionary<string, TrackedVertex<TVertex>>();

		public string Label { get; }

		public GraphSet(string label)
		{
			if (!typeof(TVertex).IsSubclassOf(typeof(Vertex)))
				throw new Exception(ResourceStrings.TrackedVertexMustInheritFromVertex(typeof(TVertex)));
			Label = label;
		}

		/// <summary>
		/// Adds the <paramref name="vertex"/> to the change tracker to be saved upon calling <see cref="GraphContext.SaveChanges()"/>.
		/// </summary>
		/// <param name="vertex">The vertex to be added to the change tracker.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="vertex"/> is null.</exception>
		/// <exception cref="VertexAlreadyInChangeTrackerException">Thrown if the <paramref name="vertex"/> already exists in the change tracker.</exception>"
		public void Add(TVertex vertex)
		{
			if (vertex is null) throw new ArgumentNullException(nameof(vertex));
			string vertexId = GetVertexId(ref vertex);
			if (_changeTracker.ContainsKey(vertexId))
				throw new VertexAlreadyInChangeTrackerException();
			else
				_changeTracker.Add(vertexId, new TrackedVertex<TVertex>(vertex, VertexState.Added));
		}

		public void Update(TVertex vertex)
		{
			if (vertex is null) throw new ArgumentNullException(nameof(vertex));
			string vertexId = GetVertexId(vertex);
			if (!string.IsNullOrWhiteSpace(vertexId))
				if (_changeTracker.ContainsKey(vertexId))
				{
					_changeTracker[vertexId].Vertex = vertex;
					_changeTracker[vertexId].State = VertexState.Modified;
				}
				else
				{
					TVertex originalVertex = vertex; // TODO: Set this to the quired valued
					_changeTracker.Add(vertexId, new TrackedVertex<TVertex>(vertex, originalVertex));
				}
		}

		public async Task<List<TVertex>> ExecuteQueryAsync(string gremlinQuery, GraphFacade graphFacade)
		{

			if (graphFacade is null) throw new ArgumentNullException(nameof(graphFacade));

			List<TVertex> reutrnValue = new List<TVertex>();

			TraversalResultset queryResultset = await graphFacade.QueryAsync(gremlinQuery).ConfigureAwait(true);


			return reutrnValue;
		}

		/// <summary>
		/// Marks the specified vertex for deletion when executing the <see cref="GraphContext.SaveChange()"/> method.
		/// </summary>
		/// <param name="vertex">The vertex to be marked for deletion</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="vertex"/> is null.</exception>
		/// <remarks>If the <paramref name="vertex"/> identifier is not set, then this method will ignore the vertex.</remarks>
		public void Delete(TVertex vertex)
		{
			if (vertex is null) throw new ArgumentNullException(nameof(vertex));
			string vertexId = GetVertexId(vertex);
			if (!string.IsNullOrWhiteSpace(vertexId))
				if (_changeTracker.ContainsKey(vertexId))
					_changeTracker[vertexId].State = VertexState.Deleted;
				else
					_changeTracker.Add(vertexId, new TrackedVertex<TVertex>(vertex, VertexState.Deleted));
		}

		/// <summary>
		/// Gets the state of the <paramref name="vertex"/> within the change tracker.
		/// </summary>
		/// <param name="vertex">The vertex to be interrogated.</param>
		/// <returns>The <see cref="VertexState"/> of <paramref name="vertex"/>.</returns>
		public VertexState GetVertexState(TVertex vertex)
		{
			string vertexId = GetVertexId(vertex);
			if (!string.IsNullOrWhiteSpace(vertexId) && _changeTracker.ContainsKey(vertexId))
				return _changeTracker[vertexId].State;
			else
				return VertexState.Detached;
		}

		public Type GetVertexType()
		{
			return typeof(TVertex);
		}

		internal void AddFromQuery(TVertex vertex, VertexState vertexState)
		{
			string vertexId = GetVertexId(vertex);
			if (_changeTracker.ContainsKey(vertexId))
			{
				_changeTracker[vertexId].Vertex = vertex;
				_changeTracker[vertexId].State = vertexState;
			}
			else
			{
				_changeTracker.Add(vertexId, new TrackedVertex<TVertex>(vertex, vertexState));
			}
		}



		/// <summary>
		/// Gets the identifier of the passed in <paramref name="vertex"/>.
		/// </summary>
		/// <param name="vertex">The vertex to be interrogated.</param>
		/// <returns>A <c>string</c> representing the identifier of the <paramref name="vertex"/>.</returns>
		/// <remarks>If there is no identifier set, then the identifier will be set to a new GUID.</remarks>
		private static string GetVertexId(ref TVertex vertex)
		{
			string returnValue = GetVertexId(vertex);
			if (string.IsNullOrWhiteSpace(returnValue))
			{
				returnValue = Guid.NewGuid().ToString();
				((Vertex)((object)(vertex))).Id = returnValue;
			}
			return returnValue;
		}

		/// <summary>
		/// Gets the identifier of the passed in <paramref name="vertex"/>.
		/// </summary>
		/// <param name="vertex">The vertex to be interrogated.</param>
		/// <returns>A <c>string</c> representing the identifier of the <paramref name="vertex"/>.</returns>
		private static string GetVertexId(TVertex vertex)
		{
			return ((Vertex)((object)(vertex))).Id; ;
		}


		void IGraphSet.AddFromQuery(object vertex, VertexState vertexState)
		{
			AddFromQuery((TVertex)vertex, vertexState);
		}
	}

}