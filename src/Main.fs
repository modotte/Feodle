module Main

open System

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

type GameState = Lost | InProgress | Won
type EntryId = EntryId of int
type Entry = {
    Letters: string
}
type Model = {
    Entries: Entry array
    EntryAnswer: string
    CurrentTries: int
    CurrentCorrectAnswer: string
    State: GameState
}

type Message =
    | EntryChanged of string
    | AddedEntry
    | TriedNext
    | GameStateUpdated of GameState

let randomChoice (choices: string array) =
    let index = Random().Next(choices |> Array.length)
    choices[index]

let init = { Entries = [||]; EntryAnswer = ""; CurrentTries = 1; CurrentCorrectAnswer = randomChoice words; State = InProgress }, Cmd.none

let update message model =
    match message with
    | EntryChanged answer -> { model with EntryAnswer = answer }, Cmd.none
    | AddedEntry -> 
        { model with
            Entries = [|{ Letters = model.EntryAnswer }|] |> Array.append model.Entries
            CurrentTries = model.CurrentTries + 1 }, Cmd.none
    | TriedNext -> failwith "Not Implemented"
    | GameStateUpdated state -> { model with State = state }, Cmd.none

let isWordInList answer = words |> Array.contains answer

module View =
    [<ReactComponent>]
    let mainView () = 
        let model, dispatch = React.useElmish(init, update, [||])
        Html.div [
            Html.h1 $"Current tries: {model.CurrentTries}"
            Html.h1 $"Current correct answer: {model.CurrentCorrectAnswer}"
            Html.h1 $"Current game state: {model.State}"
            Html.input [
                prop.autoFocus true
                prop.onKeyUp (fun key -> 
                    if key.code = "Enter" then
                        if model.EntryAnswer |> isWordInList then
                            if model.CurrentTries > MAX_TRIES then
                                dispatch AddedEntry
                                dispatch (GameStateUpdated Lost)
                            else
                                if model.EntryAnswer = model.CurrentCorrectAnswer then
                                    dispatch AddedEntry
                                    dispatch (GameStateUpdated Won)
                                else
                                    dispatch AddedEntry
                )
                prop.onTextChange (EntryChanged >> dispatch)
                prop.maxLength MAX_WORD_LENGTH
            ]

            Html.ul [
                Html.div (
                    model.Entries |> Array.map (fun a -> Html.li [ Html.h2 a.Letters ])
                )
                Html.li [
                    Html.h2 model.EntryAnswer
                ]
            ]
        ]

ReactDOM.render(
    View.mainView,
    document.getElementById "feliz-app"
)