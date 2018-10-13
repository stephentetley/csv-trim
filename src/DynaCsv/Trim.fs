// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause

module DynaCsv.Trim

open FSharp.Data

open DynaCsv.Common

type CsvTrimOptions = 
    { InputSeparator: string
      InputHasHeaders: bool
      OutputSeparator: string }


/// This one writes directly to a StreamWriter.
/// Note - input text inside double quotes is not trimmed.
let trimCsvFile (options:CsvTrimOptions) (inputFile:string) (outputFile:string) : unit =
    let rowToTrimmedString (row:string []) : string = 
        let sep = options.OutputSeparator
        String.concat sep <| Array.map (fun (s:string) -> optQuote <| s.Trim()) row
        
    let csvInput : CsvFile = 
        CsvFile.Load(uri=inputFile, 
            separators = options.InputSeparator,
            hasHeaders = options.InputHasHeaders, 
            quote= '"' )
   
    use sw = new System.IO.StreamWriter(outputFile)
   
    match csvInput.Headers with
    | Some titles -> 
        rowToTrimmedString titles |> sw.WriteLine
    | None -> ()
    
    Seq.iter (fun (row:CsvRow) -> 
                row.Columns |> rowToTrimmedString |> sw.WriteLine) csvInput.Rows 
