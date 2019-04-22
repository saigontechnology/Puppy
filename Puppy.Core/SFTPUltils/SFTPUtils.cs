#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> StringHelper.cs </Name>
//         <Created> 24 Apr 17 10:44:55 PM </Created>
//         <Key> dbf0d9b6-5968-47f0-b083-8375bb027161 </Key>
//     </File>
//     <Summary>
//         StringHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System.IO;
using Renci.SshNet;

namespace Puppy.Core.SFTPUltils
{
    public static class SFTPUtils
    {
        /// <summary>
        ///     Upload file to sftp
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="port"></param>
        public static void UploadFile(string host, string username, string password, string sourcePath, string destinationPath, int port)
        {
            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                client.ChangeDirectory(destinationPath);

                using (var fs = new FileStream(sourcePath, FileMode.Open))
                {
                    client.BufferSize = 4 * 1024;
                    client.UploadFile(fs, Path.GetFileName(sourcePath));
                }
            }
        }
    }
}
