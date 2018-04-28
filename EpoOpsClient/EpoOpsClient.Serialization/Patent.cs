/*
Copyright(c) 2018 by Keith Bielaczyc. All Right Reserved.

Unless required by applicable law or agreed to in writing,
software distributed under the License is distributed on an
"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, either express or implied.See the License for the
specific language governing permissions and limitations
under the License.
*/
using System.Runtime.Serialization;

namespace EpoOpsClient.Serialization
{
	[DataContract]
    public class Patent
    {
		public Patent()
		{
			
		}
		[DataMember]
		public string Agent { get; set; }
		[DataMember]
		public string ApplicationDate { get; set; }
		[DataMember]
		public string ApplicationNumber { get; set; }
		[DataMember]
		public string CurrentOwner { get; set; }
		[DataMember]
		public string DesignatedStates { get; set; }
		[DataMember]
		public string DocketNumber { get; set; }
		[DataMember]
		public string EntityStatus { get; set; }
		[DataMember]
		public string ExpirationDate { get; set; }
		[DataMember]
		public string FirstFilingDate { get; set; }
		[DataMember]
		public string FirstTaxDate { get; set; }
		[DataMember]
		public string FirstWorkDate { get; set; }
		[DataMember]
		public string GrantDate { get; set; }
		[DataMember]
		public int IndClaimsDesigns { get; set; }
		[DataMember]
		public string Kind { get; set; }
        [DataMember]
        public string LastEventCode { get; set; }
        [DataMember]
        public string LastEventDescription { get; set; }
        [DataMember]
        public string LastEventDate { get; set; }
        [DataMember]
		public string LegalStatus { get; set; }
		[DataMember]
		public int LicenseOfRight { get; set; }
		[DataMember]
		public string MultipleDesigns { get; set; }
		[DataMember]
		public string NationalFilingDate { get; set; }
		[DataMember]
		public int ParentCountry { get; set; }
		[DataMember]
		public string ParentExpirationDate { get; set; }
		[DataMember]
		public string ParentFilingDate { get; set; }
		[DataMember]
		public string ParentGrantDate { get; set; }
		[DataMember]
		public string ParentTaxbaseDate { get; set; }
		[DataMember]
		public string PatentNumber { get; set; }
		[DataMember]
        public string PaymentDueDate { get; set; }
        [DataMember]
		public string PriorityDate { get; set; }
		[DataMember]
		public string PublicationDate { get; set; }
		[DataMember]
		public int Quinquennial { get; set; }
		[DataMember]
		public string Status { get; set; }
		[DataMember]
		public string SystemId { get; set; }
		[DataMember]
		public string TaxBaseDate { get; set; }
		[DataMember]
		public string TaxPaidDate { get; set; }
		[DataMember]
		public int TaxYear { get; set; }
        [DataMember]
        public string Title { get; set; }
		[DataMember]
		public int TotalClaims { get; set; }
		[DataMember]
		public int TotalClasses { get; set; }
		[DataMember]
		public string Wipo { get; set; }

		public virtual Patent Clone()
		{
			var result = (Patent)MemberwiseClone();
			return result;
		}
	}
}
