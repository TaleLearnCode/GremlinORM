using System;

namespace TaleLearnCode.GremlinORM.Attributes
{

	/// <summary>
	/// Attributes for defining a graph property.
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Property)]
	public class GraphPropertyAttribute : Attribute
	{

		/// <summary>
		/// Gets or sets the name of the key for the property.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the property key name.
		/// </value>
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets the description of the property.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the description of the property.
		/// </value>
		/// <remarks>This value is used when creating schema documentation.</remarks>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the property value is required within the graph.
		/// </summary>
		/// <value>
		///   <c>true</c> if the value of the property is required within the graph; otherwise, <c>false</c>.
		/// </value>
		public bool IsRequired { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether property is included in the graph document.
		/// </summary>
		/// <value>
		///   <c>true</c> if property is included in the graph document; otherwise, <c>false</c>.
		/// </value>
		public bool IncludeInGraph { get; set; }

		// TODO: Allow retrieval of the key via the property name

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphPropertyAttribute"/> class.
		/// </summary>
		/// <param name="key">Name of the key for the property.</param>
		public GraphPropertyAttribute(string key)
		{
			Key = key;
			IsRequired = false;
			IncludeInGraph = true;
		}

	}

}