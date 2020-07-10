using Gremlin.Net.Driver;
using System;

namespace TaleLearnCode.GremlinORM.Cosmos
{

	/// <summary>
	/// Represents the parsed resultset from a Gremlin traversal.
	/// </summary>
	/// <remarks>
	/// This version of the type adds many of the specific status attributes
	/// returned by Cosmos DB.  See https://docs.microsoft.com/en-us/azure/cosmos-db/gremlin-headers#status-codes
	/// for more information about the Cosmos DB status attributes.
	/// </remarks>
	/// <seealso cref="GremlinORM.TraversalResultset" />
	public class TraversalResultset : GremlinORM.TraversalResultset
	{

		/// <summary>
		/// Gets the status code for the traversal.
		/// </summary>
		/// <value>
		/// A <c>long</c> representing the traversal status code.
		/// </value>
		/// <remarks>
		/// <para>The value comes from the 'x-ms-status-code' status attribute.</para>
		/// </remarks>
		public long StatusCode { get; }

		/// <summary>
		/// Gets the amount of collection or database throughput consumed
		/// in request units (RU/s or RUs) for the entire request.
		/// </summary>
		/// <value>
		/// A <c>double</c> representing the RUs for the entire request.
		/// </value>
		/// <remarks>
		/// <para>The value comes from the 'x-ms-total-request-charge' status attribute.</para>
		/// </remarks>
		public double RequestCharge { get; }

		/// <summary>
		/// Gets the total time, in milliseconds, that the Cosmos DB
		/// Gremlin server took to execute the entire traversal.
		/// </summary>
		/// <value>
		/// A <c>double</c> representing the total milliseconds it took to execute the traversal.
		/// </value>
		public double ServerTime { get; }

		/// <summary>
		/// Gets the sub-status code returned by Cosmos when a failure occurs.
		/// </summary>
		/// <value>
		/// A <c>long</c> representing the sub-status code returned by Cosmos when a failure occurs.
		/// </value>
		/// <remarks>
		/// <para>
		/// This value contains additional insights about the failure reason when failure
		/// occurs within lower layers of the high availability stack.  Applications are
		/// advised to store this value and use it when contacting Cosmos DB customer support.
		/// This value is useful for Cosmos DB engineer for quick troubleshooting.
		/// </para>
		/// <para>The value comes from the 'x-ms-substatus-code' status attribute.</para>
		/// </remarks>
		public long? SubstatusCode { get; }

		/// <summary>
		/// Gets the amount of time to await before retrying the traversal.
		/// </summary>
		/// <value>
		/// A <c>TimeSpan</c> representing the amount of time to await before retrying the traversal.
		/// </value>
		/// <remarks>
		/// <para>
		/// /// This value will only be included in requests failed due to provisioned
		/// throughput exhaustion.  Applications should resubmit the traversal again
		/// after the instructed period of time.
		/// </para>
		/// <para>This value comes from the 'x-ms-retry-after-ms' status attribute.</para>
		/// </remarks>
		public TimeSpan? RetryTimespan { get; }

		/// <summary>
		/// Gets the assigned unique identifier by Cosmos DB for the traversal.
		/// </summary>
		/// <value>
		/// A <see cref="Guid"/> representing the unique identifier assigned by Cosmos DB for the traversal.
		/// </value>
		/// <remarks>
		/// <para>
		/// Each request is assigned a unique identifier by Cosmos DB for tracking
		/// purposes. Applications should log activity identifiers returned by the
		/// server for requests that customers may want to contact customer support
		/// about.  Cosmos DB support personal can find specific requests by these
		/// identifiers in Cosmos DB service telemetry.
		/// </para>
		/// <para>This value comes from the 'x-ms-activity-id' status attribute.</para>
		/// </remarks>
		public Guid ActivityId { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TraversalResultset"/> class.
		/// </summary>
		/// <param name="gremlinResultSet">The result set returned from the submission of a Gremlin script to the server and presents the results provided by the server.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="gremlinResultSet"/>> is null.</exception>
		internal TraversalResultset(ResultSet<dynamic> gremlinResultSet) : base(gremlinResultSet)
		{
			StatusCode = Convert.ToInt64(gremlinResultSet.StatusAttributes["x-ms-status-code"]);
			RequestCharge = Convert.ToDouble(gremlinResultSet.StatusAttributes["x-ms-total-request-charge"]);
			ServerTime = Convert.ToDouble(gremlinResultSet.StatusAttributes["x-ms-total-server-time-ms"]);
			if (gremlinResultSet.StatusAttributes.ContainsKey("x-ms-substatus-code")) SubstatusCode = (long?)(gremlinResultSet.StatusAttributes["x-ms-substatus-code"]);
			if (gremlinResultSet.StatusAttributes.ContainsKey("x-ms-retry-after-ms")) RetryTimespan = TimeSpan.Parse(gremlinResultSet.StatusAttributes["x-ms-retry-after-ms"].ToString());
			ActivityId = Guid.Parse((string)gremlinResultSet.StatusAttributes["x-ms-activity-id"]);
		}

	}

}