module Main

open Feliz
open Feliz.UseElmish
open Elmish
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

let [<Literal>] MAX_TRIES = 6
let words = [|
    "cargo"
    "mango"
    "dafny"
    "fruit"
    "cycle"
    "darks"
    "white"
    "later"
|]

type EntryId = EntryId of int
type Entry = {
    Id: EntryId
    Letters: string array
}
type Model = {
    Entries: Entry array
    CurrentTries: int
}

type Message =
    | CurrentEntryChanged of string
    | AddedEntry
    | TriedNext

let init = { Entries = [||]; CurrentTries = 0 }, Cmd.none

let update message model =
    match message with
    | CurrentEntryChanged(_) -> failwith "Not Implemented"
    | AddedEntry -> { model with CurrentTries = model.CurrentTries + 1 }, Cmd.none
    | TriedNext -> failwith "Not Implemented"

module View =
    [<ReactComponent>]
    let mainView () = 
        let model, dispatch = React.useElmish(init, update, [||])
        Html.div [
            Html.h1 $"Current tries: {model.CurrentTries}"
            Html.input [
                prop.autoFocus true
                prop.onKeyDown (fun key -> if key.code = "Enter" then dispatch AddedEntry)
            ]
        ]

ReactDOM.render(
    View.mainView,
    document.getElementById "feliz-app"
)