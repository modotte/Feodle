module Main

open Feliz
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"


module View =
    let main = Html.h1 "Hello world"

ReactDOM.render(
    View.main,
    document.getElementById "feliz-app"
)