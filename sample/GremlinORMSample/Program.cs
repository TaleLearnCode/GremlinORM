using GremlinORMSample.Model;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TaleLearnCode.GremlinORM;

namespace GremlinORMSample
{

	// TODO: Add filter for SchemaDefinition


	class Program
	{

		static async Task Main()
		{
			//await PerformQueryCosmos();


			//var room = new Room() { Id = "corina" };
			//var x = new GraphContext();
			//x.Add(room);

			MyContext myContext = new MyContext();
			Room room = new Room() { Name = "Corina" };
			myContext.Rooms.Add(room);

			Console.WriteLine("Done");
			Console.ReadLine();



		}

		private static async Task PerformQuery()
		{

			var graphFacade = new GraphFacade(Settings.Endpoint, Settings.AuthKey, Settings.Database, Settings.Graph);

			string query = "g.V().hasLabel('room').has('eventId', 10)";
			TraversalResultset queryResultset = await graphFacade.QueryAsync(query);


			//Console.WriteLine($"Status Code: {queryResultset.StatusCode}");
			//Console.WriteLine($"Request Charge: {queryResultset.RequestCharge}");
			//Console.WriteLine($"Server Time: {queryResultset.ServerTime}");
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

		private static async Task PerformQueryCosmos()
		{

			var graphFacade = new TaleLearnCode.GremlinORM.Cosmos.GraphFacade(Settings.Endpoint, Settings.AuthKey, Settings.Database, Settings.Graph);

			string query = "g.V().hasLabel('room').has('eventId', 10)";
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


	}

	public class MyContext : GraphContext
	{
		public MyContext() : base(Settings.Endpoint, Settings.AuthKey, Settings.Database, Settings.Graph) { }

		public GraphSet<Room> Rooms { get; set; } = new GraphSet<Room>("room");

	}

	public class TestMe
	{
		MyContext _context = new MyContext();

		public void TestMy()
		{
			Console.WriteLine(_context.Rooms.GetType());
		}

	}

}