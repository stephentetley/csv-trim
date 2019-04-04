// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause

namespace DynaCsv.Old

[<AutoOpen>]
module DynaCsv = 
    
    open FSharp.Data

    open DynaCsv.Old.Common
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

        new (rows: array<string []>) = 
            { CsvHeaders = None; CsvRows = rows }
        
        new (headers:string [], rows: array<string []>) = 
            { CsvHeaders = Some headers; CsvRows = rows }
            
        new (headers:string list, rows: array<string []>) = 
            { CsvHeaders = Some (List.toArray headers); CsvRows = rows }

        member x.Headers with get () : option<string[]> = x.CsvHeaders
        member x.Rows with get () : seq<string[]> = x.CsvRows


    let load (path:string) : Dyna2 = 
        use csv = CsvFile.Load(uri = path,
                                separators = ",",
                                hasHeaders = true, 
                                quote = '"' )
        let (arr:DynaRow array) = Seq.map (fun (row:CsvRow) -> row.Columns) csv.Rows |> Seq.toArray
        new Dyna2( headers = csv.Headers, rows = arr )

    let makeHString (columns:string[]) : string = 
        String.concat "," columns

    let makeCsvRow (parent:CsvFile) (row:DynaRow) : CsvRow = 
        new CsvRow(parent=parent, columns = row)
        
    /// Makes a CsvFile with a dummy row.
    /// We can't remove the dummy row at this point because `csv.Skip 1` 
    /// changes the type to CsvFile<CsvRow>.
    let makeDummy (rowCount:int) : CsvFile = 
        let dummy =  List.replicate rowCount "1" |> String.concat ","
        let csv1:CsvFile = CsvFile.Parse(text = dummy, hasHeaders = false)
        csv1

    /// Makes a CsvFile with a dummy row.
    /// We can't remove the dummy row at this point because `csv.Skip 1` 
    /// changes the type to CsvFile<CsvRow>.
    let makeDummyHeaders (headers:string []) : CsvFile = 
        let blank = List.replicate headers.Length "1" |> String.concat ","
        let dummy = headers |> String.concat ","
        let csv1:CsvFile = CsvFile.Parse(text = dummy + "\n" + blank , hasHeaders = true)
        csv1


    let save (dcsv:Dyna2) (outputFile:string) : unit = 
        let parent:CsvFile = 
            match dcsv.Headers with
            | Some arr -> makeDummyHeaders arr
            | None -> 
                match Seq.tryHead dcsv.Rows with
                | None -> makeDummy 1
                | Some row -> makeDummy row.Length
        let rows:seq<CsvRow> = dcsv.Rows |> Seq.map (makeCsvRow parent)
        let csv1 = parent.Skip 1
        let csv2 = csv1.Append rows
        csv2.Save outputFile



