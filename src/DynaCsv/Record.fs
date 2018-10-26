// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause


module DynaCsv.Record


[<Struct>]
type Record = 
    { Fields : (string * obj) list }
   
    override x.ToString() = 
        x.Fields.ToString()

    static member empty = { Fields = [] }


let select (label:string) (source:Record) : obj = 
    let rec work xs = 
        match xs with
        | [] -> failwithf "Missing field: %s" label
        | (s,o) :: rest ->
            if s = label then o else work rest
    work source.Fields

let extend (label:string) (value:obj) (source:Record) : Record = 
    { Fields = (label,value) :: source.Fields }

        
let restrict (label:string) (source:Record) : Record = 
    let rec cat xs ys = 
        match xs with
        | [] -> ys
        | z::zs -> cat zs (z::ys)
    let rec work ac xs = 
        match xs with
        | (s,o) :: rest ->
            if s = label then 
                { Fields = cat ac rest }
            else
                work ((s,o) :: ac) rest
        | [] -> source
    work [] source.Fields


let update (label:string) (value:obj) (source:Record) : Record = 
    source |> restrict label |> extend label value

let rename (oldName:string) (newName:string) (source:Record) : Record = 
    let o = select oldName source
    source |> restrict oldName |> extend newName o

let labels (source:Record) : string list = 
    /// labels might be shadowed
    let fn  ac (name:string,_) = if List.contains name ac then ac else (name :: ac)
    List.fold fn [] source.Fields  |> List.rev


let concatL (r1:Record) (r2:Record) : Record = 
    List.fold (fun ac (s,o) -> extend s o ac) r2 r1.Fields

let concatR (r1:Record) (r2:Record) : Record = 
    List.fold (fun ac (s,o) -> extend s o ac) r1 r2.Fields