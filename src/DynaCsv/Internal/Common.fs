// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause

namespace DynaCsv.Internal

module Common = 

    open FSharp.Data

    /// Makes a CsvFile with a dummy row.
    /// We can't remove the dummy row at this point because `csv.Skip 1` 
    /// changes the type to CsvFile<CsvRow>.
    let makeDummyRow (rowCount:int) (separator:char) : string = 
        let sep = separator.ToString()
        List.replicate rowCount "1" |> String.concat sep
        
