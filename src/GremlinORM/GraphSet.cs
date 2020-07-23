using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using TaleLearnCode.GremlinORM.Attributes;
using TaleLearnCode.GremlinORM.Exceptions;
using TaleLearnCode.GremlinORM.Interfaces;

namespace TaleLearnCode.GremlinORM
{

	/// <summary>
	/// A representation of the change tracker for a graph object.
	/// </summary>
	/// <typeparam name="TVertex">The type of the vertex to tracked.</typeparam>
	/// <seealso cref="IGraphSet" />
	public class GraphSet<TVertex> : IGraphSet
		where TVertex : class, new()
	{

		private Dictionary<string, TrackedVertex<TVertex>> ChangeTracker { get; } = new Dictionary<string, TrackedVertex<TVertex>>();

		private Dictionary<string, (string PropertyName, bool IsList, GraphPropertyAttribute GraphPropertyAttribute)> VertexProperties { get; } = new Dictionary<string, (string PropertyName, bool IsList, GraphPropertyAttribute GraphPropertyAttribute)>();

		/// <summary>
		/// Gets the label of the vertex in the database.
		/// </summary>
		/// <value>
		/// A <c>string</c> representing the vertex label.
		/// </value>
		public string Label { get; }

		/// <summary>
		/// Gets the <see cref="VertexAttribute" /> added to the vertex type definition.
		/// </summary>
		/// <value>
		/// The <see cref="VertexAttribute" /> associated with the vertex type definition.
		/// </value>
		public VertexAttribute VertexAttribute { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphSet{TVertex}"/> class.
		/// </summary>
		/// <exception cref="TaleLearnCode.GremlinORM.Exceptions.MustInheritFromVertexException">Thrown if <typeparamref name="TVertex"/> does not inherit from <see cref="Vertex"/>.</exception>
		/// <exception cref="TaleLearnCode.GremlinORM.Exceptions.VertexMustImplementVertexAttributesException">Thrown if <typeparamref name="TVertex"/> does not implement the <see cref="VertexAttribute"/>.</exception>
		public GraphSet()
		{

			if (!typeof(TVertex).IsSubclassOf(typeof(Vertex)))
				throw new MustInheritFromVertexException(typeof(TVertex));

			Attribute vertexAttribute = typeof(TVertex).GetCustomAttribute(typeof(VertexAttribute));
			if (vertexAttribute is null) throw new VertexMustImplementVertexAttributesException(typeof(TVertex));
			else VertexAttribute = (VertexAttribute)vertexAttribute;

			foreach (PropertyInfo propertyInfo in typeof(TVertex).GetProperties())
			{
				GraphPropertyAttribute graphPropertyAttribute;
				Attribute rawGraphPropertyAttribute = propertyInfo.GetCustomAttribute(typeof(GraphPropertyAttribute), true);
				if (rawGraphPropertyAttribute != null)
				{
					graphPropertyAttribute = (GraphPropertyAttribute)rawGraphPropertyAttribute;
					if (graphPropertyAttribute.IncludeInGraph)
						if (IsTypeValidForGremlin(propertyInfo.PropertyType))
							VertexProperties.Add(graphPropertyAttribute.Key, (propertyInfo.Name, propertyInfo.PropertyType.IsInstanceOfType(typeof(IList)), graphPropertyAttribute));
						else
							throw new VertexPropertyNotValidTypeForGremlinException(propertyInfo);
				}
			}

		}

		/// <summary>
		/// Adds the <paramref name="vertex"/> to the change tracker to be saved upon calling <see cref="GraphContext.SaveChanges()"/>.
		/// </summary>
		/// <param name="vertex">The vertex to be added to the change tracker.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="vertex"/> is null.</exception>
		/// <exception cref="VertexAlreadyInChangeTrackerException">Thrown if the <paramref name="vertex"/> already exists in the change tracker.</exception>"
		public void Add(TVertex vertex)
		{
			if (vertex is null) throw new ArgumentNullException(nameof(vertex));
			string vertexId = GetVertexId(ref vertex);
			if (ChangeTracker.ContainsKey(vertexId))
				throw new VertexAlreadyInChangeTrackerException();
			else
				ChangeTracker.Add(vertexId, new TrackedVertex<TVertex>(vertex, VertexState.Added));
		}

		/// <summary>
		/// Updates the specified vertex within the change tracker.
		/// </summary>
		/// <param name="vertex">The vertex to be updated.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="vertex"/> is null.</exception>
		/// <exception cref="VertexMustHaveIdentifierException">Thrown if <paramref name="vertex"/> does not have a identifier value.</exception>
		/// <remarks>
		/// If the vertex does not already exist in the change tracker (i.e., it has not been queried for), it will be added and the <see cref="VertexState.Modified"/> flag will be used.
		/// </remarks>
		public void Update(TVertex vertex)
		{

			if (vertex is null) throw new ArgumentNullException(nameof(vertex));

			string vertexId = GetVertexId(vertex);
			if (string.IsNullOrWhiteSpace(vertexId)) throw new VertexMustHaveIdentifierException();

			if ((ChangeTracker.ContainsKey(vertexId)) && (VertexHasChanged(ChangeTracker[vertexId].Vertex, vertex)))
			{
				ChangeTracker[vertexId].Vertex = vertex;
				ChangeTracker[vertexId].State = VertexState.Modified;
			}

		}

		/// <summary>
		/// Marks the specified vertex for deletion when executing the <see cref="GraphContext.SaveChange()"/> method.
		/// </summary>
		/// <param name="vertex">The vertex to be marked for deletion</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="vertex"/> is null.</exception>
		/// <remarks>If the <paramref name="vertex"/> identifier is not set, then this method will ignore the vertex.</remarks>
		public void Delete(TVertex vertex)
		{
			if (vertex is null) throw new ArgumentNullException(nameof(vertex));
			string vertexId = GetVertexId(vertex);
			if (!string.IsNullOrWhiteSpace(vertexId))
				if (ChangeTracker.ContainsKey(vertexId))
					ChangeTracker[vertexId].State = VertexState.Deleted;
				else
					ChangeTracker.Add(vertexId, new TrackedVertex<TVertex>(vertex, VertexState.Deleted));
		}

		/// <summary>
		/// Gets the state of the <paramref name="vertex"/> within the change tracker.
		/// </summary>
		/// <param name="vertex">The vertex to be interrogated.</param>
		/// <returns>The <see cref="VertexState"/> of <paramref name="vertex"/>.</returns>
		public VertexState GetVertexState(TVertex vertex)
		{
			if (vertex is null) throw new ArgumentNullException(nameof(vertex));
			string vertexId = GetVertexId(vertex);
			if (!string.IsNullOrWhiteSpace(vertexId) && ChangeTracker.ContainsKey(vertexId))
				return ChangeTracker[vertexId].State;
			else
				return VertexState.Detached;
		}

		/// <summary>
		/// Gets the <see cref="Type"/> of the vertex being tracked.
		/// </summary>
		/// <returns>
		/// A <see cref="Type" /> representing the type of vertex being tracked.
		/// </returns>
		public Type GetVertexType()
		{
			return typeof(TVertex);
		}

		/// <summary>
		/// Gets the identifier of the passed in <paramref name="vertex"/>.
		/// </summary>
		/// <param name="vertex">The vertex to be interrogated.</param>
		/// <returns>A <c>string</c> representing the identifier of the <paramref name="vertex"/>.</returns>
		/// <remarks>If there is no identifier set, then the identifier will be set to a new GUID.</remarks>
		private static string GetVertexId(ref TVertex vertex)
		{
			string returnValue = GetVertexId(vertex);
			if (string.IsNullOrWhiteSpace(returnValue))
			{
				returnValue = Guid.NewGuid().ToString();
				((Vertex)((object)(vertex))).Id = returnValue;
			}
			return returnValue;
		}

		/// <summary>
		/// Gets the identifier of the passed in <paramref name="vertex"/>.
		/// </summary>
		/// <param name="vertex">The vertex to be interrogated.</param>
		/// <returns>A <c>string</c> representing the identifier of the <paramref name="vertex"/>.</returns>
		private static string GetVertexId(TVertex vertex)
		{
			return ((Vertex)((object)(vertex))).Id; ;
		}

		/// <summary>
		/// Add an appropriately typed vertex to the change tracker from a traversal ran in <see cref="GraphContext" />.
		/// </summary>
		/// <param name="queryResult">A <see cref="QueryResult" /> representing an individual result from a database traversal.</param>
		/// <returns>
		/// The serialized version of the <paramref name="queryResult" />.
		/// </returns>
		object IGraphSet.AddFromQuery(QueryResult queryResult)
		{

			if (queryResult.GremlinObjectType == GremlinObjectType.Vertex)
			{
				TVertex vertex = new TVertex();

				SetPropertyValue(ref vertex, nameof(Vertex.Id), queryResult.Id);
				SetPropertyValue(ref vertex, nameof(Vertex.Label), queryResult.Label);

				foreach (KeyValuePair<string, List<string>> property in queryResult.Properties)
				{

					PropertyInfo propertyInfo = typeof(TVertex).GetProperty(VertexProperties[property.Key].PropertyName);

					if (VertexProperties[property.Key].IsList)
						propertyInfo.SetValue(vertex, CastPropertyValuesToList(propertyInfo, property.Value));
					else
						propertyInfo.SetValue(vertex, CastPropertyValue(propertyInfo, property.Value[0]));

				}
				ChangeTracker[queryResult.Id] = new TrackedVertex<TVertex>(vertex, VertexState.Unchanged);

				return vertex;

			}

			return default;

		}

		List<string> IGraphSet.GetSaveChangesQueries()
		{
			List<string> saveChangesQueries = new List<string>();

			foreach (TrackedVertex<TVertex> trackedVertex in ChangeTracker.Values)
			{
				switch (trackedVertex.State)
				{
					case VertexState.Added:
						StringBuilder addGremlin = new StringBuilder($"g.addV('{VertexAttribute.Label}')");
						foreach (KeyValuePair<string, (string PropertyName, bool IsList, GraphPropertyAttribute GraphPropertyAttribute)> vertexProperty in VertexProperties)
						{
							if (vertexProperty.Value.GraphPropertyAttribute.IncludeInGraph)
							{
								PropertyInfo propertyInfo = trackedVertex.Vertex.GetType().GetProperty(vertexProperty.Value.PropertyName);
								if (vertexProperty.Value.IsList)
									foreach (var listItem in propertyInfo.GetValue(trackedVertex.Vertex) as List<object>)
										addGremlin.Append(GetPropertyValueForGremlinQuery(propertyInfo, vertexProperty.Value, listItem));
								else
									addGremlin.Append(GetPropertyValueForGremlinQuery(propertyInfo, vertexProperty.Value, propertyInfo.GetValue(trackedVertex.Vertex)));
							}
						}
						saveChangesQueries.Add(addGremlin.ToString());
						break;
					case VertexState.Modified:
						StringBuilder updateGremlin = new StringBuilder($"g.V('{GetVertexId(trackedVertex.Vertex)}')");
						foreach (KeyValuePair<string, (string PropertyName, bool IsList, GraphPropertyAttribute GraphPropertyAttribute)> vertexProperty in VertexProperties)
							if (vertexProperty.Value.GraphPropertyAttribute.IncludeInGraph)
							{
								PropertyInfo propertyInfo = trackedVertex.Vertex.GetType().GetProperty(vertexProperty.Value.PropertyName);
								if (propertyInfo.GetValue(trackedVertex.Vertex) != propertyInfo.GetValue(trackedVertex.OriginalVertex))
									if (vertexProperty.Value.IsList)
										foreach (var listItem in propertyInfo.GetValue(trackedVertex.Vertex) as List<object>)
											updateGremlin.Append(GetPropertyValueForGremlinQuery(propertyInfo, vertexProperty.Value, listItem));
									else
										updateGremlin.Append(GetPropertyValueForGremlinQuery(propertyInfo, vertexProperty.Value, propertyInfo.GetValue(trackedVertex.Vertex)));
							}
						saveChangesQueries.Add(updateGremlin.ToString());
						break;
					case VertexState.Deleted:
						string vertexId = GetVertexId(trackedVertex.Vertex);
						if (!string.IsNullOrWhiteSpace(vertexId))
							saveChangesQueries.Add($"g.V('{vertexId}').drop()");
						break;
				}
			}


			return saveChangesQueries;
		}

		/// <summary>
		/// Determines if the vertex has changed.
		/// </summary>
		/// <param name="originalVertex">The original vertex value.</param>
		/// <param name="newVertex">The changed vertex value.</param>
		/// <returns><c>true</c> if any of the properties are different between <paramref name="originalVertex"/> and <paramref name="newVertex"/>; otherwise, <c>false</c>.</returns>
		private static bool VertexHasChanged(TVertex originalVertex, TVertex newVertex)
		{
			foreach (PropertyInfo propertyInfo in originalVertex.GetType().GetProperties())
			{
				if (propertyInfo.GetValue(originalVertex) != propertyInfo.GetValue(newVertex))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Casts the value of <paramref name="destinationPropertyInfo"/> to the appropriate type.
		/// </summary>
		/// <param name="destinationPropertyInfo">The destination property information.</param>
		/// <param name="propertyValue">The property value.</param>
		/// <returns>An <c>object</c> casted appropriate and set to the value of the property</returns>
		private static object CastPropertyValue(PropertyInfo destinationPropertyInfo, string propertyValue)
		{

			// TODO: Look to see if Gremlin native supports other types

			if (destinationPropertyInfo.PropertyType == typeof(bool))
				return bool.Parse(propertyValue);
			else if (destinationPropertyInfo.PropertyType == typeof(byte))
				return byte.Parse(propertyValue, CultureInfo.InvariantCulture);
			else if (destinationPropertyInfo.PropertyType == typeof(double))
				return double.Parse(propertyValue, CultureInfo.InvariantCulture);
			else if (destinationPropertyInfo.PropertyType == typeof(float))
				return float.Parse(propertyValue, CultureInfo.InvariantCulture);
			else if (destinationPropertyInfo.PropertyType == typeof(int))
				return int.Parse(propertyValue, CultureInfo.InvariantCulture);
			else if (destinationPropertyInfo.PropertyType == typeof(long))
				return long.Parse(propertyValue, CultureInfo.InvariantCulture);
			else if (destinationPropertyInfo.PropertyType == typeof(DateTime))
				return DateTime.Parse(propertyValue, CultureInfo.InvariantCulture);
			else
				return propertyValue;
		}

		/// <summary>
		/// Casts the value of <paramref name="destinationPropertyInfo"/> to a generic list of the appropriate type.
		/// </summary>
		/// <param name="destinationPropertyInfo">The destination property information.</param>
		/// <param name="propertyValues">The property values.</param>
		/// <returns>A <see cref="List{object}"/> representing the casted property value.</returns>
		private static List<object> CastPropertyValuesToList(PropertyInfo destinationPropertyInfo, List<string> propertyValues)
		{

			Type destinationListType = destinationPropertyInfo.PropertyType.GetType().GetGenericArguments().Single();

			List<object> returnValue = new List<object>();
			if (destinationListType == typeof(bool))
				foreach (string propertyValue in propertyValues)
					returnValue.Add(bool.Parse(propertyValue));
			else if (destinationListType == typeof(byte))
				foreach (string propertyValue in propertyValues)
					returnValue.Add(byte.Parse(propertyValue, CultureInfo.InvariantCulture));
			else if (destinationListType == typeof(double))
				foreach (string propertyValue in propertyValues)
					returnValue.Add(double.Parse(propertyValue, CultureInfo.InvariantCulture));
			else if (destinationListType == typeof(float))
				foreach (string propertyValue in propertyValues)
					returnValue.Add(float.Parse(propertyValue, CultureInfo.InvariantCulture));
			else if (destinationListType == typeof(int))
				foreach (string propertyValue in propertyValues)
					returnValue.Add(int.Parse(propertyValue, CultureInfo.InvariantCulture));
			else if (destinationListType == typeof(long))
				foreach (string propertyValue in propertyValues)
					returnValue.Add(long.Parse(propertyValue, CultureInfo.InvariantCulture));
			else if (destinationListType == typeof(DateTime))
				foreach (string propertyValue in propertyValues)
					returnValue.Add(DateTime.Parse(propertyValue, CultureInfo.InvariantCulture));
			else
				foreach (string propertyValue in propertyValues)
					returnValue.Add(propertyValue);

			return returnValue;

		}

		/// <summary>
		/// Sets the property value.
		/// </summary>
		/// <param name="vertex">The vertex where to set the value.</param>
		/// <param name="propertyName">Name of the property to be set.</param>
		/// <param name="propertyValue">The property value to set.</param>
		private static void SetPropertyValue(ref TVertex vertex, string propertyName, string propertyValue)
		{
			PropertyInfo propertyInfo = typeof(TVertex).GetProperty(propertyName);
			propertyInfo.SetValue(vertex, CastPropertyValue(propertyInfo, propertyValue));
		}

		/// <summary>
		/// Determines whether is <paramref name="type"/> is valid for a Gremlin database.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> to be evaluated.</param>
		/// <returns>
		///   <c>true</c> if <paramref name="type"/> is valid for a Gremlin database; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsTypeValidForGremlin(Type type)
		{
			// TODO: Look at a better of figuring out what is a valid type
			if (type.IsPrimitive)
				return true;
			else if (type == typeof(string))
				return true;
			else if (type == typeof(DateTime))
				return true;
			else if (type == typeof(Uri))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Converts an numeric object to its string representation/  A return value
		/// indicates whether the conversion succeeded or failed.
		/// </summary>
		/// <param name="testValue">The numeric value to convert.</param>
		/// <param name="stringValue">The resulting string representation of the number.</param>
		/// <returns><c>true</c> if <paramref name="testValue"/> was converted successfully; otherwise, false.</returns>
		private static bool TryParseNumber(object testValue, out string stringValue)
		{

			bool returnValue = false;
			stringValue = default;

			bool isFloat = float.TryParse(testValue.ToString(), out var floatNumber);
			if (isFloat)
			{
				bool isLong = long.TryParse(testValue.ToString(), out var longNumber);
				if (isLong)

				{
					returnValue = true;
					stringValue = longNumber.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					returnValue = true;
					stringValue = floatNumber.ToString(CultureInfo.InvariantCulture);
				}
			}

			bool isDecimal = decimal.TryParse(testValue.ToString(), out var decimalNumber);
			if (isDecimal)
			{
				returnValue = true;
				stringValue = decimalNumber.ToString(CultureInfo.InvariantCulture);
			}

			return returnValue;

		}

		/// <summary>
		/// Gets the property value for a gremlin query.
		/// </summary>
		/// <param name="propertyInfo">Detail of the property being investigated.</param>
		/// <param name="rawPropertyValue">The value of the vertex property.</param>
		/// <returns>A <c>string</c> representing the property value portion of a .Property gremlin statement.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Booleans within Gremlin are required to be lowered cased.")]
		private static string GetPropertyValueForGremlinQuery(PropertyInfo propertyInfo, (string PropertyName, bool IsList, GraphPropertyAttribute GraphPropertyAttribute) propertyAttributes, object rawPropertyValue)
		{

			// TODO: Add IsRequired validation

			string propertyValue = string.Empty;

			if (rawPropertyValue != default && !string.IsNullOrWhiteSpace(rawPropertyValue.ToString()))
			{
				if (propertyInfo.PropertyType == typeof(bool))
					propertyValue = $".property('{propertyAttributes.PropertyName}', {rawPropertyValue.ToString().ToLower(CultureInfo.InvariantCulture)})";
				else if (propertyInfo.PropertyType == typeof(string))
					propertyValue = $".property('{propertyAttributes.PropertyName}', '{rawPropertyValue}')";
				else
					if (TryParseNumber(rawPropertyValue, out string stringValue))
					propertyValue = $".property('{propertyAttributes.PropertyName}', {stringValue})";
				else
					propertyValue = $".property('{propertyAttributes.PropertyName}', '{rawPropertyValue}')";
			}


			return propertyValue;

		}

	}

}