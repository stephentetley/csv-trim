// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause

open System.IO

// Use FSharp.Data for CSV reading
#I @"..\packages\FSharp.Data.3.0.0-beta3\lib\net45"
#r @"FSharp.Data.dll"
open FSharp.Data

#load "..\src\DynaCsv\CsvOutput.fs"
open DynaCsv.CsvOutput





type CsvProTable = 
    CsvProvider< Sample = "A,B,C\n1,2,3\n1,3,2\n",
                 Schema = "A(int),B(int),C(int)",
                 HasHeaders = true >

type CsvProRow = CsvProTable.Row

let csvProTableHelper : ICsvProviderDestruct<CsvProTable>  =
    let conv (row:CsvProRow) : OutputRow = 
        new OutputRow([csvInt row.A; csvInt row.B; csvInt row.C ])
    { new ICsvProviderDestruct<CsvProTable>
      with member this.GetHeaders table = table.Headers
           member this.GetRows table = table.Rows |> Seq.map conv }

let test01 () = 
    (new CsvProTable()).SaveToString()

let test02 () = 
    try 
        char <| (new CsvProTable()).Separators
    with
    | ex -> failwithf "FAILED: %s" ex.Message

/// This fails - can't seem to get a CsvFile from a typed CsvProvider "Table".
let test03 () = 
    try 
        let csv1:obj = (new CsvProTable()) :> obj 
        let csv2 = csv1 :?> CsvFile
        let mycsv = new CsvOutput(csvFile = csv2)
        mycsv.SaveToString()

    with
    | ex -> failwithf "FAILED: %s" ex.Message

let test04 () = 
    try 
        let csv1 = new CsvProTable()
        let mycsv = CsvOutput.FromCsvTable(csvProTableHelper, csv1)
        mycsv.SaveToString()

    with
    | ex -> failwithf "FAILED: %s" ex.Message







