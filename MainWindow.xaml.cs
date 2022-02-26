﻿using LibrettoUI_2.Model;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Microsoft.Win32;

namespace LibrettoUI_2;

public partial class MainWindow : Window
{
    public Model.LibrettoUnit lu = new Model.LibrettoUnit();

    string? librettoRootFolder;

    public MainWindow()
    {
        this.DataContext = lu;
            
        InitializeComponent();

        this.librettoRootFolder = ConfigurationManager.AppSettings["librettoRootFolder"];
        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);
        this.librettoRootFolder = this.librettoRootFolder.ToLower();
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

        var listInfo = DirectoryManager.GetFolderFileItems(targetDirectory, new FolderFileItem { File = "Select all schemas", Path = "" });

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
        ArgumentNullException.ThrowIfNull(lu.Template);
        ArgumentNullException.ThrowIfNull(lu.Schema); 
        ArgumentNullException.ThrowIfNull(lu.OutputPath);
        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);

        string template = lu.Template.ToLower().Replace(this.librettoRootFolder + @"\template_work\templates\", "");
        string schema = lu.Schema.ToLower().Replace(this.librettoRootFolder + @"\template_work\schemas\", "");
        string outputPath = lu.OutputPath.ToLower().Replace(this.librettoRootFolder + @"\template_work\output\", "");

        string commandLineArgs = ProcessLauncher.GetLibrettoXCommandLineArgs(template, schema, outputPath);

        ProcessLauncher pl = new ProcessLauncher();

        Directory.SetCurrentDirectory(this.librettoRootFolder);

        pl.LaunchProcess("Python", commandLineArgs);

        ArgumentNullException.ThrowIfNull(lu.Messages);

        lu.Messages.Clear();
        lu.Messages = pl.StatusMessage;

        //foreach (string msg in pl.StatusMessage)
        //{
        //    lu.Messages.Add(msg);
        //}

        listboxMessages.UpdateLayout();

        //lu.Messages = pl.StatusMessage;
        //MessageBox.Show(lu.Messages);
    }

    private void buttonSaveLibrettoSet_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);
        ArgumentNullException.ThrowIfNull(lu.Description);
        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, @"template_work\libretto_sets");

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.InitialDirectory = targetDirectory;

        string? proposedFileName = lu.Description.ToLower();
        proposedFileName = Regex.Replace(proposedFileName,@"\s+", "_");
        saveFileDialog.FileName = proposedFileName + ".librettoset.json";

        if (saveFileDialog.ShowDialog() == true)
        {
            saveLibrettoSet(saveFileDialog.FileName);
            MessageBox.Show("Written");
        }
    }

    private void comboboxTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lu.Template == null || lu.TemplateList == null)
        {
            return;
        }

        lu.Template = ((FolderFileItem)comboboxTemplates.SelectedItem).Path;
    }

    private void comboboxSchemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lu.SchemaList == null)
        {
            return; 
        }
        lu.Schema = ((FolderFileItem)comboboxSchemas.SelectedItem).Path;
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

    //private void Hyperlink_Click(object sender, RoutedEventArgs e)
    //{
    //    clearCurrentLibrettoSet();
    //}

    private void saveLibrettoSet(string fileName)
    {
        JsonSerializerOptions options = new() { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(lu, options);
        File.WriteAllText(fileName, jsonString);  
    }

    private void buttonLoadLibrettoSet_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(this.librettoRootFolder);
        string? targetDirectory = System.IO.Path.Combine(this.librettoRootFolder, @"template_work\libretto_sets");

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
        Regex re = new Regex(@"\\" + templateFileName, RegexOptions.Compiled);
        string? templatePath = re.Replace(luToLoad.Template, String.Empty);

        string? schemaFileName = Path.GetFileName(luToLoad.Schema);
        if (schemaFileName == @"*.*")
        {
               re = new Regex(@"\\\*\.\*", RegexOptions.Compiled);
        }
        else
        {
            re = new Regex(@"\\" + schemaFileName, RegexOptions.Compiled);
        }
        string? schemaPath = re.Replace(luToLoad.Schema, String.Empty);

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

        // Load schemas select list.
        files = DirectoryManager.GetFiles(schemaPath);
        if (schemaFileName == @"*.*")
        {
            FolderFileItem ffi = new FolderFileItem() { File = "Select all schemas", Path = luToLoad.Schema };
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
}
    

