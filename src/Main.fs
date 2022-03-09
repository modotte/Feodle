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
    | EntryChanged of string
    | AddedEntry
    | TriedNext

module View =
    let mainView = 
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