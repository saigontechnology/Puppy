using Puppy.DataTable.Constants;
using System;
using System.Linq;

namespace Puppy.DataTable.Models.Response
{
    public class ResponseOptionModel
    {
        public virtual ArrayOutputType? ArrayOutputType { get; set; }

        public static ResponseOptionModel<TSource> For<TSource>(IQueryable<TSource> data, Action<ResponseOptionModel<TSource>> setOptions) where TSource : class
        {
            var responseOptions = new ResponseOptionModel<TSource>();

            setOptions(responseOptions);

            return responseOptions;
        }
    }

    public class ResponseOptionModel<TSource> : ResponseOptionModel
    {
        public Func<TSource, object> DtRowId
        {
            get => _dtRowId;
            set
            {
                _dtRowId = value;
                if (value != null)
                {
                    ArrayOutputType = Constants.ArrayOutputType.ArrayOfObjects;
                }
            }
        }

        private Func<TSource, object> _dtRowId;

        public override ArrayOutputType? ArrayOutputType
        {
            get => base.ArrayOutputType;
            set
            {
                if (DtRowId != null && value != Constants.ArrayOutputType.ArrayOfObjects)
                {
                    throw new ArgumentOutOfRangeException(nameof(ArrayOutputType), $"{nameof(ArrayOutputType)} must be {nameof(Constants.ArrayOutputType.ArrayOfObjects)} when {nameof(DtRowId)} is set");
                }
                base.ArrayOutputType = value;
            }
        }
    }
}