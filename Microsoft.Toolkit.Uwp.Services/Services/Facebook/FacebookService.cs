﻿// ****************************************************************** Copyright (c) Microsoft. All
// rights reserved. This code is licensed under the MIT License (MIT). THE CODE IS PROVIDED “AS IS”,
// WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE CODE OR THE
// USE OR OTHER DEALINGS IN THE CODE. ******************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.Storage.Streams;
using winsdkfb;
using winsdkfb.Graph;
using System.Linq;

namespace Microsoft.Toolkit.Uwp.Services.Facebook
{
    /// <summary>
    /// Class for connecting to Facebook.
    /// </summary>
    public class FacebookService
    {
        /// <summary>
        /// Private singleton field.
        /// </summary>
        private static FacebookService instance;

        /// <summary>
        /// Field for tracking initialization status.
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// List of permissions required by the app.
        /// </summary>
        private FBPermissions permissions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookService"/> class.
        /// </summary>
        public FacebookService()
        {
        }

        /// <summary>
        /// Gets public singleton property.
        /// </summary>
        public static FacebookService Instance => instance ?? (instance = new FacebookService());

        public string AccessToken
        {
            get { return Provider.AccessTokenData.AccessToken; }
        }

        /// <summary>
        /// Gets the current logged user name.
        /// </summary>
        public string LoggedUser => !Provider.LoggedIn ? null : FBSession.ActiveSession.User.Name;

        /// <summary>
        /// Gets a reference to an instance of the underlying data provider.
        /// </summary>
        public FBSession Provider
        {
            get
            {
                if (!isInitialized)
                {
                    throw new InvalidOperationException("Provider not initialized.");
                }

                return FBSession.ActiveSession;
            }
        }

        /// <summary>
        /// Gets a Windows Store ID associated with the current app
        /// </summary>
        public string WindowsStoreId => WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();

        /// <summary>
        /// Returns the <see cref="FacebookPicture"/> object associated with the page
        /// </summary>
        /// <param name="pageId">id of the page for retrieving the picture</param>
        /// <returns>A <see cref="FacebookPicture"/> object</returns>
        public async Task<FacebookPicture> GetPagePictureInfoAsync(string pageId)
        {
            if (Provider.LoggedIn)
            {
                var factory = new FBJsonClassFactory(JsonConvert.DeserializeObject<FacebookDataHost<FacebookPicture>>);

                PropertySet propertySet = new PropertySet { { "redirect", "0" } };
                var singleValue = new FBSingleValue($"/{pageId}/picture", propertySet, factory);

                var result = await singleValue.GetAsync();

                if (result.Succeeded)
                {
                    return ((FacebookDataHost<FacebookPicture>)result.Object).Data;
                }

                throw new Exception(result.ErrorInfo?.Message);
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await GetPagePictureInfoAsync(pageId);
            }

            return null;
        }

        /// <summary>
        /// Retrieves a photo by id.
        /// </summary>
        /// <param name="photoId">Photo Id for the photo.</param>
        /// <returns>A single photo.</returns>
        public async Task<FacebookPhoto> GetPhotoByPhotoIdAsync(string photoId)
        {
            if (Provider.LoggedIn)
            {
                var factory = new FBJsonClassFactory(JsonConvert.DeserializeObject<FacebookPhoto>);

                PropertySet propertySet = new PropertySet { { "fields", "images" } };
                var singleValue = new FBSingleValue($"/{photoId}", propertySet, factory);

                var result = await singleValue.GetAsync();

                if (result.Succeeded)
                {
                    return (FacebookPhoto)result.Object;
                }

                throw new Exception(result.ErrorInfo?.Message);
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await GetPhotoByPhotoIdAsync(photoId);
            }

            return null;
        }

        //current_place/results?coordinates={"latitude": 53.196981,"longitude": 6.584016}&min_confidence_level=low
        public async Task<List<FacebookPlace>> GetPlacesByCoordinatesAsync(double latitude, double longitude, FacebookPlaceConfidenceLevel confidenceLevel = FacebookPlaceConfidenceLevel.Low)
        {
            var fields = FacebookPlace.Fields;
            var location = new FacebookLocation()
            {
                Latitude = latitude,
                Longitude = longitude
            };
            var locationAsJson = JsonConvert.SerializeObject(location);
            var config = new FacebookDataConfig { Query = $"/current_place/results?coordinates={locationAsJson}&min_confidence_level={confidenceLevel.ToString().ToLowerInvariant()}" };
            return await RequestAsync<FacebookPlace>(config, 25, fields);
        }

