// Copyright (c) Stephen Tetley 2018,2019
// License: BSD 3 Clause

#r "netstandard"
#r "System.Xml.Linq.dll"
open System.IO

// Use FSharp.Data for CSV reading
#I @"C:\Users\stephen\.nuget\packages\FSharp.Data\3.0.0\lib\netstandard2.0"
#r @"FSharp.Data.dll"
open FSharp.Data

#load "..\src\DynaCsv\Common.fs"
#load "..\src\DynaCsv\CsvOutput.fs"
open DynaCsv.Common
open DynaCsv.CsvOutput

let test01 () = 
    let inputPath = Path.Combine( __SOURCE_DIRECTORY__ , "..", @"data\hospitals.csv")
    let csv : FSharp.Data.CsvFile = providerReadCsv true "\t" '"' inputPath
    printfn "%s" <| csv.ToString()
    printfn "%A" <| csv.Headers
    printfn "%A" << Seq.take 3 <| csv.Rows

let test02 () = 
    let rows = List.toSeq [ new OutputRow(cells = [csvBool true; csvString "Hello, World!"; csvString "Hello World!"; csvQuoted "value"]) ]
    let csv = new CsvOutput(headers = ["One";"Two";"Three";"Four"], rows = rows )
    csv.Separator <- '\t'
    printfn "%A" <| csv.Separator
    csv.Separator <- ','
    printfn "%s" <| csv.SaveToString()

