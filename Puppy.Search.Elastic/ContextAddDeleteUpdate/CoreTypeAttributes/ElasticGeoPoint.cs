using System;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticGeoPoint : ElasticGeoTypeAttribute
    {
        private string _fieldDataPrecision;
        private bool _fieldDataPrecisionSet;
        private bool _geohash;
        private string _geohashPrecision;
        private bool _geohashPrecisionSet;
        private bool _geohashPrefix;
        private bool _geohashPrefixSet;
        private bool _geohashSet;
        private bool _latLon;
        private bool _latLonSet;
        private bool _normalize;
        private bool _normalizeLat;
        private bool _normalizeLatSet;
        private bool _normalizeLon;
        private bool _normalizeLonSet;
        private bool _normalizeSet;
        private int _precisionStep;
        private bool _precisionStepSet;
        private bool _validate;
        private bool _validateLat;
        private bool _validateLatSet;
        private bool _validateLon;
        private bool _validateLonSet;
        private bool _validateSet;

        /// <summary>
        ///     lat_lon Set to true to also index the .lat and .lon as fields. Defaults to false. 
        /// </summary>
        public virtual bool LatLon
        {
            get => _latLon;
            set
            {
                _latLon = value;
                _latLonSet = true;
            }
        }

        /// <summary>
        ///     geohash Set to true to also index the .geohash as a field. Defaults to false. 
        /// </summary>
        public virtual bool Geohash
        {
            get => _geohash;
            set
            {
                _geohash = value;
                _geohashSet = true;
            }
        }

        /// <summary>
        ///     geohash_precision Sets the geohash precision. It can be set to an absolute geohash
        ///     length or a distance value (eg 1km, 1m, 1ml) defining the size of the smallest cell.
        ///     Defaults to an absolute length of 12.
        /// </summary>
        public virtual string GeohashPrecision
        {
            get => _geohashPrecision;
            set
            {
                _geohashPrecision = value;
                _geohashPrecisionSet = true;
            }
        }

        /// <summary>
        ///     geohash_prefix If this option is set to true, not only the geohash but also all its
        ///     parent cells (true prefixes) will be indexed as well. The number of terms that will
        ///     be indexed depends on the geohash_precision. Defaults to false. Note: This option
        ///     implicitly enables geohash.
        /// </summary>
        public virtual bool GeohashPrefix
        {
            get => _geohashPrefix;
            set
            {
                _geohashPrefix = value;
                _geohashPrefixSet = true;
            }
        }

        /// <summary>
        ///     validate Set to true to reject geo points with invalid latitude or longitude (default
        ///     is false).
        ///     Note: Validation only works when normalization has been disabled.
        /// </summary>
        public virtual bool Validate
        {
            get => _validate;
            set
            {
                _validate = value;
                _validateSet = true;
            }
        }

        /// <summary>
        ///     validate_lat Set to true to reject geo points with an invalid latitude. 
        /// </summary>
        public virtual bool ValidateLat
        {
            get => _validateLat;
            set
            {
                _validateLat = value;
                _validateLatSet = true;
            }
        }

        /// <summary>
        ///     validate_lon Set to true to reject geo points with an invalid longitude. 
        /// </summary>
        public virtual bool ValidateLon
        {
            get => _validateLon;
            set
            {
                _validateLon = value;
                _validateLonSet = true;
            }
        }

        /// <summary>
        ///     normalize Set to true to normalize latitude and longitude (default is true). 
        /// </summary>
        public virtual bool Normalize
        {
            get => _normalize;
            set
            {
                _normalize = value;
                _normalizeSet = true;
            }
        }

        /// <summary>
        ///     normalize_lat Set to true to normalize latitude. 
        /// </summary>
        public virtual bool NormalizeLat
        {
            get => _normalizeLat;
            set
            {
                _normalizeLat = value;
                _normalizeLatSet = true;
            }
        }

        /// <summary>
        ///     normalize_lon Set to true to normalize longitude. 
        /// </summary>
        public virtual bool NormalizeLon
        {
            get => _normalizeLon;
            set
            {
                _normalizeLon = value;
                _normalizeLonSet = true;
            }
        }

        /// <summary>
        ///     precision_step The precision step (influences the number of terms generated for each
        ///     number value) for .lat and .lon fields if lat_lon is set to true. Defaults to 16.
        /// </summary>
        public virtual int PrecisionStep
        {
            get => _precisionStep;
            set
            {
                _precisionStep = value;
                _precisionStepSet = true;
            }
        }

        /// <summary>
        ///     "fielddata" : { "format" : "compressed", "precision" : "1cm" } By default, geo points
        ///     use the array format which loads geo points into two parallel double arrays, making
        ///     sure there is no precision loss. However, this can require a non-negligible amount of
        ///     memory (16 bytes per
        ///     document) which is why Elastic also provides a field data implementation with lossy
        ///               compression called compressed. This field data format comes with a
        ///               precision option which allows to configure how much precision can be traded
        ///               for memory. The default value is 1cm. The following table presents values
        ///               of the memory savings given various precisions: Precision Bytes per point
        ///               Size reduction 1km 4 75% 3m 6 62.5% 1cm 8 50% 1mm 10 37.5%
        /// </summary>
        public virtual string FieldDataPrecision
        {
            get => _fieldDataPrecision;
            set
            {
                _fieldDataPrecision = value;
                _fieldDataPrecisionSet = true;
            }
        }

        public override string JsonString()
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", "geo_point", elasticCrudJsonWriter);
            JsonHelper.WriteValue("lat_lon", _latLon, elasticCrudJsonWriter, _latLonSet);
            JsonHelper.WriteValue("geohash", _geohash, elasticCrudJsonWriter, _geohashSet);
            JsonHelper.WriteValue("geohash_precision", _geohashPrecision, elasticCrudJsonWriter, _geohashPrecisionSet);
            JsonHelper.WriteValue("geohash_prefix", _geohashPrefix, elasticCrudJsonWriter, _geohashPrefixSet);

            JsonHelper.WriteValue("validate", _validate, elasticCrudJsonWriter, _validateSet);
            JsonHelper.WriteValue("validate_lat", _validateLat, elasticCrudJsonWriter, _validateLatSet);
            JsonHelper.WriteValue("validate_lon", _validateLon, elasticCrudJsonWriter, _validateLonSet);
            JsonHelper.WriteValue("normalize", _normalize, elasticCrudJsonWriter, _normalizeSet);
            JsonHelper.WriteValue("normalize_lat", _normalizeLat, elasticCrudJsonWriter, _normalizeLatSet);
            JsonHelper.WriteValue("normalize_lon", _normalizeLon, elasticCrudJsonWriter, _normalizeLonSet);
            JsonHelper.WriteValue("precision_step", _precisionStep, elasticCrudJsonWriter, _precisionStepSet);

            if (_fieldDataPrecisionSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("fielddata");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                JsonHelper.WriteValue("format", "compressed", elasticCrudJsonWriter);
                JsonHelper.WriteValue("precision", _fieldDataPrecision, elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            WriteBaseValues(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            return elasticCrudJsonWriter.Stringbuilder.ToString();
        }
    }

    public struct FieldData

    {
    }
}