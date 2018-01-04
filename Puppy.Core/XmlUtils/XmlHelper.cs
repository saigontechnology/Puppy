#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> XmlHelper.cs </Name>
//         <Created> 17/07/17 4:06:29 PM </Created>
//         <Key> 25011871-5d25-41ee-85c9-21120cd75d17 </Key>
//     </File>
//     <Summary>
//         XmlHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Puppy.Core.XmlUtils
{
    /// <summary>
    ///     Xml helper 
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        ///     To serialize string XML to object 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"> The specified file cannot be found. </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     Part of the filename or directory cannot be found.
        /// </exception>
        public static T FromXmlString<T>(string source)
        {
            using (var sr = new StringReader(source))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(sr);
            }
        }

        /// <summary>
        ///     Get xml <see langword="object" /> by <paramref name="path" /> 
        /// </summary>
        /// <returns></returns>
        public static T Read<T>(string path)
        {
            return FromXmlString<T>(path);
        }

        /// <summary>
        ///     To serialize <see langword="object" /> to string XML 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="isRemoveNameSpace"></param>
        /// <returns></returns>
        public static string ToXmlString<T>(T objectToSerialize, bool isRemoveNameSpace = false)
        {
            using (var stream = new MemoryStream())
            {
                XmlWriter writer;

                var xmlSerializer = new XmlSerializer(typeof(T));

                if (isRemoveNameSpace)
                {
                    var settings = new XmlWriterSettings
                    {
                        Indent = false,
                        OmitXmlDeclaration = true
                    };

                    var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

                    writer = XmlWriter.Create(stream, settings);

                    xmlSerializer.Serialize(writer, objectToSerialize, emptyNamepsaces);
                }
                else
                {
                    writer = XmlWriter.Create(stream);

                    xmlSerializer.Serialize(writer, objectToSerialize);
                }

                return Encoding.UTF8.GetString(stream.ToArray(), 0, Convert.ToInt32(stream.Length));
            }
        }

        /// <summary>
        ///     Convert the object to json string, then use <paramref name="rootElementName" /> as
        ///     root xml to de-serialize to xml string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="rootElementName">   default value is "Root" </param>
        /// <param name="isRemoveNameSpace"></param>
        /// <returns></returns>
        public static string ToXmlStringViaJson<T>(T objectToSerialize, string rootElementName = "Root", bool isRemoveNameSpace = false)
        {
            var json = JsonConvert.SerializeObject(objectToSerialize);

            XmlDocument doc = JsonConvert.DeserializeXmlNode(json, rootElementName);

            using (var stringWriter = new StringWriter())
            {
                XmlWriter writer;

                if (isRemoveNameSpace)
                {
                    var settings = new XmlWriterSettings
                    {
                        Indent = false,
                        OmitXmlDeclaration = true
                    };

                    writer = XmlWriter.Create(stringWriter, settings);
                }
                else
                {
                    writer = XmlWriter.Create(stringWriter);
                }

                doc.WriteTo(writer);

                writer.Flush();

                return stringWriter.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        ///     Update backup
        ///     <paramref name="data" /><see langword="object" /><see langword="override" /> to xml file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public static void Write(dynamic data, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fileStream))
                {
                    string xmlStr = ToXmlString(data);
                    sw.WriteLine(xmlStr);
                }
            }
        }
    }
}