namespace TaleLearnCode.GremlinORM
{

	/// <summary>
	/// Represents the different type of objects within a Gremlin graph.
	/// </summary>
	public enum GremlinObjectType
	{
		/// <summary>Represents an undefined Gremlin type.</summary>
		/// <remarks>Generally an error will be thrown if this is the case.</remarks>
		Undefined = 0,
		/// <summary>Represents a Gremlin vertex.</summary>
		Vertex = 1,
		/// <summary>Represents a Gremlin edge.</summary>
		Edge = 2
	}
}