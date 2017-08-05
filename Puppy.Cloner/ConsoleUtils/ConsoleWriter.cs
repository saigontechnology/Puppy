#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ConsoleWriter.cs </Name>
//         <Created> 05/08/17 9:38:56 AM </Created>
//         <Key> 50b7cc68-8b3f-48de-868c-e55dbfb6dcb7 </Key>
//     </File>
//     <Summary>
//         ConsoleWriter.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace Puppy.Cloner.ConsoleUtils
{
    public class ConsoleWriter : IConsole
    {
        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        public void Write(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        public int Y
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        public int X
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public int WindowWidth => Console.WindowWidth;

        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public void PrintAt(int x, int y, string format, params object[] args)
        {
            Console.CursorTop = y;
            Console.CursorLeft = x;
            Console.WriteLine(format, args);
        }

        public void PrintAt(int x, int y, string text)
        {
            Console.CursorTop = y;
            Console.CursorLeft = x;
            Console.WriteLine(text);
        }
    }
}