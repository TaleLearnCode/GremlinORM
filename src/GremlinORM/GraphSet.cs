using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TaleLearnCode.GremlinORM.Attributes;
using TaleLearnCode.GremlinORM.Exceptions;
using TaleLearnCode.GremlinORM.Interfaces;

namespace TaleLearnCode.GremlinORM
{

	public class GraphSet<TVertex> : IGraphSet
		where TVertex : class, new()
	{




		private Dictionary<string, TrackedVertex<TVertex>> ChangeTracker { get; } = new Dictionary<string, TrackedVertex<TVertex>>();

		/// <summary>
		/// Gets the vertex properties.
		/// </summary>
		/// <value>
		/// The vertex properties.
		/// </value>
		/// <remarks>Key = Label; Value = Property Name</remarks>
		private Dictionary<string, (string PropertyName, bool IsList, GraphPropertyAttribute GraphPropertyAttribute)> VertexProperties { get; } = new Dictionary<string, (string PropertyName, bool IsList, GraphPropertyAttribute GraphPropertyAttribute)>();

		public string Label { get; }

		public VertexAttribute VertexAttribute { get; }

		public GraphSet()
		{

			if (!typeof(TVertex).IsSubclassOf(typeof(Vertex)))
				throw new Exception(ResourceStrings.TrackedVertexMustInheritFromVertex(typeof(TVertex)));

			Attribute rawVertexAttribute = typeof(TVertex).GetCustomAttribute(typeof(VertexAttribute));
			if (rawVertexAttribute is null) throw new Exception();
			else VertexAttribute = (VertexAttribute)rawVertexAttribute;

			foreach (PropertyInfo propertyInfo in typeof(TVertex).GetProperties())
			{
				GraphPropertyAttribute graphPropertyAttribute;
				Attribute rawGraphPropertyAttribute = propertyInfo.GetCustomAttribute(typeof(GraphPropertyAttribute), true);
				if (rawGraphPropertyAttribute != null)
				{
					graphPropertyAttribute = (GraphPropertyAttribute)rawGraphPropertyAttribute;
					VertexProperties.Add(graphPropertyAttribute.Key, (propertyInfo.Name, propertyInfo.PropertyType.IsInstanceOfType(typeof(IList)), graphPropertyAttribute));
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

		public void Update(TVertex vertex)
		{

			if (vertex is null) throw new ArgumentNullException(nameof(vertex));

			string vertexId = GetVertexId(vertex);
			if (string.IsNullOrWhiteSpace(vertexId)) throw new VertexMustHaveIdentifierException();

			if (ChangeTracker.ContainsKey(vertexId)) throw new VertexNotInChangeTrackerException();

			if (VertexHasChanged(ChangeTracker[vertexId].Vertex, vertex))
			{
				ChangeTracker[vertexId].Vertex = vertex;
				ChangeTracker[vertexId].State = VertexState.Modified;
			}

		}

		public async Task<List<TVertex>> ExecuteQueryAsync(string gremlinQuery, GraphFacade graphFacade)
		{

			if (graphFacade is null) throw new ArgumentNullException(nameof(graphFacade));

			List<TVertex> reutrnValue = new List<TVertex>();

			TraversalResultset queryResultset = await graphFacade.QueryAsync(gremlinQuery).ConfigureAwait(true);


			return reutrnValue;
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
			string vertexId = GetVertexId(vertex);
			if (!string.IsNullOrWhiteSpace(vertexId) && ChangeTracker.ContainsKey(vertexId))
				return ChangeTracker[vertexId].State;
			else
				return VertexState.Detached;
		}

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

		object IGraphSet.AddFromQuery(QueryResult queryResult)
		{
			if (queryResult.GremlinObjectType == GremlinObjectType.Vertex)
			{
				TVertex vertex = new TVertex();

				SetPropertyValue(ref vertex, "Id", queryResult.Id);
				SetPropertyValue(ref vertex, "Label", queryResult.Label);

				foreach (KeyValuePair<string, List<string>> property in queryResult.Properties)
				{
					PropertyInfo propertyInfo = typeof(TVertex).GetProperty(VertexProperties[property.Key].PropertyName);




					//var propertyValue = Activator.CreateInstance(propertyInfo.PropertyType);
					//if (VertexProperties[property.Key].IsList)
					//	foreach (string returnedValue in property.Value)
					//		((IList)propertyValue).Add(CastPropertyValue(propertyInfo, returnedValue));
					//else
					//	propertyValue = CastPropertyValue(propertyInfo, property.Value[0]);
					//propertyInfo.SetValue(vertex, propertyValue);


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

		private static bool VertexHasChanged(TVertex originalVertex, TVertex newVertex)
		{
			foreach (PropertyInfo propertyInfo in originalVertex.GetType().GetProperties())
			{
				if (propertyInfo.GetValue(originalVertex) != propertyInfo.GetValue(newVertex))
					return true;
			}
			return false;
		}

		private static object CastPropertyValue(PropertyInfo destinationPropertyInfo, string propertyValue)
		{
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

		private static void SetPropertyValue(ref TVertex vertex, string propertyName, string propertyValue)
		{
			PropertyInfo propertyInfo = typeof(TVertex).GetProperty(propertyName);
			propertyInfo.SetValue(vertex, CastPropertyValue(propertyInfo, propertyValue));
		}

	}

}