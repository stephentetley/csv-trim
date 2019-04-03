// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause

namespace DynaCsv.Internal

module Common = 

    open System
    open FSharp.Data

    /// Splits on Environment.NewLine
    let toLines (source:string) : string list = 
        source.Split(separator=[| Environment.NewLine |], options=StringSplitOptions.None) |> Array.toList

    /// Joins with Environment.NewLine
    let fromLines (source:string list) : string = 
        String.concat Environment.NewLine source

    /// Makes a CsvFile with a dummy row.
    /// We can't remove the dummy row at this point because `csv.Skip 1` 
    /// changes the type to CsvFile<CsvRow>.
    let makeDummyRow (columnCount:int) (separator:string) : string = 
        List.replicate columnCount "abc" |> String.concat separator
        
    /// Make an anonymous (i.e. headers free) CsvFile with a dummy row.
    /// We can't remove the dummy row at this point because `csv.Skip 1` 
    /// changes the type to CsvFile<CsvRow>.
    let makeDummyAnon (columnCount:int) (sep:char) (quote:char) : CsvFile = 
        let separators = sep.ToString()
        let dummy = makeDummyRow columnCount separators
        CsvFile.Parse(text = dummy, hasHeaders = false, quote = quote, separators = separators)

    /// Make a CsvFile with headers and a dummy row.
    /// We can't remove the dummy row at this point because `csv.Skip 1` 
    /// changes the type to CsvFile<CsvRow>.
    let makeDummyHeaders (headers:string[]) (sep:char) (quote:char) : CsvFile = 
        let separators = sep.ToString()
        let headings = String.concat separators headers
        let dummy = makeDummyRow headers.Length separators
        let body = fromLines [ headings; dummy ]
        CsvFile.Parse(text = body, hasHeaders = false, quote = quote, separators = separators)


        