        /// <summary>
        /// Get posts of a page feed by page id
        /// </summary>
        /// <param name="pageId">id of the page for retrieving the feed</param>
        /// <param name="maxRecords">Upper limit of records to return.</param>
        /// <param name="fields">which fields to retrieve from the data</param>
        /// <returns>list of posts on the page</returns>
        public async Task<List<FacebookPost>> GetPostsOfPageByPageId(string pageId, int maxRecords = 20, string fields = null)
        {
            fields = fields ?? FacebookPost.Fields;
            var config = new FacebookDataConfig { Query = $"/{pageId}/feed" };
            return await RequestAsync<FacebookPost>(config, maxRecords, fields);
        }

        /// <summary>
        /// Retrieves list of user photo albums.
        /// </summary>
        /// <param name="maxRecords">Upper limit of records to return.</param>
        /// <param name="fields">Custom list of Album fields to retrieve.</param>
        /// <returns>List of User Photo Albums.</returns>
        public async Task<List<FacebookAlbum>> GetUserAlbumsAsync(int maxRecords = 20, string fields = null)
        {
            fields = fields ?? FacebookAlbum.Fields;
            var config = new FacebookDataConfig { Query = "/me/albums" };

            return await RequestAsync<FacebookAlbum>(config, maxRecords, fields);
        }

        /// <summary>
        /// Retrieves list of user photo albums.
        /// </summary>
        /// <param name="pageSize">Number of records to retrieve per page.</param>
        /// <param name="maxPages">Upper limit of pages to return.</param>
        /// <param name="fields">Custom list of Album fields to retrieve.</param>
        /// <returns>List of User Photo Albums.</returns>
        public async Task<IncrementalLoadingCollection<FacebookRequestSource<FacebookAlbum>, FacebookAlbum>> GetUserAlbumsAsync(int pageSize, int maxPages, string fields = null)
        {
            fields = fields ?? FacebookAlbum.Fields;
            var config = new FacebookDataConfig { Query = "/me/albums" };

            return await RequestAsync<FacebookAlbum>(config, pageSize, maxPages, fields);
        }

        /// <summary>
        /// Retrieves list of user photos by album id.
        /// </summary>
        /// <param name="albumId">Albums Id for photos.</param>
        /// <param name="maxRecords">Upper limit of records to return</param>
        /// <param name="fields">Custom list of Photo fields to retrieve.</param>
        /// <returns>List of User Photos.</returns>
        public async Task<List<FacebookPhoto>> GetUserPhotosByAlbumIdAsync(string albumId, int maxRecords = 20, string fields = null)
        {
            fields = fields ?? FacebookPhoto.Fields;
            var config = new FacebookDataConfig { Query = $"/{albumId}/photos" };

            return await RequestAsync<FacebookPhoto>(config, maxRecords, fields);
        }

        /// <summary>
        /// Retrieves list of user photos by album id.
        /// </summary>
        /// <param name="albumId">Albums Id for photos.</param>
        /// <param name="pageSize">Number of records to retrieve per page.</param>
        /// <param name="maxPages">Upper limit of pages to return.</param>
        /// <param name="fields">Custom list of Photo fields to retrieve.</param>
        /// <returns>List of User Photos.</returns>
        public async Task<IncrementalLoadingCollection<FacebookRequestSource<FacebookPhoto>, FacebookPhoto>> GetUserPhotosByAlbumIdAsync(string albumId, int pageSize, int maxPages, string fields = null)
        {
            fields = fields ?? FacebookPhoto.Fields;
            var config = new FacebookDataConfig { Query = $"/{albumId}/photos" };

            return await RequestAsync<FacebookPhoto>(config, pageSize, maxPages, fields);
        }

