using System;
using Xunit;
using OneSTools.BracketsFile;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace OneSTools.BracketsFile.Tests
{
    public class BracketsFileParserTests
    {
        [Fact]
        public void ParseBlock1Test()
        {
            // Arrange
            var data = "{20201005114853,U,\n" +
                "{243b06bad83e0,7b3156},71,36,1,13732,11,I,\"\",55,\n" +
                "{\"R\",490:bace0cc47a56444311eaedd56d0dbdf8},\"Отчет производства за \"\",смену Уни00023710 от 03.09.2020 14:05:56\",1,1,0,1101,0,\n" +
                "{0}" +
                "}";

            // Act
            var parsedData = BracketsParser.ParseBlock(data);

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
            Assert.Equal("Отчет производства за \"\",смену Уни00023710 от 03.09.2020 14:05:56", (string)parsedData[12]);
            Assert.Equal(1, (int)parsedData[13]);
            Assert.Equal(1, (int)parsedData[14]);
            Assert.Equal(0, (int)parsedData[15]);
            Assert.Equal(1101, (int)parsedData[16]);
            Assert.Equal(0, (int)parsedData[17]);
            Assert.False(parsedData[18].IsValueNode);
            Assert.Equal(0, (int)parsedData[18].Nodes[0]);
        }

        [Fact]
        public void ParseBlock2Test()
        {
            // Arrange
            var data = "{20201005085729,U,\n" +
                "{0,0},75,2,5,15446,1,I,\"\",0,\n" +
                "{\"P\",\n" +
                "{2," +
                "{\"S\",\"исрв\"}\n" +
                "}\n" +
                "},\"\",1,1,0,1137,0,\n" +
                "{0}" +
                "}";

            // Act
            var parsedData = BracketsParser.ParseBlock(data);

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
            Assert.Equal("исрв", (string)parsedData[11][1][1][1]);
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
        public void ParseBlock3Test()
        {
            // Arrange
            var data = "{1,071523a4-516f-4fce-ba4b-0d11ab7a1893,\"\",1}";

            // Act
            var parsedData = BracketsParser.ParseBlock(data);

            // Assert
            Assert.Equal(4, parsedData.Count);
            Assert.Equal(1, (int)parsedData[0]);
            Assert.True("071523a4-516f-4fce-ba4b-0d11ab7a1893".Equals((string)parsedData[1]));
            Assert.True(string.Empty == (string)parsedData[2]);
            Assert.Equal(1, (int)parsedData[3]);
        }

        [Fact]
        public void ParseBlock4Test()
        {
            // Arrange
            var value = @"{{1234,N,1234N,""123"",{0},{0,0},{""U""},""Hello, symbol is '{'"",""Symbol is '}'"",""%Symbol is """"}"""""",""symbol is ','"",""2 symbol is ','""},""}"","","",""""}";

            // Act
            var block = BracketsParser.ParseBlock(value);

            // Assert
            Assert.Equal(4, block.Count);
        }
        
        [Fact]
        public void GetNodeEndIndexTest()
        {
            // Arrange 
            var strBuilder = new StringBuilder(@"{3,""WSConnection"",1},");

            // Act
            var index = BracketsParser.GetNodeEndIndex(strBuilder, 0);

            // Assert
            Assert.Equal(19, index);
        }

        [Fact]
        public void BracketsListReaderTest()
        {
            // Arrange 
            const string data = "23e32 \n{1,\"WSConnection\",1},{2,\"WSConnection\",1},\n{3,\"WSConnection\",1},{п";
            using var mStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            using var reader = new BracketsListReader(mStream);

            var count = 0;

            var resultItems = new List<string>();

            // Act
            while (!reader.EndOfStream)
            {
                var item = reader.NextNodeAsStringBuilder();

                if (item.Length == 0)
                    break;

                count++;
                resultItems.Add(item.ToString());
            }

            // Assert
            Assert.Equal(3, count);
            Assert.Equal("{1,\"WSConnection\",1}", resultItems[0]);
            Assert.Equal("{2,\"WSConnection\",1}", resultItems[1]);
            Assert.Equal("{3,\"WSConnection\",1}", resultItems[2]);
            Assert.Equal(71, reader.Position);
        }

        [Fact]
        public void BracketsParserMultilineParseTest()
        {
            const string s = @"{20210119021929,N,
                {0,0},1,1,3,149022,20,I,""Получен ответ Сервиса классификаторов:
                [{""""classifierNick"""":""""PITDeductions"""",""""classifierName"""":""""Размер вычетов НДФЛ"""",""""versionDescription"""":"""""""",""""version"""":2,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/30c90ee3-1c5b-11e8-80d8-0050569f1015"""",""""fileSize"""":18822,""""hashSum"""":""""33L4yNJbCmDw7SrtM3XZzw==""""},{""""classifierNick"""":""""MaxMonthlyInsurancePayout"""",""""classifierName"""":""""Максимальный размер ежемесячной страховой выплаты"""",""""versionDescription"""":""""В соответствии со статьей 12 Федерального закона от 24.07.1998 № 125-ФЗ проиндексирован размер выплаты с 1 февраля 2020 года."""",""""version"""":5,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/00cfae4a-3923-11ea-80ec-0050569f1015"""",""""fileSize"""":2428,""""hashSum"""":""""y28tuv/TEU12m4eVPuMnzg==""""},{""""classifierNick"""":""""EffectiveDatesOfRegulatoryActs"""",""""classifierName"""":""""Даты вступления в силу нормативных актов"""",""""versionDescription"""":""""Дата вступления в силу постановления Правления ПФР от 27 сентября 2019 № 485п."""",""""version"""":6,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/c939a4f2-4670-11ea-80ed-0050569f1015"""",""""fileSize"""":1433,""""hashSum"""":""""hmZgfRmYQBeLy8zAhuzfdA==""""},{""""classifierNick"""":""""Countries"""",""""classifierName"""":""""Общероссийский классификатор стран мира (ОКСМ)"""",""""versionDescription"""":""""Изменение 25/2019 ОКСМ Общероссийский классификатор стран мира ОК (МК (ИСО 3166) 004-97) 025-2001 Республика Северная Македония"""",""""version"""":3,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/4cce28e4-9c02-11ea-80f4-0050569f1015"""",""""fileSize"""":64730,""""hashSum"""":""""pjlupZJ96+ZRkCVR3sYreA==""""},{""""classifierNick"""":""""Currencies"""",""""classifierName"""":""""Общероссийский классификатор валют (ОКВ)"""",""""versionDescription"""":""""Сведения о валютах по состоянию на 01.06.2020"""",""""version"""":2,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/b41e13ae-a3e3-11ea-80f5-0050569f1015"""",""""fileSize"""":24109,""""hashSum"""":""""9CzW1kOCzFmojn1rg0Chqg==""""},{""""classifierNick"""":""""ChildAndDeathBenefits"""",""""classifierName"""":""""Размеры государственных пособий"""",""""versionDescription"""":""""Изменены размеры пособий по уоду за детьми до полутора лет"""",""""version"""":5,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/c15950d7-a76d-11ea-80f5-0050569f1015"""",""""fileSize"""":9611,""""hashSum"""":""""b8A6BApqFQHONAEri53Pdw==""""},{""""classifierNick"""":""""CentralBankRefinancingRate"""",""""classifierName"""":""""Ставка рефинансирования ЦБ"""",""""versionDescription"""":""""Изменение ставки рефинансирования (ключевой ставки) с 27.07.2020 в соответствии с Информацией Банка России от 24.07.2020."""",""""version"""":14,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/d9ecbdfc-cdb2-11ea-80f7-0050569f1015"""",""""fileSize"""":6599,""""hashSum"""":""""li/QL1zEA2isdEBOBBDguA==""""},{""""classifierNick"""":""""InsurancePaymentPercentagesR2"""",""""classifierName"""":""""Тарифы страховых взносов"""",""""versionDescription"""":""""Обновлены тарифы страховых взносов в соответствии с Федеральным законом от 31.07.2020 № 265-ФЗ."""",""""version"""":12,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/5f2f1bf4-0359-11eb-80f9-0050569f1015"""",""""fileSize"""":43878,""""hashSum"""":""""qG0TLoSjHf760lrvDUu/xA==""""},{""""classifierNick"""":""""MaxInsurancePaymentBasis"""",""""classifierName"""":""""Предельная величина базы страховых взносов"""",""""versionDescription"""":""""Постановление Правительства РФ от 26.11.2020 № 1935"""",""""version"""":5,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/f963e789-33b4-11eb-80fa-0050569f1015"""",""""fileSize"""":4067,""""hashSum"""":""""UvUg80Ot5DjmdcZ4eihSAQ==""""},{""""classifierNick"""":""""MinMonthlyWage"""",""""classifierName"""":""""Минимальная оплата труда РФ"""",""""versionDescription"""":""""В соответствии с Федеральным законом \""""О внесении изменений в отдельные законодательные акты Российской Федерации\"""" минимальный размер оплаты труда с 1 января 2021 года установлен в сумме 12792 рублей в месяц."""",""""version"""":6,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/82ceae1b-45b8-11eb-80fa-0050569f1015"""",""""fileSize"""":3718,""""hashSum"""":""""lzJIyURtb1/wv5SgcC0Kaw==""""},{""""classifierNick"""":""""Calendars20"""",""""classifierName"""":""""Календари"""",""""versionDescription"""":""""1. Уточнена дата праздничного дня «Сагаалган» 13 февраля 2021 года вместо 12 февраля 2021 года для Республики Бурятия, Республики Калмыкия, Республики Тыва, Забайкальского края и Усть-Ордынского Бурятского округа Иркутской области.\n\n2. Добавлены даты праздничных дней Курбан-Байрам 20 июля 2021 года и День поминовения усопших (Радоница) 11 мая 2021 года в Республике Адыгея."""",""""version"""":18,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/1cecd672-5966-11eb-80fa-86643b3f0aba"""",""""fileSize"""":115218,""""hashSum"""":""""7Qoi30QlTNeW8HCb7xcVCg==""""},{""""classifierNick"""":""""Banks"""",""""classifierName"""":""""Справочник по кредитным организациям"""",""""versionDescription"""":""""Сведения о кредитных организациях по состоянию на 17.01.2021*\n\n——\n* Не содержит территориальных отделений Федерального казначейства (ТОФК). Для соответствия 479-ФЗ от 27.12.2019 «О внесении изменений в Бюджетный кодекс Российской Федерации в части казначейского обслуживания и системы казначейских платежей» необходимо обновить версию программы и перейти на загрузку классификатора «Справочник БИК»."""",""""version"""":677,""""fileUrl"""":""""https://dl03.1c.ru/public/classifier/download/742655b8-5995-11eb-80fa-86643b3f0aba"""",""""fileSize"""":512094,""""hashSum"""":""""f9LNnQOixRjD+YihIN833g==""""}]"",0,
                {""U""},"""",1,1,0,420,0,
                {2,1,1,2,1}
                },
                {20210119000237,N,
                {0,0},1,1,3,145991,8,I,"""",1259,
                {""S"",""""},"""",1,1,0,418,0,
                {2,1,1,2,1}
                }";

            using var mStream = new MemoryStream(Encoding.UTF8.GetBytes(s));
            using var stream = new StreamReader(mStream);
            var reader = new BracketsListReader(stream);

            // Act
            var item = reader.NextNodeAsStringBuilder();
            var node = BracketsParser.ParseBlock(item);

            var item2 = reader.NextNodeAsStringBuilder();
            var node2 = BracketsParser.ParseBlock(item2);

            // Assert
            Assert.Equal(19, node.Count);
            Assert.Equal(19, node2.Count);
        }
    }
}
