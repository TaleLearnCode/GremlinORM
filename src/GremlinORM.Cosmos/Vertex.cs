namespace TaleLearnCode.GremlinORM.Cosmos
{
	/// <summary>
	/// Abstract class to be used by vertex types within a Cosmos DB Gremlin database.
	/// </summary>
	/// <seealso cref="TaleLearnCode.GremlinORM.Vertex" />
	public abstract class Vertex : GremlinORM.Vertex
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="Vertex"/> class.
		/// </summary>
		/// <param name="label">The label of the vertex.</param>
		protected Vertex(string label) : base(label) { }

		// TODO: Add PartitionKey for the Cosmos extension

	}
}