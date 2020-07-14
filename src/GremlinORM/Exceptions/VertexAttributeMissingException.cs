using System;
using TaleLearnCode.GremlinORM.Attributes;

namespace TaleLearnCode.GremlinORM.Exceptions
{

	/// <summary>
	/// Exception thrown when trying to modify an vertex in the change tracker but it does not exist in the tracker.
	/// </summary>
	/// <seealso cref="Exception" />
	public class VertexAttributeMissingException : Exception
	{

		/// <summary>
		/// Gets the type of the vertex missing the <see cref="VertexAttribute"/>.
		/// </summary>
		/// <value>
		/// A <see cref="Type"/> representing the vertex type.
		/// </value>
		public Type VertexType { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexAttributeMissingException"/> class.
		/// </summary>
		private VertexAttributeMissingException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexAttributeMissingException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public VertexAttributeMissingException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexAttributeMissingException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public VertexAttributeMissingException(string message, Exception innerException) : base(message, innerException) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexAttributeMissingException"/> class.
		/// </summary>
		/// <param name="vertexType">Type of the vertex.</param>
		public VertexAttributeMissingException(Type vertexType) : base(ResourceStrings.VertexAttributeMissingException(vertexType))
		{
			VertexType = vertexType;
		}

	}

}