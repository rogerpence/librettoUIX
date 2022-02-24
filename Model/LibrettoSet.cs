using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrettoUI_2.Model;

//[NotifyPropertyChanged]


public class LibrettoUnit : LibrettoUnitBase
{

    public LibrettoUnit()
    {
    }



    private List<FolderFileItem>? _schemaList;
    public List<FolderFileItem>? SchemaList
    {
        get { return _schemaList; }
        set { 
                SetField(ref _schemaList, value);
               // this.DataProvided = base.IsAllDataProvided(); 
            }       
    }

    private List<FolderFileItem>? _templateList;
    public List<FolderFileItem>? TemplateList
    {
        get { return _templateList; }
        set
        {
            SetField(ref _templateList, value);
            //DataProvided = IsAllDataProvided();
        }
    }

}


