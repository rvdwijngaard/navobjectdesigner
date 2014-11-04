using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI.Nav.Utils.Exceptions;
using TI.Nav.Utils.Versions;
using Xunit;
using Xunit.Extensions;

namespace TI.Nav.Utils.Test
{
    public class ObjectDesignerFactoryTest
    {
        [Theory]
        [InlineData(5, 1, typeof(Classic))]
        [InlineData(6, 0, typeof(Classic))]
        [InlineData(6, 1, typeof(Classic))]
        [InlineData(7, 0, typeof(Nav2013))]
        [InlineData(7, 1, typeof(Nav2013R2))]
        [InlineData(8, 0, typeof(Nav2015))]
        public void GetObjectDesiger(int major, int minor, Type expected)
        {
            // arrange
            var req = new Mock<IObjectDesignerConfig>();
            req.SetupGet(x => x.MajorVersion).Returns(major);
            req.SetupGet(x => x.MinorVersion).Returns(minor);

            // act
            var result = ObjectDesignerFactory.GetObjectDesigner(req.Object);

            // assert            
            Assert.IsType(expected, result);
        }

        [Fact]
        public void Import()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            var designer = new Nav2015(new ObjectDesignerConfig(), runner.Object);
            var request = GetImportRequest();

            // act
            var result = designer.Import(request);

