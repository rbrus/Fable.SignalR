﻿namespace SignalRApp

module SignalRHub =
    open Fable.SignalR
    open FSharp.Control
    open SignalRHub

    let invoke (msg: Action) =
        match msg with
        | Action.IncrementCount i -> Response.NewCount(i + 1)
        | Action.DecrementCount i -> Response.NewCount(i - 1)

    let send (msg: Action) (hubContext: FableHub<Action,Response>) =
        invoke msg
        |> hubContext.Clients.Caller.Send
    
    [<RequireQualifiedAccess>]
    module Stream =
        let sendToClient (msg: StreamFrom.Action) (hubContext: FableHub<Action,Response>) =
            match msg with
            | StreamFrom.Action.AppleStocks ->
                Stocks.appleStocks
                |> AsyncSeq.mapAsync (fun stock ->
                    async {
                        do! Async.Sleep 25
                        return StreamFrom.Response.AppleStock stock
                    }
                )
                |> AsyncSeq.toAsyncEnum
