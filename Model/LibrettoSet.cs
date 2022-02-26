using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibrettoUI_2.Model;

//[NotifyPropertyChanged]


public class LibrettoUnit : LibrettoUnitBase
{

    public LibrettoUnit()
    {
        _Messages = new List<string>();
    }

    private List<string>? _Messages;
    public List<string>? Messages
    {
        get { return _Messages; }
        set { SetField(ref _Messages, value); }
    }

    private bool? _EnableButtonSaveLibrettoSet = false;
    [JsonIgnore]
    public bool? EnableButtonSaveLibrettoSet
    {
        get { return _EnableButtonSaveLibrettoSet; }
        set { SetField(ref _EnableButtonSaveLibrettoSet, value); }
    }

    private bool? _TemplatesPopulated = false;
    [JsonIgnore]
    public bool? TemplatesPopulated
    {
        get { return _TemplatesPopulated; }
        set { SetField(ref _TemplatesPopulated, value); }
    }

    private bool? _SchemasPopulated = false;
    [JsonIgnore]
    public bool? SchemasPopulated
    {
        get { return _SchemasPopulated; }
        set { SetField(ref _SchemasPopulated, value); }
    }
    
    private List<FolderFileItem>? _templateList;

    [JsonIgnore]
    public List<FolderFileItem>? TemplateList
    {
        get { return _templateList; }
        set
        {
            SetField(ref _templateList, value);
            TemplatesPopulated = _templateList != null;
        }
    }

    private List<FolderFileItem>? _schemaList;

    [JsonIgnore]
    public List<FolderFileItem>? SchemaList
    {
        get { return _schemaList; }
        set { 
            SetField(ref _schemaList, value);
            SchemasPopulated = _schemaList != null;
        }       
    }


}


