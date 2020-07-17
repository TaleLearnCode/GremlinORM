using System;
using System.Reflection;

namespace TaleLearnCode.GremlinORM.Exceptions
{
	/// <summary>
	/// Exception thrown when trying to modify an vertex in the change tracker but it does not exist in the tracker.
	/// </summary>
	/// <seealso cref="Exception" />
	public class VertexPropertyNotValidTypeForGremlinException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexPropertyNotValidTypeForGremlinException"/> class.
		/// </summary>
		public VertexPropertyNotValidTypeForGremlinException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexPropertyNotValidTypeForGremlinException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public VertexPropertyNotValidTypeForGremlinException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexPropertyNotValidTypeForGremlinException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public VertexPropertyNotValidTypeForGremlinException(string message, Exception innerException) : base(message, innerException) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexPropertyNotValidTypeForGremlinException"/> class.
		/// </summary>
		/// <param name="propertyInfo">Information on the property that has an incompatible type for Gremlin database.</param>
		public VertexPropertyNotValidTypeForGremlinException(PropertyInfo propertyInfo) : base(ResourceStrings.VertexPropertyNotValidTypeForGremlinException(propertyInfo)) { }

	}
}
