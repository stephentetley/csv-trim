// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause

module CsvTrim.CsvOutput2

open System
open System.IO


let quoteValue (quote:char) (input:string) : string = 
    match input with
    | null -> sprintf "%c%c" quote quote
    | _ -> 
        let quoteStr = quote.ToString()
        let text = input.Replace(quoteStr, quoteStr+quoteStr) 
        sprintf "%c%s%c" quote text quote


/// Cell must allow reify whether a value is quoted.
/// Then the printer can decide what charater to use for quoting.
type Cell = 
    | Cell of String
    | Quoted of String
    override x.ToString() = 
        match x with 
        | Cell(s) -> s
        | Quoted(s) -> sprintf "'%s'" s

    member x.Output (quoteChar:char) = 
        match x with
        | Cell s -> s
        | Quoted(s) -> quoteValue quoteChar s
    


type OutputOptions = 
    { Separator: char     
      Quote: char }

[<Struct>]
type Row = 
    | Row of Cell list
    member internal x.Cells = match x with | Row(xs) -> xs
/// Prints 'true' or 'false' (unquoted).
let csvBool (value:bool) : Cell = Cell <| value.ToString()

let private outputRow (sw:StreamWriter) (opts:OutputOptions) (row:Row) : unit = 
    let rec work (cells:Cell list) = 
        match cells with
        | [] -> sw.Write("\n")
        | [x] -> 
            sw.Write (x.Output (opts.Quote)); sw.Write("\n")
        | x :: xs -> 
            sw.Write (x.Output (opts.Quote)); sw.Write(opts.Separator); work xs
    work row.Cells

/// FSharp.Data API:
/// table.Save(writer = sw, separator = ',', quote = '"' )
/// table.SaveToString() 

/// So favor an object oriented API.

let outputCsv (sw:StreamWriter) (opts:OutputOptions) (rows:seq<Row>) : unit = 
    Seq.iter (outputRow sw opts) rows




