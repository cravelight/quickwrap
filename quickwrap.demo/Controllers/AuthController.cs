using System.Web.Mvc;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using quickwrap.connections;

namespace quickwrap.demo.Controllers
{
    /// <summary>
    /// Demonstrates how to create the initial set of access credentials via oauth
    /// For more info see: 
    ///     https://developer.intuit.com/docs/0100_accounting/0060_authentication_and_authorization/connect_from_within_your_app
    /// </summary>
    public class AuthController : Controller
    {

        /// <summary>
        /// We'll use this to persist our OAuth connection info as we gather it through the multi-step process
        /// </summary>
        public QboOauthConnectionInfo OauthInfo
        {
            get
            {
                if (this.Session["ConnInfo"] == null)
                {
                    this.Session["ConnInfo"] = new QboOauthConnectionInfo()
                    {
                        ApiUrl = "https://sandbox-quickbooks.api.intuit.com/"
                    };

                }
                return Session["ConnInfo"] as QboOauthConnectionInfo;
            }
        }


        
        public ActionResult Index()
        {
            ViewBag.UrlForTheRequestTokenEndpoint = Url.Action("RequestTokenEndpoint", "Auth", null, Request.Url.Scheme);
            return View(OauthInfo);
        }



        // If you want, you could skip this part and just grab the consumer key/secret from a config or const.
        // We just used form fields for demo purposes - so the demo is zero config.
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            try
            {
                // Store the consumer key from the form inputs.
                OauthInfo.ConsumerKey = collection["consumerKey"];
                OauthInfo.ConsumerSecret = collection["consumerSecret"];
                return RedirectToAction("Index");
            }
            catch
            {
                return View(OauthInfo);
            }
        }




        #region OAuth Endpoints and helper methods


        /// <summary>
        /// From: https://developer.intuit.com/docs/0100_accounting/0060_authentication_and_authorization/connect_from_within_your_app
        /// ----------
        /// 1. Get the OAuth request token
        /// Implement your Request Token REST endpoint.  The Request Token endpoint is the first of your endpoints that gets invoked 
        /// during the OAuth 1.0 flow. It is responsible for asking the Intuit OAuth API for a request token tuple—request token and 
        /// request token secret, persisting said tuple on the appropriate company, and redirecting the user’s browser to Intuit’s 
        /// Authorize URL.
        /// </summary>
        public ActionResult RequestTokenEndpoint()
        {
            var authSession = GetAnOauthSessionInstance();
            var requestToken = authSession.GetRequestToken();
            var oauthCallbackUrl = Url.Action("OauthCallbackEndpoint", "Auth", null, Request.Url.Scheme);
            var authUrl = string.Format("{0}?oauth_token={1}&oauth_callback={2}",
                IntuitUserAuthorizationUrl,
                requestToken.Token,
                UriUtility.UrlEncode(oauthCallbackUrl));

            // store the requestToken in the Session for use by the callback handler
            Session["requestToken"] = requestToken;

            // initiate the call for the request token
            return new RedirectResult(authUrl);
        }


        /// <summary>
        /// QuickBooks calls this endpoint and sends us:
        /// - the realm id
        /// - a set of request tokens (used to get access tokens)
        /// - the datasource
        /// 
        /// From: https://developer.intuit.com/docs/0100_accounting/0060_authentication_and_authorization/connect_from_within_your_app
        /// ----------
        /// 2. Get the OAuth access token
        /// Implement your Access Token Ready REST endpoint. The Access Token Ready endpoint is the second of your endpoints that gets 
        /// invoked during the OAuth 1.0 flow. It is responsible for asking the Intuit OAuth API for an access token tuple (access 
        /// token and access token secret), persisting said tuple on the appropriate company, and redirecting the user’s browser to a 
        /// web page that will reload the parent page and close the popup. This flow begins when the user is redirected back to your 
        /// OAuth callback URL, which was passed to the OAuth request token endpoint. 
        /// </summary>
        public ActionResult OauthCallbackEndpoint()
        {
            // Store the realm id
            OauthInfo.RealmId = Request.QueryString["realmId"];

            // Ignore the datasource. We are always connecting to QBO for now
            // var dataSource = Request.QueryString["dataSource"];

            // Use the request tokens to get the access tokens
            var authSession = GetAnOauthSessionInstance();
            var accessToken = authSession.ExchangeRequestTokenForAccessToken((IToken)Session["requestToken"], Request.QueryString["oauth_verifier"]);

            // Store the access token and secret
            OauthInfo.AccessToken = accessToken.Token;
            OauthInfo.AccessTokenSecret = accessToken.TokenSecret;

            // redirect to index... we should have all the OauthInfo now
            return RedirectToAction("Index");
        }


        private IOAuthSession GetAnOauthSessionInstance()
        {
            var consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = OauthInfo.ConsumerKey,
                ConsumerSecret = OauthInfo.ConsumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1
            };
            return new OAuthSession(consumerContext, IntuitRequestTokenUrl, IntuitOauthUrl, IntuitAccessTokenUrl);
        }


        // From developer docs...
        private const string IntuitOauthUrl = "https://oauth.intuit.com/oauth/v1";
        private const string IntuitRequestTokenUrl = "https://oauth.intuit.com/oauth/v1/get_request_token";
        private const string IntuitAccessTokenUrl = "https://oauth.intuit.com/oauth/v1/get_access_token";
        private const string IntuitUserAuthorizationUrl = "https://appcenter.intuit.com/Connect/Begin";


        #endregion // OAuth Endpoints and helper methods

    }
}