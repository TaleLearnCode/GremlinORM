using System;
using System.Collections.Generic;

namespace TaleLearnCode.GremlinORM
{

	public abstract class GraphSet<TVertex> where TVertex : class
	{

		Dictionary<string, TrackedVertex<TVertex>> _changeTracker = new Dictionary<string, TrackedVertex<TVertex>>();

		public GraphSet()
		{
			if (!typeof(TVertex).IsSubclassOf(typeof(Vertex)))
				throw new Exception(ResourceStrings.TrackedVertexMustInheritFromVertex(typeof(TVertex)));
		}

		public void Add(TVertex vertex)
		{
			if (vertex is null) throw new ArgumentNullException(nameof(vertex));
			_changeTracker.Add(GetVertexId(vertex), new TrackedVertex<TVertex>(vertex, VertexState.Added));
		}

		public void Update(TVertex vertex)
		{
			if (vertex is null) throw new ArgumentNullException(nameof(vertex));
			string vertexId = GetVertexId(vertex);
			if (_changeTracker.ContainsKey(vertexId))
			{
				_changeTracker[vertexId].Vertex = vertex;
				_changeTracker[vertexId].State = VertexState.Modified;
			}
		}

		public void Delete(TVertex vertex)
		{
			if (vertex is null) throw new ArgumentNullException(nameof(vertex));
			string vertexId = GetVertexId(vertex);
			if (_changeTracker.ContainsKey(vertexId))
				_changeTracker[vertexId].State = VertexState.Deleted;
			else
				_changeTracker.Add(vertexId, new TrackedVertex<TVertex>(vertex, VertexState.Added));
		}

		private static string GetVertexId(TVertex vertex)
		{
			return ((Vertex)((object)(vertex))).Id;
		}


	}

}