// Copyright (c) Stephen Tetley 2018,2019
// License: BSD 3 Clause

#r "netstandard"
#r "System.Xml.Linq.dll"
open System.IO

// Use FSharp.Data for CSV reading
#I @"C:\Users\stephen\.nuget\packages\FSharp.Data\3.0.0\lib\netstandard2.0"
#r @"FSharp.Data.dll"
open FSharp.Data


open System.IO

[<Literal>]
let HOSPITALS =  __SOURCE_DIRECTORY__ + "\..\data\hospitals.csv"


type HospitalTable = 
    CsvProvider< Sample = HOSPITALS,
                 HasHeaders = true >

type HospitalRow = HospitalTable.Row

let getHosiptals () = new HospitalTable()

let test01 () = 
    (getHosiptals ()).Rows |> Seq.iter (printfn "%A")

/// Idiomatic processing with CsvProvider "Tables" are the builtin row 
/// collection transformers (like Truncate) and row-by-row processing.
/// The Map transformer seems almost unusable as you have to build a 
/// replacement record (maybe this has improved since the docs were written?).
let test02 () = 
    let hideAddress (row:HospitalRow) : HospitalRow = 
        HospitalRow(name = row.Name, telephone = row.Telephone,
            address = "Hidden", postcode = row.Postcode, gridReference = row.``Grid Reference``)
    let f1 = (getHosiptals ()).Truncate(2) 
    let f2 =  (getHosiptals ()).Map (fun (r:HospitalRow) -> hideAddress r )
    printfn "%A" f1.Rows 
    printfn "%A" f2.Rows