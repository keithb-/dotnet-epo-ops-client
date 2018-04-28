/*
Copyright(c) 2018 by Keith Bielaczyc. All Right Reserved.

Unless required by applicable law or agreed to in writing,
software distributed under the License is distributed on an
"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, either express or implied.See the License for the
specific language governing permissions and limitations
under the License.
*/
using System.IO;
using System.IO.Compression;
using System.Net;

namespace EpoOpsClient
{
	public class ResponseStream
	{
		public HttpStatusCode StatusCode { get; protected set; }
		public string StatusDescription { get; protected set; }
		public string ContentType { get; protected set; }
		public long ContentLength { get; protected set; }
		public string IndividualQuotaPerHourUsed { get; protected set; }
		public string RegisteredQuotaPerWeekUsed { get; protected set; }
		public string RegisteredPayingQuotaPerWeekUsed { get; protected set; }
		public string RejectionReason { get; protected set; }

		public string RetryAfter { get; protected set; }

		public ThrottlingControl[] ThrottlingControl { get; protected set; }

		public Stream Stream {get; protected set;}

		public ResponseStream(HttpWebResponse response)
		{
			if (response != null)
			{
				StatusCode = response.StatusCode;
				StatusDescription = response.StatusDescription;
				ContentType = response.ContentType;
				ContentLength = response.ContentLength;

				var value = response.Headers[QuotaHeaders.IndividualQuotaPerHourUsedHeader];
				if (value != null)
				{
					IndividualQuotaPerHourUsed = value;
				}
				value = response.Headers[QuotaHeaders.RegisteredQuotaPerWeekUsedHeader];
				if (value != null)
				{
					RegisteredQuotaPerWeekUsed = value;
				}
				value = response.Headers[QuotaHeaders.RegisteredPayingQuotaPerWeekUsedHeader];
				if (value != null)
				{
					RegisteredPayingQuotaPerWeekUsed = value;
				}

				var throttles = response.Headers.GetValues(SelfThrottlingHeaders.ThrottlingControlHeader);
				if (throttles != null)
				{
					RetryAfter = response.Headers[SelfThrottlingHeaders.RetryAfterHeader]; // TODO: Add default, e.g.  ?? "1000";
					ThrottlingControl = new ThrottlingControl[throttles.Length];
					for(var index = 0; index < throttles.Length; index++)
					{
						var start = throttles[index].IndexOf('(');
						ThrottlingControl[index] = new ThrottlingControl();
						ThrottlingControl[index].SystemState = throttles[index].Substring(0, start).TrimEnd();
						var length = throttles[index].Length;
						start++;
						var serviceDetails = throttles[index].Substring(start, length - start - 1).Split(new[] { ',' });
						ThrottlingControl[index].SystemDetails = new SystemDetail[serviceDetails.Length];
						for(var k = 0; k < serviceDetails.Length; k++)
						{
							length = serviceDetails[k].Length;
							start = serviceDetails[k].IndexOf("=");
							ThrottlingControl[index].SystemDetails[k] = new SystemDetail();
							ThrottlingControl[index].SystemDetails[k].ServiceName = serviceDetails[k].Substring(0, start).TrimStart();
							start++;
							var separator = serviceDetails[k].IndexOf(":", start);
							ThrottlingControl[index].SystemDetails[k].TrafficLightPosition = serviceDetails[k].Substring(start, separator - start);
							separator++;
							ThrottlingControl[index].SystemDetails[k].RequestLimit = serviceDetails[k].Substring(separator, length - separator);
						}
					}
				}
				Stream = GetResponseStream(response, 5 * 60);
			}
		}

		//http://stackoverflow.com/questions/839888/httpwebrequest-native-gzip-compression
		public static Stream GetResponseStream(HttpWebResponse response, int readTimeOut)
		{
			Stream stream;
			switch (response.ContentEncoding.ToUpperInvariant())
			{
				case "GZIP":
					stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress, false);
					break;
				case "DEFLATE":
					stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress, false);
					break;

				default:
					stream = response.GetResponseStream();
					stream.ReadTimeout = readTimeOut;
					break;
			}
			return stream;
		}

		//http://stackoverflow.com/questions/411592/how-do-i-save-a-stream-to-a-file
		public static void CopyStream(Stream input, Stream output)
		{
			var buffer = new byte[8 * 1024];
			int len;
			while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, len);
			}
		}
	}
}
