using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.Script.Serialization;
using System.Globalization;
using Newtonsoft.Json;

namespace SFDC_TwitterChallenge
{
    public partial class SFDCTwitterChallenge : System.Web.UI.Page
    {
        string oAuthConsumerKey = "8A0bDsWevLaPiyZjKmHdtV2N5";
        string oAuthConsumerSecret = "1kZQ6uDOuvli7uPNxZvQUBPBPz738GkS6TDd40Y7pBI7U2eEZG";
        string oAuthToken = "101725456-XfQInPPePUEVkFNQeaPSwYGePT3kcsI71sLNJOBu";
        string oAuthTokenSecret = "ubGKzq7WbMI5XEyFLQ8j1BRvthtyXlJIPj5ZfvnaeXrbh";
        string accessToken = null;
        Timer timerRefreshData;
        
        List<CustomTweetClass> processedData;
        List<CustomTweetClass> searchedData;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.form1.Attributes.Add("onkeypress", "button_click(this,'" + this.btnSearch.ClientID + "')");
                this.txtSearch.Attributes.Add("onfocusout", "txtSearch_OnFocusOut(this,'" + this.btnSearch.ClientID + "')");
            }
            TimeRefresh.Enabled = false;
            GetData();
        }

        public void GetToken()
        {
            try
            {
                // get these from somewhere nice and secure...
                var key = oAuthConsumerKey;
                var secret = oAuthConsumerSecret;
                var server = HttpContext.Current.Server;
                var bearerToken = server.UrlEncode(key) + ":" + server.UrlEncode(secret);
                var b64Bearer = Convert.ToBase64String(Encoding.Default.GetBytes(bearerToken));

                using (var wc = new WebClient())
                {
                    wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
                    wc.Headers.Add("Authorization", "Basic " + b64Bearer);
                    var tokenPayload = wc.UploadString("https://api.twitter.com/oauth2/token", "grant_type=client_credentials");
                    var rgx = new Regex("\"access_token\"\\s*:\\s*\"([^\"]*)\"");

                    // you can store this accessToken and just do the next bit if you want
                    accessToken = rgx.Match(tokenPayload).Groups[1].Value;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public string GetTweetsFromAPI()
        {
            string respbody = null;
            try
            {
                if (string.IsNullOrEmpty(accessToken))
                    GetToken();
                var gettimeline = WebRequest.Create("https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name=salesforce&count=10&tweet_mode=extended&include_entities=true") as HttpWebRequest;

                gettimeline.Method = "GET";
                gettimeline.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                
                using (var resp = gettimeline.GetResponse().GetResponseStream())//there request sends
                {
                    var respR = new StreamReader(resp);
                    respbody = respR.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                //TODO
            }
            return respbody;
        }

        public void GetData()
        {
            TimeRefresh.Enabled = false;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String tweetsJSON = GetTweetsFromAPI();
            processedData = searchedData = ProcessTweets(tweetsJSON);
            string name = searchedData[0].Name;
            string Screen_Name = searchedData[0].Screen_Name;
            string profile_image_url_https = searchedData[0].Profile_image_url_https;

            string media_url_https = "";
            if (searchedData[0].Media_url_https != null)
            {
                media_url_https = searchedData[0].Media_url_https;
            }
            else
            {
                media_url_https = "--NA--";
            }
            

            int retweet_coun = searchedData[0].Retweet_count;
            string createdAt = searchedData[0].CreatedAt;

            imgUserProfile.ImageUrl = searchedData[0].Profile_image_url_https;
            lblUserName.Text = searchedData[0].Name;
            lblUserScreenName.Text = searchedData[0].Screen_Name;

            SearchWithinData(txtSearch.Text.ToString());
        }

        public void SearchWithinData(string searchValue)
        {
            if (!string.IsNullOrEmpty(searchValue))
                searchedData = searchedData.FindAll(x => x.Text.ToLower().Contains(searchValue.ToLower()));
            else
                searchedData = processedData;
            grdTwitter.DataSource = searchedData;
            grdTwitter.DataBind();
            TimeRefresh.Enabled = true;
        }

        public List<CustomTweetClass> ProcessTweets(string twitterResponse)
        {
            List<CustomTweetClass> CustomTweets = new List<CustomTweetClass>();
            dynamic tweetsFromResponse = JsonConvert.DeserializeObject(twitterResponse);

            foreach (var tweetItem in tweetsFromResponse)
            {
                CustomTweetClass tweet = new CustomTweetClass();
                if (tweetItem["full_text"] != null)
                    tweet.Full_text = (string)tweetItem["full_text"];
                else
                    tweet.Full_text = "";
                if (tweetItem["user"] != null)
                    tweet.Name = (string)tweetItem["user"]["name"];
                else
                    tweet.Name = "";

                tweet.Screen_Name = (string)tweetItem["user"]["screen_name"];
                tweet.Profile_image_url_https = (string)tweetItem["user"]["profile_image_url_https"];
                if (tweetItem["entities"]["media"] != null)
                    tweet.Media_url_https = (string)tweetItem["entities"]["media"][0]["media_url_https"];
                else if (tweetItem["quoted_status"] != null && tweetItem["quoted_status"]["entities"]["media"] != null)
                {
                    tweet.Media_url_https = (string)tweetItem["quoted_status"]["entities"]["media"][0]["media_url_https"];
                }
                else
                    tweet.Media_url_https = "";

                if (tweetItem["retweet_count"] != null)
                    tweet.Retweet_count = (int)tweetItem["retweet_count"];
                else
                    tweetItem["retweet_count"] = "";

                tweet.CreatedAt = (string)tweetItem["created_at"];
                CustomTweets.Add(tweet);
            }

            return CustomTweets;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchWithinData(txtSearch.Text.ToString());
        }

        protected void TimeRefresh_Tick(object sender, EventArgs e)
        {
            GetData();
        }

    }
}