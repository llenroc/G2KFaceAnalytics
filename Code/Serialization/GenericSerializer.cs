using System.IO;
using System.Runtime.Serialization;

namespace K4W.Face.Analytics.Serialization
{
    internal class GenericSerializer<T>
    {
        /// <summary>
        /// Serialize an object to XML
        /// </summary>
        /// <param name="obj">The original object</param>
        /// <returns>XML representation</returns>
        public static string SerializeToString(T obj)
        {
            using (MemoryStream memStm = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memStm, obj);

                memStm.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(memStm))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
