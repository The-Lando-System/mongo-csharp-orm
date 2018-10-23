using System;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoOrm
{
    [DataContract]
    public class MongoModel
    {
        [DataMember]
        [BsonId]
        public string Id { get; set; }

        [DataMember]
        public DateTime LastModified { get; set; }
    }
}
