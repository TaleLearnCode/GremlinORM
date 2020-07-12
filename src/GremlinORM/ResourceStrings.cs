using System;
using TaleLearnCode.GremlinORM.Attributes;

namespace TaleLearnCode.GremlinORM
{

	internal static class ResourceStrings
	{

		// TODO: Update logic to use resource manager

		internal static string InvalidEnumValue(object argumentName, object enumType)
		{
			return $"The value provided for argument '{argumentName}' must be a valid value of enum type '{enumType}'.";
		}

		internal static string TrackedVertexMustInheritFromVertex(Type type)
		{
			return $"{type.Name} does not inherit from {typeof(Vertex).Name}. Tracked vertices must inherit from {typeof(Vertex).Name}.";
		}

		internal static string VertexNotInChagneTrackerException() => "Vertex is not in the change tracker; unable to continue.";

		internal static string VertexMustHaveIdentifierException() => "Vertex must have an identifier in order to evaluate it within the change tracker; unable to continue.";

		internal static string VertexAttributeMissingException(Type type)
		{
			return $"{type.Name} must use the {typeof(VertexAttribute).Name} in order to use this type with the Gremlin ORM.";
		}

	}

}