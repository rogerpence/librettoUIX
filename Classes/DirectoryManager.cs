using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace LibrettoUI_2.Model;

internal class DirectoryManager
{
    public static List<FolderFileItem> GetFiles(string folder, string pattern = "*.*")
    {
        List<FolderFileItem> files = new List<FolderFileItem>();

        
        string[]? result = Directory.GetFiles(folder, pattern);


        foreach (string fullPath in result)
        {
            string? filename = Path.GetFileName(fullPath);
            string? foldername = fullPath; 

            files.Add(new FolderFileItem { Path = foldername, File = filename });
        }

        return files;
    }

    public static Tuple<List<FolderFileItem>, string>? GetFolderFileItems(string? initialDirectory, FolderFileItem? firstItem=null)
    {
        ArgumentNullException.ThrowIfNull(initialDirectory);

        System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();

        openFileDlg.InitialDirectory = initialDirectory;

        var result = openFileDlg.ShowDialog();
        if (result.ToString() != string.Empty)
        {
            List<FolderFileItem> files = DirectoryManager.GetFiles(openFileDlg.SelectedPath);

            if (firstItem != null)
            {
                firstItem.Path = openFileDlg.SelectedPath + @"\*.*";
                files.Insert(0, firstItem);
            }

            return Tuple.Create(files, openFileDlg.SelectedPath);
        }

        return null;
    }

}

