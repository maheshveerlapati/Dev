using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFDC_TwitterChallenge
{
    public class CustomTweetClass
    {
        string _name;
        string _Screen_Name;
        string _profile_image_url_https;
        string _media_url_https;
        string _text;
        int _retweet_count;
        string _createdAt;

        public string Name { get => _name; set => _name = value; }
        public string Screen_Name { get => _Screen_Name; set => _Screen_Name = value; }
        public string Profile_image_url_https { get => _profile_image_url_https; set => _profile_image_url_https = value; }
        public string Media_url_https { get => _media_url_https; set => _media_url_https = value; }
        public int Retweet_count { get => _retweet_count; set => _retweet_count = value; }
        public string CreatedAt { get => _createdAt; set => _createdAt = value; }
        public string Text { get => _text; set => _text = value; }
    }
}