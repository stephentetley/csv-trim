﻿// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause

module DynaCsv.DynamicCsv

open FSharp.Data

open DynaCsv.Common
open DynaCsv.CsvOutput
open DynaCsv.Record

let private anonColumn (column:int) : string  = 
    let alphabet : char [] = [| 'A' .. 'Z' |]
    let char1 (ix:int) : char = 
        alphabet.[ix]
    if column < 26 then 
        (char1 column).ToString ()
    else if column < 702 then
        let a = (column / 26) - 1 
        let b = column % 26
        sprintf "%c%c" (char1 a) (char1 b)
    else 
        failwith "Column limit (702) exceeded"

let anonHeaders (count:int) : string list = 
    List.map anonColumn [0.. count-1]



// Records are heterogenous assoc lists
type DynaRow = 
    val private row : Record

    new () = { row = Record.empty }

    /// Build a row from lists of columns and values.
    new (columnNames : string list, values: string list) = 
        let rec work (ns: string list) (vs: string list) (ac:Record) = 
            match ns, vs with
            | [], _ -> ac
            | (x :: xs), (y :: ys) -> work xs ys (extend x y ac) 
            | (x :: xs), _ -> work xs [] (extend x null ac)
        { row = work columnNames values Record.empty }

    member internal x.ToOutputRow (fieldNames: string list) : OutputRow = 
        let cells = 
            List.map (fun name -> let o = select name x.row in Cell (o.ToString())) fieldNames
        new OutputRow (cells= cells)
        
        
type CsvFileOptions = 
    { HasHeaders: bool 
      Separator: char
      Quote: char }
    

type DynamicCsv = 
    val private rows : seq<DynaRow>

    new () = { rows = Seq.empty }

    new (csvFile:CsvFile) = 
        let headers = 
            match csvFile.Headers with
            | None -> anonHeaders csvFile.NumberOfColumns
            | Some arr -> Array.toList arr
        let makeRow (row:CsvRow) : DynaRow = 
            new DynaRow (columnNames = headers, values = Array.toList row.Columns )
        let rows = Seq.map makeRow csvFile.Rows
        { rows = rows }

    new (options: CsvFileOptions, path:string) =
        let sep = options.Separator.ToString()
        let csvFile = providerReadCsv options.HasHeaders sep options.Quote path
        new DynamicCsv (csvFile = csvFile)


    member internal x.ToOutputCsv (fieldNames: string list) : CsvOutput = 
        let csvRows = Seq.map (fun (row:DynaRow) -> row.ToOutputRow(fieldNames)) x.rows
        new CsvOutput(headers = fieldNames, rows = csvRows)
    
    member x.SaveToString (fields: string list) : string = 
        let output = x.ToOutputCsv(fields) in output.SaveToString()


        