// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


#r "netstandard"
#r "System.Xml.Linq.dll"

open System.IO

// Use FSharp.Data for CSV reading
#I @"C:\Users\stephen\.nuget\packages\FSharp.Data\3.0.0\lib\netstandard2.0"
#r @"FSharp.Data.dll"
open FSharp.Data


// New attempt for 2019...

type CsvReadConfig = 
    { HasHeaders:bool 
      Separator: char
      QuoteChar: char }


let providerReadCsv (config:CsvReadConfig) (path:string) : CsvFile = 
    CsvFile.Load( uri=path,
        hasHeaders = config.HasHeaders,
        separators = config.Separator.ToString(),
        quote = config.QuoteChar )


let trimRow (row1:CsvRow) : CsvRow = 
    for ix in 0 .. row1.Columns.Length - 1 do
        row1.Columns.[ix] <- (row1.Columns.[ix].Trim())
    row1    

let trimRowFunc : System.Func<CsvRow,CsvRow> = 
    new System.Func<CsvRow,CsvRow> (trimRow)

let demo01 ()  = 
    let config = { HasHeaders = true; Separator = '\t'; QuoteChar = '"'}
    let csv = providerReadCsv config @"G:\work\ADB-exports\RTS_OUTSTATIONS.tab.csv"
    let csv1 = csv.Map(trimRowFunc)
    use sw = new System.IO.StreamWriter(@"G:\work\ADB-exports\RTS_OUTSTATIONS.trim2.csv")
    csv1.Save(sw, separator=',', quote='"')



    