using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using CachingFramework.Redis;
using CachingFramework.Redis.Contracts.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace EservicesAPI.Auth
{
    public static class RedisCache
    {
        private static readonly string _host = ConfigurationManager.AppSettings["redis_host"];

        private static readonly Lazy<ConnectionMultiplexer> LazyConnection
            = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_host));

        private static ConnectionMultiplexer Connection
        {
            get
            {
                return LazyConnection.Value;
            }
        }

        public static ICacheProvider Current()
        {
            return new RedisContext(Connection, JsonSerializer.GetJsonSerializer).Cache;
        }
    }

    class CustomResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            property.ShouldSerialize = instance =>
            {
                try
                {
                    PropertyInfo prop = (PropertyInfo)member;
                    if (prop.PropertyType.Name != "HttpPostedFileBase")
                    {
                        if (prop.CanRead)
                        {
                            prop.GetValue(instance, null);
                            return true;
                        }
                    }
                }
                catch
                {
                }
                return false;
            };

            return property;
        }
    }

    public class JsonSerializer : CachingFramework.Redis.Contracts.ISerializer
    {
        private static readonly Lazy<JsonSerializer> LazyJsonSerializer
            = new Lazy<JsonSerializer>(() => new JsonSerializer());
        private static JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CustomResolver(),
            Formatting = Formatting.Indented
        };

        public byte[] Serialize(object data)
        {
            string stringified = "";
            try
            {
                stringified = JsonConvert.SerializeObject(data, _settings);
            }
            catch (Exception e)
            {
                var error = e;
                Console.WriteLine(e);
                throw;
            }

            var byteArr = Encoding.UTF8.GetBytes(stringified);
            return byteArr;
        }

        public object Deserialize(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            return
                Newtonsoft.Json.JsonConvert.DeserializeObject(
                    Encoding.UTF8
                        .GetString(
                            data), _settings); //JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), _settings);
        }

        public byte[] Serialize<T>(T data)
        {
            string stringified = "";

            try
            {
                stringified = JsonConvert.SerializeObject(data, typeof(T), _settings);
            }
            catch (Exception e)
            {
                var error = e;
                Console.WriteLine(e);
                throw;
            }

            var byteArr = Encoding.UTF8.GetBytes(stringified);
            return byteArr;
        }
        public T Deserialize<T>(byte[] data)
        {
            if (data == null)
            {
                return default(T);
            }

            return
                JsonConvert.DeserializeObject<T>(
                    Encoding.UTF8
                        .GetString(
                            data), _settings); //JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), _settings);
        }
        public T Deserialize<T>(string data)
        {
            if (data == null)
            {
                return default(T);
            }

            return
                JsonConvert.DeserializeObject<T>(data, _settings); //JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), _settings);
        }

        public static JsonSerializer GetJsonSerializer
        {
            get
            {
                return LazyJsonSerializer.Value;
            }
        }
    }
}