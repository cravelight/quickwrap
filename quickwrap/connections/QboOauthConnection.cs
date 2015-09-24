using Intuit.Ipp.Core;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;

namespace quickwrap.connections
{
    public class QboOauthConnection : IQboConnection
    {
        private readonly OauthConnectionInfo _connInfo;
        private const IntuitServicesType ServiceType = IntuitServicesType.QBO;


        public QboOauthConnection(OauthConnectionInfo oauthConnectionInfo)
        {
            _connInfo = oauthConnectionInfo;
        }

        public DataService DataService {
            get
            {
                if (_dataService == null)
                {
                    var validator = new OAuthRequestValidator(_connInfo.AccessToken, _connInfo.AccessTokenSecret, _connInfo.ConsumerKey, _connInfo.ConsumerSecret);
                    var context = new ServiceContext(_connInfo.RealmId, ServiceType, validator);
                    context.IppConfiguration.BaseUrl.Qbo = _connInfo.ApiUrl; // customize url programmatically per https://developer.intuit.com/docs/0100_accounting/0500_developer_kits/0150_ipp_.net_devkit_3.0/0002_configuration
                    _dataService = new DataService(context);
                }
                return _dataService;
            }
        }
        private DataService _dataService;

    }
}
