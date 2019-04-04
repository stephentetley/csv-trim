// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause


module DynaCsv.Old.Common

open FSharp.Data

let internal defaultQuote : char = '"'
let internal defaultSeparator : char = ','

let escapeDoubleQuote (input:string) : string = 
    match input with
    | null -> ""
    | _ -> input.Replace("\"", "\"\"")


let quoteField (input:string) : string = 
    match input with
    | null -> "\"\""
    | _ -> sprintf "\"%s\"" (input.Replace("\"", "\"\""))

let optQuote(s:string) : string = 
    if s.Contains "," then quoteField s else s


/// Don't bother writing a CSV parser, the one supplied with FSharp.Data's 
/// CsvProvider is very good. This library is concerned only with adding some 
/// extra dynamism on top of CsvProvider.
let providerReadCsv (inputHasHeaders:bool) (inputSeparator:string) (inputQuote:char) (path:string) : CsvFile = 
    CsvFile.Load(uri=path, 
        separators = inputSeparator,
        hasHeaders = inputHasHeaders, 
        quote= inputQuote )
    
