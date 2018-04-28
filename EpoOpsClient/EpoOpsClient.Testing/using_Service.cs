/*
Copyright(c) 2018 by Keith Bielaczyc. All Right Reserved.

Unless required by applicable law or agreed to in writing,
software distributed under the License is distributed on an
"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, either express or implied.See the License for the
specific language governing permissions and limitations
under the License.
*/
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Common.Logging;
using System.Net;

namespace EpoOpsClient.Testing
{
	[TestClass]
	public class using_Service
	{
		public static Service sut { get; protected set; }

		[ClassInitialize]
		public static void Arrange(TestContext ctx)
		{
			sut = new Service(
				LogManager.GetLogger<Service>(), 
				Configuration.GetNamedSection<Service>(), 
				() => { return sut.CreateAccessToken().Value; });
		}

		[TestMethod]
		public void getAccessToken()
		{
			var token = sut.CreateAccessToken();

			Assert.IsNotNull(token);
			Assert.IsTrue(!string.IsNullOrEmpty(token.Value));
		}

		[TestMethod]
		public void getPatent()
		{
			var output = sut.GetPatent("EP1883031").Result;

			Assert.IsNotNull(output);

			string result = null;
			using (var reader = new StreamReader(output.Stream))
			{
				result = reader.ReadToEnd();
				reader.Close();
			}

			Assert.IsTrue(!string.IsNullOrEmpty(result));
			Console.WriteLine(result);
		}

		[TestMethod]
		public void getApplication()
		{
			var output = sut.GetApplication("EP20070010825").Result;

			Assert.IsNotNull(output);

			string result = null;
			using (var reader = new StreamReader(output.Stream))
			{
				result = reader.ReadToEnd();
				reader.Close();
			}

			Assert.IsTrue(!string.IsNullOrEmpty(result));
			Console.WriteLine(result);
		}

		// Example of checking StatusCode to verify response. The StatusCode should
		// be checked before attempting to deserialize since the XSLT logic is built
		// for a valid response, not for the "SERVER.EntityNotFound" response.
		[TestMethod]
		public void notFound()
		{
			var output = sut.GetPatent("EP188303X").Result;

			Assert.IsNotNull(output);

			string result = null;
			using (var reader = new StreamReader(output.Stream))
			{
				result = reader.ReadToEnd();
				reader.Close();
			}

			Assert.IsTrue(!string.IsNullOrEmpty(result));
			Console.WriteLine(result);

			Assert.AreEqual(output.StatusCode, HttpStatusCode.NotFound);
		}
	}
}
