/*
Copyright(c) 2018 by Keith Bielaczyc. All Right Reserved.

Unless required by applicable law or agreed to in writing,
software distributed under the License is distributed on an
"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, either express or implied.See the License for the
specific language governing permissions and limitations
under the License.
*/
using System.Collections.Specialized;
using System.Configuration;

namespace EpoOpsClient
{
	public class Configuration
	{
		public const string DefaultGroupSectionName = "EpoOpsClient";

		public static NameValueCollection GetNamedSection<T>()
		{
			var type = typeof(T);
			var nm = type.FullName;
			var settings = ConfigurationManager.GetSection(string.Concat(DefaultGroupSectionName, "/", nm)) as NameValueCollection;
			return settings;
		}
	}
}