        /// <summary>
        /// Returns the <see cref="FacebookPicture"/> object associated with the logged user
        /// </summary>
        /// <returns>A <see cref="FacebookPicture"/> object</returns>
        public async Task<FacebookPicture> GetUserPictureInfoAsync()
        {
            if (Provider.LoggedIn)
            {
                var factory = new FBJsonClassFactory(JsonConvert.DeserializeObject<FacebookDataHost<FacebookPicture>>);

                PropertySet propertySet = new PropertySet { { "redirect", "0" } };
                var singleValue = new FBSingleValue("/me/picture", propertySet, factory);

                var result = await singleValue.GetAsync();

                if (result.Succeeded)
                {
                    return ((FacebookDataHost<FacebookPicture>)result.Object).Data;
                }

                throw new Exception(result.ErrorInfo?.Message);
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await GetUserPictureInfoAsync();
            }

            return null;
        }

        /// <summary>
        /// Initialize underlying provider with relevent token information.
        /// </summary>
<<<<<<< HEAD
        /// <param name="title">Title of the post.</param>
        /// <param name="message">Message of the post.</param>
        /// <param name="description">Description of the post.</param>
        /// <param name="link">Link contained as part of the post. Cannot be null</param>
        /// <param name="pictureUrl">URL of a picture attached to this post. Can be null</param>
        /// <returns>Task to support await of async call.</returns>
        [Obsolete("This method has been deprecated by Facebook Graph API v2.9. Please use PostToFeedAsync(link) instead.")]
        public async Task<bool> PostToFeedAsync(string title, string message, string description, string link, string pictureUrl = null)
=======
        /// <param name="oAuthTokens">Token instance.</param>
        /// <param name="requiredPermissions">
        /// List of required required permissions. public_profile and user_posts permissions will be
        /// used by default.
        /// </param>
        /// <returns>Success or failure.</returns>
        public bool Initialize(FacebookOAuthTokens oAuthTokens, FacebookPermissions requiredPermissions = FacebookPermissions.PublicProfile | FacebookPermissions.UserPosts | FacebookPermissions.PublishActions)
>>>>>>> fb2912293936b8803e6224af5086e6d0c8780bcd
        {
            if (oAuthTokens == null)
            {
                throw new ArgumentNullException(nameof(oAuthTokens));
            }

            return Initialize(oAuthTokens.AppId, requiredPermissions, oAuthTokens.WindowsStoreId);
        }

        /// <summary>
        /// Initialize underlying provider with relevent token information.
        /// </summary>
        /// <param name="appId">Application ID (Provided by Facebook developer site)</param>
        /// <param name="requiredPermissions">
        /// List of required required permissions. public_profile and user_posts permissions will be
        /// used by default.
        /// </param>
        /// <param name="windowsStoreId">Windows Store SID</param>
        /// <returns>Success or failure.</returns>
        public bool Initialize(string appId, FacebookPermissions requiredPermissions = FacebookPermissions.PublicProfile | FacebookPermissions.UserPosts | FacebookPermissions.PublishActions, string windowsStoreId = null)
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw new ArgumentNullException(nameof(appId));
            }

            if (string.IsNullOrEmpty(windowsStoreId))
            {
                windowsStoreId = WindowsStoreId;
            }

            isInitialized = true;

            Provider.FBAppId = appId;
            Provider.WinAppId = windowsStoreId;

            // Permissions
            var permissionList = new List<string>();

            foreach (FacebookPermissions value in Enum.GetValues(typeof(FacebookPermissions)))
            {
                if ((requiredPermissions & value) != 0)
                {
                    var name = value.ToString();
                    var finalName = new StringBuilder();

                    foreach (var c in name)
                    {
                        if (char.IsUpper(c))
                        {
                            if (finalName.Length > 0)
                            {
                                finalName.Append('_');
                            }

                            finalName.Append(char.ToLower(c));
                        }
                        else
                        {
                            finalName.Append(c);
                        }
                    }

                    permissionList.Add(finalName.ToString());
                }
            }

            permissions = new FBPermissions(permissionList);

