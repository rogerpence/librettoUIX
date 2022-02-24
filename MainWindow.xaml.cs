using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using LibrettoUI_2.Model;
using System.Configuration;

namespace LibrettoUI_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Model.LibrettoUnit lu = new Model.LibrettoUnit();

        string? librettoRootFolder;

        public MainWindow()
        {
            InitializeComponent();
            lu.description = "Hello, World";

            this.librettoRootFolder = ConfigurationManager.AppSettings["librettoRootFolder"];
            ArgumentNullException.ThrowIfNull(librettoRootFolder);

            this.DataContext = lu;
        }

        private void buttonGetTemplateFolder_Click(object sender, RoutedEventArgs e)
        {
            List<FolderFileItem> listContents;
            string selectedFolder;

            var listInfo = DirectoryManager.GetFolderFileItems(this.librettoRootFolder, new FolderFileItem { File = "Select a template", Path = "xxx" });

            if (listInfo != null)
            {
                listContents = listInfo.Item1;
                selectedFolder = listInfo.Item2;

                lu.schemaList = listContents;
            }
        }


        private void buttonGetSchemasFolder_Click(object sender, RoutedEventArgs e)
        {
            List<FolderFileItem> listContents;
            string selectedFolder;

            var listInfo = DirectoryManager.GetFolderFileItems(this.librettoRootFolder, new FolderFileItem { File = "Select a template", Path = "xxx" });

            if (listInfo != null)
            {
                listContents = listInfo.Item1;
                selectedFolder = listInfo.Item2;

                lu.schemaList = listContents;
            }
        }

        private void buttonOutputPath_Click(object sender, RoutedEventArgs e)
        {
        }

        private void buttonLaunchLibretto_Click(object sender, RoutedEventArgs e)
        {
            lu.description = "Roger";
        }

        private void buttonSaveLibrettoSet_Click(object sender, RoutedEventArgs e)
        {

        }




    }
}
