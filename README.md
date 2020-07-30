# OneSTools.BracketsFile
[![Nuget](https://img.shields.io/nuget/v/OneSTools.BracketsFile)](https://www.nuget.org/packages/OneSTools.BracketsFile)<br>
Библиотека для парсинга внутрисистемного формата файлов 1С.

Файлы данного формата запакованы в raw deflate (сырой deflate) и и в распакованном виде представляют из себя иерархический массив значений:
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
