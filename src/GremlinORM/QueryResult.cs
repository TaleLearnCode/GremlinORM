using System;
using System.Collections.Generic;

namespace TaleLearnCode.GremlinORM
{

	/// <summary>
	/// Represents an individual result from a Gremlin query.
	/// </summary>
	public class QueryResult
	{

		/// <summary>
		/// Gets or sets the label of the object being returned.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the returned object's label.
		/// </value>
		public string Label { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the object being returned.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the returned object's identifier.
		/// </value>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the Gremlin type of the returned object.
		/// </summary>
		/// <value>
		/// A <see cref="GremlinObjectType"/> representing the returned object's type.
		/// </value>
		public GremlinObjectType GremlinObjectType { get; set; }

		/// <summary>
		/// Gets the property values of the returned object.
		/// </summary>
		/// <value>
		/// A <see cref="Dictionary{string, List{String}}"/> representing the returned object's type.
		/// </value>
		/// <remarks>The key is the label of the of the property and the value is the list of values for the property.</remarks>
		public Dictionary<string, List<string>> Properties { get; } = new Dictionary<string, List<string>>();

	}

}