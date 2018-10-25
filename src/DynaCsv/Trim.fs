// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause

module DynaCsv.Trim

open FSharp.Data

open DynaCsv.Common
open DynaCsv.CsvOutput

type CsvTrimOptions = 
    { InputSeparator: string
      InputQuote: char
      InputHasHeaders: bool
      OutputSeparator: char
      OutputQuote:char }


/// This one writes directly to a StreamWriter.
/// Note - input text inside double quotes is not trimmed.
let trimCsvFile (options:CsvTrimOptions) (inputFile:string) (outputFile:string) : unit =

    let csvInput : CsvFile = 
        providerReadCsv options.InputHasHeaders options.InputSeparator options.InputQuote inputFile 

    let headers : option<string list> = 
        match csvInput.Headers with
        | None -> None
        | Some arr -> Some <| Array.toList arr
    
    let providerRowToDyna (row1:CsvRow) : Row = 
        new Row (Array.map (fun (s:string) -> csvString (s.Trim())) row1.Columns )
        

    let rows = Seq.map providerRowToDyna csvInput.Rows

    let csv1 = 
        match headers with
        | None -> new CsvOutput(rows = rows)
        | Some headers -> new CsvOutput(headers = headers, rows = rows)
    csv1.Separator <- options.OutputSeparator
    csv1.QuoteChar <- options.OutputQuote  

    use sw = new System.IO.StreamWriter(outputFile)
    csv1.Save(sw)
