open System.IO

// Use FSharp.Data for CSV reading
#I @"..\packages\FSharp.Data.3.0.0-beta3\lib\net45"
#r @"FSharp.Data.dll"
open FSharp.Data

#load @"CsvTrim\Common.fs"
#load @"CsvTrim\CsvOutput.fs"
#load @"CsvTrim\Trim.fs"
open CsvTrim.Trim


let test01 () = 
    let input  = @"G:\work\Projects\rtu\AR-asset-expired-2011\RTS-outstations.2018-07-06.tab.csv"
    let output = @"G:\work\Projects\rtu\AR-asset-expired-2011\RTS-outstations.2018-07-06.trim.csv"
    let options = 
        { InputSeparator = "\t"
          InputHasHeaders = true
          OutputSeparator = "," }
    trimCsvFile options input output


let test02 () = 
    let input  = @"G:\work\Projects\rtu\AR-asset-expired-2011\site_list1.csv"
    let output = @"G:\work\Projects\rtu\AR-asset-expired-2011\site_list1.trim.csv"
    let options = 
        { InputSeparator = "\t"
          InputHasHeaders = true
          OutputSeparator = "," }
    trimCsvFile options input output


let getFilesMatching (sourceDirectory:string) (pattern:string) : string list =
    DirectoryInfo(sourceDirectory).GetFiles(searchPattern = pattern) 
        |> Array.map (fun (info:FileInfo)  -> info.FullName)
        |> Array.toList


let test04 () = 
    let sourceDir = @"G:\work\Projects\uquart\rts-data"
    let options = 
        { InputSeparator = "\t"
          InputHasHeaders = true
          OutputSeparator = "," }

    let trim1 (inputPath:string) : unit = 
        let outputPath = inputPath.Replace(".tab.txt", ".csv")
        if outputPath <> inputPath then
            printfn  "Triming: '%s'" inputPath
            trimCsvFile options inputPath outputPath
        else ()

    let files = getFilesMatching sourceDir "*.tab.*"

    List.iter trim1 files
