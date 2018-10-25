// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause

module DynaCsv.CsvOutput2

open System
open System.IO
open System.Text


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

    member internal x.Output (quoteChar:char, separator:char) : string = 
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
    val private cells : Cell []

    new (cells: Cell list) = { cells = List.toArray cells }

    member x.Cells
        with get() = x.cells

    member internal x.StreamOutput (sw:StreamWriter, quoteChar:char, separator:char) : unit = 
        let helper (ix:int) (cell:Cell) : unit = 
            if ix > 0 then 
                sw.Write(separator); sw.Write (cell.Output(quoteChar,separator))
            else
                sw.Write (cell.Output(quoteChar,separator))
        Array.iteri helper x.cells; sw.Write("\n")

    member internal x.BufferOutput (sb:StringBuilder, quoteChar:char, separator:char) : unit =
        let helper (ix:int) (x:Cell) : unit = 
            if ix > 0 then 
                sb.Append(separator) |> ignore
                sb.Append(x.Output(quoteChar,separator)) |> ignore
            else
                sb.Append(x.Output(quoteChar,separator)) |> ignore
        Array.iteri helper x.cells; sb.Append("\n") |> ignore





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
    
    new (rows : seq<Row>) = 
        { headers = None 
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
            let row = new Row(List.map Cell xs)
            row.StreamOutput(sw,x.quoteChar, x.separator)
        | None -> ()
        Seq.iter (fun (row:Row) -> row.StreamOutput(sw,x.quoteChar,x.separator)) x.rows
            
    member x.SaveToString () : string = 
        let sb = new StringBuilder()
        match x.headers with
        | Some xs -> 
            let row = new Row(List.map Cell xs)
            row.BufferOutput(sb, x.quoteChar, x.separator)
        | None -> ()
        Seq.iter (fun (row:Row) -> row.BufferOutput(sb,x.quoteChar,x.separator)) x.rows
        sb.ToString()



/// Prints 'true' or 'false' (unquoted).
let csvBool (value:bool) : Cell = Cell <| value.ToString()

let csvString (value:string) : Cell = Cell <| value

let csvQuoted (value:string) : Cell = Quoted <| value


let csvDateTime (value:System.DateTime) (format:string) : Cell = 
    Cell <| value.ToString(format)

let csvLongDate (value:System.DateTime) (format:string) : Cell = 
    Cell <| value.ToLongDateString()

let csvShortDate (value:System.DateTime) (format:string) : Cell = 
    Cell <| value.ToShortDateString()

let csvLongTime (value:System.DateTime) (format:string) : Cell = 
    Cell <| value.ToLongTimeString()

let csvShortTime (value:System.DateTime) (format:string) : Cell = 
    Cell <| value.ToShortTimeString()

let csvDecimal (value:decimal) : Cell = Cell <| value.ToString()
let csvFloat (value:float) : Cell = Cell <| value.ToString()
let csvGuid (value:System.Guid) : Cell = Cell <| value.ToString() 
let csvInteger (value:int) : Cell = Cell <| value.ToString()
let csvInteger64 (value:int64) : Cell = Cell <| value.ToString()


let csvInt (value:int) : Cell = Cell <| value.ToString()
let csvInt64 (value:int64) : Cell = Cell <| value.ToString()

