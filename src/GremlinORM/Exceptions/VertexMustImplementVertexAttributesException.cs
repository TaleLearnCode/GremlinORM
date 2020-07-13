using System;

namespace TaleLearnCode.GremlinORM.Exceptions
{

	public class VertexMustImplementVertexAttributesException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexMustImplementVertexAttributesException"/> class.
		/// </summary>
		public VertexMustImplementVertexAttributesException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexMustImplementVertexAttributesException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public VertexMustImplementVertexAttributesException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexMustImplementVertexAttributesException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public VertexMustImplementVertexAttributesException(string message, Exception innerException) : base(message, innerException) { }


		/// <summary>
		/// Initializes a new instance of the <see cref="VertexMustImplementVertexAttributesException"/> class.
		/// </summary>
		/// <param name="type">The type that is incorrectly trying to be tracked.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Known that contractor is only called from one place.")]
		public VertexMustImplementVertexAttributesException(Type type) : base(ResourceStrings.TrackedVertexMustInheritFromVertex(type)) { }

	}

}