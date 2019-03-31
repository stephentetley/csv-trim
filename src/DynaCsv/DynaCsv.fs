// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause

namespace DynaCsv

[<AutoOpen>]
module DynaCsv = 
    
    open System.IO
    open FSharp.Data

    open DynaCsv.Common
    open System

    // Principles:
    // We want to reuse FSharp.Data for reading and writing Csv.
    // We want more "dynamism" than typed FSharp.Data offers.



    type DynaCsv<'row> = 
        // TODO - too promiscuous, we only need (a,_) for headers and
        // (_,b) for rows.
        | DynaCsv of FSharp.Data.CsvFile * Runtime.CsvFile<CsvRow>
        
        member internal x.Parent
            with get() : FSharp.Data.CsvFile = match x with |DynaCsv(csv,_) -> csv

        member internal x.Csv
            with get() : Runtime.CsvFile<CsvRow> = match x with |DynaCsv(_,csv) -> csv

        member x.SaveToString (?separator:char, ?quote:char) : string = 
            x.Csv.SaveToString(separator = defaultArg separator ',', 
                               quote = defaultArg quote '"')

        member x.Append1 (values:string[]) : DynaCsv<'row> = 
            let row = new CsvRow(parent= x.Parent, columns=values)
            let csv1 : Runtime.CsvFile<CsvRow> = x.Csv.Append [row]
            DynaCsv(x.Parent, csv1)
        
        member x.Append (values:seq<string[]>) : DynaCsv<'row> = 
            Seq.fold (fun (o:DynaCsv<'row>) (row:string[]) -> o.Append1 row)
                x
                values


    let fromHeaders (headers:string[]) = 
        let spec = headers |> Array.map escapeDoubleQuote |> String.concat "," 
        let csv : CsvFile = 
            CsvFile.Parse(text = spec,
                             separators=",", 
                             quote = '"', 
                             hasHeaders=true)
        DynaCsv(csv, csv.Cache())

    [<Struct>]
    type MapFunc<'row> = 
        | MapFunc of (CsvRow -> CsvRow)
        
        member x.Func
            with get() : (CsvRow -> CsvRow) = match x with | MapFunc(fn) -> fn

    let map (mapper:MapFunc<'row>) (dcsv:DynaCsv<'row>) : DynaCsv<'row> = 
        let func = new Func<CsvRow, CsvRow>(mapper.Func)
        let csv1 : CsvFile = dcsv.Csv.Map func :?> CsvFile
        DynaCsv(dcsv.Parent, csv1)


    type DynaRow = string []

    type Dyna2 = 
        val private CsvHeaders : option<string []>
        val private CsvRows : seq<string []> 

        new (headers:option<string []>, rows: seq<string []>) = 
            { CsvHeaders = headers; CsvRows = rows }

        new (headers:string []) = 
            { CsvHeaders = Some headers; CsvRows = Seq.empty} 

        new (headers:string list) = 
            { CsvHeaders = Some (List.toArray headers); CsvRows = Seq.empty }

        new (rows: seq<string []>) = 
            { CsvHeaders = None; CsvRows = rows }
        
        new (headers:string [], rows: seq<string []>) = 
            { CsvHeaders = Some headers; CsvRows = rows }
            
        new (headers:string list, rows: seq<string []>) = 
            { CsvHeaders = Some (List.toArray headers); CsvRows = rows }

        member x.Headers with get () : option<string[]> = x.CsvHeaders
        member x.Rows with get () : seq<string[]> = x.Rows

    let xlaterow (row:CsvRow) : DynaRow = Array.empty

    let load (uri:string) : Dyna2 = 
        let csv = CsvFile.Load(uri=uri, 
                                separators = ",",
                                hasHeaders = true, 
                                quote = '"' )
        new Dyna2( headers = csv.Headers, rows = Seq.map xlaterow csv.Rows )