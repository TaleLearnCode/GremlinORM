using GremlinORMSample.Model;
using TaleLearnCode.GremlinORM;

namespace GremlinORMSample
{

	class EventContext : GraphContext
	{

		private EventContext() : base(Settings.Endpoint, Settings.AuthKey, Settings.Database, Settings.Graph) { }

		public GraphSet<Room> Rooms { get; set; }

		public void Test()
		{

			Rooms.Add(new Room());

		}

	}

}