module Main

open Feliz
open Feliz.UseElmish
open Elmish
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

let [<Literal>] MAX_TRIES = 6
let [<Literal>] MAX_WORD_LENGTH = 5
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
    NewEntryAnswer: string
    CurrentTries: int
}

type Message =
    | EntryChanged of string
    | AddedEntry
    | TriedNext

let init = { Entries = [||]; NewEntryAnswer = ""; CurrentTries = 0 }, Cmd.none

let update message model =
    match message with
    | EntryChanged(answer) -> { model with NewEntryAnswer = answer }, Cmd.none
    | AddedEntry -> { model with CurrentTries = model.CurrentTries + 1 }, Cmd.none
    | TriedNext -> failwith "Not Implemented"

let isWordInList answer = words |> Array.contains answer

module View =
    [<ReactComponent>]
    let mainView () = 
        let model, dispatch = React.useElmish(init, update, [||])
        Html.div [
            Html.h1 $"Current tries: {model.CurrentTries}"
            Html.input [
                prop.autoFocus true
                prop.onKeyUp (fun key -> 
                    if key.code = "Enter" then
                        if model.NewEntryAnswer |> isWordInList then
                            dispatch AddedEntry
                )
                prop.onTextChange (EntryChanged >> dispatch)
                prop.maxLength MAX_WORD_LENGTH
            ]

            Html.ul [
                Html.h2 [
                    prop.text model.NewEntryAnswer
                ]
            ]
        ]

ReactDOM.render(
    View.mainView,
    document.getElementById "feliz-app"
)