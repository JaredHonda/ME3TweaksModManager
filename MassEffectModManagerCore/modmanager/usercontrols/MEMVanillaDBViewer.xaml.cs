﻿using MassEffectModManagerCore.GameDirectories;
using MassEffectModManagerCore.gamefileformats.sfar;
using MassEffectModManagerCore.modmanager.helpers;
using MassEffectModManagerCore.modmanager.localizations;
using MassEffectModManagerCore.modmanager.objects;
using MassEffectModManagerCore.ui;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MassEffectModManagerCore.modmanager.usercontrols
{
    /// <summary>
    /// Interaction logic for MEMVanillaDBViewer.xaml
    /// </summary>
    public partial class MEMVanillaDBViewer : MMBusyPanelBase
    {
        public MEMVanillaDBViewer()
        {
            DataContext = this;
            LoadCommands();
            InitializeComponent();
        }

        public bool LoadingInProgress { get; set; } = true;
        public ICommand CloseCommand { get; set; }

        private void LoadCommands()
        {
            CloseCommand = new GenericCommand(ClosePanel);
        }

        private void ClosePanel()
        {
            OnClosing(DataEventArgs.Empty);
        }

        public string FilterTextME1 { get; set; }
        public string FilterTextME2 { get; set; }
        public string FilterTextME3 { get; set; }

        public override void HandleKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                OnClosing(DataEventArgs.Empty);
            }
        }


        public override void OnPanelVisible()
        {

            var db = VanillaDatabaseService.LoadDatabaseFor(Mod.MEGame.ME1, false);
            ME1Files.ReplaceAll(getDBItems(db));
            db = VanillaDatabaseService.LoadDatabaseFor(Mod.MEGame.ME2,false);
            ME2Files.ReplaceAll(getDBItems(db));
            db = VanillaDatabaseService.LoadDatabaseFor(Mod.MEGame.ME3, false);
            ME3Files.ReplaceAll(getDBItems(db));

            LoadingInProgress = false;
            ME1FilesView.Filter = FilterBackupFilesME1;
            ME2FilesView.Filter = FilterBackupFilesME2;
            ME3FilesView.Filter = FilterBackupFilesME3;

        }

        private IEnumerable<VanillaEntry> getDBItems(CaseInsensitiveDictionary<List<(int size, string md5)>> db)
        {
            var files = new List<VanillaEntry>();
            foreach (var v in db)
            {
                foreach (var sf in v.Value)
                {
                    files.Add(new VanillaEntry
                    {
                        Filepath = v.Key,
                        MD5 = sf.md5,
                        Size = sf.size
                    });
                }
            }
            return files;
        }


        private ObservableCollectionExtended<VanillaEntry> ME1Files { get; } = new ObservableCollectionExtended<VanillaEntry>();
        private ObservableCollectionExtended<VanillaEntry> ME2Files { get; } = new ObservableCollectionExtended<VanillaEntry>();
        private ObservableCollectionExtended<VanillaEntry> ME3Files { get; } = new ObservableCollectionExtended<VanillaEntry>();
        public ICollectionView ME1FilesView => CollectionViewSource.GetDefaultView(ME1Files);
        public ICollectionView ME2FilesView => CollectionViewSource.GetDefaultView(ME2Files);
        public ICollectionView ME3FilesView => CollectionViewSource.GetDefaultView(ME3Files);
        public int SelectedGameIndex { get; set; }

        public VanillaEntry SelectedME1File { get; set; }
        public VanillaEntry SelectedME2File { get; set; }
        public VanillaEntry SelectedME3File { get; set; }

        //These are separate methods because I don t want to have to do a looped if statement 6000 times for me3 for example.
        private bool FilterBackupFilesME1(object obj)
        {
            if (!string.IsNullOrWhiteSpace(FilterTextME1) && obj is VanillaEntry bobj)
            {
                return bobj.Filepath.Contains(FilterTextME1, StringComparison.InvariantCultureIgnoreCase);
            }
            return true;
        }

        private bool FilterBackupFilesME2(object obj)
        {
            if (!string.IsNullOrWhiteSpace(FilterTextME2) && obj is VanillaEntry bobj)
            {
                return bobj.Filepath.Contains(FilterTextME2, StringComparison.InvariantCultureIgnoreCase);
            }
            return true;
        }

        private bool FilterBackupFilesME3(object obj)
        {
            if (!string.IsNullOrWhiteSpace(FilterTextME3) && obj is VanillaEntry bobj)
            {
                return bobj.Filepath.Contains(FilterTextME3, StringComparison.InvariantCultureIgnoreCase);
            }
            return true;
        }

        public void OnFilterTextME1Changed()
        {
            ME1FilesView.Refresh();
        }
        public void OnFilterTextME2Changed()
        {
            ME2FilesView.Refresh();
        }
        public void OnFilterTextME3Changed()
        {
            ME3FilesView.Refresh();
        }

        public class VanillaEntry
        {
            public string Filepath { get; set; }
            public string MD5 { get; set; }
            public int Size { get; set; }
        }
    }
}
