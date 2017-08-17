#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> IoErrors.cs </Name>
//         <Created> 17/08/17 10:24:20 PM </Created>
//         <Key> 67236459-cf93-46e8-9046-022181f6d868 </Key>
//     </File>
//     <Summary>
//         IoErrors.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.IO;

namespace Puppy.Logger.RollingFile
{
    internal static class IoErrors
    {
        public static bool IsLockedFile(IOException ex)
        {
#if HRESULTS
            var errorCode = System.Runtime.InteropServices.Marshal.GetHRForException(ex) & ((1 << 16) - 1);
            return errorCode == 32 || errorCode == 33;
#else
            return true;
#endif
        }
    }
}