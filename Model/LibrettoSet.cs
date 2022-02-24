using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrettoUI_2.Model;

//[NotifyPropertyChanged]


public class LibrettoUnit : ObservableObject
{

    public LibrettoUnit()
    {

    }


    private bool? _tester;
    public bool? tester
    {
        get { return _tester; }
        set { SetField(ref _tester, value); }
    }


    private string? _description;
    public string? description
    {
        get { return _description; }
        set { SetField(ref _description, value); }
    }


    /*
        private string? _description;
        public string? description
        {
            get { return _description; }
            set { SetField(ref _description, value); }
        }
    */
    private bool? _dataProvided = false;
    public bool? dataProvided
    {
        get { return _dataProvided; }
        set { SetField(ref _dataProvided, value);}
    }

    public string? schema { get; set; }
    public string? template { get; set; }
    public string? outputPath { get; set; }

    private List<FolderFileItem>? _schemaList;
    public List<FolderFileItem>? schemaList
    {
        get { return _schemaList; }
        set { 
                SetField(ref _schemaList, value);
                dataProvided = IsAllDataProvided(); 
            }       
    }

    private bool IsAllDataProvided()
    {
        return (! (_schemaList == null));
    }
}


