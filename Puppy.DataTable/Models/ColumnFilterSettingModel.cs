using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;
using Puppy.DataTable.Models;

namespace Puppy.DataTable
{
    public class ColumnFilterSettingModel : Hashtable
    {
        private readonly DataTableConfigModel _model;

        public ColumnFilterSettingModel(DataTableConfigModel model)
        {
            _model = model;

            this["sPlaceHolder"] = "head:after";
        }

        public JObject ColumnBuilders = new JObject();

        public override string ToString()
        {
            this["bUseColVis"] = _model.IsEnableColVis;
            this["aoColumns"] = _model.Columns
                //.Where(c => c.Visible || c.Filter["sSelector"] != null)
                .Select(c => c.IsSearchable && c.Filter != null ? c.Filter : null).ToArray();
            return JsonConvert.SerializeObject(this);
        }
    }
}