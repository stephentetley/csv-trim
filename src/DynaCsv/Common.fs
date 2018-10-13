// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause


module DynaCsv.Common


let quoteField (input:string) : string = 
    match input with
    | null -> "\"\""
    | _ -> sprintf "\"%s\"" (input.Replace("\"", "\"\""))

let optQuote(s:string) : string = 
    if s.Contains "," then quoteField s else s
    
