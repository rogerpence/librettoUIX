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

#pragma warning disable CS8600 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8602 // Possible null reference assignment.
#pragma warning disable CS8603 // Possible null reference assignment.
#pragma warning disable CS8604 // Possible null reference assignment.

namespace LibrettoUI_2;

public partial class MainWindow : Window
{
    // Patterns are DOS file patterns unless there is a "REGEX" in the name--in which
    // case the patter is a regular expression.
    public const string ALL_FILES_PATTERN = "*.*";
    public const string NEWLINE = "\n";
    public const string DOS_CHANGE_DIR = "CD ";
    public const string DOS_PATH_BACKSLASH = @"\";
    public const string FILE_EXPLORER = "fileExplorer";
    public const string JSON_FILE_EXTENSION = @"*.json";
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

    public Model.LibrettoSet librettoSet = new Model.LibrettoSet();

    string? librettoRootFolder;
    string? textEditor;
    string? fileExplorer;

    public MainWindow()
    {
        InitializeComponent();

        this.DataContext = librettoSet;
  
        this.librettoRootFolder = ConfigurationManager.AppSettings[LIBRETTO_ROOT_FOLDER_KEY];

        this.textEditor = ConfigurationManager.AppSettings[TEXT_EDITOR];
        this.fileExplorer = ConfigurationManager.AppSettings[FILE_EXPLORER];

        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);
        this.librettoRootFolder = this.librettoRootFolder.ToLower();
    }

    private void buttonGetTemplateFolder_Click(object sender, RoutedEventArgs e)
    {
        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, LIBRETTO_TEMPLATES_ROOT_FOLDER);

        var listInfo = DirectoryManager.GetFolderFileItems(targetDirectory, TEMPLATE_FILE_PATTERN);
        if (listInfo != null)
        {
            librettoSet.TemplateList = listInfo.Item1;
            librettoSet.Template = listInfo.Item2;
        }
        comboboxTemplates.SelectedIndex = 0;
    }

    private void buttonGetSchemasFolder_Click(object sender, RoutedEventArgs e)
    {
        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, LIBRETTO_SCHEMAS_ROOT_FOLDER);

        var listInfo = DirectoryManager.GetFolderFileItems(targetDirectory,
                                                               SCHEMA_FILE_PATTERN,
                                                               new FolderFileItem { File = SELECT_ALL_SCHEMAS_TEXT, Path = String.Empty });
        if (listInfo != null)
        {
            librettoSet.SchemaList = listInfo.Item1;
            librettoSet.Schema = listInfo.Item2;
        }
        comboboxSchemas.SelectedIndex = 0;
    }

    private void buttonOutputPath_Click(object sender, RoutedEventArgs e)
    {
        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, LIBRETTO_OUTPUT_ROOT_FOLDER);

        System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();

        openFileDlg.InitialDirectory = targetDirectory;

        var result = openFileDlg.ShowDialog();
        if (result.ToString() != string.Empty)
        {
            librettoSet.OutputPath = openFileDlg.SelectedPath;
        }
    }

    private Tuple<string, string, string>? getRelativePaths()
    {
        // Get relative paths for template, schema, and outputPath.
        string? template = librettoSet.Template.ToLower().Replace(Path.Combine(this.librettoRootFolder.ToLower(), LIBRETTO_TEMPLATES_ROOT_FOLDER.ToLower()), String.Empty);
        string? schema = librettoSet.Schema.ToLower().Replace(Path.Combine(this.librettoRootFolder.ToLower(), LIBRETTO_SCHEMAS_ROOT_FOLDER.ToLower()), String.Empty);
        string? outputPath = librettoSet.OutputPath.ToLower().Replace(Path.Combine(this.librettoRootFolder.ToLower(), LIBRETTO_OUTPUT_ROOT_FOLDER.ToLower()), String.Empty);

        return Tuple.Create(template, schema, outputPath);
    }

    private void buttonLaunchLibretto_Click(object sender, RoutedEventArgs e)
    {
        Tuple<string, string, string>? paths = getRelativePaths();
        //ArgumentNullException.ThrowIfNull(paths);
        string template = paths.Item1;
        string schema = paths.Item2;
        string outputPath = paths.Item3;

        string commandLineArgs = ProcessLauncher.GetLibrettoXCommandLineArgs(template, schema, outputPath);

        System.Windows.Input.Cursor currentCursor = this.Cursor;
        this.Cursor = System.Windows.Input.Cursors.Wait;

        librettoSet.Messages.Clear();
        listboxMessages.UpdateLayout();

        ProcessLauncher pl = new ProcessLauncher();

        Directory.SetCurrentDirectory(this.librettoRootFolder);

        pl.LaunchProcess(PYTHON_EXECUTABLE, commandLineArgs);

        librettoSet.Messages.Clear();
        librettoSet.Messages = pl.StatusMessage;

        listboxMessages.UpdateLayout();
        
        this.Cursor = currentCursor;
    }

    private string getLibrettoSetBatchFileContents()
    {
        Tuple<string, string, string>? paths = getRelativePaths();

        string template = paths.Item1;
        string schema = paths.Item2;
        string outputPath = paths.Item3;

        string contents = DOS_CHANGE_DIR + this.librettoRootFolder + NEWLINE;

        return contents + PYTHON_EXECUTABLE + SPACE + ProcessLauncher.GetLibrettoXCommandLineArgs(template, schema, outputPath);
    }

    private void buttonSaveLibrettoSet_Click(object sender, RoutedEventArgs e)
    {
        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, LIBRETTO_SETS_ROOT_FOLDER);

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.InitialDirectory = targetDirectory;

        string? proposedFileName = librettoSet.Description.ToLower();
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

            librettoSet.Messages = msgs;
            listboxMessages.UpdateLayout();
        }
    }

    private void comboboxTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (librettoSet.Template == null || librettoSet.TemplateList == null || comboboxTemplates.SelectedItem == null)
        {
            return;
        }

        librettoSet.Template = ((FolderFileItem)comboboxTemplates.SelectedItem).Path;
    }

    private void comboboxSchemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (librettoSet.SchemaList == null || comboboxSchemas.SelectedItem == null)
        {
            return; 
        }
        librettoSet.Schema = ((FolderFileItem)comboboxSchemas.SelectedItem).Path;

        librettoSet.SchemaLinkButtonLabel = 
            (librettoSet.Schema.ToLower().EndsWith("*.json")) ? "Open schema path" : "Open schema file";
    }

    private void LibrettoSetName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (textbox_LibrettoSetName.Text == null)
        {
            return;
        }
        librettoSet.EnableButtonSaveLibrettoSet = (!String.IsNullOrEmpty(textbox_LibrettoSetName.Text)) & librettoSet.DataProvided;
    }
  
    private void clearCurrentLibrettoSet()
    {
        librettoSet.SchemaList = null;
        librettoSet.TemplateList = null;
        librettoSet.OutputPath = null;
        librettoSet.Description = null;
    }

    private void saveLibrettoSet(string fileName)
    {
        JsonSerializerOptions options = new() { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(librettoSet, options);
        File.WriteAllText(fileName, jsonString);  
    }

    private void buttonLoadLibrettoSet_Click(object sender, RoutedEventArgs e)
    {
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

        LibrettoSet? luToLoad = JsonSerializer.Deserialize<LibrettoSet>(jsonString);

        string? templateFileName = Path.GetFileName(luToLoad.Template);
        Regex re = new Regex(LEADING_BACKSLASH_REGEX_PATTERN + templateFileName, RegexOptions.Compiled);
        string? templatePath = re.Replace(luToLoad.Template, String.Empty);

        clearCurrentLibrettoSet();

        librettoSet.OutputPath = luToLoad.OutputPath;
        librettoSet.Description = luToLoad.Description;
        librettoSet.Schema = luToLoad.Schema;
        librettoSet.Template = luToLoad.Template;

        List<FolderFileItem> files = DirectoryManager.GetFiles(templatePath);
        librettoSet.TemplateList = files;
        comboboxTemplates.SelectedValue = templateFileName;
        comboboxTemplates.UpdateLayout();

        openLibrettoSetSchema(librettoSet);
    }

    public void openLibrettoSetSchema(LibrettoSet luToLoad)
    {
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
            librettoSet.SchemaList = files;
            comboboxSchemas.SelectedIndex = 0;
        }
        else
        {
            librettoSet.SchemaList = files;
            comboboxSchemas.SelectedValue = schemaFileName;
        }

        comboboxSchemas.UpdateLayout();
    }

    private void launchProcess(string command, string argument)
    {
        ProcessLauncher pl = new ProcessLauncher();
        pl.LaunchProcess(command, argument, wait: false);
    }

    private void button_openOutputPath(object sender, RoutedEventArgs e)
    {
        launchProcess(this.fileExplorer, librettoSet.OutputPath);
    }

    private void button_openTemplatePath(object sender, RoutedEventArgs e)
    {
        string? path = Path.GetDirectoryName(librettoSet.Template);
        launchProcess(this.fileExplorer, path);
    }

    private void button_openSchemaPathOrFile(object sender, RoutedEventArgs e)
    {
        string schemaPath = librettoSet.Schema;
        if (schemaPath.EndsWith("*.json")) {
            schemaPath = schemaPath.ToLower().Replace(JSON_FILE_EXTENSION, "");
            launchProcess(this.fileExplorer, schemaPath);
        }
        else {
            launchProcess(this.textEditor, schemaPath);
        }
    }

    private void button_openTemplateFile(object sender, RoutedEventArgs e)
    {
        launchProcess(this.textEditor, librettoSet.Template);
    }
}