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
    "color"
    "pedal"
    "coton"
    "radar"
    "blues"
|]

type GameState = Lost | InProgress | Won
type Entry = {
    UserGuess: string
    ColoredGuess: string
}
type Model = {
    Guesses: Entry array
    Guess: string
    Tries: int
    Answer: string
    State: GameState
}

type Message =
    | GuessChanged of string
    | AddedGuess
    | GameStateUpdated of GameState

let randomChoiceOf (choices: string array) =
    let index = Random().Next(choices |> Array.length)
    choices[index]

let init = { Guesses = [||]; Guess = ""; Tries = 1; Answer = randomChoiceOf words; State = InProgress }, Cmd.none

let simpleValidate (answer: string) (letter: char) = if answer.Contains(letter) then 'Y' else 'B'

let asColored (answer: string) (guess: string) =
    let guesses = guess |> Seq.toArray
    let occurences = guesses |> Seq.countBy id |> Seq.toArray
    let isWithDuplicates occurences = occurences |> Array.forall (fun x -> snd x <> 1)
    let r = 
        guesses
        |> Array.mapi (fun i _ -> 
            if answer.IndexOf(guess[i]) = -1 then
                'R'
            else
                if guess[i] = answer[i] then
                    'G'
                else
                    'Y'       
        ) 

    new string(r)





let update message model =
    match message with
    | GuessChanged answer -> { model with Guess = answer }, Cmd.none
    | AddedGuess -> 
        { model with
            Guesses = [|{ UserGuess = model.Guess; ColoredGuess = asColored model.Answer model.Guess }|] |> Array.append model.Guesses
            Tries = model.Tries + 1 }, Cmd.none
    | GameStateUpdated state -> { model with State = state }, Cmd.none

let isWordInList answer = words |> Array.contains answer

module View =
    [<ReactComponent>]
    let mainView () = 
        let model, dispatch = React.useElmish(init, update, [||])
        Html.div [
            Html.h1 $"Tries: {model.Tries - 1}"
            Html.h1 $"Answer: {model.Answer}"
            Html.h1 $"State: {model.State}"
            Html.input [
                prop.autoFocus true
                prop.onKeyUp (fun key -> 
                    if key.code = "Enter" then
                        if model.Guess |> isWordInList then
                            if model.Tries = MAX_TRIES then
                                dispatch AddedGuess
                                dispatch (GameStateUpdated Lost)
                            else
                                if model.Guess = model.Answer then
                                    dispatch AddedGuess
                                    dispatch (GameStateUpdated Won)
                                else
                                    dispatch AddedGuess
                )
                prop.onTextChange (GuessChanged >> dispatch)
                prop.maxLength MAX_WORD_LENGTH
                prop.disabled (
                    match model.State with
                    | InProgress -> false
                    | _ -> true
                )
            ]

            Html.ul [
                Html.div (
                    model.Guesses |> Array.map (fun x -> 
                        Html.li [
                            Html.span [
                                prop.children [
                                    Html.h2 x.UserGuess 
                                    Html.h2 x.ColoredGuess
                                ]
                            ]
                        ]
                    )
                )
            ]
        ]

ReactDOM.render(
    View.mainView,
    document.getElementById "feliz-app"
)