![Logo](favicon.ico)
# Puppy.Instagram
> Project Created by [**Top Nguyen**](http://topnguyen.net)


1. Register Instagram app via instagram.com/developer
2. Add Client and change security tab => un-check Disable implict OAuth
add security redirect uri, ex: `localhost:10001`

- Get AccessToken and UserId.
  + Find Instagram User Id by [this link](https://smashballoon.com/instagram-feed/find-instagram-user-id).
  + api.instagram.com/oauth/authorize/?client_id=`[clientID]`&redirect_uri=`[redirectURI]`&response_type=token

- Request Feeds via api.instagram.com/v1/users/`<userid>`/media/recent?access_token=`<userid>.<access_token>`.

- Refer: Service Integrate multiple social [Snap Widget](https://snapwidget.com/).

- Use Result - InstagramuserFeeds
```csharp
@foreach (var datum in InstagramuserFeeds.data)
{
    <a href="@datum.link">
        <img src="@datum.images.low_resolution.url" alt="@datum.caption">
    </a>
}
```