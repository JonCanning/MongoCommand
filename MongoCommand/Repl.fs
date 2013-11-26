module Repl

open MongoCommand
open System
open System.Text.RegularExpressions
 
let isMatch s regex = 
    Regex.IsMatch(s, regex, RegexOptions.IgnoreCase)
 
let (|Matches|_|) r s = 
   if Regex.IsMatch(s, r, RegexOptions.IgnoreCase) then Some s else None

let rec repl() =

    Console.Write("> ")
    let args = Console.ReadLine().Split(' ') |> Array.map (fun x -> x.Trim())
    match args.[0] with
    | Matches "server" _ -> server args.[1]
    | Matches "dbs" _ -> dbs()
    | Matches "db" _ -> db args.[1]
    | Matches "colls" _ -> colls()
    | Matches "coll" _ -> coll args.[1]
    | Matches "where" _ -> where()
    | Matches "quit" _ -> Environment.Exit(0)
    | Matches "findone" _ -> findOne()
    | _ -> ()  

    repl()
 
[<EntryPoint>]
let main args = 
    if args.Length = 1 then server args.[0]
    repl()
    0