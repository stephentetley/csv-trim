﻿// Copyright (c) Stephen Tetley 2018,2019
// License: BSD 3 Clause

module DynaCsv.Old.DynamicCsv

open FSharp.Data

open DynaCsv.Old.Common
open DynaCsv.Old.CsvOutput
open DynaCsv.Old.Record



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

type IFieldMapper<'a,'b> = 
    abstract member FieldName : string
    abstract member Typecast: obj -> 'a
    abstract member Update: 'a -> 'b


// Records are heterogenous assoc lists
type Row = 
    val private row : Record

    new () = { row = Record.empty }

    private new (record:Record) = { row = record }

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
        new OutputRow (cells = cells)
      

    member x.MapField(helper:IFieldMapper<'a,'b>) : Row = 
        new Row (mapField helper.FieldName helper.Typecast helper.Update x.row)

        
type CsvFileOptions = 
    { HasHeaders: bool 
      Separator: char
      Quote: char }
    

type DynaCsv = 
    val private rows : seq<Row>
    val mutable private separator : char
    val mutable private quoteChar : char


    new () = { rows = Seq.empty
             ; separator = defaultSeparator
             ; quoteChar = defaultQuote }

    private new (rows:seq<Row>, sep:char, quote:char) = 
        { rows = rows; separator = sep; quoteChar = quote }

    new (csvFile:CsvFile) = 
        let headers = 
            match csvFile.Headers with
            | None -> anonHeaders csvFile.NumberOfColumns
            | Some arr -> Array.toList arr
        let makeRow (row:CsvRow) : Row = 
            new Row (columnNames = headers, values = Array.toList row.Columns )
        let rows = Seq.map makeRow csvFile.Rows
        { rows = rows; separator = defaultSeparator; quoteChar = defaultQuote }

    new (options: CsvFileOptions, path:string) =
        let sep = options.Separator.ToString()
        let csvFile = providerReadCsv options.HasHeaders sep options.Quote path
        new DynaCsv (csvFile = csvFile)


    member x.Separator
        with get() = x.separator
        and  set(v:char) = x.separator <- v

    member x.QuoteChar
        with get() = x.quoteChar
        and  set(v:char) = x.quoteChar <- v


    member internal x.ToOutputCsv (fieldNames: string list) : CsvOutput = 
        let csvRows = Seq.map (fun (row:Row) -> row.ToOutputRow(fieldNames)) x.rows
        new CsvOutput(headers = fieldNames
                     , rows = csvRows
                     , separator = x.separator
                     , quote = x.quoteChar )
    
    member x.SaveToString (fields: string list) : string = 
        let output = x.ToOutputCsv(fields) in output.SaveToString()

    member x.SaveToString (fields: string []) : string = 
        x.SaveToString(Array.toList fields)

    member x.Map (mapping:Row -> Row) : DynaCsv = 
        let rows = Seq.map mapping x.rows
        new DynaCsv (rows = rows, sep = x.separator, quote = x.quoteChar)