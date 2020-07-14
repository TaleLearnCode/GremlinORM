using System;
using System.Linq;
using System.Reflection;

namespace TaleLearnCode.GremlinORM
{

	/// <summary>
	/// Provides access to change tracking information and operations for vertex instances the context is tracking.
	/// </summary>
	/// <typeparam name="TVertex">The type of the vertex being tracked.</typeparam>
	internal class TrackedVertex<TVertex>
	{

		private VertexState _vertexState;
		private TVertex _vertex;
		private TVertex _originalVertex;

		private static readonly int _maxVertexState = Enum.GetValues(typeof(VertexState)).Cast<int>().Max();

		/// <summary>
		/// Gets or sets the state of the vertex being tracked.
		/// </summary>
		/// <value>
		/// A <see cref="VertexState"/> representing the state of the vertex being tracked.
		/// </value>
		/// <exception cref="ArgumentException">Thrown if set <see cref="VertexState"/> is not a valid enumeration value.</exception>
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

		/// <summary>
		/// Gets or sets the vertex being tracked.
		/// </summary>
		/// <value>
		/// The vertex being tracked.
		/// </value>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="TrackedVertex{TVertex}"/> class.
		/// </summary>
		/// <param name="vertex">The vertex to be tracked.</param>
		/// <param name="vertexState">State of the vertex being tracked.</param>
		internal TrackedVertex(TVertex vertex, VertexState vertexState)
		{
			_originalVertex = vertex;
			_vertex = vertex;
			_vertexState = vertexState;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TrackedVertex{TVertex}"/> class.
		/// </summary>
		/// <param name="originalVertex">The original vertex value.</param>
		/// <param name="currentVertex">The current vertex value.</param>
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