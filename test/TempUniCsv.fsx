// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


#r "netstandard"
#r "System.Xml.Linq.dll"
open System.IO
open System.Text

// Use FSharp.Data for CSV reading
#I @"C:\Users\stephen\.nuget\packages\FSharp.Data\3.1.1\lib\netstandard2.0"
#r @"FSharp.Data.dll"

#load "..\src\DynaCsv\Internal\Common.fs"
#load "..\src\DynaCsv\Internal\UniCsv.fs"
open DynaCsv.Internal.UniCsv

let getDataFile (name:string) = Path.Combine( __SOURCE_DIRECTORY__ , "..\data", name)

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


let demo02 () = 
    let inpath = getDataFile "weather-horizontal.csv"
    let outpath = getDataFile "weather-vertical.csv"
    let horizontal = UniCsv.Load( { Separators = ","; Quote = '"' ; HasHeaders = true; Encoding = Some Encoding.UTF8 }, inpath)
    let vertical = horizontal.Transpose ()
    vertical.Save({ Separator = ','; Quote = '"'}, outpath)

/// TODO - provide non-proprietary sample data
let demo03 () = 
    let inpath = @"G:\work\ADB-exports\RTS-outstations-Sept18.tab.csv"
    let outpath = @"G:\work\ADB-exports\RTS-outstations-Sept18.trim2.csv"
    let ucsv = UniCsv.Load( { Separators = "\t"; Quote = '"' ; HasHeaders = false; Encoding = None }, inpath)
    let output = trimCsv ucsv
    output.Save({ Separator = ','; Quote = '"'}, outpath) 
