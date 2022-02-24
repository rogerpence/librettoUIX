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
            ArgumentNullException.ThrowIfNull(this.librettoRootFolder);

            this.DataContext = lu;
        }

        private void buttonGetTemplateFolder_Click(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(this.librettoRootFolder);
            string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, @"template_work\templates");

            var listInfo = DirectoryManager.GetFolderFileItems(targetDirectory);
            if (listInfo != null)
            {
                lu.TemplateList = listInfo.Item1;
                lu.Template = listInfo.Item2;
            }
            comboboxTemplates.SelectedIndex = 0;
        }


        private void buttonGetSchemasFolder_Click(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(this.librettoRootFolder);
            string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, @"template_work\schemas");

            var listInfo = DirectoryManager.GetFolderFileItems(targetDirectory, new FolderFileItem { File = "Select a template", Path = "" });

            if (listInfo != null)
            {
                lu.SchemaList = listInfo.Item1;
                lu.Schema = listInfo.Item2;
            }
            comboboxSchemas.SelectedIndex = 0;
        }

        private void buttonOutputPath_Click(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(this.librettoRootFolder);
            string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, @"template_work\output");

            System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();

            openFileDlg.InitialDirectory = targetDirectory;

            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                lu.OutputPath = openFileDlg.SelectedPath;
            }
        }

        private void buttonLaunchLibretto_Click(object sender, RoutedEventArgs e)
        {
            lu.description = "Roger";
        }

        private void buttonSaveLibrettoSet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void comboboxTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lu.Template = ((FolderFileItem)comboboxTemplates.SelectedItem).Path;
        }

        private void comboboxSchemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lu.Schema = ((FolderFileItem)comboboxSchemas.SelectedItem).Path;
        }
    }
}
