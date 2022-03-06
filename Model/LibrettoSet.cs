using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LibrettoUI_2.Model;

public class LibrettoSet : ObservableObject //: LibrettoUnitBase
{
    public LibrettoSet()
    {
        _Messages = new List<string>();
    }

    private string? _Description;
    public string? Description
    {
        get { return _Description; }
        set
        {
            SetField(ref _Description, value);
            this.DataProvided = IsAllDataProvided();
        }
    }

    private string? _Schema;
    public string? Schema
    {
        get { return _Schema; }
        set
        {
            SetField(ref _Schema, value);
            this.DataProvided = IsAllDataProvided();
        }
    }

    private string? _Template;
    public string? Template
    {
        get { return _Template; }
        set
        {
            SetField(ref _Template, value);
            this.DataProvided = IsAllDataProvided();
        }
    }

    private string? _OutputPath;
    public string? OutputPath
    {
        get { return _OutputPath; }
        set
        {
            SetField(ref _OutputPath, value);
            this.DataProvided = IsAllDataProvided();
        }
    }

    public bool IsAllDataProvided()
    {
        return (this.Schema != null && this.Template != null && this.OutputPath != null);
    }

    private List<string>? _Messages;
    public List<string>? Messages
    {
        get { return _Messages; }
        set { SetField(ref _Messages, value); }
    }

    private bool? _DataProvided = false;
    [JsonIgnore]
    public bool? DataProvided
    {
        get { return _DataProvided; }
        set { SetField(ref _DataProvided, value); }
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
