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
using EpoOpsClient.Serialization;
using System.Web.Script.Serialization;
using Common.Logging;

namespace EpoOpsClient.Testing
{
	[TestClass]
	public class using_XmlSerializer
	{
		public static Service service { get; protected set; }
		public static XmlSerializer sut { get; protected set; }

		[ClassInitialize]
		public static void Arrange(TestContext ctx)
		{
			service = new Service(
				LogManager.GetLogger<Service>(), 
				Configuration.GetNamedSection<Service>(), 
				() => { return service.CreateAccessToken().Value; });
			sut = new XmlSerializer(
				LogManager.GetLogger<XmlSerializer>(), 
				Configuration.GetNamedSection<XmlSerializer>());
		}

		[TestMethod]
		public void deserializeResult()
		{
			const string query = "EP1883031";
			var output = service.GetPatent(query).Result;
			Assert.IsNotNull(output);

			var patent = sut.Deserialize(output.Stream);

			Assert.IsNotNull(patent);
			Assert.IsTrue(!string.IsNullOrEmpty(patent.ApplicationNumber));

			var js = new JavaScriptSerializer();
			System.Console.WriteLine(js.Serialize(patent));

		}
	}
}
