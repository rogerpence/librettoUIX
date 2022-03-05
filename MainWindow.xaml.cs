using LibrettoUI_2.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;


#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8602 // Possible null reference assignment.
#pragma warning disable CS8604 // Possible null reference assignment.

namespace LibrettoUI_2;

public partial class MainWindow : Window
{
    // Patterns are DOS file patterns unless there is a "REGEX" in the name--in which
    // case the patter is a regular expression.
    public const string ALL_FILES_PATTERN = "*.*";
    public const string CARRIAGE_RETURN = "\n";
    public const string DOS_CHANGE_DIR = "CD ";
    public const string DOS_PATH_BACKSLASH = @"\";
    public const string FILE_EXPLORER = "fileExplorer";
    public const string LEADING_BACKSLASH_REGEX_PATTERN = @"\\";
    
    public const string LIBRETTO_ROOT_FOLDER_KEY = "librettoRootFolder";

    public const string LIBRETTO_BATCH_FILE_ROOT_FOLDER = @"template_work\libretto_batch_files";
    public const string LIBRETTO_OUTPUT_ROOT_FOLDER = @"template_work\output\";
    public const string LIBRETTO_SCHEMAS_ROOT_FOLDER = @"template_work\schemas\";
    public const string LIBRETTO_SET_FILE_EXTENSION = ".librettoset.json";
    public const string LIBRETTO_SETS_ROOT_FOLDER = @"template_work\libretto_sets";
    public const string LIBRETTO_TEMPLATES_ROOT_FOLDER = @"template_work\templates\";

    public const string ONE_OR_MORE_BLANKS_REGEX = @"\s+";
    public const string PYTHON_EXECUTABLE = "Python";
    public const string SCHEMA_FILE_PATTERN = "*.json";
    public const string SCHEMA_FILE_REGEX_PATTERN = @"\\\*\.json";
    public const string SELECT_ALL_SCHEMAS_TEXT = "Select all schemas";
    public const string SPACE = " ";
    public const string TEMPLATE_FILE_PATTERN = "*.tpl.*";
    public const string TEXT_EDITOR = "textEditor";
    public const string UNDERSCORE = "_";

    public Model.LibrettoUnit lu = new Model.LibrettoUnit();

    string? librettoRootFolder;
    string? textEditor;
    string? fileExplorer;

