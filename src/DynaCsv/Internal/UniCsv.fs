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

    type UniCsvRow = string []

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
            new UniCsv ( headers = x.Headers, rows = slice )
  
  
        member x.Take(count:int) : UniCsv = x.[0..count-1]

        member x.Skip(count:int) : UniCsv = x.[count..]

        member x.Map (mapping: UniCsvRow -> UniCsvRow) : UniCsv = 
            let rows = Seq.map mapping x.UcRows
            new UniCsv ( headers = x.Headers, rows = rows )

        member x.MapHeaders (mapping: string[] -> string[]) : UniCsv = 
            let headers = Option.map mapping x.UcHeaders
            new UniCsv ( headers = headers, rows = x.UcRows )

        member x.DropHeaders () : UniCsv = new UniCsv (headers = None, rows = x.UcRows )
            

        /// "Pull up" the first row of data to make the headers of the Csv file.
        /// If the data has headers they will be replaced.
        member x.PullUpHeaders () : UniCsv = 
            match Seq.tryHead x.UcRows with
            | None -> new UniCsv (headers = None, rows = Seq.empty )
            | Some hd -> 
                // Seq is know to have at least one element...
                let rest = Seq.skip 1 x.UcRows
                new UniCsv (headers = Some hd, rows = rest )

        /// "Push down" the headers of the Csv File making them the first row of data.
        /// If there are no headers the input is returned as is.
        member x.PushDownHeaders () : UniCsv = 
            match x.UcHeaders with
            | None -> new UniCsv (headers = None, rows = Seq.empty )
            | Some hd -> 
                // Seq is know to have at least one element...
                let rest = seq { yield hd; yield! x.UcRows }
                new UniCsv (headers = None, rows = rest )


        /// TODO curently drops headers...
        member x.Transpose () : UniCsv = 
            let src = x.PushDownHeaders ()
            let rows = src.UcRows |> Seq.transpose |> Seq.map Seq.toArray
            new UniCsv ( headers = None, rows = rows )

    let trimCsv (src:UniCsv) : UniCsv = 
        let trimRow (row1:UniCsvRow) : UniCsvRow = row1 |> Array.map (fun s -> s.Trim())
        let csv1 = src.MapHeaders trimRow
        csv1.Map trimRow
        
        