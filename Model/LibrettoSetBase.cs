using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace LibrettoUI_2.Model
{
    public class LibrettoUnitBase : ObservableObject
    {

        public LibrettoUnitBase()
        {
        }

        private bool? _DataProvided = false;
        [JsonIgnore]
        public bool? DataProvided
        {
            get { return _DataProvided; }
            set { SetField(ref _DataProvided, value); }
        }

        private string? _Description;
        public string? Description
        {
            get { return _Description; }
            set { SetField(ref _Description, value);
                this.DataProvided = IsAllDataProvided();
            }
        }

        private string? _Schema;
        public string? Schema
        {
            get { return _Schema; }
            set { SetField(ref _Schema, value);
                  this.DataProvided = IsAllDataProvided();
                }
        }

        private string? _Template;
        public string? Template
        {
            get { return _Template; }
            set { SetField(ref _Template, value);
                  this.DataProvided = IsAllDataProvided();
                }
        }

        private string? _OutputPath;
        public string? OutputPath
        {
            get { return _OutputPath; }
            set { SetField(ref _OutputPath, value);
                  this.DataProvided = IsAllDataProvided();
                }
        }

        public bool IsAllDataProvided()
        {
            //return (! (_schemaList == null));
            return (this.Schema != null && this.Template != null && this.OutputPath != null);
        }

    }
}

