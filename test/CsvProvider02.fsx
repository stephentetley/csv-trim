// Copyright (c) Stephen Tetley 2018,2019
// License: BSD 3 Clause

#r "netstandard"
#r "System.Xml.Linq.dll"
open System.IO

// Use FSharp.Data for CSV reading
#I @"C:\Users\stephen\.nuget\packages\FSharp.Data\3.0.1\lib\netstandard2.0"
#r @"FSharp.Data.dll"
open FSharp.Data


open System.IO
open FSharp.Reflection



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

let hospitalRowType = typeof<HospitalRow>

let test03 () = 
    printfn "%s" hospitalRowType.AssemblyQualifiedName
    let headers = Option.defaultValue Array.empty <| (getHosiptals ()).Headers
    let typeNames = FSharpType.GetTupleElements(hospitalRowType) |> Array.map (fun ty -> ty.AssemblyQualifiedName)
    (headers, typeNames)

    // A HospitalRow is a SystemTuple not a record


type MyTuple3 = int * int * int

let myTuple3Type = typeof<MyTuple3>

let test04 () = 
    FSharpType.GetTupleElements(myTuple3Type)

type MyRecord = { Book : string; Author : string } 

let test05 () = 
    let myRecordType = typeof<MyRecord>
    myRecordType.AssemblyQualifiedName
    // A HospitalRow is a SystemTuple not a record


