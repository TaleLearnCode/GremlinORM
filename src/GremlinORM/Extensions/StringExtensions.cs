using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleLearnCode.GremlinORM.Extensions
{

	/// <summary>
	/// Provides a set of extension methods for the <c>string</c> type.
	/// </summary>
	public static class StringExtensions
	{

		/// <summary>
		/// Converts a string to the corresponding <see cref="GremlinObjectType"/>.
		/// </summary>
		/// <param name="input">The input to be converted.</param>
		/// <returns>The corresponding <see cref="GremlinObjectType"/> matching the <paramref name="input"/>.</returns>
		/// <remarks>If the string is null, empty, or consists only of whitespace characters, <see cref="GremlinObjectType.Undefined"/> will be returned.</remarks>
		public static GremlinObjectType ToGremlinObjectType(this string input)
		{
			if (string.IsNullOrWhiteSpace(input)) return GremlinObjectType.Undefined;
			return (input.ToUpperInvariant()) switch
			{
				"VERTEX" => GremlinObjectType.Vertex,
				"EDGE" => GremlinObjectType.Edge,
				_ => GremlinObjectType.Undefined,
			};
		}

		/// <summary>
		/// Splits a CSV formatted string into substrings that are based on the characters in an array.
		/// </summary>
		/// <param name="input">The input to be split.</param>
		/// <returns>An array whose elements contain the substrings from this instance.</returns>
		public static string[] SplitCSV(this string input)
		{
			List<string> result = new List<string>();
			if (!string.IsNullOrWhiteSpace(input))
			{
				StringBuilder currentStr = new StringBuilder("");
				bool inQuotes = false;
				for (int i = 0; i < input.Length; i++)
				{
					if (input[i] == '\"')
						inQuotes = !inQuotes;
					else if (input[i] == ',')
					{
						if (!inQuotes)
						{
							result.Add(currentStr.ToString());
							currentStr.Clear();
						}
						else
							currentStr.Append(input[i]);
					}
					else
						currentStr.Append(input[i]);
				}
				//result.Add(currentStr.ToString().Replace("\\u0022", "\""));
				result.Add(Regex.Unescape(currentStr.ToString()));

			}
			return result.ToArray();
		}

	}

}