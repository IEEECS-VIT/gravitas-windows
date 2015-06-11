using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Runtime.Serialization;
using GravitasSDK.DataModel;
using System.IO;
using System.Xml;


namespace GravitasSDK.Providers
{
    public static class ContentSerializer
    {

        #region Public Methods (API)

        public static async Task<bool> TryWriteEventsAsync(StorageFile outFile, IEnumerable<Event> events)
        {
            return await TryWriteAsync(outFile, events);
        }

        public static async Task<IEnumerable<Event>> ParseEventsAsync(StorageFile inputFile)
        {
            return await DeserializeContent<IEnumerable<Event>>(inputFile);
        }

        public static async Task<bool> TrySaveChecklistsAsync<T>(StorageFile outFile, FilterCriteria<T> filterCriteria)
        {
            XmlWriter writer = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.CloseOutput = false;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                writer = XmlWriter.Create(sb, settings);

                foreach (dynamic filter in filterCriteria)
                    WriteXmlLine(writer, filter.InternalChecklist);
                writer.Flush();
                await FileIO.WriteTextAsync(outFile, sb.ToString());

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }
        }

        public static async Task<bool> TryRestoreChecklistsAsync<T>(StorageFile inputFile, FilterCriteria<T> filterCriteria)
        {
            bool result;
            try
            {
                IList<string> xmlStrings = await FileIO.ReadLinesAsync(inputFile);
                for (int i = 0; i < filterCriteria.Count; i++)
                {
                    dynamic filter = filterCriteria[i];
                    filter.InternalChecklist = DeserializeXmlString(xmlStrings[i], filter.InternalChecklist.GetType());
                }
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        #endregion

        #region Private Helpers

        private static void WriteXmlLine(XmlWriter writer, object contentGraph)
        {
            DataContractSerializer ser = new DataContractSerializer(contentGraph.GetType());
            ser.WriteObject(writer, contentGraph);
            writer.WriteWhitespace("\r\n");
        }

        private static async Task<bool> TryWriteAsync(StorageFile outFile, object contentGraph)
        {
            Stream writeStream = null;
            bool result;
            try
            {
                DataContractSerializer ser = new DataContractSerializer(contentGraph.GetType());
                writeStream = await outFile.OpenStreamForWriteAsync();
                ser.WriteObject(writeStream, contentGraph);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (writeStream != null)
                    writeStream.Dispose();
            }
            return result;
        }

        private static async Task<T> DeserializeContent<T>(StorageFile inputFile)
        {
            Stream readStream = null;
            try
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                readStream = await inputFile.OpenStreamForReadAsync();
                T content = (T)ser.ReadObject(readStream);
                return content;
            }
            catch
            {
                return default(T);
            }
            finally
            {
                if (readStream != null)
                    readStream.Dispose();
            }
        }

        private static object DeserializeXmlString(string xmlString, Type expectedType)
        {
            XmlReader reader = null;
            try
            {
                DataContractSerializer ser = new DataContractSerializer(expectedType);
                reader = XmlReader.Create(new StringReader(xmlString));
                return ser.ReadObject(reader);
            }
            catch
            {
                return default(object);
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }
        }

        #endregion

    }
}
