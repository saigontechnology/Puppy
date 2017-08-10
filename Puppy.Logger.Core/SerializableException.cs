#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SerializableException.cs </Name>
//         <Created> 10/08/17 5:58:52 PM </Created>
//         <Key> 69f64980-151f-44db-ba2a-d05775526df7 </Key>
//     </File>
//     <Summary>
//         SerializableException.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Puppy.Logger.Core
{
    [Serializable]
    [DesignerCategory(nameof(Puppy))]
    public class SerializableException
    {
        public string HelpLink { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }

        public string StackTrace { get; set; }

        public Type Type { get; set; }

        public Type BaseType { get; set; }

        public SerializableException InternalException { get; set; }

        public SerializableException()
        {
        }

        public SerializableException(string message) : this()
        {
            Message = message;
        }

        public SerializableException(Exception ex) : this(ex.Message)
        {
            HelpLink = ex.HelpLink;
            Source = ex.Source;
            StackTrace = ex.StackTrace;
            Type = ex.GetType();
            BaseType = ex.GetBaseException()?.GetType();
            InternalException = ex.InnerException != null ? new SerializableException(ex.InnerException) : null;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}