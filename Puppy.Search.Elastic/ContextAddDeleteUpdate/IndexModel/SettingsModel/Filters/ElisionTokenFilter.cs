using System.Collections.Generic;
using Puppy.Search.Elastic.Model;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class ElisionTokenFilter : AnalysisFilterBase
    {
        private List<string> _articles;
        private bool _articlesSet;

        /// <summary>
        ///     A token filter which removes elisions. For example, "l’avion" (the plane) will
        ///     tokenized as "avion" (plane). Accepts articles setting which is a set of stop words articles.
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public ElisionTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Elision;
        }

        /// <summary>
        ///     articles Accepts articles setting which is a set of stop words articles. 
        /// </summary>
        public List<string> Articles
        {
            get => _articles;
            set
            {
                _articles = value;
                _articlesSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteListValue("articles", _articles, elasticCrudJsonWriter, _articlesSet);
        }
    }
}