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


        List<TweetObject> tweetObject;
        List<CustomTweetClass> processedData;
        List<CustomTweetClass> searchedData;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if(!Page.IsPostBack)
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
                var gettimeline = WebRequest.Create("https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name=salesforce&count=10") as HttpWebRequest;

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
            tweetObject = new List<TweetObject>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String tweetsJSON = GetTweetsFromAPI();
            //String tweetsJSON = File.ReadAllText(@"C:\Users\mveerla\Desktop\SampleTwitterTweets_New.Json");
            tweetObject = serializer.Deserialize<List<TweetObject>>(tweetsJSON);
            string name = tweetObject[0].user.name;
            string Screen_Name = tweetObject[0].user.screen_name;
            string profile_image_url_https = tweetObject[0].user.profile_image_url_https;
            string media_url_https = tweetObject[0].entities.media[0].media_url_https;
            int retweet_coun = tweetObject[0].retweet_count;
            string createdAt = tweetObject[0].created_at;

            //string[] formats = { "hhmm", "hmm", @"hh\:mm", @"h\:mm\:ss", @"h\:mm", "hh:mm tt" };
            //TimeSpan parseSuccess = new TimeSpan();
            //TimeSpan.TryParseExact("Sat Dec 23 20:00:07 + 0000 2017", formats, CultureInfo.CurrentCulture, TimeSpanStyles.None, out parseSuccess);

            //Sat Dec 23 20:00:07 + 0000 2017
            //DateTime createdAt = DateTime.ParseExact("Sat Dec 23 20:00:07 + 0000 2017", "ddd MMMM DD hh:mm:ss + 0000 yyyy", null, DateTimeStyles.None);
            processedData = searchedData = ProcessTweets(tweetObject);
            imgUserProfile.ImageUrl = tweetObject[0].user.profile_image_url_https;
            lblUserName.Text = tweetObject[0].user.name;
            lblUserScreenName.Text = tweetObject[0].user.screen_name;

            SearchWithinData(txtSearch.Text.ToString());

            //JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();
            //var json = jsonSerialiser.Serialize(processedData);
        }

        public void SearchWithinData(string searchValue)
        {
            if (!string.IsNullOrEmpty(searchValue))
                searchedData = searchedData.FindAll(x => x.Text.Contains(searchValue));
            else
                searchedData = processedData;
            grdTwitter.DataSource = searchedData;
            grdTwitter.DataBind();
            TimeRefresh.Enabled = true;
        }

        public List<CustomTweetClass> ProcessTweets(List<TweetObject> tweets)
        {
            List<CustomTweetClass> tweetsForView = new List<CustomTweetClass>();
            CustomTweetClass customTweets = null;
            int index = 0;
            foreach (TweetObject tweetObj in tweets)
            {
                customTweets = new CustomTweetClass();
                index = tweets.IndexOf(tweetObj);
                customTweets.Name = tweetObject[index].user.name;
                customTweets.Screen_Name = tweetObject[index].user.screen_name;
                customTweets.Profile_image_url_https = tweetObject[index].user.profile_image_url_https;
                if (tweetObject[index].entities.media != null && tweetObject[index].entities.media.Count > 0 && !string.IsNullOrEmpty(tweetObject[index].entities.media[0].media_url_https.ToString()))
                    customTweets.Media_url_https = tweetObject[index].entities.media[0].media_url_https;
                else
                    customTweets.Media_url_https = "";
                customTweets.Retweet_count = tweetObject[index].retweet_count;
                customTweets.CreatedAt = tweetObject[index].created_at;
                customTweets.Text = tweetObject[index].text;
                tweetsForView.Add(customTweets);
            }
            return tweetsForView;
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