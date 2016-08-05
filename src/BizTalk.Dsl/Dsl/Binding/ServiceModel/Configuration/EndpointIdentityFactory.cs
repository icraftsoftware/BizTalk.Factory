#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Configuration;

namespace Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration
{
	public static class EndpointIdentityFactory
	{
		public static IdentityElement CreateCertificateIdentity(
			StoreLocation storeLocation,
			StoreName storeName,
			X509FindType findType,
			string findValue,
			bool isChainIncluded = false)
		{
			var ie = new IdentityElement();
			ie.CertificateReference.StoreLocation = storeLocation;
			ie.CertificateReference.StoreName = storeName;
			ie.CertificateReference.X509FindType = findType;
			ie.CertificateReference.FindValue = findValue;
			ie.CertificateReference.IsChainIncluded = isChainIncluded;
			return ie;
		}

		public static IdentityElement CreateDnsIdentity(string dnsName)
		{
			var ie = new IdentityElement();
			ie.Dns.Value = dnsName;
			return ie;
		}

		public static IdentityElement CreateSpnIdentity(string spnName)
		{
			var ie = new IdentityElement();
			ie.ServicePrincipalName.Value = spnName;
			return ie;
		}
	}
}
