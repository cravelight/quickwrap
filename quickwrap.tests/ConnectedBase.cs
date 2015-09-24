using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quickwrap.connections;

namespace quickwrap.tests
{
    /// <summary>
    /// Allows for centralized management of oauth credentials and target environment
    /// </summary>
    public class ConnectedBase
    {

        protected QboOauthConnection GetOauthConnection()
        {
            return new QboOauthConnection(ConnInfo);
        }

        protected readonly OauthConnectionInfo ConnInfo = new QboOauthConnectionInfo
        {
            ConsumerKey = "qyprd7kWxwHpnq2YP9UkR4YuCb5ijz",
            ConsumerSecret = "puF6IGzEAGSZ6fMwrz1z6ssmJnRoZmt9iEy3gAsE",
            AccessToken = "qyprdgz716hsYi7Z4SdnaixGqxXS6kniq08pch0uNM1ugJx5",
            AccessTokenSecret = "GFwtQy0tfqTNFktaQrccBGMX2rKUIi4blKi1VwJ3",
            RealmId = "404669156",
            ApiUrl = "https://sandbox-quickbooks.api.intuit.com/"
        };

    }
}
