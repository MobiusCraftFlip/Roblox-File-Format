using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RobloxFiles
{
    /// <summary>
    /// Represents a loaded Roblox place/model file.
    /// RobloxFile is an Instance and its children are the contents of the file.
    /// </summary>
    public abstract class RobloxFile : Instance
    {
        protected abstract Task ReadFile(byte[] buffer);

        /// <summary>
        /// Saves this RobloxFile to the provided stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        public abstract void Save(Stream stream);

        /// <summary>
        /// Asynchronously opens a RobloxFile using the provided buffer.
        /// </summary>
        /// <returns>A task which will complete once the file is opened, with the resulting RobloxFile.</returns>
        public static async Task<RobloxFile> OpenAsync(byte[] buffer)
        {
            if (buffer.Length > 14)
            {
                string header = Encoding.UTF7.GetString(buffer, 0, 14);
                RobloxFile file = null;

                if (header == BinaryRobloxFile.MagicHeader)
                    file = new BinaryRobloxFile();
                else if (header.StartsWith("<roblox"))
                    file = new XmlRobloxFile();

                if (file != null)
                {
                    await file.ReadFile(buffer);
                    return file;
                }
            }

            throw new Exception("Unrecognized header!");
        }

        /// <summary>
        /// Asynchronously opens a Roblox file by reading from the provided Stream.
        /// </summary>
        /// <param name="stream">The stream to read the Roblox file from.</param>
        /// <returns>A task which will complete once the file is opened, with the resulting RobloxFile.</returns>
        public static async Task<RobloxFile> OpenAsync(Stream stream)
        {
            byte[] buffer;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                buffer = memoryStream.ToArray();
            }

            return await OpenAsync(buffer);
        }

        /// <summary>
        /// Opens a Roblox file from a provided file path.
        /// </summary>
        /// <param name="filePath">A path to a Roblox file to be opened.</param>
        /// <returns>A task which will complete once the file is opened, with the resulting RobloxFile.</returns>
        public static async Task<RobloxFile> OpenAsync(string filePath)
        {
            byte[] buffer = File.ReadAllBytes(filePath);
            return await OpenAsync(buffer);
        }

        /// <summary>
        /// Synchronously opens a Roblox file from the provided buffer.</param>
        /// <returns>The opened RobloxFile.</returns>
        public static RobloxFile Open(byte[] buffer)
        {
            var fileTask = OpenAsync(buffer);
            fileTask.Wait();

            return fileTask.Result;
        }

        /// <summary>
        /// Synchronously opens a Roblox file from the provided stream.
        /// </summary>
        /// <param name="stream">The stream to read the Roblox file from.</param>
        /// <returns>The opened RobloxFile.</returns>
        public static RobloxFile Open(Stream stream)
        {
            var fileTask = OpenAsync(stream);
            fileTask.Wait();

            return fileTask.Result;
        }

        /// <summary>
        /// Synchronously opens a Roblox file from a provided file path.
        /// </summary>
        /// <param name="filePath">A path to a Roblox file to be opened.</param>
        /// <returns>The opened RobloxFile.</returns>
        public static RobloxFile Open(string filePath)
        {
            var fileTask = OpenAsync(filePath);
            fileTask.Wait();

            return fileTask.Result;
        }

        /// <summary>
        /// Saves this RobloxFile to the provided file path.
        /// </summary>
        /// <param name="filePath">A path to where the file should be saved.</param>
        public void Save(string filePath)
        {
            using (FileStream stream = File.OpenWrite(filePath))
            {
                Save(stream);
            }
        }

        /// <summary>
        /// Asynchronously saves this RobloxFile to the provided stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <returns>A task which will complete upon the save's completion.</returns>
        public Task SaveAsync(Stream stream)
        {
            return Task.Run(() => Save(stream));
        }

        /// <summary>
        /// Asynchronously saves this RobloxFile to the provided file path.
        /// </summary>
        /// <param name="filePath">A path to where the file should be saved.</param>
        /// <returns>A task which will complete upon the save's completion.</returns>
        public Task SaveAsync(string filePath)
        {
            return Task.Run(() => Save(filePath));
        }
    }
}