            return true;
        }

        /// <summary>
        /// Login with set of required requiredPermissions.
        /// </summary>
        /// <returns>Success or failure.</returns>
        public async Task<bool> LoginAsync()
        {
            if (Provider != null)
            {
                var result = await Provider.LoginAsync(permissions, SessionLoginBehavior.WebView);

                if (result.Succeeded)
                {
                    return true;
                }

                if (result.ErrorInfo != null)
                {
                    Debug.WriteLine(string.Format("Error logging in: {0}", result.ErrorInfo.Message));
                }

                return false;
            }

            Debug.WriteLine("Error logging in - no Active session found");
            return false;
        }

        /// <summary>
        /// Log out of the underlying service instance.
        /// </summary>
        /// <returns>Task to support await of async call.</returns>
        public Task LogoutAsync()
        {
            return Provider.LogoutAsync().AsTask();
        }

        /// <summary>
        /// Enables posting a picture to the timeline
        /// </summary>
        /// <param name="feedId">id of user feed or page feed</param>
        /// <param name="published">set photo in published or unpublished state</param>
        /// <param name="title">Title of the post.</param>
        /// <param name="pictureName">Picture name.</param>
        /// <param name="pictureStream">Picture stream to upload.</param>
        /// <returns>Return picture information</returns>
        public async Task<FacebookPicture> PostPictureToFeedAsync(string feedId, bool published, string title, string pictureName, IRandomAccessStreamWithContentType pictureStream)
        {
            if (pictureStream == null)
            {
                return null;
            }

            if (Provider.LoggedIn)
            {
                var facebookPictureStream = new FBMediaStream(pictureName, pictureStream);
                var parameters = new PropertySet
                {
                    { "source", facebookPictureStream },
                    { "name", title },
                    { "published", published }
                };

                string path = feedId + "/photos";
                var factory = new FBJsonClassFactory(JsonConvert.DeserializeObject<FacebookPicture>);

                var singleValue = new FBSingleValue(path, parameters, factory);
                var result = await singleValue.PostAsync();
                if (result.Succeeded)
                {
                    var photoResponse = result.Object as FacebookPicture;
                    if (photoResponse != null)
                    {
                        return photoResponse;
                    }
                }

                return null;
            }

            return null;
        }

        /// <summary>
        /// Enables posting a picture to the timeline
        /// </summary>
        /// <param name="title">Title of the post.</param>
        /// <param name="pictureName">Picture name.</param>
        /// <param name="pictureStream">Picture stream to upload.</param>
        /// <returns>Return ID of the picture</returns>
        public async Task<string> PostPictureToFeedAsync(string title, string pictureName, IRandomAccessStreamWithContentType pictureStream)
        {
            var photoInformation = await PostPictureToFeedAsync(FBSession.ActiveSession.User.Id, true, title, pictureName, pictureStream);
            if (photoInformation != null)
            {
                return photoInformation.Id;
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await PostPictureToFeedAsync(title, pictureName, pictureStream);
            }

            return null;
        }

        /// <summary>
        /// Enables direct posting data to the timeline.
        /// </summary>
        /// <param name="title">Title of the post.</param>
        /// <param name="message">Message of the post.</param>
        /// <param name="description">Description of the post.</param>
        /// <param name="link">Link contained as part of the post</param>
        /// <param name="pictureUrl">URL of a picture attached to this post. Can be null</param>
        /// <returns>Task to support await of async call.</returns>
        [Obsolete("This method has been deprecated by Facebook Graph API v2.9. Please use PostToFeedAsync(link) instead.")]
        public async Task<bool> PostToFeedAsync(string message, string title = null, string description = null, string link = null, string pictureUrl = null, string place = null)
        {
            /*
             $response = $facebook->api("/me/feed", 'POST',
  array(
    'access_token=' => $access_token,
    'message' => 'Testing multi-photo post!',
    'attached_media[0]' => '{"media_fbid":"1002088839996"}',
    'attached_media[1]' => '{"media_fbid":"1002088840149"}'
  )
);
*/
            if (Provider.LoggedIn)
            {
                var parameters = new PropertySet { { "message", message } };
                if (!string.IsNullOrEmpty(title))
                {
                    parameters.Add(new KeyValuePair<string, object>("title", title));
                }

                if (!string.IsNullOrEmpty(description))
                {
                    parameters.Add(new KeyValuePair<string, object>("description", description));
                }

                if (!string.IsNullOrEmpty(link))
                {
                    parameters.Add(new KeyValuePair<string, object>("link", link));
                }

                if (!string.IsNullOrEmpty(pictureUrl))
                {
                    parameters.Add(new KeyValuePair<string, object>("picture", pictureUrl));
                }

                if (place != null)
                {
                    parameters.Add(new KeyValuePair<string, object>("place", place));
                }

                string path = FBSession.ActiveSession.User.Id + "/feed";
                var factory = new FBJsonClassFactory(JsonConvert.DeserializeObject<FacebookPost>);

                var singleValue = new FBSingleValue(path, parameters, factory);
                var result = await singleValue.PostAsync();
                if (result.Succeeded)
                {
                    var postResponse = result.Object as FacebookPost;
                    if (postResponse != null)
                    {
                        return true;
                    }
                }

                Debug.WriteLine(string.Format("Could not post. {0}", result.ErrorInfo?.ErrorUserMessage));
                return false;
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await PostToFeedAsync(title, message, description, link, pictureUrl);
            }

            return false;
        }

        /// <summary>
        /// Enables direct posting data to the timeline.
        /// </summary>
        /// <param name="link">Link contained as part of the post. Cannot be null.</param>
        /// <returns>Task to support await of async call.</returns>
        public async Task<bool> PostToFeedAsync(string link)
        {
            if (Provider.LoggedIn)
            {
                var parameters = new PropertySet { { "link", link } };

                string path = FBSession.ActiveSession.User.Id + "/feed";
                var factory = new FBJsonClassFactory(JsonConvert.DeserializeObject<FacebookPost>);

                var singleValue = new FBSingleValue(path, parameters, factory);
                var result = await singleValue.PostAsync();
                if (result.Succeeded)
                {
                    var postResponse = result.Object as FacebookPost;
                    if (postResponse != null)
                    {
                        return true;
                    }
                }

                Debug.WriteLine(string.Format("Could not post. {0}", result.ErrorInfo?.ErrorUserMessage));
                return false;
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await PostToFeedAsync(link);
            }

            return false;
        }

        /// <summary>
        /// Enables posting data to the timeline using Facebook dialog.
        /// </summary>
        /// <param name="title">Title of the post.</param>
        /// <param name="description">Description of the post.</param>
        /// <param name="link">Link contained as part of the post. Cannot be null</param>
        /// <param name="pictureUrl">URL of a picture attached to this post. Can be null</param>
        /// <returns>Task to support await of async call.</returns>
        [Obsolete("This method has been deprecated by Facebook Graph API v2.9. Please use PostToFeedWithDialogAsync(link) instead.")]
        public async Task<bool> PostToFeedWithDialogAsync(string title, string description, string link, string pictureUrl = null)
        {
            if (Provider.LoggedIn)
            {
                var parameters = new PropertySet { { "title", title }, { "description", description }, { "link", link } };

                if (!string.IsNullOrEmpty(pictureUrl))
                {
                    parameters.Add(new KeyValuePair<string, object>("picture", pictureUrl));
                }

                var result = await Provider.ShowFeedDialogAsync(parameters);

                if (result.Succeeded)
                {
                    return true;
                }

                Debug.WriteLine(string.Format("Could not post. {0}", result.ErrorInfo?.ErrorUserMessage));
                return false;
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await PostToFeedWithDialogAsync(title, description, link, pictureUrl);
            }

            return false;
        }

        /// <summary>
