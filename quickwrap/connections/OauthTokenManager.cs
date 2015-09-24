using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Intuit.Ipp.Exception;

namespace quickwrap.connections
{
    /// <summary>
    /// Simplifies the process of creating, destroying and refreshing access tokens
    /// </summary>
    public class OauthTokenManager
    {
        private readonly string _intuitUserAuthorizationUrl;
        private IToken _requestToken;
        private readonly IOAuthSession _authSession;

        public QboOauthConnectionInfo OauthInfo { get; private set; }



        /// <summary>
        /// Note: Default urls are taken from the developer documentation. Override if necessary.
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecret"></param>
        /// <param name="intuitQboApiUrl"></param>
        /// <param name="intuitOauthUrl"></param>
        /// <param name="intuitRequestTokenUrl"></param>
        /// <param name="intuitAccessTokenUrl"></param>
        /// <param name="intuitUserAuthorizationUrl"></param>
        public OauthTokenManager(string consumerKey, string consumerSecret, string intuitQboApiUrl,
            string intuitOauthUrl = "https://oauth.intuit.com/oauth/v1",
            string intuitRequestTokenUrl = "https://oauth.intuit.com/oauth/v1/get_request_token",
            string intuitAccessTokenUrl = "https://oauth.intuit.com/oauth/v1/get_access_token",
            string intuitUserAuthorizationUrl = "https://appcenter.intuit.com/Connect/Begin")
        {
            if (string.IsNullOrEmpty(consumerKey)) { throw new InvalidParameterException("Oops. ConsumerKey is required."); }
            if (string.IsNullOrEmpty(consumerSecret)) { throw new InvalidParameterException("Oops. ConsumerSecret is required."); }
            if (string.IsNullOrEmpty(intuitQboApiUrl)) { throw new InvalidParameterException("Oops. API Url is required."); }

            _intuitUserAuthorizationUrl = intuitUserAuthorizationUrl;


            OauthInfo = new QboOauthConnectionInfo
            {
                ApiUrl = intuitQboApiUrl,
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret
            };

            var consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = OauthInfo.ConsumerKey,
                ConsumerSecret = OauthInfo.ConsumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1
            };
            _authSession = new OAuthSession(consumerContext, intuitRequestTokenUrl, intuitOauthUrl, intuitAccessTokenUrl);

        }

        /// <summary>
        /// Step one in getting an access token is to get a request token.
        /// This will give you a url that when executed will initiate the user's authorization process.
        /// Once the user approves the app, QB will send a request token to the given callback url.
        /// </summary>
        /// <param name="callbackUrlForEndpointThatWillReceiveRequestTokenFromQuickBooks">The callback url QB will send the request token to.</param>
        /// <returns>A url that when executed will initiate the user's authorization process</returns>
        public string GetUrlThatAsksQuickBooksForRequestToken(string callbackUrlForEndpointThatWillReceiveRequestTokenFromQuickBooks)
        {
            _requestToken = _authSession.GetRequestToken();
            var authUrl = string.Format("{0}?oauth_token={1}&oauth_callback={2}",
                _intuitUserAuthorizationUrl,
                _requestToken.Token,
                UriUtility.UrlEncode(callbackUrlForEndpointThatWillReceiveRequestTokenFromQuickBooks));
            return authUrl;
        }


        /// <summary>
        /// Use the request token to acquire an access token. 
        /// OauthInfo object info is complete after this is called.
        /// </summary>
        /// <param name="oauthVerifier">Request.QueryString["oauth_verifier"]</param>
        /// <param name="realmId">Request.QueryString["realmId"]</param>
        public void ProcessRequestToken(string oauthVerifier, string realmId)
        {
            // Store the realm id
            OauthInfo.RealmId = realmId;

            // Use the request tokens to get the access tokens
            var accessToken = _authSession.ExchangeRequestTokenForAccessToken(_requestToken, oauthVerifier);

            // Store the access token and secret
            OauthInfo.AccessToken = accessToken.Token;
            OauthInfo.AccessTokenSecret = accessToken.TokenSecret;

        }



    }
}
