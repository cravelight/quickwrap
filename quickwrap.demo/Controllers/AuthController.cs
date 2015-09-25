using System.Web.Mvc;
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

        private const string QbApiUrl = "https://sandbox-quickbooks.api.intuit.com/";

        /// <summary>
        /// The OauthTokenManager helps up build up the information we need to connect with the QBO api 
        /// We'll use this to persist our OAuth connection info as we gather it through the multi-step process
        /// At the end of the workflow, the OauthTokenManager.OauthInfo should be fully populated with all
        /// the info you need to store in order to make QB api requests
        /// </summary>
        public OauthTokenManager OauthTokenManager
        {
            get
            {
                return Session["OauthTokenManager"] as OauthTokenManager;
            }
            set
            {
                Session["OauthTokenManager"] = value;
            }
        }


        
        public ActionResult Index()
        {
            ViewBag.UrlForTheRequestTokenEndpoint = Url.Action("RequestTokenEndpoint", "Auth", null, Request.Url.Scheme);

            var model = OauthTokenManager == null
                ? new QboOauthConnectionInfo { ApiUrl = QbApiUrl }
                : OauthTokenManager.OauthInfo;
            
            return View(model);
        }


        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            // You could skip this postback and just grab the consumer key/secret from a config or const.
            // We just used form fields and a postback for demo purposes 
            // It keeps the demo zero config.

            // Store the consumer key from the form inputs.
            OauthTokenManager = new OauthTokenManager(collection["consumerKey"], collection["consumerSecret"], QbApiUrl);

            return RedirectToAction("Index");
        }




        #region OAuth Endpoints


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
            // get the full url to our callback endpoint
            var oauthCallbackUrl = Url.Action("OauthCallbackEndpoint", "Auth", null, Request.Url.Scheme);

            // build a url that asks QB to give us a request token
            var urlToAskForRequestToken = OauthTokenManager.GetUrlThatAsksQuickBooksForRequestToken(oauthCallbackUrl);

            // initiate the call for the request token
            return new RedirectResult(urlToAskForRequestToken);
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
            var oauthVerifier = Request.QueryString["oauth_verifier"];
            var realmId = Request.QueryString["realmId"];

            // Stores the realm id and trades request token for access token
            OauthTokenManager.ProcessRequestToken(oauthVerifier,realmId);

            // redirect to index... we should have all the OauthInfo now
            return RedirectToAction("Index");
        }


        #endregion // OAuth Endpoints

    }
}