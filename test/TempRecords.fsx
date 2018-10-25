
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
let r1 = empty |> extend "Name" ("stephen" :> obj) |> extend "Age" (46 :> obj)

let r2 = r1 |> restrict "Name"

let r3 = r1 |> update "Age" (47 :> obj)

let r4 = r1 |> rename "Name" "First Name"

let ls = labels r4