// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause

namespace DynaCsv

[<AutoOpen>]
module DynaCsv = 
    
    open System.IO
    open FSharp.Data

    open DynaCsv.Common
    open System



    [<Struct>]
    type DynaCsv<'row> = 
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

