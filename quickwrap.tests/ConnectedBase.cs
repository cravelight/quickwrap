using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quickwrap.connections;

namespace quickwrap.tests
{
    //todo: make this class use nifty config/environment foo

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

            // These items change every time you go through the oauth approval process
            // If your tests are failing due to auth issues, use the UI to get another
            // set of access credentials
            AccessToken = "qyprdYyXjBFDtsDsuN6HH4BZswNh4kAbzDSGmlZBCMdpdkgK",
            AccessTokenSecret = "XOxqky4WtMWXdUkylWxaluvkiKZMjJj1wNGNxZ98",
            
            RealmId = "404669156",
            ApiUrl = "https://sandbox-quickbooks.api.intuit.com/"
        };

    }
}
