// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause

module DynaCsv.DynamicCsv

open FSharp.Data

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
    val private row : (string * string) list 

    new () = { row = [] }

    /// Build a row from lists of columns and values.
    new (columnNames : string list, values: string list) = 
        let rec work (ns: string list) (vs: string list) (ac:(string * string) list) = 
            match ns, vs with
            | [], _ -> List.rev ac
            | (x :: xs), (y :: ys) -> work xs ys ((x,y)::ac) 
            | (x :: xs), _ -> work xs [] ((x,null)::ac)
        { row = work columnNames values [] }



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

