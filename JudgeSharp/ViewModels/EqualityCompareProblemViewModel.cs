//╔═════════════════════════════════════════════════════════════════════════════════╗
//║                                                                                 ║
//║   ╔╗      ╔╗   ╔╗╔╗╔╗╔╗╔╗   ╔╗╔╗╔╗╔╗     ╔╗      ╔╗   ╔╗╔╗╔╗╔╗╔╗   ╔╗           ║
//║   ╚╝    ╔╗╚╝   ╚╝╚╝╚╝╚╝╚╝   ╚╝╚╝╚╝╚╝╔╗   ╚╝╔╗    ╚╝   ╚╝╚╝╚╝╚╝╚╝   ╚╝           ║
//║   ╔╗  ╔╗╚╝     ╔╗           ╔╗      ╚╝   ╔╗╚╝    ╔╗   ╔╗           ╔╗           ║
//║   ╚╝╔╗╚╝       ╚╝╔╗╔╗╔╗╔╗   ╚╝╔╗╔╗╔╗     ╚╝  ╔╗  ╚╝   ╚╝╔╗╔╗╔╗╔╗   ╚╝           ║
//║   ╔╗╚╝╔╗       ╔╗╚╝╚╝╚╝╚╝   ╔╗╚╝╚╝╚╝╔╗   ╔╗  ╚╝  ╔╗   ╔╗╚╝╚╝╚╝╚╝   ╔╗           ║
//║   ╚╝  ╚╝╔╗     ╚╝           ╚╝      ╚╝   ╚╝    ╔╗╚╝   ╚╝           ╚╝           ║
//║   ╔╗    ╚╝╔╗   ╔╗╔╗╔╗╔╗╔╗   ╔╗      ╔╗   ╔╗    ╚╝╔╗   ╔╗╔╗╔╗╔╗╔╗   ╔╗╔╗╔╗╔╗╔╗   ║
//║   ╚╝      ╚╝   ╚╝╚╝╚╝╚╝╚╝   ╚╝      ╚╝   ╚╝      ╚╝   ╚╝╚╝╚╝╚╝╚╝   ╚╝╚╝╚╝╚╝╚╝   ║
//║                                                                                 ║
//║   This file is a part of the project Judge Sharp done by Ahmad Bashar Eter.     ║
//║   This program is free software: you can redistribute it and/or modify          ║
//║   it under the terms of the GNU General Public License version 3.               ║
//║   This program is distributed in the hope that it will be useful, but WITHOUT   ║
//║   ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS ║
//║   FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details ║
//║   GNU General Public: http://www.gnu.org/licenses.                              ║
//║   For usage not under GPL please request my approval for commercial license.    ║
//║   Copyright(C) 2017 Ahmad Bashar Eter.                                          ║
//║   KernelGD@Hotmail.com                                                          ║
//║                                                                                 ║
//╚═════════════════════════════════════════════════════════════════════════════════╝

using JudgeSharp.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JudgeSharp.ViewModels
{
    public class EqualityCompareProblemViewModel : ProblemViewModel
    {

        EqualityCompareProblem EqualityCompareProblem
        {
            get { return ProblemSpecification as EqualityCompareProblem; }
        }

        public RelayCommand LoadInpputTestFileCommand { get; private set; }
        public RelayCommand LoadOutputTestFileCommand { get; private set; }

        public RelayCommand LoadInpputBatchTestFileCommand { get; private set; }
        public RelayCommand LoadOutputBatchTestFileCommand { get; private set; }

        public RelayCommand AddTestCaseDocumentCommand { get; protected set; }
        public RelayCommand DeleteTestCaseDocumentCommand { get; protected set; }


        public EqualityCompareProblemViewModel()
        {
            ProblemSpecification = new EqualityCompareProblem();
            LoadInpputTestFileCommand = new RelayCommand(p => OnLoadInpputTestFile(), p => IsEditing && SelectedTestCase != null);
            LoadOutputTestFileCommand = new RelayCommand(p => OnLoadOutputTestFile(), p => IsEditing && SelectedTestCase != null);

            LoadInpputBatchTestFileCommand = new RelayCommand(p => OnLoadBatchInpputTestFile(), p => IsEditing);
            LoadOutputBatchTestFileCommand = new RelayCommand(p => OnLoadBatchOutputTestFile(), p => IsEditing);

            AddTestCaseDocumentCommand = new RelayCommand(p => OnAddTestCase(), p => IsEditing);
            DeleteTestCaseDocumentCommand = new RelayCommand(p => OnDeleteTestCase(), p => IsEditing && SelectedTestCase != null);
        }



        protected override void CommitChanges()
        {
            EqualityCompareProblem.Name = Name;
            EqualityCompareProblem.MemoryLimit = MemoryLimit * 1024 * 1024;
            EqualityCompareProblem.TimeLimit = TimeLimit;
            EqualityCompareProblem.Initialize(TestCases.Select(p => p.Output).ToArray(), TestCases.Select(p => p.Input).ToArray(), Password);
            EqualityCompareProblem.DocumentData = Document != null ? DocumentHelper.XpsDocumentToBytes(Document) : null;
        }

        private void OnDeleteTestCase()
        {
            TestCases.Remove(SelectedTestCase);
        }

        private void OnAddTestCase()
        {
            TestCases.Add(new TestCaseViewModel());
        }

        private void OnLoadInpputTestFile()
        {
            string text = OpenTextFile();
            if(text != null)
            {
               SelectedTestCase.Input = text;
            }
        }

        private void OnLoadOutputTestFile()
        {
            string text = OpenTextFile();
            if (text != null)
            {
                SelectedTestCase.Output = text;
            }
        }


        private void OnLoadBatchOutputTestFile()
        {
            var str = OpenTextBatch();
            int i = 0;
            for (int j = 0; j < TestCases.Count; j++)
            {
                if (!string.IsNullOrEmpty(TestCases[j].Output))
                    continue;
                TestCases[j].Output = str[i];
                i++;
            }
            for (; i < str.Length; i++)
            {
                TestCaseViewModel ts = new TestCaseViewModel();
                ts.Output = str[i];
                TestCases.Add(ts);
            }
        }

        private void OnLoadBatchInpputTestFile()
        {
            var str = OpenTextBatch();
            int i = 0;
            for (int j = 0; j < TestCases.Count; j++)
            {
                if (!string.IsNullOrEmpty(TestCases[j].Input))
                    continue;
                TestCases[j].Input = str[i];
            }
            for (; i < str.Length; i++)
            {
                TestCaseViewModel ts = new TestCaseViewModel();
                ts.Input = str[i];
                TestCases.Add(ts);
            }
        }

        private string[] OpenTextBatch()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
                ofd.Multiselect = true;
                var r = ofd.ShowDialog();
                if (r.HasValue && r.Value)
                {
                    List<string> strs = new List<string>();
                    foreach (var item in ofd.FileNames)
                    {
                        strs.Add(File.ReadAllText(item));
                    }
                    return strs.ToArray();
                }
            }
            catch
            { }
            return null;
        }

        private string OpenTextFile()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
                var r = ofd.ShowDialog();
                if (r.HasValue && r.Value)
                {
                    return File.ReadAllText(ofd.FileName);
                }
            }
            catch
            { }
            return null;
        }
    }
}
