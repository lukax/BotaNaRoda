using System.Collections.Generic;
using System.IO;
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
            var json = await _httpClient.GetStringAsync(BotaNaRodaItemsEndpoint + "/" + id);
            return JsonConvert.DeserializeObject<ItemDetailViewModel>(json);
        }

        public async Task<string> SaveItem(ItemCreateBindingModel item)
        {
            var response = await _httpClient.PostAsync(BotaNaRodaItemsEndpoint, new StringContent(JsonConvert.SerializeObject(item)));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }

        public async Task<bool> DeleteItem(string id)
        {
            var response = await _httpClient.DeleteAsync(BotaNaRodaItemsEndpoint + "/" + id);
            return response.IsSuccessStatusCode;
        }

        public string GetImageFileName(string id)
		{
			return Path.Combine(_storagePath, string.Format("itemImg_{0}.jpg", id));
        }
    }
}