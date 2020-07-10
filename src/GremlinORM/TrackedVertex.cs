using System;
using System.Linq;
using System.Reflection;

namespace TaleLearnCode.GremlinORM
{

	internal class TrackedVertex<TVertex>
	{

		private VertexState _vertexState;
		private TVertex _vertex;
		private TVertex _originalVertex;

		private static readonly int _maxVertexState = Enum.GetValues(typeof(VertexState)).Cast<int>().Max();

		internal VertexState State
		{
			get => _vertexState;
			set
			{
				if (value < 0 || (int)value > _maxVertexState)
					throw new ArgumentException(ResourceStrings.InvalidEnumValue(nameof(value), typeof(VertexState)));
				_vertexState = value;
			}
		}

		internal TVertex Vertex
		{
			get => _vertex;
			set
			{
				if (HasVertexChanged(value))
				{
					_vertex = value;
					_vertexState = VertexState.Modified;
				}
			}
		}

		internal TrackedVertex(TVertex vertex, VertexState vertexState)
		{
			_originalVertex = vertex;
			_vertex = vertex;
			_vertexState = vertexState;
		}

		internal TrackedVertex(TVertex originalVertex, TVertex currentVertex)
		{
			_originalVertex = originalVertex;
			_vertex = currentVertex;
			_vertexState = (HasVertexChanged(currentVertex)) ? VertexState.Modified : VertexState.Unchanged;
		}

		/// <summary>
		/// Determines whether the vertex has been changed based upon <paramref name="input"/>.
		/// </summary>
		/// <param name="input">The vertex to be evaluated..</param>
		/// <returns>
		///   <c>true</c> if the vertex has changed; otherwise, <c>false</c>.
		/// </returns>
		private bool HasVertexChanged(TVertex input)
		{
			bool hasVertexChange = false;
			foreach (PropertyInfo propertyInfo in _vertex.GetType().GetProperties())
			{
				if (propertyInfo.GetValue(_vertex) != propertyInfo.GetValue(input))
				{
					hasVertexChange = true;
					break;
				}
			}
			return hasVertexChange;
		}

	}

}