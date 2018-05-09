using System;
using System.Collections.Generic;
using System.IO;
using WebService.Helpers.Exceptions;

namespace WebService.Helpers.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToBytes(this Stream This, int maxFileSize = int.MaxValue)
        {
            using (This)
            {
                if (!This.CanRead)
                    throw new Exception("cannot read stream");

                if (This.Length > maxFileSize)
                    throw new FileToLargeException(maxFileSize);

                var bytes = new List<byte>((int) This.Length);
                for (var i = 0; i < This.Length; i++)
                    bytes.Add((byte) This.ReadByte());

                return bytes.ToArray();
            }
        }
    }
}