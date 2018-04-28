/*
Copyright(c) 2018 by Keith Bielaczyc. All Right Reserved.

Unless required by applicable law or agreed to in writing,
software distributed under the License is distributed on an
"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, either express or implied.See the License for the
specific language governing permissions and limitations
under the License.
*/
namespace EpoOpsClient
{
	public class SystemDetail
	{
		public string ServiceName { get; set; }
		public string TrafficLightPosition { get; set; }
		public string RequestLimit { get; set; }
	}

	public class ThrottlingControl
	{
		public string SystemState { get; set; }
		public SystemDetail[] SystemDetails { get; set; }
	}
}
