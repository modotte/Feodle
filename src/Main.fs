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

type Color = Red | Green | Yellow
type Entry = {
    Letters: string
    ColoredLetters: string
}
type Model = {
    Entries: Entry array
    EntryAnswer: string
    Tries: int
    CorrectAnswer: string
    State: GameState
}

type Message =
    | EntryChanged of string
    | AddedEntry
    | TriedNext
    | GameStateUpdated of GameState

let randomChoiceOf (choices: string array) =
    let index = Random().Next(choices |> Array.length)
    choices[index]

let init = { Entries = [||]; EntryAnswer = ""; Tries = 1; CorrectAnswer = randomChoiceOf words; State = InProgress }, Cmd.none

let asColored (correctAnswer: string) (letters: string) =
    if letters = correctAnswer then
        "GGGGGG"
    else
        let r =
            letters
            |> Seq.toArray
            |> Array.mapi (
                fun i x ->
                    if x = correctAnswer[i] then
                        'G'
                    else
                        if not (correctAnswer.Contains(x)) then
                            'R'
                        else 'Y'
            )

        new string(r)

let update message model =
    match message with
    | EntryChanged answer -> { model with EntryAnswer = answer }, Cmd.none
    | AddedEntry -> 
        { model with
            Entries = [|{ Letters = model.EntryAnswer; ColoredLetters = asColored model.CorrectAnswer model.EntryAnswer }|] |> Array.append model.Entries
            Tries = model.Tries + 1 }, Cmd.none
    | TriedNext -> failwith "Not Implemented"
    | GameStateUpdated state -> { model with State = state }, Cmd.none

let isWordInList answer = words |> Array.contains answer

module View =
    [<ReactComponent>]
    let mainView () = 
        let model, dispatch = React.useElmish(init, update, [||])
        Html.div [
            Html.h1 $"Tries: {model.Tries - 1}"
            Html.h1 $"Answer: {model.CorrectAnswer}"
            Html.h1 $"State: {model.State}"
            Html.input [
                prop.autoFocus true
                prop.onKeyUp (fun key -> 
                    if key.code = "Enter" then
                        if model.EntryAnswer |> isWordInList then
                            if model.Tries = MAX_TRIES then
                                dispatch AddedEntry
                                dispatch (GameStateUpdated Lost)
                            else
                                if model.EntryAnswer = model.CorrectAnswer then
                                    dispatch AddedEntry
                                    dispatch (GameStateUpdated Won)
                                else
                                    dispatch AddedEntry
                )
                prop.onTextChange (EntryChanged >> dispatch)
                prop.maxLength MAX_WORD_LENGTH
                prop.disabled (
                    match model.State with
                    | InProgress -> false
                    | _ -> true
                )
            ]

            Html.ul [
                Html.div (
                    model.Entries |> Array.map (fun a -> 
                        Html.li [
                            Html.span [
                                prop.children [
                                    Html.h2 a.Letters 
                                    Html.h2 a.ColoredLetters
                                ]
                            ]
                        ]
                    )
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