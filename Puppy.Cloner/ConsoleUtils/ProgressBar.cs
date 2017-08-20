#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ProgressBar.cs </Name>
//         <Created> 05/08/17 9:36:50 AM </Created>
//         <Key> dbb5d72b-0a9c-4f2a-8d67-99acbbdb9377 </Key>
//     </File>
//     <Summary>
//         ProgressBar.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.Cloner.ConsoleUtils
{
    public class ProgressBar
    {
        private readonly int _max;
        private readonly char _character;
        private readonly IConsole _console;
        private readonly string _format;
        private readonly int _y;
        private int _current;
        private readonly ConsoleColor _consoleColor;
        private readonly ConsoleColor _percentConsoleColor;

        public const string DefaultFormat = "Item {0,-5} of {1,-5}. ({2,-3}%) ";

        public ProgressBar(int max) : this(max, '#', DefaultFormat, ConsoleColor.Green, new ConsoleWriter())
        {
        }

        public ProgressBar(int max, IConsole console) : this(max, '#', DefaultFormat, ConsoleColor.Green, console)
        {
        }

        public ProgressBar(int max, char character, string format, ConsoleColor percentColor, IConsole console)
        {
            _console = console;
            _y = _console.Y;
            _consoleColor = _console.ForegroundColor;
            _percentConsoleColor = percentColor;
            _current = 0;
            _max = max;
            _character = character;
            _format = format ?? DefaultFormat;
        }

        public void Refresh(int current, string format, params object[] args)
        {
            var item = string.Format(format, args);
            Refresh(current, item);
        }

        private static readonly object Locker = new object();

        public void Refresh(int current, string item)
        {
            lock (Locker)
            {
                // Save current position
                _current = current;

                try
                {
                    // %
                    var percent = current / (float)_max;

                    // Text Write
                    var line = string.Format(_format, current, _max, (int)(percent * 100));

                    // Percent Write Length of percent is console - line
                    var lineLength = item.Length;
                    while (lineLength > _console.WindowWidth)
                    {
                        lineLength -= _console.WindowWidth;
                    }

                    int percentBarMaxLength = _console.WindowWidth - line.Length;
                    int currentBarLength = (int)(percentBarMaxLength * percent);
                    var bar = new string(_character, currentBarLength);

                    // Write Process
                    _console.Y = _y;
                    _console.ForegroundColor = _consoleColor;
                    _console.Write(line);
                    _console.ForegroundColor = _percentConsoleColor;
                    _console.WriteLine(bar);
                    _console.ForegroundColor = _consoleColor;
                    _console.WriteLine(item.PadRight(_console.WindowWidth - 2));
                }
                finally
                {
                    _console.ForegroundColor = _consoleColor;
                }
            }
        }

        public void Next(string item)
        {
            _current++;
            Refresh(_current, item);
        }
    }
}