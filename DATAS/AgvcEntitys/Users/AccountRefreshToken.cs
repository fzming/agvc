using System;
using CoreData.Core;
using MongoDB.Bson.Serialization.Attributes;

namespace AgvcEntitys.Users
{
    public class AccountRefreshToken : MongoEntity
    {

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime IssuedUtc { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ExpiresUtc { get; set; }

        //==========================================

        public string refresh_token { get; set; }
        public string refresh_ticket { get; set; }


        public string IdentityName { get; set; }
    }
}