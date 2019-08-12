// Copyright (c) Stephen Tetley 2018,2019
// License: BSD 3 Clause

#r "netstandard"
#r "System.Xml.Linq.dll"
open System.IO

// Use FSharp.Data for CSV reading
#I @"C:\Users\stephen\.nuget\packages\FSharp.Data\3.1.1\lib\netstandard2.0"
#r @"FSharp.Data.dll"
open FSharp.Data

#load "..\src\DynaCsv\Old\Common.fs"
#load "..\src\DynaCsv\Old\Record.fs"
#load "..\src\DynaCsv\Old\CsvOutput.fs"
#load "..\src\DynaCsv\Old\DynamicCsv.fs"
open DynaCsv.Old.Common
open DynaCsv.Old.DynamicCsv


let getHospitals () : FSharp.Data.CsvFile = 
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


