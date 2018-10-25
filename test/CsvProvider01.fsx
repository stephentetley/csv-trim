// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause

open System.IO

// Use FSharp.Data for CSV reading
#I @"..\packages\FSharp.Data.3.0.0-beta3\lib\net45"
#r @"FSharp.Data.dll"
open FSharp.Data


type CsvProTable = 
    CsvProvider< Sample = "A,B,C\n1,2,3\n1,3,2\n",
                 Schema = "A(int),B(int),C(int)",
                 HasHeaders = true >

type CsvProRow = CsvProTable.Row

let test01 () = 
    (new CsvProTable()).SaveToString()

