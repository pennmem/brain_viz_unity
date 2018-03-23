using UnityEngine;
using System.IO;

namespace TriLib {
    /// <summary>
    /// Contains stream helper functions.
    /// </summary>
    public class StreamUtils : MonoBehaviour
    {
        /// <summary>
        /// Reads a full stream contents.
        /// </summary>
        /// <returns>The stream contents.</returns>
        /// <param name="input">Inpu stream.</param>
        public static byte[] ReadFullStream(Stream stream)
        {
            var buffer = new byte[4096];
            using (var memoryStream = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }
                return memoryStream.ToArray();
            }
        }
    }
}

