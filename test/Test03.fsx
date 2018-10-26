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
    let dyna = new DynaCsv(csvFile = getHospitals ())
    dyna.SaveToString([| "Grid Reference"; "Name"; "Telephone" |])

let test02 () = 
    let dyna = new DynaCsv(csvFile = getHospitals ())
    let helper : IFieldMapper<string,int> = 
        { new IFieldMapper<string,int> 
          with member __.FieldName = "Grid Reference" 
               member __.Typecast (arg:obj) = arg :?> string
               member __.Update (ref:string) = 0 }
    let conv (row:Row) = row.MapField(helper) 
    let dyna2 = dyna.Map(conv)
    dyna2.SaveToString([| "Name"; "Telephone"; "Grid Reference" |])


