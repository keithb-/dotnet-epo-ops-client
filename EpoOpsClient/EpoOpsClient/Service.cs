/*
Copyright(c) 2018 by Keith Bielaczyc. All Right Reserved.

Unless required by applicable law or agreed to in writing,
software distributed under the License is distributed on an
"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, either express or implied.See the License for the
specific language governing permissions and limitations
under the License.
*/
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EpoOpsClient
{

	public class QuotaHeaders
	{
		public const string IndividualQuotaPerHourUsedHeader = "X-IndividualQuotaPerHour-Used";
		public const string RegisteredQuotaPerWeekUsedHeader = "X-RegisteredQuotaPerWeek-Used";
		public const string RegisteredPayingQuotaPerWeekUsedHeader = "X-RegisteredPayingQuotaPerWeek-Used";
		public const string RejectionReasonHeader = "X-Rejection-Reason";
	}

	public class SelfThrottlingHeaders
	{
		public const string ThrottlingControlHeader = "X-Throttling-Control";
		public const string RetryAfterHeader = "RetryAfter";
	}

	public class Service
	{
		// Configuration settings
		public const string AuthEndPointAddressKey = "authEndPointAddress";
		public const string ApplicationServiceEndPointAddressKey = "applicationServiceEndPointAddress";
		public const string PatentServiceEndPointAddressKey = "patentServiceEndPointAddress";
		public const string ServiceConsumerKeyKey = "serviceConsumerKey";
		public const string ServiceConsumerSecretKey = "serviceConsumerSecret";
		public const string UserAgentNameKey = "userAgentName";
		public const string ApplicationServiceUrlKey = "applicationServiceEndPointAddress";
		public const string PatentServiceUrlKey = "patentServiceEndPointAddress";
		public const int DefaultRequestWaitTime = 300;

		public string AuthEndPointAddress { get; set; }
		public string ApplicationServiceEndPointAddress { get; set; }
		public string PatentServiceEndPointAddress { get; set; }
		public string ServiceConsumerKey { get; set; }
		public string ServiceConsumerSecret { get; set; }
		public string UserAgentName { get; set; }
		public int RequestWaitTime { get; set; }
		public string ApplicationServiceUrl { get; set; }
		public string PatentServiceUrl { get; set; }
		// Access tokens expire in roughly 20 minutes, so allow the caller to provide
		// a method that can encapsulate the logic to re-authenticate when the token expires.
		public Func<string> GetAccessToken;

		private ILog _log;

		public Service(ILog log, NameValueCollection config, Func<string> getAccessToken)
		{
			_log = log;

			// TODO: Add some argument validation.

			GetAccessToken = getAccessToken;

			var item = config[AuthEndPointAddressKey];
			if (item != null) AuthEndPointAddress = item;
			_log.Debug($"{AuthEndPointAddressKey}: {item}");

			item = config[ApplicationServiceEndPointAddressKey];
			if (item != null) ApplicationServiceEndPointAddress = item;
			_log.Debug($"{ApplicationServiceEndPointAddressKey}: {item}");

			item = config[PatentServiceEndPointAddressKey];
			if (item != null) PatentServiceEndPointAddress = item;
			_log.Debug($"{PatentServiceEndPointAddressKey}: {item}");

			item = config[ServiceConsumerKeyKey];
			if (item != null) ServiceConsumerKey = item;
			_log.Debug($"{ServiceConsumerKeyKey}: {item}");

			item = config[ServiceConsumerSecretKey];
			if (item != null) ServiceConsumerSecret = item;
			_log.Debug($"{ServiceConsumerSecretKey}: {item}");

			item = config[UserAgentNameKey];
			if (item != null) UserAgentName = item;
			_log.Debug($"{UserAgentNameKey}: {item}");

			item = config[ApplicationServiceUrlKey];
			if (item != null) ApplicationServiceUrl = item;
			_log.Debug($"{ApplicationServiceUrlKey}: {item}");

			item = config[PatentServiceUrlKey];
			if (item != null) PatentServiceUrl = item;
			_log.Debug($"{PatentServiceUrlKey}: {item}");
		}

		public AccessToken CreateAccessToken()
		{
			var credentials = ServiceConsumerKey + ":" + ServiceConsumerSecret;
			var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

			var client = WebRequest.Create(AuthEndPointAddress) as HttpWebRequest;
			//TODO: Make a meaningful exception.
			if (client == null) throw new Exception();
			client.Method = "POST";
			client.UserAgent = UserAgentName;
			client.Headers.Add("Authorization", $"Basic {encodedCredentials}");
			client.ContentType = "application/x-www-form-urlencoded";
			var encoding = new ASCIIEncoding();
			var data = encoding.GetBytes("grant_type=client_credentials");
			client.ContentLength = data.Length;
			using (var writer = client.GetRequestStream())
			{
				writer.Write(data, 0, data.Length);
				writer.Close();
			}
			var response = client.GetResponse() as HttpWebResponse;
			//TODO: Make a meaningful exception.
			if (response == null) throw new Exception();
			var stream = response.GetResponseStream();
			if (stream == null) throw new Exception();
			var result = new AccessToken();
			using (var reader = new StreamReader(stream))
			{
				var js = new JavaScriptSerializer();
				var obj = js.Deserialize<Dictionary<string, object>>(reader.ReadToEnd());
				result.IssuedAt = DateTime.UtcNow;
				result.ExpiresIn = Int32.Parse(obj["expires_in"].ToString());
				result.Value = (string)obj["access_token"];
				_log.Info($"{response.StatusCode} Access token: {result.Value} Expires At: {result.ExpiresAt.ToString("U")}");
				reader.Close();
			}
			return result;
		}

		private async Task<ResponseStream> Get(string url)
		{
			if (string.IsNullOrEmpty(url)) return null;

			var client = WebRequest.Create(url) as HttpWebRequest;
			if (client == null) return null;
			client.Headers.Add("Authorization", $"Bearer {GetAccessToken.Invoke()}");
			HttpWebResponse response = null;
			try
			{
				response = await client.GetResponseAsync() as HttpWebResponse;
			}
			catch (WebException webex)
			{
				response = webex.Response as HttpWebResponse;
			}
			return new ResponseStream(response);
		}

		public async Task<ResponseStream> GetPatent(string patentNumber)
		{
			string url = null;
			if (!string.IsNullOrEmpty(patentNumber))
			{
				url = string.Format(PatentServiceUrl, patentNumber);
			}
			if (string.IsNullOrEmpty(url)) return null;

			return await Get(url);
		}

		public async Task<ResponseStream> GetApplication(string applicationNumber)
		{
			string url = null;
			if (!string.IsNullOrEmpty(applicationNumber))
			{
				url = string.Format(ApplicationServiceUrl, applicationNumber);
			}
			if (string.IsNullOrEmpty(url)) return null;

			return await Get(url);
		}
	}
}
