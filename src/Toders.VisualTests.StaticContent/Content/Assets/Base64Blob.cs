using System;
using System.IO;
using EPiServer.Framework.Blobs;

namespace Toders.VisualTests.StaticContent.Content.Assets
{
    public class Base64Blob : Blob
    {
        private readonly string _base64;

        public Base64Blob(Uri id, string base64)
            : base(id)
        {
            _base64 = base64;
        }

        public override Stream OpenRead()
        {
            var base64Image = new Base64Image(_base64);
            return new MemoryStream(base64Image.Bytes);
        }
    }
}