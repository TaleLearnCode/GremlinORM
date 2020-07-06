using GremlinORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace GremlinORMSample
{

	// TODO: Add filter for SchemaDefinition


	class Program
	{

		static async Task Main()
		{
			await DeserializeQueryAsync();
		}

		private static async Task PrintOutQueryAsync()
		{
			var graphFacade = new GraphFacade(Settings.Endpoint, Settings.AuthKey, Settings.Database, Settings.Graph);

			string query = "g.V().hasLabel('room').has('eventId', 10)";
			//string query = "g.V().hasLabel('room').has('eventId', 'schemaDefinition')";
			var results = await graphFacade.QueryAsync(query);

			if (results.Any())
			{
				foreach (dynamic result in results)
				{
					foreach (dynamic r in result)
					{
						if (r.Key == "properties")
						{
							foreach (dynamic v in r.Value)
							{

								JsonSerializerOptions options = new JsonSerializerOptions
								{
									Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
								};

								string[] x = SplitCsv3(JsonSerializer.Serialize(v.Value, options));
								if (x.Length > 0)
									for (int i = 1; i < x.Length; i++)
									{
										if (i % 2 != 0)
										{

											//string propertyValue = (x[i].EndsWith("}]")) ? x[i].Substring(6, x[i].Length - 8) : x[i].Substring(6, x[i].Length - 7);
											string propertyValue = (x[i].EndsWith("}]")) ? x[i][6..^2] : x[i][6..^1];

											PrintKeyValue(v.Key, propertyValue);

										}
									}

							}
						}
						else
						{
							PrintKeyValue(r.Key, r.Value);
						}
					}
					Console.WriteLine();
				}
			}
		}

		private static async Task DeserializeQueryAsync()
		{

			List<QueryResult> queryResults = new List<QueryResult>();

			var graphFacade = new GraphFacade(Settings.Endpoint, Settings.AuthKey, Settings.Database, Settings.Graph);

			string query = "g.V().hasLabel('room').has('eventId', 10)";
			var results = await graphFacade.QueryAsync(query);

			if (results.Any())
			{
				foreach (dynamic result in results)
				{

					QueryResult queryResult = new QueryResult();

					foreach (dynamic r in result)
					{
						if (r.Key == "properties")
						{
							foreach (dynamic v in r.Value)
							{

								JsonSerializerOptions options = new JsonSerializerOptions
								{
									Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
								};

								string[] x = SplitCsv3(JsonSerializer.Serialize(v.Value, options));
								if (x.Length > 0)
									for (int i = 1; i < x.Length; i++)
									{
										if (i % 2 != 0)
										{

											//string propertyValue = (x[i].EndsWith("}]")) ? x[i].Substring(6, x[i].Length - 8) : x[i].Substring(6, x[i].Length - 7);
											string propertyValue = (x[i].EndsWith("}]")) ? x[i][6..^2] : x[i][6..^1];

											queryResult.Properties.Add(v.Key, new List<string> { propertyValue });

										}
									}

							}
						}
						else if (r.Key == "id")
						{
							queryResult.Id = r.Value;
						}
						else if (r.Key == "type")
						{
							switch (r.Value)
							{
								case "vertex":
									queryResult.GremlinType = GremlinType.Vertex;
									break;
								case "edge":
									queryResult.GremlinType = GremlinType.Edge;
									break;
							}
						}
						else if (r.Key == "label")
						{
							queryResult.Label = r.Value;
						}
					}

					queryResults.Add(queryResult);
				}
			}

			if (queryResults.Any())
			{
				foreach (var queryResult in queryResults)
				{
					Console.WriteLine($"{queryResult.GremlinType}\t{queryResult.Label}\t{queryResult.Id}\t{queryResult.Properties.Count}");
				}
			}
		}


		private static void PrintKeyValue(string key, string value)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(key.PadRight(15));
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(value);
			Console.WriteLine();
		}

		// TODO: Make this an extension method
		public static string[] SplitCsv3(string line)
		{
			List<string> result = new List<string>();
			StringBuilder currentStr = new StringBuilder("");
			bool inQuotes = false;
			for (int i = 0; i < line.Length; i++) // For each character
			{
				if (line[i] == '\"') // Quotes are closing or opening
					inQuotes = !inQuotes;
				else if (line[i] == ',') // Comma
				{
					if (!inQuotes) // If not in quotes, end of current string, add it to result
					{
						result.Add(currentStr.ToString());
						currentStr.Clear();
					}
					else
						currentStr.Append(line[i]); // If in quotes, just add it 
				}
				else // Add any other character to current string
					currentStr.Append(line[i]);
			}
			result.Add(currentStr.ToString().Replace("\\u0022", "\""));
			return result.ToArray(); // Return array of all strings
		}
	}

}