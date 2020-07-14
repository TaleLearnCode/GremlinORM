using System;

namespace TaleLearnCode.GremlinORM.Attributes
{
	/// <summary>
	/// Attribute values for a vertex (entity).
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Class)]
	public class VertexAttribute : Attribute
	{

		/// <summary>
		/// Gets or sets the label for the corresponding graph vertex.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the label for the corresponding graph vertex.
		/// </value>
		public string Label { get; set; }

		/// <summary>
		/// Gets or sets the description of the vertex.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the description of the vertex.
		/// </value>
		/// <remarks>This value is used when creating schema documentation.</remarks>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether delete operations should only perform a soft delete.
		/// </summary>
		/// <value>
		///   <c>true</c> if delete operations should only perform a soft delete of the vertex; otherwise, <c>false</c>.
		/// </value>
		public bool SoftDelete { get; set; }

		/// <summary>
		/// Gets or sets the property naming policy.
		/// </summary>
		/// <value>
		/// The property naming policy.
		/// </value>
		public PropertyNamingPolicy PropertyNamingPolicy { get; set; } = PropertyNamingPolicy.CamelCase;

		/// <summary>
		/// Initializes a new instance of the <see cref="VertexAttribute"/> class.
		/// </summary>
		/// <param name="label">The label for the corresponding vertex.</param>
		/// <param name="softDelete">Flag indicating whether delete operations should be soft.</param>
		public VertexAttribute(string label, bool softDelete = true)
		{
			Label = label;
			SoftDelete = softDelete;
		}

	}

}