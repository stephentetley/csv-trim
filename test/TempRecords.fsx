// Copyright (c) Stephen Tetley 2018
// License: BSD 3 Clause

// A Prototype...


// Records are heterogenous assoc lists
type Record = (string * obj) list

let empty : Record = []

let select (label:string) (source:Record) : obj = 
    let rec work xs = 
        match xs with
        | [] -> failwithf "Missing field: %s" label
        | (s,o) :: rest ->
            if s = label then o else work rest
    work source

let extend (label:string) (value:obj) (source:Record) : Record = 
    (label,value) :: source


    
let restrict (label:string) (source:Record) : Record = 
    let rec cat xs ys = 
        match xs with
        | [] -> ys
        | z::zs -> cat zs (z::ys)
    let rec work ac xs = 
        match xs with
        | (s,o) :: rest ->
            if s = label then 
                cat ac rest 
            else
                work ((s,o) :: ac) rest
        | [] -> source
    work [] source

let update (label:string) (value:obj) (source:Record) : Record = 
    source |> restrict label |> extend label value

let rename (oldName:string) (newName:string) (source:Record) : Record = 
    let o = select oldName source
    source |> restrict oldName |> extend newName o

let labels (source:Record) : string list = 
    List.map fst source

let concatL (r1:Record) (r2:Record) : Record = 
    List.fold (fun ac (s,o) -> extend s o ac) r2 r1

let concatR (r1:Record) (r2:Record) : Record = 
    List.fold (fun ac (s,o) -> extend s o ac) r1 r2




// dummy
let r1 = empty |> extend "Name" ("stephen" :> obj) |> extend "Age" (box 46)

let r2 = r1 |> restrict "Name"

let r3 = r1 |> update "Age" (47 :> obj)

let r4 = r1 |> rename "Name" "First Name"

let ls = labels r4

// mapping fields 

let incrAgeFst (r1:Record) : Record = 
    let incr (x:int) : int = x+1
    let age1 = select "Age" r1
    update "Age" (incr (age1 :?> int)) r1

let mapField (name:string) (typecast:obj -> 'a) (fn:'a -> 'b) (r1:Record) : Record = 
    let fn1 (arg:obj) :'b = fn (typecast arg)
    update name (fn1 <| select name r1) r1


let incrAgeSnd (r1:Record) : Record = 
    let incr (x:int) : int = x+1
    let typecast (arg:obj) : int = arg :?> int
    mapField "Age" typecast incr r1

let ageToLabel (r1:Record) : Record = 
    let toLabel (x:int) : string = if x > 45 then "old" else "young"
    let typecast (arg:obj) : int = arg :?> int
    mapField "Age" typecast toLabel r1

// > missing r4 ;; throws missing field error
let missing (r1:Record) : Record = 
    mapField "Planet of Birth" (fun arg -> arg :?> string) (fun _ -> "Earth") r1
    
