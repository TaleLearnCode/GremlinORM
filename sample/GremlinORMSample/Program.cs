using GremlinORMSample.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleLearnCode.GremlinORM;

namespace GremlinORMSample
{

	class Program
	{

		static async Task Main()
		{

			//string query = "g.V().hasLabel('room').has('eventId', 10)";
			string query = "g.V().has('eventId', 10)";

			int mainOption = 3;
			switch (mainOption)
			{
				case 1:
					await PerformQueryCosmos(query);
					break;
				case 2:
					await PerformContextQuery(query);
					break;
				case 3:
					await CRUD();
					break;
			}

		}

		private static async Task PerformQueryCosmos(string query)
		{

			var graphFacade = new TaleLearnCode.GremlinORM.Cosmos.GraphFacade(Settings.Endpoint, Settings.AuthKey, Settings.Database, Settings.Graph);

			TaleLearnCode.GremlinORM.Cosmos.TraversalResultset queryResultset = await graphFacade.QueryAsync(query);


			Console.WriteLine($"Status Code: {queryResultset.StatusCode}");
			Console.WriteLine($"Request Charge: {queryResultset.RequestCharge}");
			Console.WriteLine($"Server Time: {queryResultset.ServerTime}");
			Console.WriteLine();

			if (queryResultset.Count > 0)
			{
				foreach (var queryResult in queryResultset.Results)
				{
					Console.WriteLine($"{queryResult.GremlinObjectType}\t{queryResult.Label}\t{queryResult.Id}\t{queryResult.Properties.Count}");
				}
			}

			Console.WriteLine("Done");

		}

		private static async Task PerformContextQuery(string query)
		{
			MyContext myContext = new MyContext();
			Dictionary<Type, List<object>> queryResults = await myContext.ExecuteQueryAsync(query);

			foreach (var tag in queryResults[typeof(Tag)])
				Console.WriteLine(((Tag)tag).Name);

			Console.WriteLine("Done");
			Console.ReadLine();
		}

		private static async Task CRUD()
		{

			MyContext myContext = new MyContext();
			Tag newTag = new Tag() { EventId = "11", Name = "TaleLearnCode" };
			myContext.Tags.Add(newTag);
			await myContext.SaveChangesAsync();

			//Dictionary<Type, List<object>> queryResults = await myContext.ExecuteQueryAsync("g.V('dd4f438f-351c-47a8-9e9d-adf005848e5c')");
			//foreach (var tag in queryResults[typeof(Tag)])
			////Console.WriteLine(((Tag)tag).Name);
			//{
			//	((Tag)tag).Name = "TLC";
			//	myContext.Tags.Update((Tag)tag);
			//}




			Console.WriteLine("Done");
			Console.ReadLine();
		}

	}

	public class MyContext : GraphContext
	{
		public MyContext() : base(Settings.Endpoint, Settings.AuthKey, Settings.Database, Settings.Graph) { }

		public GraphSet<Room> Rooms { get; set; } = new GraphSet<Room>();
		public GraphSet<Tag> Tags { get; set; } = new GraphSet<Tag>();

	}

}