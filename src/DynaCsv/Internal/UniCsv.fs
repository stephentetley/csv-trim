// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause

namespace DynaCsv.Internal


module UniCsv = 

    open System
    open FSharp.Data

    open DynaCsv.Internal.Common

    type CsvReadOptions = 
        { HasHeaders: bool 
          Separators: string
          Quote: char 
          Encoding: Text.Encoding option }

    type CsvWriteOptions = 
        { Separator: char
          Quote: char }


    type UniCsv = 
        val private UcHeaders : option<string []>
        val private UcRows : seq<string []>


        new (headers: option<string []>, rows: seq<string []>) = 
            { UcHeaders = headers; UcRows = rows }


        member x.Headers with get () : option<string[]> = x.UcHeaders
        member x.Rows with get () : seq<string[]> = x.UcRows


        /// Arity is is column count
        member x.Arity 
            with get () : int = 
                match x.Headers with
                | Some arr -> arr.Length
                | None -> 
                    match Seq.tryHead x.Rows with
                    | Some row -> row.Length
                    | _ -> 0

        /// Length is number of Rows
        member x.Length 
            with get () : int = 
                x.Rows |> Seq.length



        /// Loading is strict
        static member Load (opts:CsvReadOptions, path:string) : UniCsv = 
            use csv = 
                match opts.Encoding with
                | None -> 
                    CsvFile.Load(uri = path,
                                    separators = opts.Separators,
                                    hasHeaders = opts.HasHeaders, 
                                    quote = opts.Quote )
                | Some enc -> 
                    CsvFile.Load(uri = path,
                                    separators = opts.Separators,
                                    hasHeaders = opts.HasHeaders, 
                                    quote = opts.Quote,
                                    encoding = enc )
            let (arr:(string []) array) = Seq.map (fun (row:CsvRow) -> row.Columns) csv.Rows |> Seq.toArray
            new UniCsv( headers = csv.Headers, rows = arr )



        member x.Save (opts:CsvWriteOptions, outputFile:string) : unit = 
            let makeCsvRow (parent:CsvFile) (row:string []) : CsvRow = 
                new CsvRow(parent=parent, columns = row)

            let parent:CsvFile = 
                match x.Headers with
                | Some arr -> makeDummyHeaders arr opts.Separator opts.Quote
                | None -> 
                    match Seq.tryHead x.Rows with
                    | None -> makeDummyAnon 1 opts.Separator opts.Quote
                    | Some row -> makeDummyAnon row.Length opts.Separator opts.Quote
            let rows:seq<CsvRow> = x.Rows |> Seq.map (makeCsvRow parent)
            let csv1 = parent.Skip 1
            let csv2 = csv1.Append rows
            csv2.Save(path = outputFile, separator = opts.Separator, quote = opts.Quote)

        
        member x.GetSlice(start:int option, finish:int option) : UniCsv = 
            let start = defaultArg start 0
            let finish = defaultArg finish (x.Length - 1)
            let slice = x.UcRows |> Seq.toArray |> fun arr -> arr.[start..finish]
            new UniCsv( headers = x.Headers, rows = slice )
  
