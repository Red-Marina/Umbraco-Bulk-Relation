using RedMarina.Umbraco.BulkRelation.Interfaces;
using System.IO;
using System.Text;

namespace RedMarina.Umbraco.BulkRelation.Application.Services
{
    public class EmbeddedResourceProvider : IEmbeddedResourceProvider
    {
        public string ReadEmbeddedResource(string path)
        {
            string result = string.Empty;
            using (var stream = GetType().Assembly.GetManifestResourceStream(path))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
    }
}