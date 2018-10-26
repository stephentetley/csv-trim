open System.IO

// Use FSharp.Data for CSV reading
#I @"..\packages\FSharp.Data.3.0.0-beta3\lib\net45"
#r @"FSharp.Data.dll"
open FSharp.Data

#load "..\src\DynaCsv\Common.fs"
#load "..\src\DynaCsv\Record.fs"
#load "..\src\DynaCsv\CsvOutput.fs"
#load "..\src\DynaCsv\DynamicCsv.fs"
open DynaCsv.Common
open DynaCsv.DynamicCsv


let getHospitals () : CsvFile = 
    let path = Path.Combine ( __SOURCE_DIRECTORY__ , "..", "data\hospitals.csv")
    providerReadCsv true "," '"' path

let test01 () = 
    let dyna = new DynamicCsv(csvFile = getHospitals ())
    dyna.SaveToString([| "Grid Reference"; "Name"; "Telephone" |])

let test02 () = 
    let dyna = new DynamicCsv(csvFile = getHospitals ())
    let conv (row:DynaRow) = row.MapField("Grid Reference", (fun arg -> arg :?> string), (fun _ -> 0)) 
    let dyna2 = dyna.Map(conv)
    dyna2.SaveToString([| "Name"; "Telephone"; "Grid Reference"  |])


