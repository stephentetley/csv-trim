﻿// Copyright (c) Stephen Tetley 2018,2019
// License: BSD 3 Clause

#r "netstandard"
#r "System.Xml.Linq.dll"
open System.IO


// Use FSharp.Data for CSV reading
#I @"C:\Users\stephen\.nuget\packages\FSharp.Data\3.3.3\lib\netstandard2.0"
#r @"FSharp.Data.dll"
open FSharp.Data

#load "..\src\DynaCsv\Old\Common.fs"
#load "..\src\DynaCsv\Old\CsvOutput.fs"
#load "..\src\DynaCsv\Old\Trim.fs"
open DynaCsv.Old.Trim


let test01 () = 
    let input  = @"G:\work\ADB-exports\RTS-outstations-Sept18.tab.csv"
    let output = @"G:\work\ADB-exports\RTS-outstations-Sept18.trim.csv"
    let options = 
        { InputSeparator = '\t'
        ; InputQuote = '"'
        ; InputHasHeaders = true
        ; OutputSeparator = ','
        ; OutputQuote = '"' }
    trimCsvFile options input output


let test02 () = 
    let input  = @"G:\work\Projects\rtu\AR-asset-expired-2011\site_list1.csv"
    let output = @"G:\work\Projects\rtu\AR-asset-expired-2011\site_list1.trim.csv"
    let options = 
        { InputSeparator = '\t'
        ; InputQuote = '"'
        ; InputHasHeaders = true
        ; OutputSeparator = ','
        ; OutputQuote = '"' }
    trimCsvFile options input output


let getFilesMatching (sourceDirectory:string) (pattern:string) : string list =
    DirectoryInfo(sourceDirectory).GetFiles(searchPattern = pattern) 
        |> Array.map (fun (info:FileInfo)  -> info.FullName)
        |> Array.toList


let test04 () = 
    let sourceDir = @"G:\work\ADB-exports\rts"
    let options = 
        { InputSeparator = '\t'
        ; InputQuote = '"'
        ; InputHasHeaders = true
        ; OutputSeparator = ','
        ; OutputQuote = '"' }
          

    let trim1 (inputPath:string) : unit = 
        let outputPath = inputPath.Replace(".tab.csv", ".trim.csv")
        if outputPath <> inputPath then
            printfn  "Triming: '%s'" inputPath
            trimCsvFile options inputPath outputPath
        else ()

    let files = getFilesMatching sourceDir "*.tab.csv"

    List.iter trim1 files
