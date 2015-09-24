using Intuit.Ipp.Core;

namespace quickwrap.connections
{
    public abstract class OauthConnectionInfo
    {
        protected OauthConnectionInfo(IntuitServicesType servicesType)
        {
            ServicesType = servicesType;
        }

        // typically sandbox (https://sandbox-quickbooks.api.intuit.com/) 
        // or production (https://quickbooks.api.intuit.com/)
        public string ApiUrl { get; set; }

        // get from user account
        public string ConsumerKey { get; set; }

        // get from user account
        public string ConsumerSecret { get; set; }

        // get from interactive oauth session
        public string AccessToken { get; set; }

        // get from interactive oauth session
        public string AccessTokenSecret { get; set; }

        // get from interactive oauth session
        public string RealmId { get; set; }


        public IntuitServicesType ServicesType { get; private set; }

        public bool HasAuthenticated 
        {
            get
            {
                return (!string.IsNullOrEmpty(AccessToken)
                        && !string.IsNullOrEmpty(AccessTokenSecret)
                        && !string.IsNullOrEmpty(RealmId));
            }
        }
    }
}
