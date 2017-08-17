#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> RollingLogFile.cs </Name>
//         <Created> 17/08/17 10:29:31 PM </Created>
//         <Key> 5356bac4-17fe-4245-9ca2-24e51d4e65d0 </Key>
//     </File>
//     <Summary>
//         RollingLogFile.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.Logger.RollingFile
{
    internal class RollingLogFile
    {
        public RollingLogFile(string fileName, DateTime dateTime, int sequenceNumber)
        {
            FileName = fileName;
            DateTime = dateTime;
            SequenceNumber = sequenceNumber;
        }

        public string FileName { get; }

        public DateTime DateTime { get; }

        public int SequenceNumber { get; }
    }
}