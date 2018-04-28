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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace EpoOpsClient.Serialization
{
    public class XmlSerializer
    {
		public const string TransformFilenameKey = "transformFilename";
		public string TransformFilename { get; set; }

		private readonly XslCompiledTransform _transformer;
		private ILog _log;

		public XmlSerializer(ILog log, NameValueCollection config)
		{
			_log = log;

			// TODO: Add some argument validation.

			var item = config[TransformFilenameKey];
			if (item != null) TransformFilename = item;
			_log.Debug($"{TransformFilenameKey}: {item}");

			_transformer = new XslCompiledTransform();
			_transformer.Load(TransformFilename);
		}

		public static int CalculateCheckDigits(string applicationNumber)
		{
			var temp = "";
			for (var k = 0; k < applicationNumber.Length; k++)
			{
				if ((k + 1) % 2 != 0)
				{
					temp += applicationNumber[k].ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					var n = Convert.ToInt32(applicationNumber[k].ToString(CultureInfo.InvariantCulture)) * 2;
					temp += n.ToString(CultureInfo.InvariantCulture);
				}
			}
			var sum = 0;
			for (var k = 0; k < temp.Length; k++)
			{
				sum += Convert.ToInt32(temp[k].ToString(CultureInfo.InvariantCulture));
			}
			var remainder = sum % 10;
			int sub;
			if (0 < remainder)
			{
				sub = ((remainder / 10) + 1) * 10;
			}
			else
			{
				sub = 0;
			}
			var result = sub - remainder;
#if DEBUG
			for (var k = 0; k < applicationNumber.Length; k++)
			{
				System.Console.Write(applicationNumber[k].ToString(CultureInfo.InvariantCulture) + "\t");
			}
			System.Console.WriteLine();
			for (var k = 0; k < applicationNumber.Length; k++)
			{
				System.Console.Write("x\t");
			}
			System.Console.WriteLine();
			for (var k = 0; k < applicationNumber.Length; k++)
			{
				var n = ((k + 1) % 2 == 0) ? 2 : 1;
				System.Console.Write(n + "\t");
			}
			System.Console.WriteLine();
			System.Console.WriteLine("-------------------------------------------------------");
			for (var k = 0; k < applicationNumber.Length; k++)
			{
				var n = ((k + 1) % 2 == 0) ? 2 : 1;
				var m = Convert.ToInt32(applicationNumber[k].ToString(CultureInfo.InvariantCulture)) * n;
				System.Console.Write(m + "\t");
			}
			System.Console.WriteLine();
			for (var k = 0; k < temp.Length; k++)
			{
				if (0 < k)
				{
					System.Console.Write(" + ");
				}
				System.Console.Write(temp[k].ToString(CultureInfo.InvariantCulture));
			}
			System.Console.Write(" = " + sum);
			System.Console.WriteLine();
			System.Console.Write("Divide " + sum + " : 10 and the remainder is " + remainder);
			System.Console.WriteLine();
			System.Console.Write("Subtract " + sub + " - " + remainder + " = " + result + " check digit");
			System.Console.WriteLine();
#endif
			return result;
		}

		public Patent Deserialize(Stream output)
		{
			using (var memory = new MemoryStream())
			{
				var writer = new StreamWriter(memory, Encoding.UTF8);
				using (var reader = XmlReader.Create(new StreamReader(output)))
				{
					_transformer.Transform(reader, null, writer);
					writer.Flush();
					memory.Position = 0;
					reader.Close();
				}
				output.Close();
				string data = null;
				using (var reader = new StreamReader(memory))
				{
					data = reader.ReadToEnd();
					reader.Close();
				}
				writer.Close();
				memory.Close();
				if (0 < data.Length)
				{
					var cols = data.Split(new[] { '\t' });
					var result = new Patent
					{
						ApplicationDate = cols[0].Replace("\"", ""),
						ApplicationNumber = cols[1].Replace("\"", ""),
						CurrentOwner = cols[2].Replace("\"", ""),
						SystemId = cols[3].Replace("\"", ""),
						GrantDate = cols[4].Replace("\"", ""),
						PatentNumber = cols[5].Replace("\"", ""),
						TaxYear = Int32.Parse(cols[6].Replace("\"", "")),
						TaxPaidDate = cols[7].Replace("\"", "")
					};
					// Add the check digit to the application number.
					var check = CalculateCheckDigits(result.ApplicationNumber.Replace("EP", ""));
					result.ApplicationNumber += "." + check;
					return result;
				}
				return null;
			}
		}
	}
}
