using System;
using Xunit;
using OneSTools.BracketsFile;

namespace OneSTools.BracketsFile.Tests
{
    public class BracketsFileParserTests
    {
        [Fact]
        public void Parse1Test()
        {
            // Arrange
            var data = "{20201005114853,U,\n" +
                "{243b06bad83e0,7b3156},71,36,1,13732,11,I,\"\",55,\n" +
                "{\"R\",490:bace0cc47a56444311eaedd56d0dbdf8},\"����� ������������ �� \"\",����� ���00023710 �� 03.09.2020 14:05:56\",1,1,0,1101,0,\n" +
                "{0}" +
                "}";

            // Act
            var parsedData = BracketsFileParser.ParseBlock(ref data);

            // Assert
            Assert.Equal(19, parsedData.Count);
            Assert.Equal("20201005114853", (string)parsedData[0]);
            Assert.Equal("U", (string)parsedData[1]);
            Assert.False(parsedData[2].IsValueNode);
            Assert.Equal("243b06bad83e0", (string)parsedData[2].Nodes[0]);
            Assert.Equal("7b3156", (string)parsedData[2].Nodes[1]);
            Assert.Equal(71, (int)parsedData[3]);
            Assert.Equal(36, (int)parsedData[4]);
            Assert.Equal(1, (int)parsedData[5]);
            Assert.Equal(13732, (int)parsedData[6]);
            Assert.Equal(11, (int)parsedData[7]);
            Assert.Equal("I", (string)parsedData[8]);
            Assert.Equal("", (string)parsedData[9]);
            Assert.Equal(55, (int)parsedData[10]);
            Assert.False(parsedData[11].IsValueNode);
            Assert.Equal("R", (string)parsedData[11].Nodes[0]);
            Assert.Equal("490:bace0cc47a56444311eaedd56d0dbdf8", (string)parsedData[11].Nodes[1]);
            Assert.Equal("����� ������������ �� \"\",����� ���00023710 �� 03.09.2020 14:05:56", (string)parsedData[12]);
            Assert.Equal(1, (int)parsedData[13]);
            Assert.Equal(1, (int)parsedData[14]);
            Assert.Equal(0, (int)parsedData[15]);
            Assert.Equal(1101, (int)parsedData[16]);
            Assert.Equal(0, (int)parsedData[17]);
            Assert.False(parsedData[18].IsValueNode);
            Assert.Equal(0, (int)parsedData[18].Nodes[0]);
        }

        [Fact]
        public void Parse2Test()
        {
            // Arrange
            var data = "{20201005085729,U,\n" +
                "{0,0},75,2,5,15446,1,I,\"\",0,\n" +
                "{\"P\",\n" +
                "{2," +
                "{\"S\",\"����\"}\n" +
                "}\n" +
                "},\"\",1,1,0,1137,0,\n" +
                "{0}" +
                "}";

            // Act
            var parsedData = BracketsFileParser.ParseBlock(ref data);

            // Assert
            Assert.Equal(19, parsedData.Count);
            Assert.Equal("20201005085729", (string)parsedData[0]);
            Assert.Equal("U", (string)parsedData[1]);
            Assert.False(parsedData[2].IsValueNode);
            Assert.Equal("0", (string)parsedData[2].Nodes[0]);
            Assert.Equal("0", (string)parsedData[2].Nodes[1]);
            Assert.Equal(75, (int)parsedData[3]);
            Assert.Equal(2, (int)parsedData[4]);
            Assert.Equal(5, (int)parsedData[5]);
            Assert.Equal(15446, (int)parsedData[6]);
            Assert.Equal(1, (int)parsedData[7]);
            Assert.Equal("I", (string)parsedData[8]);
            Assert.Equal("", (string)parsedData[9]);
            Assert.Equal(0, (int)parsedData[10]);
            Assert.False(parsedData[11].IsValueNode);
            Assert.Equal("P", (string)parsedData[11].Nodes[0]);
            Assert.False(parsedData[11][1].IsValueNode);
            Assert.Equal(2, (int)parsedData[11][1][0]);
            Assert.False(parsedData[11][1][1].IsValueNode);
            Assert.Equal("S", (string)parsedData[11][1][1][0]);
            Assert.Equal("����", (string)parsedData[11][1][1][1]);
            Assert.Equal("", (string)parsedData[12]);
            Assert.Equal(1, (int)parsedData[13]);
            Assert.Equal(1, (int)parsedData[14]);
            Assert.Equal(0, (int)parsedData[15]);
            Assert.Equal(1137, (int)parsedData[16]);
            Assert.Equal(0, (int)parsedData[17]);
            Assert.False(parsedData[18].IsValueNode);
            Assert.Equal(0, (int)parsedData[18].Nodes[0]);
        }

        [Fact]
        public void Parse3Test()
        {
            // Arrange
            var data = "{1,071523a4-516f-4fce-ba4b-0d11ab7a1893,\"\",1}";

            // Act
            var parsedData = BracketsFileParser.ParseBlock(ref data);

            // Assert
            Assert.Equal(4, parsedData.Count);
            Assert.Equal(1, (int)parsedData[0]);
            Assert.Equal("071523a4-516f-4fce-ba4b-0d11ab7a1893", (string)parsedData[1]);
            Assert.Equal("", (string)parsedData[2]);
            Assert.Equal(1, (int)parsedData[3]);
        }

        [Fact]
        public void Parse4Test()
        {
            // Arrange
            var value = @"{{1234,N,1234N,""123"",{0},{0,0},{""U""},""Hello, symbol is '{'"",""Symbol is '}'"",""%Symbol is """"}"""""",""symbol is ','"",""2 symbol is ','""},""}"","","",""""}";

            // Act
            var block = BracketsFileParser.ParseBlock(ref value);

            // Assert
            Assert.Equal(4, block.Count);
        }

        [Fact]
        public void Parse5Test()
        {
            // Arrange
            var value = @"{{1234,N,1234N,""123"",{0},{0,0},{""U""},""Hello, symbol is '{'"",""Symbol is '}'"",""%Symbol is """"}"""""",""symbol is ','"",""2 symbol is ','""},""}"","","",""""}";

            // Act
            var block = BracketsFileParser.ParseBlock(value);

            // Assert
            Assert.Equal(4, block.Count);
        }
    }
}
