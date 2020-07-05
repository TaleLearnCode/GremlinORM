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

	class Program
	{

		static async Task Main(string[] args)
		{
			var graphFacade = new GraphFacade(Settings.Endpoint, Settings.AuthKey, Settings.Database, Settings.Graph);

			string query = "g.V().hasLabel('room').has('eventId', 10)";
			//string query = "g.V().hasLabel('room').has('eventId', 'schemaDefinition')";
			var results = await graphFacade.QueryAsync(query);

			if (results.Any())
			{
				foreach (var result in results)
				{
					foreach (var r in result)
					{
						if (r.Key == "properties")
						{
							foreach (var v in r.Value)
							{

								var options = new JsonSerializerOptions
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