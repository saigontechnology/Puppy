#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LoggerModel.cs </Name>
//         <Created> 10/08/17 5:58:52 PM </Created>
//         <Key> 69f64980-151f-44db-ba2a-d05775526df7 </Key>
//     </File>
//     <Summary>
//         LoggerModel.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using System;

namespace Puppy.Logger
{
    [Serializable]
    public class LoggerModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Level { get; set; }

        public DateTimeOffset TimeStamp { get; set; } = DateTime.Now;

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public LoggerModel()
        {
        }

        public LoggerModel(string message) : this()
        {
            Message = message;
        }

        public LoggerModel(Exception ex) : this(ex.Message)
        {
            StackTrace = ex.StackTrace;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}