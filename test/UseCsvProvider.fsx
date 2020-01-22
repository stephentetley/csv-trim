// Copyright (c) Stephen Tetley 2020
// License: BSD 3 Clause

#r "netstandard"
#r "System.Xml.Linq.dll"
open System.IO

// Use FSharp.Data for CSV reading
#I @"C:\Users\stephen\.nuget\packages\FSharp.Data\3.3.3\lib\netstandard2.0"
#r @"FSharp.Data.dll"
open FSharp.Data

/// Joins with Environment.NewLine
let fromLines (source:string list) : string = 
    String.concat System.Environment.NewLine source

/// Makes a dummy row for a CsvFile.
/// We can't remove the dummy row at this point because `csv.Skip 1` 
/// changes the type to CsvFile<CsvRow>.
let makeDummyRow (columnCount:int) (separator:string) : string = 
    List.replicate columnCount "abc" |> String.concat separator


/// Make a CsvFile with headers and a dummy row.
/// We can't remove the dummy row at this point because `csv.Skip 1` 
/// changes the type to CsvFile<CsvRow>.
let makeDummyHeaders (headers:string[]) (sep:char) (quote:char) : CsvFile = 
    let separators = sep.ToString()
    let headings = String.concat separators headers
    let dummy = makeDummyRow headers.Length separators
    let body = fromLines [ headings; dummy ]
    CsvFile.Parse(text = body, hasHeaders = true, quote = quote, separators = separators)


let saveCsv (headers : string []) (sep: char) (quote: char) (rows: seq<string []>) (outputFile: string) : unit = 
    let makeCsvRow (parent:CsvFile) (row:string []) : CsvRow = new CsvRow(parent=parent, columns = row)
    let parent:CsvFile = makeDummyHeaders headers sep quote
    let rows:seq<CsvRow> = rows |> Seq.map (makeCsvRow parent)
    let csv1 = parent.Skip 1
    let csv2 = csv1.Append rows
    csv2.Save(path = outputFile, separator = sep, quote = quote)

let demo01 () = 
    let headers = [| "name"; "country" |]
    let rows = Seq.ofList [ [| "Robert"; "USA" |]; [| "Robert"; "USA" |] ]
    saveCsv headers ',' '"' rows @"E:\coding\fsharp\dyna-csv\output\dyna.csv"




