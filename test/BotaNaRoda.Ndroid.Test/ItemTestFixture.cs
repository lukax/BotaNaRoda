using System;
using System.IO;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using NUnit.Framework;

namespace BotaNaRoda.Ndroid.Test
{
    [TestFixture]
    public class TestsSample
    {
        private IItemService _itemService;

        [SetUp]
        public void Setup()
        {
            var storagePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _itemService = new ItemJsonService(storagePath);

            foreach (var fileName in Directory.EnumerateFiles(storagePath, "*.json"))
            {
                File.Delete(fileName);
            }
        }

        [Test]
        public void CreateItem()
        {
            ItemCreateBindingModel newItem = new ItemCreateBindingModel {Description = "Luva de neve"};

            string id = _itemService.SaveItem(newItem).Result;

            _itemService.RefreshCache();
            ItemDetailViewModel item = _itemService.GetItem(id).Result;
            Assert.IsNotNull(item);
            Assert.AreEqual("Luva de neve", item.Description);
        }

        [Test]
        public void UpdateItem()
        {
            ItemCreateBindingModel newItem = new ItemCreateBindingModel {Description = "Luva de neve"};

            string testId = _itemService.SaveItem(newItem).Result;

            _itemService.RefreshCache();
            ItemDetailViewModel item = _itemService.GetItem(testId).Result;
            item.Description = "Luva de neve novíssima";

            _itemService.SaveItem(new ItemCreateBindingModel
            {
                Description = item.Description
            });

            _itemService.RefreshCache();

            item = _itemService.GetItem(testId).Result;
            Assert.IsNotNull(item);
            Assert.AreEqual("Luva de neve novíssima", item.Description);
        }

        [Test]
        public void DeleteItem()
        {
            ItemCreateBindingModel newItem = new ItemCreateBindingModel {Description = "Luva de neve"};

            string testId = _itemService.SaveItem(newItem).Result;

            _itemService.RefreshCache();
            ItemDetailViewModel item = _itemService.GetItem(testId).Result;
            Assert.IsNotNull(item);
            _itemService.DeleteItem(item.Id);

            _itemService.RefreshCache();
            item = _itemService.GetItem(testId).Result;
            Assert.Null(item);
        }
    }
}