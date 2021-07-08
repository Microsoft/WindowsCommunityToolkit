// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunityToolkit.WinUI.Notifications
{
#if !WINRT
#pragma warning disable SA1008
#pragma warning disable SA1009
    /// <summary>
    /// Builder class used to create <see cref="TileContent"/>
    /// </summary>
    public partial class TileContentBuilder
    {
        /// <summary>
        /// Helper method for creating a tile notification content for using Contact tile template.
        /// </summary>
        /// <param name="contactImageUri">Source for the contact picture</param>
        /// <param name="contactName">Name of the contact</param>
        /// <param name="contactImageAltText">A description of the contact image, for users of assistive technologies.</param>
        /// <param name="contactImageAddImageQuery">Indicating whether Windows should append a query string to the image URI supplied in the Tile notification.</param>
        /// <param name="textLanguage">Gets or sets the target locale of the XML payload, specified as a BCP-47 language tags such as "en-US" or "fr-FR". The locale specified here overrides any other specified locale, such as that in binding or visual.</param>
        /// <returns>An instance of <see cref="TileBindingContentContact"/> represent a payload of a tile notification.</returns>
        public static TileBindingContentContact CreateContactTileContent(Uri contactImageUri, string contactName, string contactImageAltText = default(string), bool? contactImageAddImageQuery = default(bool?), string textLanguage = default(string))
        {
            var contactTileContent = new TileBindingContentContact();
            contactTileContent.Image = CreateTileBasicImage(contactImageUri, contactImageAltText, contactImageAddImageQuery);

            contactTileContent.Text = new TileBasicText();
            contactTileContent.Text.Text = contactName;

            if (textLanguage != default(string))
            {
                contactTileContent.Text.Lang = textLanguage;
            }

            return contactTileContent;
        }

        /// <summary>
        /// Helper method for creating a tile notification content for using Iconic tile template.
        /// </summary>
        /// <param name="iconImageUri">Source of the icon image.</param>
        /// <param name="iconImageAltText">A description of the icon image, for users of assistive technologies.</param>
        /// <param name="iconImageAddImageQuery">Indicating whether Windows should append a query string to the image URI supplied in the Tile notification.</param>
        /// <returns>An instance of <see cref="TileBindingContentIconic"/> represent a payload of a tile notification.</returns>
        public static TileBindingContentIconic CreateIconicTileContent(Uri iconImageUri, string iconImageAltText = default(string), bool? iconImageAddImageQuery = default(bool?))
        {
            var iconicTileContent = new TileBindingContentIconic();
            iconicTileContent.Icon = CreateTileBasicImage(iconImageUri, iconImageAltText, iconImageAddImageQuery);

            return iconicTileContent;
        }

        /// <summary>
        /// Helper method for creating a tile notification content for using People tile template.
        /// </summary>
        /// <param name="peoplePictureSources">Sources of pictures that will be used on the people tile.</param>
        /// <returns>An instance of <see cref="TileBindingContentPeople"/> represent a payload of a tile notification.</returns>
        public static TileBindingContentPeople CreatePeopleTileContent(params Uri[] peoplePictureSources)
        {
            IEnumerable<TileBasicImage> images = peoplePictureSources.Select(u => CreateTileBasicImage(u, default(string), default(bool?)));

            return CreatePeopleTileContent(images);
        }

        /// <summary>
        /// Helper method for creating a tile notification content for using People tile template.
        /// </summary>
        /// <param name="peoplePictures">Sources of pictures with description and image query indicator that will be used on the people tile.</param>
        /// <returns>An instance of <see cref="TileBindingContentPeople"/> represent a payload of a tile notification.</returns>
        public static TileBindingContentPeople CreatePeopleTileContent(params (Uri source, string imageAltText, bool? addImageQuery)[] peoplePictures)
        {
            IEnumerable<TileBasicImage> images = peoplePictures.Select(t => CreateTileBasicImage(t.source, t.imageAltText, t.addImageQuery));

            return CreatePeopleTileContent(images);
        }

        /// <summary>
        /// Helper method for creating a tile notification content for using People tile template.
        /// </summary>
        /// <param name="peoplePictures">Pictures that will be used on the people tile.</param>
        /// <returns>An instance of <see cref="TileBindingContentPeople"/> represent a payload of a tile notification.</returns>
        public static TileBindingContentPeople CreatePeopleTileContent(IEnumerable<TileBasicImage> peoplePictures)
        {
            var peopleTileContent = new TileBindingContentPeople();

            foreach (var image in peoplePictures)
            {
                peopleTileContent.Images.Add(image);
            }

            return peopleTileContent;
        }

        /// <summary>
        /// Helper method for creating a tile notification content for using Photos tile template.
        /// </summary>
        /// <param name="photoSources">Sources of pictures that will be used on the photos tile.</param>
        /// <returns>An instance of <see cref="TileBindingContentPhotos"/> represent a payload of a tile notification.</returns>
        public static TileBindingContentPhotos CreatePhotosTileContent(params Uri[] photoSources)
        {
            IEnumerable<TileBasicImage> images = photoSources.Select(u => CreateTileBasicImage(u, default(string), default(bool?)));

            return CreatePhotosTileContent(images);
        }

        /// <summary>
        /// Helper method for creating a tile notification content for using Photos tile template.
        /// </summary>
        /// <param name="photos">Sources of pictures with description and addImageQuery indicator that will be used for the photos tile.</param>
        /// <returns>An instance of <see cref="TileBindingContentPhotos"/> represent a payload of a tile notification.</returns>
        public static TileBindingContentPhotos CreatePhotosTileContent(params (Uri source, string imageAltText, bool? addImageQuery)[] photos)
        {
            IEnumerable<TileBasicImage> images = photos.Select(t => CreateTileBasicImage(t.source, t.imageAltText, t.addImageQuery));

            return CreatePhotosTileContent(images);
        }

        /// <summary>
        /// Helper method for creating a tile notification content for using Photos tile template.
        /// </summary>
        /// <param name="photos">Pictures that will be used for the photos tile.</param>
        /// <returns>An instance of <see cref="TileBindingContentPhotos"/> represent a payload of a tile notification.</returns>
        public static TileBindingContentPhotos CreatePhotosTileContent(IEnumerable<TileBasicImage> photos)
        {
            var photoTileContent = new TileBindingContentPhotos();

            foreach (var image in photos)
            {
                photoTileContent.Images.Add(image);
            }

            return photoTileContent;
        }

        private static TileBasicImage CreateTileBasicImage(Uri imageUri, string imageAltText, bool? addImageQuery)
        {
            var tileImage = new TileBasicImage();
            tileImage.Source = imageUri.OriginalString;

            if (imageAltText != default(string))
            {
                tileImage.AlternateText = imageAltText;
            }

            if (addImageQuery != default(bool?))
            {
                tileImage.AddImageQuery = addImageQuery;
            }

            return tileImage;
        }
    }
#pragma warning restore SA1008
#pragma warning restore SA1009
#endif
}