<<<<<<< HEAD
        /// Enables posting data to the timeline using Facebook dialog.
        /// </summary>
        /// <param name="link">Link contained as part of the post. Cannot be null.</param>
        /// <returns>Task to support await of async call.</returns>
        public async Task<bool> PostToFeedWithDialogAsync(string link)
        {
            if (Provider.LoggedIn)
            {
                var parameters = new PropertySet { { "link", link } };

                var result = await Provider.ShowFeedDialogAsync(parameters);

                if (result.Succeeded)
                {
                    return true;
                }

                Debug.WriteLine(string.Format("Could not post. {0}", result.ErrorInfo?.ErrorUserMessage));
                return false;
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await PostToFeedWithDialogAsync(link);
            }

            return false;
        }

        /// <summary>
        /// Enables posting a picture to the timeline
=======
        /// Request list data from service provider based upon a given config / query. Enables
        /// posting data to the timeline using Facebook dialog.
>>>>>>> fb2912293936b8803e6224af5086e6d0c8780bcd
        /// </summary>
        /// <param name="link">Link contained as part of the post. Cannot be null.</param>
        /// <returns>Task to support await of async call.</returns>
        public async Task<bool> PostToFeedWithDialogAsync(string link)
        {
            if (Provider.LoggedIn)
            {
                var parameters = new PropertySet { { "link", link } };

                var result = await Provider.ShowFeedDialogAsync(parameters);

                if (result.Succeeded)
                {
                    return true;
                }

                Debug.WriteLine(string.Format("Could not post. {0}", result.ErrorInfo?.ErrorUserMessage));
                return false;
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await PostToFeedWithDialogAsync(link);
            }

            return false;
        }

        /// <summary>
        /// Enables posting a picture to the timeline
        /// </summary>
        /// <param name="config">FacebookDataConfig instance.</param>
        /// <param name="maxRecords">Upper limit of records to return.</param>
        /// <returns>Strongly typed list of data returned from the service.</returns>
        public Task<List<FacebookPost>> RequestAsync(FacebookDataConfig config, int maxRecords = 20)
        {
            return RequestAsync<FacebookPost>(config, maxRecords, FacebookPost.Fields);
        }

        /// <summary>
        /// Request list data from service provider based upon a given config / query.
        /// </summary>
        /// <typeparam name="T">Strong type of model.</typeparam>
        /// <param name="config">FacebookDataConfig instance.</param>
        /// <param name="maxRecords">Upper limit of records to return.</param>
        /// <param name="fields">
        /// A comma seperated string of required fields, which will have strongly typed
        /// representation in the model passed in.
        /// </param>
        /// <returns>Strongly typed list of data returned from the service.</returns>
        public async Task<List<T>> RequestAsync<T>(FacebookDataConfig config, int maxRecords = 20, string fields = "id,message,from,created_time,link,full_picture,name")
        {
            if (Provider.LoggedIn)
            {
                var requestSource = new FacebookRequestSource<T>(config, fields, maxRecords.ToString(), 1);

                var list = await requestSource.GetPagedItemsAsync(0, maxRecords);

                return new List<T>(list);
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await RequestAsync<T>(config, maxRecords, fields);
            }

            return null;
        }

        /// <summary>
        /// Request list data from service provider based upon a given config / query.
        /// </summary>
        /// <param name="config">FacebookDataConfig instance.</param>
        /// <param name="pageSize">Upper limit of records to return.</param>
        /// <param name="maxPages">Upper limit of pages to return.</param>
        /// <returns>Strongly typed list of data returned from the service.</returns>
        public Task<IncrementalLoadingCollection<FacebookRequestSource<FacebookPost>, FacebookPost>> RequestAsync(FacebookDataConfig config, int pageSize, int maxPages)
        {
            return RequestAsync<FacebookPost>(config, pageSize, maxPages, FacebookPost.Fields);
        }

        /// <summary>
        /// Request generic list data from service provider based upon a given config / query.
        /// </summary>
        /// <typeparam name="T">Strong type of model.</typeparam>
        /// <param name="config">FacebookDataConfig instance.</param>
        /// <param name="pageSize">Upper limit of records to return.</param>
        /// <param name="maxPages">Upper limit of pages to return.</param>
        /// <param name="fields">
        /// A comma seperated string of required fields, which will have strongly typed
        /// representation in the model passed in.
        /// </param>
        /// <returns>Strongly typed list of data returned from the service.</returns>
        public async Task<IncrementalLoadingCollection<FacebookRequestSource<T>, T>> RequestAsync<T>(FacebookDataConfig config, int pageSize, int maxPages, string fields = "id,message,from,created_time,link,full_picture")
        {
            if (Provider.LoggedIn)
            {
                var requestSource = new FacebookRequestSource<T>(config, fields, pageSize.ToString(), maxPages);

                return new IncrementalLoadingCollection<FacebookRequestSource<T>, T>(requestSource);
            }

            var isLoggedIn = await LoginAsync();
            if (isLoggedIn)
            {
                return await RequestAsync<T>(config, pageSize, maxPages, fields);
            }

            return null;
        }
    }
}