    public MainWindow()
    {
        this.DataContext = lu;

        InitializeComponent();

        this.librettoRootFolder = ConfigurationManager.AppSettings[LIBRETTO_ROOT_FOLDER_KEY];

        this.textEditor = ConfigurationManager.AppSettings[TEXT_EDITOR];
        this.fileExplorer = ConfigurationManager.AppSettings[FILE_EXPLORER];

        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);
        this.librettoRootFolder = this.librettoRootFolder.ToLower();
    }

    private void buttonGetTemplateFolder_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);
        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, LIBRETTO_TEMPLATES_ROOT_FOLDER);

        var listInfo = DirectoryManager.GetFolderFileItems(targetDirectory, TEMPLATE_FILE_PATTERN);
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
        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, LIBRETTO_SCHEMAS_ROOT_FOLDER);

        var listInfo = DirectoryManager.GetFolderFileItems(targetDirectory,
                                                               SCHEMA_FILE_PATTERN,
                                                               new FolderFileItem { File = SELECT_ALL_SCHEMAS_TEXT, Path = String.Empty });
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

        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, LIBRETTO_OUTPUT_ROOT_FOLDER);

        System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();

        openFileDlg.InitialDirectory = targetDirectory;

        var result = openFileDlg.ShowDialog();
        if (result.ToString() != string.Empty)
        {
            lu.OutputPath = openFileDlg.SelectedPath;
        }
    }

    private Tuple<string, string, string>? getRelativePaths()
    {
        ArgumentNullException.ThrowIfNull(lu.Template);
        ArgumentNullException.ThrowIfNull(lu.Schema);
        ArgumentNullException.ThrowIfNull(lu.OutputPath);
        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);

        // Get relative paths for template, schema, and outputPath.
        string? template = lu.Template.ToLower().Replace(Path.Combine(this.librettoRootFolder.ToLower(), LIBRETTO_TEMPLATES_ROOT_FOLDER.ToLower()), String.Empty);
        string? schema = lu.Schema.ToLower().Replace(Path.Combine(this.librettoRootFolder.ToLower(), LIBRETTO_SCHEMAS_ROOT_FOLDER.ToLower()), String.Empty);
        string? outputPath = lu.OutputPath.ToLower().Replace(Path.Combine(this.librettoRootFolder.ToLower(), LIBRETTO_OUTPUT_ROOT_FOLDER.ToLower()), String.Empty);

        return Tuple.Create(template, schema, outputPath);
    }

    private void buttonLaunchLibretto_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(lu.Template);
        ArgumentNullException.ThrowIfNull(lu.Schema);
        ArgumentNullException.ThrowIfNull(lu.OutputPath);
        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);

        Tuple<string, string, string>? paths = getRelativePaths();
        ArgumentNullException.ThrowIfNull(paths);
        string template = paths.Item1;
        string schema = paths.Item2;
        string outputPath = paths.Item3;

        string commandLineArgs = ProcessLauncher.GetLibrettoXCommandLineArgs(template, schema, outputPath);

        lu.Messages.Clear();
        listboxMessages.UpdateLayout();

        ProcessLauncher pl = new ProcessLauncher();

        Directory.SetCurrentDirectory(this.librettoRootFolder);

        pl.LaunchProcess(PYTHON_EXECUTABLE, commandLineArgs);

        ArgumentNullException.ThrowIfNull(lu.Messages);

        lu.Messages.Clear();
        lu.Messages = pl.StatusMessage;

        listboxMessages.UpdateLayout();
    }

    private string getLibrettoSetBatchFileContents()
    {
        Tuple<string, string, string>? paths = getRelativePaths();
        ArgumentNullException.ThrowIfNull(paths);
        string template = paths.Item1;
        string schema = paths.Item2;
        string outputPath = paths.Item3;

        string contents = DOS_CHANGE_DIR + this.librettoRootFolder + CARRIAGE_RETURN;

        return contents + PYTHON_EXECUTABLE + SPACE + ProcessLauncher.GetLibrettoXCommandLineArgs(template, schema, outputPath);
    }

    private void buttonSaveLibrettoSet_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);
        ArgumentNullException.ThrowIfNull(lu.Description);
        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, LIBRETTO_SETS_ROOT_FOLDER);

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.InitialDirectory = targetDirectory;

        string? proposedFileName = lu.Description.ToLower();
        proposedFileName = Regex.Replace(proposedFileName, ONE_OR_MORE_BLANKS_REGEX, UNDERSCORE);
        saveFileDialog.FileName = proposedFileName + LIBRETTO_SET_FILE_EXTENSION;

        if (saveFileDialog.ShowDialog() == true)
        {
            List<string> msgs = new();  
            msgs.Clear();
            msgs.Add($"Libretto set saved as: {saveFileDialog.FileName}");

            saveLibrettoSet(saveFileDialog.FileName);


            string batchFileName = Path.GetFileName(saveFileDialog.FileName.ToLower()).Replace(".json", ".bat");
            string batchFile = Path.Combine(this.librettoRootFolder, LIBRETTO_BATCH_FILE_ROOT_FOLDER, batchFileName);
            File.WriteAllText(batchFile, getLibrettoSetBatchFileContents());
            
            msgs.Add($"Libretto batch file saved as: {batchFile}");

            lu.Messages = msgs;
            listboxMessages.UpdateLayout();
        }
    }

    private void comboboxTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lu.Template == null || lu.TemplateList == null || comboboxTemplates.SelectedItem == null)
        {
            return;
        }

        lu.Template = ((FolderFileItem)comboboxTemplates.SelectedItem).Path;
    }

    private void comboboxSchemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lu.SchemaList == null || comboboxSchemas.SelectedItem == null)
        {
            return; 
        }
        lu.Schema = ((FolderFileItem)comboboxSchemas.SelectedItem).Path;
        ArgumentNullException.ThrowIfNull(lu.Schema);

        openSchemaButton.Content = (lu.Schema.EndsWith("*.json")) ? "Open schema path" : "Open schema file";
    }

    private void LibrettoSetName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (textbox_LibrettoSetName.Text == null)
        {
            return;
        }
        lu.EnableButtonSaveLibrettoSet = (!String.IsNullOrEmpty(textbox_LibrettoSetName.Text)) & lu.DataProvided;
    }
  
    private void clearCurrentLibrettoSet()
    {
        lu.SchemaList = null;
        lu.TemplateList = null;
        lu.OutputPath = null;
        lu.Description = null;
    }

    private void saveLibrettoSet(string fileName)
    {
        JsonSerializerOptions options = new() { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(lu, options);
        File.WriteAllText(fileName, jsonString);  
    }

    private void buttonLoadLibrettoSet_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);                   
        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, LIBRETTO_SETS_ROOT_FOLDER);

        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = targetDirectory;

        if (openFileDialog.ShowDialog() == true)
        {
            openLibrettoSet(openFileDialog.FileName);
        }
    }

    private void openLibrettoSet(string fileName)
    {
        string jsonString = File.ReadAllText(fileName);

        LibrettoUnit? luToLoad = JsonSerializer.Deserialize<LibrettoUnit>(jsonString);
        ArgumentNullException.ThrowIfNull(luToLoad);
        ArgumentNullException.ThrowIfNull(luToLoad.Template);
        ArgumentNullException.ThrowIfNull(luToLoad.Schema);

        string? templateFileName = Path.GetFileName(luToLoad.Template);
        Regex re = new Regex(LEADING_BACKSLASH_REGEX_PATTERN + templateFileName, RegexOptions.Compiled);
        string? templatePath = re.Replace(luToLoad.Template, String.Empty);

        clearCurrentLibrettoSet();

        lu.OutputPath = luToLoad.OutputPath;
        lu.Description = luToLoad.Description;
        lu.Schema = luToLoad.Schema;
        lu.Template = luToLoad.Template;

        // Load templates select list.
        List<FolderFileItem> files = DirectoryManager.GetFiles(templatePath);
        lu.TemplateList = files;
        comboboxTemplates.SelectedValue = templateFileName;
        comboboxTemplates.UpdateLayout();

        openLibrettoSetSchema(lu);
    }

    public void openLibrettoSetSchema(LibrettoUnit luToLoad)
    {
        ArgumentNullException.ThrowIfNull(luToLoad);
        ArgumentNullException.ThrowIfNull(luToLoad.Schema);

        Regex re;

        string? schemaFileName = Path.GetFileName(luToLoad.Schema);
        if (schemaFileName == SCHEMA_FILE_PATTERN)
        {
            re = new Regex(SCHEMA_FILE_REGEX_PATTERN, RegexOptions.Compiled);
        }
        else
        {
            re = new Regex(LEADING_BACKSLASH_REGEX_PATTERN + schemaFileName, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
        string? schemaPath = re.Replace(luToLoad.Schema, String.Empty);

        // Load schemas select list.
        List<FolderFileItem> files = DirectoryManager.GetFiles(schemaPath);
        if (schemaFileName == SCHEMA_FILE_PATTERN)
        {
            FolderFileItem ffi = new FolderFileItem() { File = SELECT_ALL_SCHEMAS_TEXT, Path = luToLoad.Schema };
            files.Insert(0, ffi);
            lu.SchemaList = files;
            comboboxSchemas.SelectedIndex = 0;
        }
        else
        {
            lu.SchemaList = files;
            comboboxSchemas.SelectedValue = schemaFileName;
        }

        comboboxSchemas.UpdateLayout();
    }

    private void button_openoutputpath(object sender, RoutedEventArgs e)
    {
        ProcessLauncher pl = new ProcessLauncher();

        ArgumentNullException.ThrowIfNull(lu.OutputPath);
        pl.LaunchProcess(this.fileExplorer, lu.OutputPath, false);
    }

    private void button_openTemplatePath(object sender, RoutedEventArgs e)
    {
        ProcessLauncher pl = new ProcessLauncher();

        ArgumentNullException.ThrowIfNull(lu.Template);

        string? path = Path.GetDirectoryName(lu.Template);
        ArgumentNullException.ThrowIfNull(path);

        pl.LaunchProcess(this.fileExplorer, path, false);
    }

    private void button_openSchema(object sender, RoutedEventArgs e)
    {
        ProcessLauncher pl = new ProcessLauncher();
        ArgumentNullException.ThrowIfNull(lu.Schema);

        string schema = lu.Schema;
        if (schema.EndsWith("*.json")) {
            schema = schema.ToLower().Replace(@"\*.json", "");
            pl.LaunchProcess(this.fileExplorer, schema, false);
        }
        else {
            pl.LaunchProcess(this.textEditor, lu.Schema, false);
        }
    }

    private void button_openTemplateFile(object sender, RoutedEventArgs e)
    {
        ProcessLauncher pl = new ProcessLauncher();

        ArgumentNullException.ThrowIfNull(lu.Template);
        pl.LaunchProcess(this.textEditor, lu.Template, false);
    }
}