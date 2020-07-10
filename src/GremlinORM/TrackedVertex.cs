using System;
using System.Linq;

namespace TaleLearnCode.GremlinORM
{

	internal class TrackedVertex<TVertex>
	{

		private VertexState _vertexState;
		private TVertex _vertex;

		private static readonly int _maxVertexState = Enum.GetValues(typeof(VertexState)).Cast<int>().Max();

		public VertexState State
		{
			get => _vertexState;
			set
			{
				if (value < 0 || (int)value > _maxVertexState)
					throw new ArgumentException(ResourceStrings.InvalidEnumValue(nameof(value), typeof(VertexState)));
				_vertexState = value;
			}
		}

		public TVertex Vertex
		{
			get => _vertex;
			set
			{
				_vertex = value;
				_vertexState = VertexState.Modified;
			}
		}

		internal TrackedVertex(TVertex vertex, VertexState vertexState)
		{
			_vertex = vertex;
			_vertexState = vertexState;
		}

	}

}