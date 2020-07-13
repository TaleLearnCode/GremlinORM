using System;

namespace TaleLearnCode.GremlinORM.Exceptions
{

	/// <summary>
	/// Exception thrown when initializing a <see cref="GraphSet{TVertex}"/> with a type that does not inherit from <see cref="Vertex"/>.
	/// </summary>
	/// <seealso cref="Exception" />
	public class MustInheritFromVertexException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="MustInheritFromVertexException"/> class.
		/// </summary>
		public MustInheritFromVertexException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexAlreadyInChangeTrackerException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public MustInheritFromVertexException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="MustInheritFromVertexException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public MustInheritFromVertexException(string message, Exception innerException) : base(message, innerException) { }


		/// <summary>
		/// Initializes a new instance of the <see cref="MustInheritFromVertexException"/> class.
		/// </summary>
		/// <param name="type">The type that is incorrectly trying to be tracked.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Known that contractor is only called from one place.")]
		public MustInheritFromVertexException(Type type) : base(ResourceStrings.TrackedVertexMustInheritFromVertex(type)) { }

	}

}