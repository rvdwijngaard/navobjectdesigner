﻿using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var req = new Mock<IObjectDesignerRequest>();
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
            var designer = new Nav2015(new ObjectDesignerRequest(), runner.Object);
            var request = GetImportRequest();

            // act
            var result = designer.Import(request);

            // assert
            Assert.True(result.Result);
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerRequest>(), It.IsAny<string>()), Times.Exactly(request.Files.Count()));
        }

        [Fact]
        public void ImportWithErrors()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            runner.Setup(x => x.RunCommand(It.IsAny<IObjectDesignerRequest>(), It.IsRegex("Codeunit3.txt"))).Returns("[12345] Import Error");
            var designer = new Nav2015(new ObjectDesignerRequest(), runner.Object);
            var request = GetImportRequest();

            // act
            var result = designer.Import(request);

            // assert
            Assert.False(result.Result);
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerRequest>(), It.IsAny<string>()), Times.Exactly(request.Files.Count()));
            Assert.IsType<ObjectDesignerException>(result.Exceptions.First());
            Assert.Equal("Codeunit3.txt", result.Exceptions.First().Source);
        }

        [Fact]
        public void LicenseWarning_ShouldBeIgnored()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            runner.Setup(x => x.RunCommand(It.IsAny<IObjectDesignerRequest>(), It.IsRegex("Codeunit3.txt"))).Returns("[18023763] Your license has expired");
            var designer = new Nav2015(new ObjectDesignerRequest(), runner.Object);
            var request = GetImportRequest();

            // act            
            var result = designer.Import(request);

            // assert
            Assert.True(result.Result);
        }

        [Fact]
        public void Nav2015_TestImportCommandParameters()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            var designer = new Nav2015(new ObjectDesignerRequest(), runner.Object);
            var request = GetImportRequest(1);

            // act
            var result = designer.Import(request);

            // assert
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerRequest>(), It.IsRegex("synchronizeschemachanges=force")));
        }

        [Fact]
        public void Nav2013R2_TestImportCommandParameters()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            var designer = new Nav2013R2(new ObjectDesignerRequest(), runner.Object);
            var request = GetImportRequest(1);

            // act
            var result = designer.Import(request);

            // assert
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerRequest>(), It.IsRegex("validatetablechanges=0")));
        }

        [Fact]
        public void Compile()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            var designer = new Nav2013R2(new ObjectDesignerRequest(), runner.Object);
            var request = new CompileRequest() { Filter = "Locked=1" };

            // act
            var result = designer.Compile(request);

            // assert
            Assert.True(result.Succesful);
            Assert.Null(result.Exceptions.FirstOrDefault());
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerRequest>(), It.IsRegex(request.Filter)), Times.Once);
        }

        [Fact]
        public void CompileWithErrors()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            string error = "[23462397] You have specified an unknown variable.\r\n\r\nsaerwer\r\n\r\nDefine the variable under 'Global C/AL symbols'. -- Object: Codeunit 90001 nxxx tribute\r\n[31588355] Could not load type 'TI.DocumentManagement.DocumentUploaderActivity.'TI.DocumentManagement, Version=13.4.0.0, Culture=neutral, PublicKeyToken=f476381a83e1102f''. -- Object: Codeunit 11068711 N108 Document Upload Activity\r\n[31588355] Could not load type 'TI.DocumentManagement.DocumentManagement.'TI.DocumentManagement, Version=13.4.0.0, Culture=neutral, PublicKeyToken=f476381a83e1102f''. -- Object: Page 11068709 N108 Document Mgt. Factbox\r\n";
            runner.Setup(x => x.RunCommand(It.IsAny<IObjectDesignerRequest>(), It.IsAny<string>())).Returns(error);
            var designer = new Nav2013R2(new ObjectDesignerRequest(), runner.Object);
            var request = new CompileRequest() { Filter = "Locked=1" };

            // act
            var result = designer.Compile(request);

            // assert
            Assert.False(result.Succesful);
            Assert.IsType<CompilationException>(result.Exceptions.FirstOrDefault());
            Assert.Equal(3, result.Exceptions.Count());
        }

        [Fact]
        public void Export()
        {
            // arrange
            var runner = new Mock<ICommandRunner>();
            var designer = new Nav2013R2(new ObjectDesignerRequest(), runner.Object);
            var request = new ExportRequest() { Filter = "Locked=1" };

            // act
            var result = designer.Export(request);

            // assert
            Assert.True(result.Succesful);
            Assert.Null(result.Exception);
            runner.Verify(x => x.RunCommand(It.IsAny<IObjectDesignerRequest>(), It.IsRegex(request.Filter)), Times.Once);
        }


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
    }
}
