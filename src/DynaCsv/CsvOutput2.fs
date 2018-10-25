// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause

module DynaCsv.CsvOutput2

open System
open System.IO


/// The quoting behavior of Excel appears to be to be dynamic.
/// Strings are quoted if the contain the quote character or the separator.
/// Quoting is piecemeal on individual cell values it is not "controlled" by the column.
///
/// An example:
///
/// Id,Value,Comment
/// 1,Hello world,"No comma, no quotes"
/// 2,"Hello, world","Comma, no quotes"
/// 3,"""Hello, world""",Comma and quotes
/// 4,"""Hello word""","Quotes, no comma"


let quotedValue (quote:char) (input:string) : string = 
    match input with
    | null -> sprintf "%c%c" quote quote
    | _ -> 
        let quoteStr = quote.ToString()
        let text = input.Replace(quoteStr, quoteStr+quoteStr) 
        sprintf "%c%s%c" quote text quote

let quoteIfNecessary (quote:char) (separator:char) (input:string) : string = 
    if input.Contains(quote.ToString()) || input.Contains(separator.ToString()) || input.Contains("\n") then
        quotedValue quote input
    else input



/// A cell can be always quoted (Quoted) or quoted if it contains
/// the quote character or the separator.
type Cell = 
    | Cell of String
    | Quoted of String
    override x.ToString() = 
        match x with 
        | Cell(s) -> s
        | Quoted(s) -> sprintf "'%s'" s

    member internal x.Output (quoteChar:char) (separator:char) : string = 
        match x with
        | Cell s -> quoteIfNecessary quoteChar separator s
        | Quoted s -> quotedValue quoteChar s
    


type OutputOptions = 
    { Separator: char     
      Quote: char }

let defaultOutputOptions : OutputOptions = 
    { Separator = ','; Quote = '"' }

[<Struct>]
type Row = 
    val private cells : Cell list

    new (cells: Cell list) = { cells = cells }

    member x.Cells
        with get() = x.cells

    member internal x.StreamOutput (sw:StreamWriter) (quoteChar:char) (separator:char) : unit = 
        let rec work (cells:Cell list) = 
            match cells with
            | [] -> sw.Write("\n")
            | [x] -> 
                sw.Write (x.Output quoteChar separator); sw.Write("\n")
            | x :: xs -> 
                sw.Write (x.Output quoteChar separator); sw.Write(separator); work xs
        work x.Cells


/// Prints 'true' or 'false' (unquoted).
let csvBool (value:bool) : Cell = Cell <| value.ToString()

let csvString (value:string) : Cell = Cell <| value


/// FSharp.Data API:
/// table.Save(writer = sw, separator = ',', quote = '"' )
/// table.SaveToString() 

/// So favor an object oriented API.

let defaultQuote : Char = '"'
let defaultSeparator : Char = ','

type Csv = 
    val private headers : option<string list>
    val mutable private separator : char
    val mutable private quoteChar : char
    val private rows : seq<Row>

    new (headers: string list, rows : seq<Row>) = 
        { headers = Some <| headers 
        ; separator = defaultSeparator
        ; quoteChar = defaultQuote
        ; rows = rows }
    
    member x.Separator
        with get() = x.separator
        and  set(v:char) = x.separator <- v

    member x.QuoteChar
        with get() = x.quoteChar
        and  set(v:char) = x.quoteChar <- v

    member x.Save (sw:StreamWriter) : unit = 
        match x.headers with
        | Some xs -> 
            let row = new Row(List.map csvString xs)
            row.StreamOutput sw x.quoteChar x.separator  
        | None -> ()
            
    
//let outputCsv (sw:StreamWriter) (opts:OutputOptions) (rows:seq<Row>) : unit = 
//    Seq.iter (outputRow sw opts) rows




