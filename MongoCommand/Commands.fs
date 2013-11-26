module MongoCommand

open MongoDB.Bson
open MongoDB.Driver
open MongoDB.Driver.Builders
open System.Linq
open System
open System.Collections.Generic
open JsonPrettyPrinterPlus

let mutable currentMongoServer = null
let mutable currentDb = null
let mutable currentColl = null

let formatJson (json : string) =
    json.PrettyPrintJson()

let printDoc doc = 
    doc.ToString() |> formatJson |> printfn "%s\r\n-----------------------------------------------"

let printDocs docs =
    docs |> Seq.iter printDoc

let server serverName = 
    let mongoClient = new MongoClient("mongodb://" + serverName)
    currentMongoServer <- mongoClient.GetServer();
    printfn "Server set to %s" serverName
    ignore()

let private printList (list : IEnumerable<string>) = 
    list.ToList().Select(fun x i -> sprintf "%i\t%s" i x) |> Seq.iter (printfn "%s")

let dbs() = 
    currentMongoServer.GetDatabaseNames() |> printList

let private setDbByName (dbName : string) = 
    currentDb <- currentMongoServer.GetDatabase(dbName)
    printfn "db set to %s" dbName

let private setDbByNumber i = 
    let dbName = currentMongoServer.GetDatabaseNames().ElementAtOrDefault(i)
    setDbByName dbName

let db p = 
    match box p with
    | :? System.String as s -> setDbByName s
    | :? System.Int32 as i -> setDbByNumber i
    | _ -> ()

let setCollByName (collName : string) = 
    currentColl <- currentDb.GetCollection(collName)
    printfn "collection set to %s" collName

let setCollByNumber i = 
    let collName = currentDb.GetCollectionNames().ElementAtOrDefault(i)
    setCollByName collName

let colls() = 
    currentDb.GetCollectionNames() |> printList

let coll p =
    match box p with
    | :? System.String as s -> setCollByName s
    | :? System.Int32 as i -> setCollByNumber i
    | _ -> ()

let find property value = 
    currentColl.Find(Query.EQ(property, value)) |> printDocs

let findOne() = 
    currentColl.FindOne() |> printDoc

let where() = 
    match currentMongoServer, currentDb, currentColl with
        | null, null, null -> printfn "You are nowhere"
        | _, null, null -> printfn "You are on server %s" currentMongoServer.Settings.Server.Host
        | _, _, null -> printfn "You are on server %s, database %s" currentMongoServer.Settings.Server.Host currentDb.Name
        | _ -> printfn "You are on server %s, database %s, in collection %s" currentMongoServer.Settings.Server.Host currentDb.Name currentColl.Name