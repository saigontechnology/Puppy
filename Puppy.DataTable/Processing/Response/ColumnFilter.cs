using Puppy.Core.EnumUtils;
using Puppy.Core.TypeUtils;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Models.Config.Column;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppy.DataTable.Processing.Response
{
    public class ColumnFilter : Hashtable
    {
        internal object[] FilterValues { set => this[FilterConst.Values] = value; }

        internal string FilterType { set => this[FilterConst.Type] = value; }

        public ColumnFilter(Type t)
        {
            SetDefaultValuesAccordingToColumnType(t);
        }

        private static readonly List<Type> DateTypes = new List<Type> { typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?) };

        private void SetDefaultValuesAccordingToColumnType(Type type)
        {
            if (type == null)
            {
                Remove(FilterConst.Type);
            }
            else if (DateTypes.Contains(type))
            {
                FilterType = FilterConst.Text;
            }
            else if (type == typeof(bool))
            {
                FilterType = FilterConst.Select;
                FilterValues = new object[]
                {
                    DataConst.True, DataConst.False
                };
            }
            else if (type == typeof(bool?))
            {
                FilterType = FilterConst.Select;
                FilterValues = new object[]
                {
                    DataConst.Null, DataConst.True, DataConst.False
                };
            }
            else if (type.IsEnumType() || type.IsNullableEnumType())
            {
                FilterType = FilterConst.Select;
                FilterValues = type.GetEnumValueLabelPair().Select(x => new
                {
                    value = string.IsNullOrWhiteSpace(x.Value) ? DataConst.Null : x.Value,
                    label = x.Label
                }).ToArray<object>();
            }
            else
            {
                FilterType = FilterConst.Text;
            }
        }
    }

    public class ColumnFilter<TTarget>
    {
        private readonly TTarget _target;

        private readonly ColumnModel _columnModel;

        public ColumnFilter(TTarget target, ColumnModel columnModel)
        {
            _target = target;
            _columnModel = columnModel;
        }

        public TTarget Text()
        {
            _columnModel.ColumnFilter.FilterType = FilterConst.Text;
            return _target;
        }

        public TTarget CheckBoxes(params string[] options)
        {
            _columnModel.ColumnFilter.FilterType = FilterConst.Checkbox;
            _columnModel.ColumnFilter.FilterValues = options.Cast<object>().ToArray();
            if (_columnModel.Type.GetTypeInfo().IsEnum)
            {
                _columnModel.ColumnFilter.FilterValues = _columnModel.Type.GetEnumValueLabelPair().Select(x => new
                {
                    value = string.IsNullOrWhiteSpace(x.Value) ? DataConst.Null : x.Value,
                    label = x.Label
                }).ToArray<object>();
            }
            return _target;
        }

        public TTarget Select(params string[] options)
        {
            _columnModel.ColumnFilter.FilterType = FilterConst.Select;
            _columnModel.ColumnFilter.FilterValues = options.Cast<object>().ToArray();
            if (_columnModel.Type.GetTypeInfo().IsEnum)
            {
                _columnModel.ColumnFilter.FilterValues = _columnModel.Type.GetEnumValueLabelPair().Select(x => new
                {
                    value = string.IsNullOrWhiteSpace(x.Value) ? DataConst.Null : x.Value,
                    label = x.Label
                }).ToArray<object>();
            }
            return _target;
        }

        public TTarget Number()
        {
            _columnModel.ColumnFilter.FilterType = FilterConst.Number;
            return _target;
        }

        public TTarget NumberRange()
        {
            _columnModel.ColumnFilter.FilterType = FilterConst.NumberRange;
            return _target;
        }

        public TTarget DateRange()
        {
            _columnModel.ColumnFilter.FilterType = FilterConst.DateRange;
            return _target;
        }

        public TTarget None()
        {
            _columnModel.ColumnFilter = null;
            return _target;
        }
    }
}