using Gremlin.Net.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TaleLearnCode.GremlinORM.Extensions;

namespace TaleLearnCode.GremlinORM
{

	/// <summary>
	/// Represents the results from a query.
	/// </summary>
	public class TraversalResultset
	{

		/// <summary>
		/// Gets the results from the query.
		/// </summary>
		/// <value>
		/// A <see cref="List{QueryResult}"/> containing the returned Gremlin objects from the query.
		/// </value>
		public List<QueryResult> Results { get; } = new List<QueryResult>();

		/// <summary>
		/// Gets the number of results that were returned.
		/// </summary>
		/// <value>
		/// An <c>int</c> representing the number of returned results.
		/// </value>
		/// <remarks>This will be minutely faster than getting the count from <see cref="Results"/>.</remarks>
		public int Count
		{
			get
			{
				if (Results.Any())
					return Results.Count;
				else
					return 0;
			}
		}

		/// <summary>
		/// Gets the status attributes from the gremlin response.
		/// </summary>
		/// <value>
		/// A <see cref="IReadOnlyDictionary{string, object}"/> representing the gremlin response status attributes.
		/// </value>
		public IReadOnlyDictionary<string, object> StatusAttributes { get; } = new Dictionary<string, object>();

		/// <summary>
		/// Initializes a new instance of the <see cref="TraversalResultset"/> class.
		/// </summary>
		/// <param name="gremlinResultSet">The result set returned from the submission of a Gremlin script to the server and presents the results provided by the server.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="gremlinResultSet"/>> is null.</exception>
		internal protected TraversalResultset(ResultSet<dynamic> gremlinResultSet)
		{

			if (gremlinResultSet is null) throw new ArgumentNullException(nameof(gremlinResultSet));

			StatusAttributes = gremlinResultSet.StatusAttributes;

			if (gremlinResultSet.Any())
			{
				foreach (dynamic queryResults in gremlinResultSet)
				{

					QueryResult queryResult = new QueryResult();

					foreach (dynamic result in queryResults)
					{
						if (result.Key == "properties")
						{
							foreach (dynamic value in result.Value)
							{

								JsonSerializerOptions options = new JsonSerializerOptions
								{
									Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
								};

								string[] propertyValueArray = SplitCSV(JsonSerializer.Serialize(value.Value, options));

								if (propertyValueArray.Length > 0)
									for (int i = 1; i < propertyValueArray.Length; i++)
									{
										if (i % 2 != 0)
										{

											string propertyValue = propertyValueArray[i].EndsWith("}]", StringComparison.InvariantCultureIgnoreCase) ? propertyValueArray[i].Substring(6, propertyValueArray[i].Length - 8) : propertyValueArray[i].Substring(6, propertyValueArray[i].Length - 7);
											queryResult.Properties.Add(value.Key, new List<string> { propertyValue });

										}
									}

							}
						}
						else if (result.Key == "id")
						{
							queryResult.Id = result.Value;
						}
						else if (result.Key == "type")
						{
							queryResult.GremlinObjectType = ((string)result.Value).ToGremlinObjectType();
						}
						else if (result.Key == "label")
						{
							queryResult.Label = result.Value;
						}
					}

					Results.Add(queryResult);

				}
			}

		}

		private static string[] SplitCSV(string line)
		{
			return line.SplitCSV();
			// TODO: Make this an extension method
		}

	}

}