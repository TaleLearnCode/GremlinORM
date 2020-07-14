namespace TaleLearnCode.GremlinORM.Attributes
{

	/// <summary>
	/// Represents the property naming policy to use when storing properties.
	/// </summary>
	public enum PropertyNamingPolicy
	{
		/// <summary>
		/// Keep the case of the property name the same as source.
		/// </summary>
		SourceCase = 0,
		/// <summary>
		/// Use camel casing when saving the property value.
		/// </summary>
		CamelCase = 1,
		/// <summary>
		/// Use pascal casing when saving the property value.
		/// </summary>
		PascalCase = 2
	}

}