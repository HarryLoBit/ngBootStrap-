using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
namespace WApiService
{
    public class PlainTextTypeFormatter : MediaTypeFormatter
    {
        public PlainTextTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

        public override async Task WriteToStreamAsync(Type type, object value,
            Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            using (var sw = new StreamWriter(writeStream))
            {
                await sw.WriteAsync(value.ToString());
            }
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream,
            HttpContent content, IFormatterLogger formatterLogger)
        {
            using (var sr = new StreamReader(readStream))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}