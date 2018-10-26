open System.IO

// Use FSharp.Data for CSV reading
#I @"..\packages\FSharp.Data.3.0.0-beta3\lib\net45"
#r @"FSharp.Data.dll"
open FSharp.Data

#load "..\src\DynaCsv\Common.fs"
#load "..\src\DynaCsv\CsvOutput.fs"
open DynaCsv.Common
open DynaCsv.CsvOutput

let test01 () = 
    let inputPath = Path.Combine( __SOURCE_DIRECTORY__ , "..", @"data\hospitals.csv")
    let csv = providerReadCsv true "\t" '"' inputPath
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

