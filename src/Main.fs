module Main

open Feliz
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

let main = Html.h1 "Hello world"

ReactDOM.render(
    main,
    document.getElementById "feliz-app"
)