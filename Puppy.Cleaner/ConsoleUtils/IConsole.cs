#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IConsole.cs </Name>
//         <Created> 05/08/17 9:37:51 AM </Created>
//         <Key> 2aa95f03-3668-4302-a895-4c268407a442 </Key>
//     </File>
//     <Summary>
//         IConsole.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.Cleaner.ConsoleUtils
{
    public interface IConsole
    {
        int WindowWidth { get; }

        ConsoleColor ForegroundColor { get; set; }

        int Y { get; set; }
        int X { get; set; }

        void WriteLine(string format, params object[] args);

        void Write(string format, params object[] args);

        void PrintAt(int x, int y, string format, params object[] args);

        void PrintAt(int x, int y, string text);
    }
}