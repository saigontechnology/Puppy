#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> StringWriterWithEncoding.cs </Name>
//         <Created> 07/06/2017 9:56:26 PM </Created>
//         <Key> 22358aaa-c2d5-4662-a4cb-418ce329350f </Key>
//     </File>
//     <Summary>
//         StringWriterWithEncoding.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.IO;
using System.Text;

namespace Puppy.Core.StringUtils
{
    /// <summary>
    ///     <para>
    ///         The <see cref="StringWriter" /> class always outputs UTF-16 encoded strings. To use a
    ///         different encoding, we must inherit from <see cref="StringWriter" />.
    ///     </para>
    ///     <para> See http://stackoverflow.com/questions/9459184/why-is-the-xmlwriter-always-outputing-utf-16-encoding. </para>
    /// </summary>
    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringWriterWithEncoding" /> class. 
        /// </summary>
        public StringWriterWithEncoding()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringWriterWithEncoding" /> class. 
        /// </summary>
        /// <param name="formatProvider">
        ///     An <see cref="T:System.IFormatProvider" /> object that controls formatting.
        /// </param>
        public StringWriterWithEncoding(IFormatProvider formatProvider)
            : base(formatProvider)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringWriterWithEncoding" /> class. 
        /// </summary>
        /// <param name="stringBuilder"> The string builder to write to. </param>
        public StringWriterWithEncoding(StringBuilder stringBuilder)
            : base(stringBuilder)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringWriterWithEncoding" /> class. 
        /// </summary>
        /// <param name="stringBuilder">  The string builder to write to. </param>
        /// <param name="formatProvider"> The format provider. </param>
        public StringWriterWithEncoding(StringBuilder stringBuilder, IFormatProvider formatProvider)
            : base(stringBuilder, formatProvider)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringWriterWithEncoding" /> class. 
        /// </summary>
        /// <param name="encoding"> The encoding. </param>
        public StringWriterWithEncoding(Encoding encoding)
        {
            this._encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringWriterWithEncoding" /> class. 
        /// </summary>
        /// <param name="formatProvider"> The format provider. </param>
        /// <param name="encoding">       The encoding. </param>
        public StringWriterWithEncoding(IFormatProvider formatProvider, Encoding encoding)
            : base(formatProvider)
        {
            this._encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringWriterWithEncoding" /> class. 
        /// </summary>
        /// <param name="stringBuilder"> The string builder to write to. </param>
        /// <param name="encoding">      The encoding. </param>
        public StringWriterWithEncoding(StringBuilder stringBuilder, Encoding encoding)
            : base(stringBuilder)
        {
            this._encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringWriterWithEncoding" /> class. 
        /// </summary>
        /// <param name="stringBuilder">  The string builder to write to. </param>
        /// <param name="formatProvider"> The format provider. </param>
        /// <param name="encoding">       The encoding. </param>
        public StringWriterWithEncoding(StringBuilder stringBuilder, IFormatProvider formatProvider, Encoding encoding)
            : base(stringBuilder, formatProvider)
        {
            this._encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        ///     Gets the <see cref="T:System.Text.Encoding" /> in which the output is written. 
        /// </summary>
        public override Encoding Encoding => _encoding ?? base.Encoding;
    }
}