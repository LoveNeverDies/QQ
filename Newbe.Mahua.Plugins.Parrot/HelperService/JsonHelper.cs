using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Newbe.Mahua.Plugins.Parrot.Helper
{
    public class JsonModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    interface IJsonHelper
    {
        /// <summary>
        /// 读取json节点数据 返回通用model list
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        List<JsonModel> ReadJsonByList(string nodeName);
        /// <summary>
        /// 读取json节点数据 返回通用model
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        JsonModel ReadJsonByModel(string nodeName);
        /// <summary>
        /// 读取json节点数据 返回string
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        string ReadJsonByString(string nodeName);
    }
    class JsonHelper : IJsonHelper
    {
        readonly string JsonPath = "../../../../config.json";

        JToken ReadJson(string nodeName)
        {
            using (System.IO.StreamReader file = System.IO.File.OpenText(JsonPath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    return o[nodeName];
                }
            }
        }

        /// <summary>
        /// 读取json节点数据 返回通用model list
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        List<JsonModel> IJsonHelper.ReadJsonByList(string nodeName)
        {
            var res = ReadJson(nodeName);
            List<JsonModel> jsonModel = new List<JsonModel>();
            foreach (var item in res)
            {
                jsonModel.Add(new JsonModel
                {
                    Key = item["Key"].ToString(),
                    Value = item["Value"].ToString()
                });
            }
            return jsonModel;
        }

        /// <summary>
        /// 读取json节点数据 返回通用model
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        JsonModel IJsonHelper.ReadJsonByModel(string nodeName)
        {
            var res = ReadJson(nodeName);
            JsonModel jsonModel = new JsonModel
            {
                Key = res["Key"].ToString(),
                Value = res["Value"].ToString()
            };
            return jsonModel;
        }

        /// <summary>
        /// 读取json节点数据 返回string
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        string IJsonHelper.ReadJsonByString(string nodeName)
        {
            return ReadJson(nodeName).ToString();
        }
    }
}
