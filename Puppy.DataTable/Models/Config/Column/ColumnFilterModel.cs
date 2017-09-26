using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puppy.DataTable.Constants;
using System.Collections;
using System.Linq;

namespace Puppy.DataTable.Models.Config.Column
{
    public class ColumnFilterModel : Hashtable
    {
        private readonly DataTableModel _model;

        public ColumnFilterModel(DataTableModel model)
        {
            _model = model;

            this[FilterConst.PlaceHolder] = "head:after";
        }

        public JObject ColumnBuilders = new JObject();

        public override string ToString()
        {
            this[FilterConst.UseColVis] = _model.IsEnableColVis;

            this[FilterConst.Columns] = _model.Columns.Select(c => c.IsSearchable && c.ColumnFilter != null ? c.ColumnFilter : null).ToArray();

            return JsonConvert.SerializeObject(this);
        }
    }
}