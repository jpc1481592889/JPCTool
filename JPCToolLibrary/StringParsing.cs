using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JPCToolLibrary
{
    /// <summary>
    /// 字符串解析
    /// </summary>
    public class StringParsing
    {
        /// <summary>
        /// 解析XML字符串 解析结果放到字典里，后续可以将字典中的键通过**.Keys.ToArray()放到数组中
        /// </summary>
        /// <param name="xml">需要解析的字符串</param>
        /// <param name="root">字符串的根节点</param>
        /// <returns>result返回解析的结果</returns>
        public static Dictionary<string, string> GetReturn(string xml, string root)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            XmlNodeList nodeList = xdoc.GetElementsByTagName(root);//获取根节点
            //循环xml数据
            foreach (XmlNode xmlNode in nodeList)
            {
                XmlNodeList childList = xmlNode.ChildNodes; //取得根节点下的子节点集合
                foreach (XmlNode item in childList)
                {
                    var val = item.InnerText;//值
                    var name = item.Name;//节点
                    result.Add(name, val);
                }
            }
            return result;
        }
        /// <summary>
        /// 解析Json字符串 Json版本 Newtonsoft.Json.12.0.1 
        /// 解析结果放到字典里，后续可以将字典中的键通过**.Keys.ToArray()放到数组中
        /// </summary>
        /// <param name="JsonString">待解析的Json字符串</param>
        /// <returns>解析结果</returns>
        public static Dictionary<string, string> JsonResult(string JsonString)
        {

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonString);
            }
            catch (Exception ex)
            {
                throw new FormatException($"转换出错：{ex.Message}");
            }
        }
    }
}
