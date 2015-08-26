using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Entity;
using BotaNaRoda.WebApi.Models;
using BotaNaRoda.WebApi.Util;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Linq;
using Thinktecture.IdentityServer.Core.Extensions;

namespace BotaNaRoda.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        private readonly ItemsContext _itemsContext;
        private readonly IOptions<AppSettings> _appSettings;

        public ItemsController(ItemsContext itemsContext, IOptions<AppSettings>  appSettings)
        {
            _itemsContext = itemsContext;
            _appSettings = appSettings;
        }

        // GET: api/items
        [HttpGet]
        public async Task<IEnumerable<ItemListViewModel>> GetAll(double latitude, double longitude, double radius, int offset)
        {
            const double earthRadiusInKm = 6371.009;

            var items = await _itemsContext.Items.Find(new BsonDocument
            {
                { "loc", new BsonDocument
                    {
                        { "$geoWithin", new BsonDocument
                            {
                                { "$centerSphere", new BsonArray { new BsonArray { longitude, latitude }, radius / earthRadiusInKm } }
                            }
                        } 
                    }
                },
                { "status", 0 }
            }).Skip(offset).Limit(20).ToListAsync();
            return items.Select(x => new ItemListViewModel(x));
        }

        // GET api/items/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (item == null)
            {
                return HttpNotFound();
            }

            var user = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();
            return new ObjectResult(new ItemDetailViewModel(item, new UserViewModel(user)));
        }

        // POST api/items
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] ItemCreateBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var item = new Item(model, User.GetSubjectId());
            await _itemsContext.Items.InsertOneAsync(item);
            return Created(Request.Path + "/" + item.Id, null);
        }

        // DELETE api/items/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.GetSubjectId();
            var result = await _itemsContext.Items.DeleteOneAsync(x => x.Id == id && x.UserId == userId);
            //TODO acknowledge result
            //if (result.DeletedCount > 0)
            //{
                return new HttpStatusCodeResult(204); // No content
            //}
            //return HttpNotFound();
        }

        [Route("images")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostItem(IList<IFormFile> files)
        {
            if (!ModelState.IsValid || !files.Any() || files.Count > 3)
            {
                return HttpBadRequest(ModelState);
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_appSettings.Options.StorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("item-imgs");
            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();
            container.SetPermissions(new BlobContainerPermissions{ PublicAccess = BlobContainerPublicAccessType.Blob });

            string imagesId = ObjectId.GenerateNewId().ToString();
            string imagePartUrl = $"{imagesId}/";

            List<ImageInfo> imageInfoList = new List<ImageInfo>();
            foreach (var file in files)
            {
                if (file.Length > 2000000 || file.ContentType != "image/jpeg")
                {
                    return HttpBadRequest("Imagem inválida");
                }

                var imageInfo = new ImageInfo
                {
                    Name = imagePartUrl + $"image_{files.IndexOf(file)}.jpg"
                };

                // Retrieve reference to a blob named "myblob".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageInfo.Name);
                
                // Create or overwrite the "myblob" blob with contents from a local file.
                using (var fs = file.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(fs);
                }

                imageInfo.Url = blockBlob.Uri.ToString();
                imageInfoList.Add(imageInfo);
            }

            //Thumbnail image processing
            var thumbImageInfo = new ImageInfo
            {
                Name = imagePartUrl + "image_thumb.jpg"
            };

            Bitmap thumbBitmap = ImageUtil.ResizeImageProportionally(files.First().OpenReadStream(), 200);
            var thumbMs = new MemoryStream();
            thumbBitmap.Save(thumbMs, ImageFormat.Jpeg);
            thumbMs.Position = 0;

            CloudBlockBlob thumbBlockBlob = container.GetBlockBlobReference(thumbImageInfo.Name);
            using (thumbMs)
            {
                await thumbBlockBlob.UploadFromStreamAsync(thumbMs);
            }

            thumbImageInfo.Url = thumbBlockBlob.Uri.ToString();
            imageInfoList.Add(thumbImageInfo);

            return new ObjectResult(imageInfoList);
        }
    }
}
