﻿using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;
using NatureRecorder.Tests.UnitTests;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class ReportingCommandTest
    {
        private NatureRecorderFactory _factory;
        private string _currentFolder;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);

            _currentFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ImportExportManagerTest)).Location);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownReportTypeException))]
        public void InvalidReportTypeTest()
        {
            string[] arguments = new string[] { "invalid" };
            TestHelpers.RunCommand(_factory, arguments, new ReportCommand(), CommandMode.CommandLine, null, null, null, null, 0);
        }

        [TestMethod]
        public void SummaryReportTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.SightingsImport.Import(importFilePath);

            string[] arguments = new string[] { "summary", "1996-11-23", "2000-01-01" };
            TestHelpers.RunCommand(_factory, arguments, new ReportCommand(), CommandMode.CommandLine, null, null, null, "report-summary.txt", 0);
        }

        [TestMethod]
        public void LocationReportTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.SightingsImport.Import(importFilePath);

            string[] arguments = new string[] { "location", "bagley wood", "1996-11-23", "2000-01-31" };
            TestHelpers.RunCommand(_factory, arguments, new ReportCommand(), CommandMode.CommandLine, null, null, null, "report-location.txt", 0);
        }

        [TestMethod]
        public void CategoryReportTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.SightingsImport.Import(importFilePath);

            string[] arguments = new string[] { "category", "birds", "bagley wood", "1996-11-23", "2000-01-01" };
            TestHelpers.RunCommand(_factory, arguments, new ReportCommand(), CommandMode.CommandLine, null, null, null, "report-category.txt", 0);
        }

        [TestMethod]
        public void SpeciesReportTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.SightingsImport.Import(importFilePath);

            string[] arguments = new string[] { "species", "jackdaw", "bagley wood", "1996-11-23", "2000-01-01" };
            TestHelpers.RunCommand(_factory, arguments, new ReportCommand(), CommandMode.CommandLine, null, null, null, "report-species.txt", 0);
        }

        [TestMethod]
        public void StatusReportTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            _factory.SpeciesStatusImport.Import(importFilePath);

            string[] arguments = new string[] { "status", "white-fronted goose" };
            TestHelpers.RunCommand(_factory, arguments, new ReportCommand(), CommandMode.CommandLine, null, null, null, "report-status.txt", 0);
        }

        [TestMethod]
        public void StatusReportForSchemeTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            _factory.SpeciesStatusImport.Import(importFilePath);

            string[] arguments = new string[] { "status", "white-fronted goose", "BOCC4" };
            TestHelpers.RunCommand(_factory, arguments, new ReportCommand(), CommandMode.CommandLine, null, null, null, "report-status-for-scheme.txt", 0);
        }

        [TestMethod]
        public void StatusReportAtDateTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            _factory.SpeciesStatusImport.Import(importFilePath);

            string[] arguments = new string[] { "status", "white-fronted goose", "BOCC4", "2016-12-31" };
            TestHelpers.RunCommand(_factory, arguments, new ReportCommand(), CommandMode.CommandLine, null, null, null, "report-status-at-date.txt", 0);
        }
    }
}
