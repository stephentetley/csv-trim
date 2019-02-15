// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


#r "netstandard"
#r "System.Xml.Linq.dll"

open System.IO

// Use FSharp.Data for CSV reading
#I @"C:\Users\stephen\.nuget\packages\FSharp.Data\3.0.0\lib\netstandard2.0"
#r @"FSharp.Data.dll"
open FSharp.Data


// New attempt for 2019...

let blankCsv () = 
    use sr = new StringReader("")
    CsvFile.Load(sr)

let blankCsv1 (headers:string) = 
    CsvFile.Parse(text = headers)

let demo01 () = 
    let csv = blankCsv1 ("City,Country")
    match csv.Headers with
    | None -> printfn "#N/A"
    | Some arr -> printfn "%O" (arr |> Array.toList)
    printfn "Rows: %i" (csv.Rows |> Seq.length)
    let row1 = new CsvRow(parent= csv, columns=[| "Leeds"; "UK" |])
    // Append is "functional"
    let csv1 = csv.Append <| List.toSeq [row1]

    csv1.SaveToString () |> printfn "%s"

    