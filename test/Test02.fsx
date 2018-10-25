open System.IO

// Use FSharp.Data for CSV reading
#I @"..\packages\FSharp.Data.3.0.0-beta3\lib\net45"
#r @"FSharp.Data.dll"
open FSharp.Data

#load "..\src\DynaCsv\Common.fs"
#load "..\src\DynaCsv\CsvOutput2.fs"
open DynaCsv.Common
open DynaCsv.CsvOutput2

let test01 () = 
    let inputPath = Path.Combine( __SOURCE_DIRECTORY__ , "..", @"data\hospitals.csv")
    let csv = providerReadCsv true "\t" '"' inputPath
    printfn "%s" <| csv.ToString()
    printfn "%A" <| csv.Headers
    printfn "%A" << Seq.take 3 <| csv.Rows

let test02 () = 
    let csv = new Csv(headers = ["One";"Two"], rows = List.toSeq [] )
    csv.Separator <- '\t'
    printfn "%A" <| csv.Separator
