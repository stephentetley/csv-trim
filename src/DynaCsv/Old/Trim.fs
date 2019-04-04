// Copyright (c) Stephen Tetley 2018,2019
// License: BSD 3 Clause

module DynaCsv.Old.Trim

open FSharp.Data


type CsvTrimOptions = 
    { InputSeparator: char
      InputQuote: char
      InputHasHeaders: bool
      OutputSeparator: char
      OutputQuote:char }


let private trimRow (row1:CsvRow) : CsvRow = 
    for ix in 0 .. row1.Columns.Length - 1 do
        row1.Columns.[ix] <- (row1.Columns.[ix].Trim())
    row1    

let private trimRowFunc : System.Func<CsvRow,CsvRow> = 
    new System.Func<CsvRow,CsvRow> (trimRow)


/// This one writes directly to a StreamWriter.
/// Note - input text inside double quotes is not trimmed.
let trimCsvFile (options:CsvTrimOptions) (inputFile:string) (outputFile:string) : unit =

    let csv = CsvFile.Load(uri=inputFile, 
                            separators = options.InputSeparator.ToString(),
                            hasHeaders = options.InputHasHeaders, 
                            quote = options.InputQuote )
    let csv1 = csv.Map(trimRowFunc)
    use sw = new System.IO.StreamWriter(outputFile)
    csv1.Save(writer = sw, 
                separator = options.OutputSeparator,
                quote = options.OutputQuote)


