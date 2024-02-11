using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Domain.Entities
{
    public class PropertyActivityLog : BaseEntity
    {
        public long Id { get; set; }
        public long BrokerId { get; set; }
        public string Params
        {
            get
            {
                var value = JsonSerializer.Serialize(_Paramemter);
                return value;
            }
            set
            {
                _Paramemter = string.IsNullOrEmpty(value) ? new List<ParamObject>() : JsonSerializer.Deserialize<List<ParamObject>>(value);
            }
        }
        private List<ParamObject> _Paramemter;
        [NotMapped]
        public List<ParamObject> Parameters
        {
            get { return _Paramemter; }
            set { _Paramemter = value; }
        }
        public string Comment { get; set; }
        public virtual Broker Broker { get; set; }
    }

    public class ParamObject
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
