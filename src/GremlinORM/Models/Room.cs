using TaleLearnCode.GremlinORM.Attributes;

namespace TaleLearnCode.GremlinORM.Model
{

	/// <summary>
	/// Represents a "room" where a session is presented.
	/// </summary>
	/// <seealso cref="SortedLookupType" />
	[Vertex("room", Description = "Represents a \"room\" where a session is presented.")]
	public class Room : Vertex
	{

		[GraphProperty("sessionizeId")]
		public string SessionizeId { get; set; }

		[GraphProperty("eventId")]
		public string EventId { get; set; }

		[GraphProperty("name")]
		public string Name { get; set; }

		[GraphProperty("sortOrder")]
		public int SortOrder { get; set; }

		public Room() : base("room") { }

	}

}