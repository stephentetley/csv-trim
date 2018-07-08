module CsvTrim.CpTrim

open FSharp.Data

type CsvTrimOptions = 
    { InputSeparator: string
      InputHasHeaders: bool
      OutputSeparator: string }

let quoteField (input:string) : string = 
    match input with
    | null -> "\"\""
    | _ -> sprintf "\"%s\"" (input.Replace("\"", "\"\""))

let private optQuote(s:string) : string = 
    if s.Contains "," then quoteField s else s
    

/// This one writes directly to a StreamWriter.
/// Note - input text inside double quotes is not trimmed.
let trimCsvFile2 (options:CsvTrimOptions) (inputFile:string) (outputFile:string) : unit =
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
