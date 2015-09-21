using Intuit.Ipp.Core;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;

namespace quickwrap.connections
{
    public class QboOauthConnection : IQboConnection
    {
        //Consumer Key = qyprd7kWxwHpnq2YP9UkR4YuCb5ijz
        //Consumer Secret = puF6IGzEAGSZ6fMwrz1z6ssmJnRoZmt9iEy3gAsE
        //Access Token = qyprdgz716hsYi7Z4SdnaixGqxXS6kniq08pch0uNM1ugJx5 
        //Access Token Secret = GFwtQy0tfqTNFktaQrccBGMX2rKUIi4blKi1VwJ3
        //Realm Id = 404669156

        private readonly string _appUrl;            // typically sandbox (https://sandbox-quickbooks.api.intuit.com/) 
                                                    // or production (https://quickbooks.api.intuit.com/)
        private readonly string _consumerKey;       // get from user account
        private readonly string _consumerSecret;    // get from user account
        private readonly string _accessToken;       // get from interactive oauth session
        private readonly string _accessTokenSecret; // get from interactive oauth session
        private readonly string _realmId;           // get from interactive oauth session
        private const IntuitServicesType ServiceType = IntuitServicesType.QBO;


        public QboOauthConnection(string appUrl, string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret, string realmId)
        {
            _appUrl = appUrl;
            _accessToken = accessToken;
            _accessTokenSecret = accessTokenSecret;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _realmId = realmId;
        }

        public DataService DataService {
            get
            {
                if (_dataService == null)
                {
                    var validator = new OAuthRequestValidator(_accessToken, _accessTokenSecret, _consumerKey, _consumerSecret);
                    var context = new ServiceContext(_realmId, ServiceType, validator);
                    context.IppConfiguration.BaseUrl.Qbo = _appUrl; // customize url programmatically per https://developer.intuit.com/docs/0100_accounting/0500_developer_kits/0150_ipp_.net_devkit_3.0/0002_configuration
                    _dataService = new DataService(context);
                }
                return _dataService;
            }
        }
        private DataService _dataService;



    }
}
