using Puppy.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Elastic.Model.SearchModel
{
    /// <summary>
    ///     Allows to highlight search results on one or more fields. The implementation uses either
    ///     the lucene highlighter, fast-vector-highlighter or postings-highlighter.
    /// </summary>
    public class Highlight
    {
        private readonly List<HighlightField> _fields;
        private string _boundaryChars;
        private bool _boundaryCharsSet;
        private uint _boundaryMaxScan;
        private bool _boundaryMaxScanSet;
        private uint _phraseLimit;
        private bool _phraseLimitSet;
        private List<string> _postTags;
        private bool _postTagsSet;
        private List<string> _preTags;
        private bool _preTagsSet;
        private bool _requireFieldMatch;
        private bool _requireFieldMatchSet;
        private bool _tagsSchemaStyled;
        private bool _tagsSchemaStyledSet;

        public Highlight(List<HighlightField> fields)
        {
            _fields = fields;
        }

        /// <summary>
        ///     pre_tags 
        /// </summary>
        public List<string> PreTags
        {
            get => _preTags;
            set
            {
                _preTags = value;
                _preTagsSet = true;
            }
        }

        /// <summary>
        ///     post_tags 
        /// </summary>
        public List<string> PostTags
        {
            get => _postTags;
            set
            {
                _postTags = value;
                _postTagsSet = true;
            }
        }

        /// <summary>
        ///     tags_schema : styled 
        /// </summary>
        public bool TagsSchemaStyled
        {
            get => _tagsSchemaStyled;
            set
            {
                _tagsSchemaStyled = value;
                _tagsSchemaStyledSet = true;
            }
        }

        /// <summary>
        ///     require_field_match can be set to true which will cause a field to be highlighted
        ///     only if a query matched that field. false means that terms are highlighted on all
        ///     requested fields regardless if the query matches specifically on them.
        /// </summary>
        public bool RequireFieldMatch
        {
            get => _requireFieldMatch;
            set
            {
                _requireFieldMatch = value;
                _requireFieldMatchSet = true;
            }
        }

        /// <summary>
        ///     boundary_chars When highlighting a field using the fast vector highlighter,
        ///     boundary_chars can be configured to define what constitutes a boundary for
        ///     highlighting. It’s a single string with each boundary character defined in it. It
        ///     defaults to .,!? \t\n.
        /// </summary>
        public string BoundaryChars
        {
            get => _boundaryChars;
            set
            {
                _boundaryChars = value;
                _boundaryCharsSet = true;
            }
        }

        /// <summary>
        ///     boundary_max_scan The boundary_max_scan allows to control how far to look for
        ///     boundary characters, and defaults to 20.
        /// </summary>
        public uint BoundaryMaxScan
        {
            get => _boundaryMaxScan;
            set
            {
                _boundaryMaxScan = value;
                _boundaryMaxScanSet = true;
            }
        }

        /// <summary>
        ///     phrase_limit The fast-vector-highlighter has a phrase_limit parameter that prevents
        ///     it from analyzing too many phrases and eating tons of memory. It defaults to 256 so
        ///     only the first 256 matching phrases in the document scored considered. You can raise
        ///     the limit with the phrase_limit parameter but keep in mind that scoring more phrases
        ///     consumes more time and memory. If using matched_fields keep in mind that phrase_limit
        ///     phrases per matched field are considered
        /// </summary>
        public uint PhraseLimit
        {
            get => _phraseLimit;
            set
            {
                _phraseLimit = value;
                _phraseLimitSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("highlight");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("fields");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            foreach (var highlightField in _fields)
                highlightField.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            JsonHelper.WriteListValue("pre_tags", _preTags, elasticCrudJsonWriter, _preTagsSet);
            JsonHelper.WriteListValue("post_tags", _postTags, elasticCrudJsonWriter, _postTagsSet);
            JsonHelper.WriteValue("tags_schema", "styled", elasticCrudJsonWriter, _tagsSchemaStyledSet);
            JsonHelper.WriteValue("require_field_match", _requireFieldMatch, elasticCrudJsonWriter,
                _requireFieldMatchSet);

            JsonHelper.WriteValue("boundary_chars", _boundaryChars, elasticCrudJsonWriter, _boundaryCharsSet);
            JsonHelper.WriteValue("boundary_max_scan", _boundaryMaxScan, elasticCrudJsonWriter, _boundaryMaxScanSet);
            JsonHelper.WriteValue("phrase_limit", _phraseLimit, elasticCrudJsonWriter, _phraseLimitSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}