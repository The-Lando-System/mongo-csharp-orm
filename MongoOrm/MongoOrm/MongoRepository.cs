using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace MongoOrm
{
    public class MongoRepository<T> where T : MongoModel
    {
        internal MongoClient Client;
        internal IMongoDatabase Database;
        internal IMongoCollection<T> Collection;

        public MongoRepository()
        {
            Client = new MongoClient(Environment.GetEnvironmentVariable(ConfigurationManager.AppSettings["ConnectionEnvVar"]));
            Database = Client.GetDatabase(ConfigurationManager.AppSettings["DatabaseName"]);
            Collection = Database.GetCollection<T>(typeof(T).Name);
        }

        public virtual T Add(T item)
        {
            item.Id = Guid.NewGuid().ToString();
            item.LastModified = DateTime.UtcNow;
            Collection.InsertOne(item);
            return item;
        }

        public virtual bool Delete(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            DeleteResult dr = Collection.DeleteOne(filter);
            return dr.IsAcknowledged && dr.DeletedCount > 0;
        }

        public virtual IEnumerable<T> FindAll()
        {
            return Collection.Find(new BsonDocument()).ToList();
        }

        public virtual T FindById(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return Collection.Find(filter).FirstOrDefault();
        }

        public virtual T FindByProperty(string propName, object propValue)
        {
            var filter = Builders<T>.Filter.Eq(propName, propValue);
            return Collection.Find(filter).FirstOrDefault();
        }

        public virtual bool Update(string id, T item)
        {
            var findFilter = Builders<T>.Filter.Eq("_id", id);
            var updateDef = Builders<T>.Update.Set("LastModified", DateTime.UtcNow);

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo propInfo in properties)
            {
                if (propInfo.Name != "Id" && propInfo.Name != "LastModified")
                {
                    updateDef = updateDef.Set(propInfo.Name, propInfo.GetValue(item));
                }
            }

            UpdateResult ur = Collection.UpdateOne(findFilter, updateDef);

            return ur.IsAcknowledged && ur.ModifiedCount == 1;
        }
    }
}