            // assert
            Assert.True(result.Successful);
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsAny<string>()), Times.Exactly(request.Files.Count()));
        }

        [Fact]
        public void ImportWithErrors()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            runner.Setup(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsRegex("Codeunit3.txt"))).Returns("[12345] Import Error");
            var designer = new Nav2015(new ObjectDesignerConfig(), runner.Object);
            var request = GetImportRequest();

            // act
            var result = designer.Import(request);

            // assert
            Assert.False(result.Successful);
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsAny<string>()), Times.Exactly(request.Files.Count()));
            Assert.IsType<ObjectDesignerException>(result.Exceptions.First());
            Assert.Equal("Codeunit3.txt", result.Exceptions.First().Source);
        }

        [Fact]
        public void ImportWithDeadlocks()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            runner.Setup(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsRegex("Codeunit3.txt"))).Returns("[22926089] Deadlock Error");

            var designer = new Nav2015(new ObjectDesignerConfig(), runner.Object);
            var request = GetImportRequest(10);

            // act
            var result = designer.Import(request);
            // assert
            Assert.True(result.Successful);
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsAny<string>()), Times.Exactly(request.Files.Count() + 1));
        }

        [Fact]
        public void LicenseWarning_ShouldBeIgnored()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            runner.Setup(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsRegex("Codeunit3.txt"))).Returns("[18023763] Your license has expired");
            var designer = new Nav2015(new ObjectDesignerConfig(), runner.Object);
            var request = GetImportRequest();

            // act            
            var result = designer.Import(request);

            // assert
            Assert.True(result.Successful);
        }

        [Fact]
        public void Nav2015_TestImportCommandParameters()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            var designer = new Nav2015(new ObjectDesignerConfig(), runner.Object);
            var request = GetImportRequest(1);

            // act
            var result = designer.Import(request);

            // assert
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsRegex("synchronizeschemachanges=no")));
        }

        [Fact]
        public void Nav2013R2_TestImportCommandParameters()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            var designer = new Nav2013R2(new ObjectDesignerConfig(), runner.Object);
            var request = GetImportRequest(1);

            // act
            var result = designer.Import(request);

            // assert
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsRegex("validatetablechanges=0")));
        }

        [Fact]
        public void Compile()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            var designer = new Nav2013R2(new ObjectDesignerConfig(), runner.Object);
            var request = new CompileRequest() { Filter = "Locked=1" };

            // act
            var result = designer.Compile(request);

            // assert
            Assert.True(result.Successful);
            Assert.Null(result.Exceptions.FirstOrDefault());
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsRegex(request.Filter)), Times.Exactly(7));
        }

        [Fact]
        public void CompileWithErrors()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            string error = "[23462397] You have specified an unknown variable.\r\n\r\nsaerwer\r\n\r\nDefine the variable under 'Global C/AL symbols'. -- Object: Codeunit 90001 nxxx tribute\r\n[31588355] Could not load type 'TI.DocumentManagement.DocumentUploaderActivity.'TI.DocumentManagement, Version=13.4.0.0, Culture=neutral, PublicKeyToken=f476381a83e1102f''. -- Object: Codeunit 11068711 N108 Document Upload Activity\r\n[31588355] Could not load type 'TI.DocumentManagement.DocumentManagement.'TI.DocumentManagement, Version=13.4.0.0, Culture=neutral, PublicKeyToken=f476381a83e1102f''. -- Object: Page 11068709 N108 Document Mgt. Factbox\r\n";
            runner.Setup(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsAny<string>())).Returns(error);
            var designer = new Nav2013R2(new ObjectDesignerConfig(), runner.Object);
            var request = new CompileRequest() { Filter = "Locked=1" };

            // act
            var result = designer.Compile(request);

            // assert
            Assert.False(result.Successful);
            Assert.IsType<CompilationException>(result.Exceptions.FirstOrDefault());
            Assert.Equal(21, result.Exceptions.Count());
        }

        [Fact]
        public void Export()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            var designer = new Nav2013R2(new ObjectDesignerConfig(), runner.Object);
            var filters = new List<ExportFilter>() { new ExportFilter() { Filter = "Locked=1", FileName = "export.txt" } };
            var request = new ExportRequest() { Filters = filters };

            // act
            var result = designer.Export(request);

            // assert
            Assert.True(result.Successful);
            Assert.Null(result.Exceptions.FirstOrDefault());
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsRegex(request.Filters.FirstOrDefault().Filter)), Times.Once);
        }

        [Fact]
        public void ExportWithMultipleFilters()
        {
            // arrange                        
            var filters = new List<ExportFilter>(){
                new ExportFilter() {FileName = "{0}_std.fob", Filter ="Version List=*NAVBIS*"}, 
                new ExportFilter() {FileName = "{0}_std.txt", Filter ="Version List=*NAVBIS*"},
                new ExportFilter() {FileName = "{0}_test.fob", Filter ="Version List=*NAVBIS*"},
            };

            var request = new ExportRequest() { Filters = filters };

            string lastCommand = null;
            var runner = new Mock<ICommandRunner>();
            runner.Setup(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsAny<string>()))
                .Callback<IObjectDesignerConfig, string>((c, s) =>
                    {
                        lastCommand = s;
                    });
            var designer = new Nav2013R2(new ObjectDesignerConfig(), runner.Object);

            // act
            var result = designer.Export(request);

            // assert

            Assert.Equal(filters.Count(), result.Files.Count());
            foreach (var f in filters)
            {
                Assert.NotNull(result.Files.ToList().FirstOrDefault(x => x == f.FileName));
            }
        }

        [Fact]
        public void ExportWithException()
        {
            // arrange
            var request = new ExportRequest { Filters = new List<ExportFilter>() { new ExportFilter() } };
            var runner = new Mock<ICommandRunner>();
            runner.Setup(x => x.RunCommand(It.IsAny<IObjectDesignerConfig>(), It.IsAny<string>())).Throws<IOException>();
            var designer = new Nav2013R2(new ObjectDesignerConfig(), runner.Object);

            // act            
            var result = designer.Export(request);

            // assert
            Assert.False(result.Successful);
            Assert.NotNull(result.Exceptions.FirstOrDefault());

        }

        #region private
        private ImportRequest GetImportRequest()
        {
            return GetImportRequest(4);
        }

        private ImportRequest GetImportRequest(int len)
        {
            var request = new ImportRequest();

            for (int i = 0; i < len; i++)
            {
                request.Files.Add(string.Format("Codeunit{0}.txt", i));
            }

            return request;
        }
        #endregion
    }
}
