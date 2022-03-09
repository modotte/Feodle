module Main

open Feliz
open Feliz.UseElmish
open Elmish
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

type EntryId = EntryId of int
type Entry = {
    Id: EntryId
    Letters: string array
}
type Model = {
    Entries: Entry array
}

type Message =
    | CurrentEntryChanged of string
    | AddedEntry
    | TriedNext

let init = {Entries = [||]}, Cmd.none

let update message model =
    match message with
    | AddedEntry -> model, Cmd.none
    | CurrentEntryChanged(_) -> failwith "Not Implemented"
    | TriedNext -> failwith "Not Implemented"

module View =
    [<ReactComponent>]
    let mainView () = 
        let model, dispatch = React.useElmish(init, update, [||])
        Html.div [
            Html.h1 "Hello world!"
            Html.input [
                prop.hidden true
                prop.autoFocus true
            ]
        ]

ReactDOM.render(
    View.mainView,
    document.getElementById "feliz-app"
)