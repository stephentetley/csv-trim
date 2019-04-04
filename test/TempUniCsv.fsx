// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


#r "netstandard"
#r "System.Xml.Linq.dll"
open System.IO
open System.Text

// Use FSharp.Data for CSV reading
#I @"C:\Users\stephen\.nuget\packages\FSharp.Data\3.0.1\lib\netstandard2.0"
#r @"FSharp.Data.dll"

#load "..\src\DynaCsv\Internal\Common.fs"
#load "..\src\DynaCsv\Internal\UniCsv.fs"
open DynaCsv.Internal.UniCsv

let inputPath = Path.Combine( __SOURCE_DIRECTORY__ , "..", @"data\hospitals.csv")
let outputPath = Path.Combine( __SOURCE_DIRECTORY__ , "..", @"data\hospitals.1.csv")

let demo01 () = 
    let ucsv = UniCsv.Load( { Separators = ","; Quote = '"' ; HasHeaders = true; Encoding = Some Encoding.UTF8 }, inputPath)
    ucsv.Rows |> Seq.iter (printfn "%A")
    printfn "Rows: %i" ucsv.Length
    ucsv.Save({ Separator = ','; Quote = '"'}, outputPath)

    let slice = ucsv.[0..3]
    slice.Rows |> Seq.iter (printfn "%A")
    printfn "Rows: %i" slice.Length




