using System;
using System.Globalization;
using System.Resources;
using TaleLearnCode.GremlinORM.Attributes;

namespace TaleLearnCode.GremlinORM
{

	/// <summary>
	/// String resources used in the Gremlin ORM exceptions, et.c
	/// </summary>
	internal static class ResourceStrings
	{

		private static readonly ResourceManager _resourceManager = new ResourceManager(typeof(GremlinORMResources));

		/// <summary>
		/// Gets the string from the resource manager.
		/// </summary>
		/// <param name="name">The name resource string to be retrieved.</param>
		/// <param name="formatterNames">The formatter names.</param>
		/// <returns></returns>
		private static string GetString(string name, params string[] formatterNames)
		{
			var value = _resourceManager.GetString(name, CultureInfo.CurrentCulture);
			for (var i = 0; i <= formatterNames.Length; i++)
				value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
			return value;
		}

		/// <summary>
		/// Message for exception thrown when supplied an invalid enumeration value.
		/// </summary>
		/// <param name="argumentName">Name of the argument containing the invalid enumeration value.</param>
		/// <param name="enumType">Type of the enumeration within an invalid value.</param>
		/// <returns>The value provided for argument '{argumentName}' must be a valid value of enum type '{enumType}'.</returns>
		internal static string InvalidEnumValue(string argumentName, Type enumType)
			=> string.Format(CultureInfo.CurrentCulture,
				GetString(nameof(InvalidEnumValue), nameof(argumentName), nameof(enumType)), argumentName, enumType.Name);

		/// <summary>
		/// Message for exception thrown when the tracked vertex does not inherit from <see cref="Vertex"/>.
		/// </summary>
		/// <param name="type">The type of the vertex that does not inherit from <see cref="Vertex"/>.</param>
		/// <returns>{type} does not inherit from {vertexType}. Tracked vertices must inherit from {vertexType}.</returns>
		internal static string TrackedVertexMustInheritFromVertex(Type type)
			=> string.Format(CultureInfo.CurrentCulture,
						GetString(nameof(TrackedVertexMustInheritFromVertex),
							nameof(type), "vertexType"),
						type.Name, typeof(Vertex).Name);

		/// <summary>
		/// Message for exception thrown when the tracked vertex does not implement the <see cref="VertexAttribute"/> attribute.
		/// </summary>
		/// <param name="type">The type of the vertex that does not implement the <see cref="VertexAttribute"/> attribute.</param>
		/// <returns>{type} does not implement the {VertexAttribute} which is required in order for the vertex to be tracked.</returns>
		internal static string VertexMustImplementVertexAttributes(Type type)
			=> string.Format(CultureInfo.CurrentCulture,
						GetString(nameof(VertexMustImplementVertexAttributes),
							nameof(type), "VertexAttribute"),
						type.Name, typeof(VertexAttribute));

		/// <summary>
		/// Message for exception thrown when a vertex needs to be in the change tracker but its not.
		/// </summary>
		/// <returns>Vertex is not in the change tracker; unable to continue.</returns>
		internal static string VertexNotInChangeTrackerException()
			=> string.Format(CultureInfo.CurrentCulture,
						GetString(nameof(VertexNotInChangeTrackerException)));

		/// <summary>
		/// Message for exception thrown when a vertex does not have an identifier but is required by the operation.
		/// </summary>
		/// <returns>Vertex must have an identifier in order to evaluate it within the change tracker; unable to continue.</returns>
		internal static string VertexMustHaveIdentifierException()
			=> string.Format(CultureInfo.InvariantCulture,
						GetString(nameof(VertexMustHaveIdentifierException)));

		/// <summary>
		/// Message for exception thrown when the <see cref="VertexAttribute"/> is missing from a vertex.
		/// </summary>
		/// <param name="type">The type of the vertex missing the <see cref="VertexAttribute"/> attribute.</param>
		/// <returns>{type} must use the {VertexAttribute} in order to use this type with the Gremlin ORM.</returns>
		internal static string VertexAttributeMissingException(Type type)
			=> string.Format(CultureInfo.CurrentCulture,
						GetString(nameof(VertexAttributeMissingException),
							nameof(type), "VertexAttribute"),
						type.Name, typeof(VertexAttribute));

	}

}