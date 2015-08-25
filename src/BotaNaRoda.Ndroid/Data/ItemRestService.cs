using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BotaNaRoda.Ndroid.Models;
using ModernHttpClient;
using Newtonsoft.Json;
using Xamarin.Auth;

namespace BotaNaRoda.Ndroid.Data
{
    public class ItemRestService : IItemService
    {
        private const string BotaNaRodaItemsEndpoint = "https://botanaroda.azurewebsites.net/api/items";
        private readonly Account _account;
        private readonly HttpClient _httpClient;
        private readonly string _storagePath;

        public ItemRestService(string storagePath, Account account)
        {
            _storagePath = storagePath;

            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);

            _account = account;
            //On android NativeMessageHandler will resolve to OkHttp
            _httpClient = new HttpClient(new NativeMessageHandler());
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", account.Properties["access_token"]);
        }

        public async Task<IEnumerable<ItemListViewModel>> GetAllItems()
        {
            var json = await _httpClient.GetStringAsync(BotaNaRodaItemsEndpoint);
            return JsonConvert.DeserializeObject<IEnumerable<ItemListViewModel>>(json);
        }

        public void RefreshCache()
        {
        }

        public async Task<ItemDetailViewModel> GetItem(string id)
        {
            var json = await _httpClient.GetStringAsync(Path.Combine(BotaNaRodaItemsEndpoint, id));
            return JsonConvert.DeserializeObject<ItemDetailViewModel>(json);
        }

        public async Task<string> SaveItem(ItemCreateBindingModel item)
        {
            if (item.Images == null || item.Images.Length > 3)
            {
                throw new ArgumentException("item.Images");
            }

            var imgs = await UploadImages(item.Images.Select(x => x.Url).ToArray());
            item.ThumbImage = imgs.Last();
            imgs.Remove(imgs.Last());
            item.Images = imgs.ToArray();

            var response = await _httpClient.PostAsync(BotaNaRodaItemsEndpoint, 
                new StringContent(JsonConvert.SerializeObject(item)));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }

        private async Task<IList<ImageInfo>> UploadImages(string[] imgsUrl)
        {
            using (var content = new MultipartFormDataContent())
            {
                foreach (var name in imgsUrl)
                {
                    var fs = new FileStream(Path.Combine(_storagePath, name), FileMode.Open);
                    content.Add(new StreamContent(fs));
                }
                var response = await _httpClient.PostAsync(Path.Combine(BotaNaRodaItemsEndpoint, "images"), content);
                if (response.IsSuccessStatusCode)
                {
                    var imageInfosJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IList<ImageInfo>>(imageInfosJson);
                }
            }
            return null;
        }

        public async Task<bool> DeleteItem(string id)
        {
            var response = await _httpClient.DeleteAsync(Path.Combine(BotaNaRodaItemsEndpoint, id));
            return response.IsSuccessStatusCode;
        }
    }
}