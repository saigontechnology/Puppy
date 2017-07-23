#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> InstagramUserFeeds.cs </Name>
//         <Created> 17/06/2017 2:06:32 AM </Created>
//         <Key> d415518a-1534-4c0d-9e03-eb54e17a30b8 </Key>
//     </File>
//     <Summary>
//         InstagramUserFeeds.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

namespace Puppy.Core.InstagramUtils
{
    public class InstagramUserFeeds
    {
        public Pagination pagination { get; set; }

        public Datum[] data { get; set; }

        public Meta meta { get; set; }
    }

    public class Pagination
    {
    }

    public class Meta
    {
        public int code { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }

        public User user { get; set; }

        public Images images { get; set; }

        public string created_time { get; set; }

        public object caption { get; set; }

        public bool user_has_liked { get; set; }

        public Likes likes { get; set; }

        public object[] tags { get; set; }

        public string filter { get; set; }

        public Comments comments { get; set; }

        public string type { get; set; }

        public string link { get; set; }

        public object location { get; set; }

        public object attribution { get; set; }

        public object[] users_in_photo { get; set; }

        public Carousel_Media[] carousel_media { get; set; }
    }

    public class User
    {
        public string id { get; set; }

        public string full_name { get; set; }

        public string profile_picture { get; set; }

        public string username { get; set; }
    }

    public class Images
    {
        public Thumbnail thumbnail { get; set; }

        public Low_Resolution low_resolution { get; set; }

        public Standard_Resolution standard_resolution { get; set; }
    }

    public class Thumbnail
    {
        public int width { get; set; }

        public int height { get; set; }

        public string url { get; set; }
    }

    public class Low_Resolution
    {
        public int width { get; set; }

        public int height { get; set; }

        public string url { get; set; }
    }

    public class Standard_Resolution
    {
        public int width { get; set; }

        public int height { get; set; }

        public string url { get; set; }
    }

    public class Likes
    {
        public int count { get; set; }
    }

    public class Comments
    {
        public int count { get; set; }
    }

    public class Carousel_Media
    {
        public Images images { get; set; }

        public object[] users_in_photo { get; set; }

        public string type { get; set; }
    }
}