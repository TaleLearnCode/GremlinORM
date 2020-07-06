using TaleLearnCode.GremlinORM;
using TaleLearnCode.GremlinORM.Attributes;

namespace GremlinORMSample.Entities
{

	/// <summary>
	/// Represents a "room" where a session is presented.
	/// </summary>
	/// <seealso cref="SortedLookupType" />
	[Vertex("room", Description = "Represents a \"room\" where a session is presented.")]
	public class Room : Vertex
	{

		[GraphProperty("sessionizeId")]
		public string sessionizeId { get; set; }

		[GraphProperty("eventId")]
		public string eventId { get; set; }

		[GraphProperty("name")]
		public string name { get; set; }

		[GraphProperty("sortOrder")]
		public int sortOrder { get; set; }

		public Room() : base("room") { }

	}

}