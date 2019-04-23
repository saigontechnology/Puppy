using System.Collections.Generic;
using System.IO;
using Renci.SshNet;

namespace Puppy.Core.SFTPUltils
{
    public static class SFTPUtils
    {
        /// <summary>
        ///     Upload file to sftp
        /// </summary>
        /// <param name="host">IP of SFTP</param>
        /// <param name="privateKeyPath">Private key path. Must be .ppk</param>
        /// <param name="username">Username to authenticate</param>
        /// <param name="password">Password to authenticate</param>
        /// <param name="sourcePaths">Files to upload</param>
        /// <param name="destinationPath">Destination folder</param>
        /// <param name="port">Port</param>
        /// <param name="bufferSize">Buffer size</param>
        public static void UploadFiles(string host, string privateKeyPath, string username, string password, string[] sourcePaths, string destinationPath, int port = 22, uint bufferSize = 4*2014)
        {
            var methods = new List<AuthenticationMethod> {new PasswordAuthenticationMethod(username, password)};

            // Add private key
            if (!string.IsNullOrEmpty(privateKeyPath))
            {
                var keyFile = new PrivateKeyFile(privateKeyPath);
                var keyFiles = new[] { keyFile };

                methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));
            }

            var con = new ConnectionInfo(host, port, username, methods.ToArray());

            using (var client = new SftpClient(con))
            {
                client.Connect();
                client.ChangeDirectory(destinationPath);

                foreach (var sourcePath in sourcePaths)
                {
                    if(!File.Exists(sourcePath)) continue;

                    using (var fs = new FileStream(sourcePath, FileMode.Open))
                    {
                        client.BufferSize = bufferSize;
                        client.UploadFile(fs, Path.GetFileName(sourcePath), true);
                    }
                }

                client.Disconnect();
            }
        }
    }
}
