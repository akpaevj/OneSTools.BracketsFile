# OneSTools.BracketsFile
[![Nuget](https://img.shields.io/nuget/v/OneSTools.BracketsFile)](https://www.nuget.org/packages/OneSTools.BracketsFile)<br>
Библиотека для парсинга внутрисистемного формата файлов 1С.

Файлы данного формата размещаются либо в чистом виде, либо запакованы в raw deflate (сырой deflate) и представляют из себя дерево значений:
```
{
  Value,
  Value1,
  Value2,
  {
    Value3,
    "TextValue"
  },
  Value5
}.
```
