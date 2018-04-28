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

namespace EpoOpsClient
{
	public class AccessToken
	{
		private DateTime _issuedAt;
		public DateTime IssuedAt
		{
			get
			{
				return _issuedAt;
			}
			set
			{
				_issuedAt = value;
				ExpiresAt = CalculateExpiration();
			}
		}
		private int _expiresIn;
		/// <summary>
		/// Number of seconds after the time when issued until token expires.
		/// </summary>
		public int ExpiresIn
		{
			get
			{
				return _expiresIn;
			}
			set
			{
				_expiresIn = value;
				ExpiresAt = CalculateExpiration();
			}
		}

		public string Value { get; set; }

		/// <summary>
		/// Number of minutes that will be trimmed from the expiration time to
		/// ensure that we re-authenticate before the token expires.
		/// </summary>
		public const int ExpiryBuffer = 2;
		public DateTime ExpiresAt { get; private set; }

		public DateTime CalculateExpiration()
		{
			return IssuedAt
				.AddSeconds(ExpiresIn)
				.AddMinutes(-ExpiryBuffer);
		}
	}
}
