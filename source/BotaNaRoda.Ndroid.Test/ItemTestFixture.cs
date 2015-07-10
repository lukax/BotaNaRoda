using System;
using System.IO;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Entity;
using NUnit.Framework;

namespace BotaNaRoda.Ndroid.Test
{
    [TestFixture]
    public class TestsSample
    {
        private IItemDataService _itemService;

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
            Item newItem = new Item();
            newItem.Description = "Luva de neve";
            
            _itemService.SaveItem(newItem);
            string testId = newItem.Id;

            _itemService.RefreshCache();
            Item item = _itemService.GetItem(testId);
            Assert.IsNotNull(item);
            Assert.AreEqual("Luva de neve", item.Description);
        }

        [Test]
        public void UpdateItem()
        {
            Item newItem = new Item();
            newItem.Description = "Luva de neve";

            _itemService.SaveItem(newItem);
            string testId = newItem.Id;

            _itemService.RefreshCache();
            Item item = _itemService.GetItem(testId);
            item.Description = "Luva de neve novíssima";

            _itemService.SaveItem(item);

            _itemService.RefreshCache();

            item = _itemService.GetItem(testId);
            Assert.IsNotNull(item);
            Assert.AreEqual("Luva de neve novíssima", item.Description);
        }

        [Test]
        public void DeleteItem()
        {
            Item newItem = new Item();
            newItem.Description = "Luva de neve";

            _itemService.SaveItem(newItem);
            string testId = newItem.Id;

            _itemService.RefreshCache();
            Item item = _itemService.GetItem(testId);
            Assert.IsNotNull(item);
            _itemService.DeleteItem(item);

            _itemService.RefreshCache();
            item = _itemService.GetItem(testId);
            Assert.Null(item);
        }
    }
}