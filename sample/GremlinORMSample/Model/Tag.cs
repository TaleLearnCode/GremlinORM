using TaleLearnCode.GremlinORM;
using TaleLearnCode.GremlinORM.Attributes;

namespace GremlinORMSample.Model
{

	/// <summary>
	/// Represents a tag for sessions and such.
	/// </summary>
	/// <seealso cref="Vertex" />
	[Vertex("tag", Description = "A tag for sessions and such.")]
	public class Tag : Vertex
	{

		/// <summary>
		/// Gets or sets the identifier of the event.  Serves as the partition key.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the event identifier.
		/// </value>
		[GraphProperty("eventId", Description = "Identifier of the event the vertex is associated with. Serves as the partition key.")]
		public int EventId { get; set; }

		/// <summary>
		/// Gets or sets the name of the vertex object.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the vertex object name.
		/// </value>
		[GraphProperty("name", Description = "The name of the vertex object.")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Sessionize identifier for the Tag.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the identifier of the Tag within Sessionize.
		/// </value>
		[GraphProperty("sessionizeId", Description = "The Sessionize identifier for the Tag.")]
		public string SessionizeId { get; set; }

		public Tag() : base("tag") { }

	}

}