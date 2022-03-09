module Main

open Feliz
open Feliz.UseElmish
open Elmish
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

type Message =
    | EntryChanged of string
    | AddedEntry
    | TriedNext


module View =
    let main = 
        Html.div [
            Html.h1 "Hello world!"
            Html.input [
                prop.hidden true
                prop.autoFocus true
            ]
        ]

ReactDOM.render(
    View.main,
    document.getElementById "feliz-app"
)