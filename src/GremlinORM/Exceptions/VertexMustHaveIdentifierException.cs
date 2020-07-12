using System;

namespace TaleLearnCode.GremlinORM.Exceptions
{

	/// <summary>
	/// Exception thrown when trying to modify an vertex in the change tracker but it does not exist in the tracker.
	/// </summary>
	/// <seealso cref="Exception" />
	public class VertexMustHaveIdentifierException : Exception
	{

		// TODO: Chane the exception to be helpful

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexMustHaveIdentifierException"/> class.
		/// </summary>
		public VertexMustHaveIdentifierException() : base(ResourceStrings.VertexMustHaveIdentifierException()) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexMustHaveIdentifierException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public VertexMustHaveIdentifierException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexMustHaveIdentifierException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public VertexMustHaveIdentifierException(string message, Exception innerException) : base(message, innerException) { }

	}

}