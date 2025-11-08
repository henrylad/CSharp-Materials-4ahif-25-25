import {Component, input, InputSignal, signal, WritableSignal} from '@angular/core';

@Component({
  selector: 'app-greeting-instructions',
  imports: [],
  templateUrl: './greeting-instructions.html',
  styleUrl: './greeting-instructions.scss'
})


export class GreetingInstructions {
  public readonly notStared: InputSignal<boolean> = input.required<boolean>();
  protected readonly currentColor:WritableSignal<Color> = signal(Color.Transparent)

  public changeColor(): void {
    this.currentColor.update(oldColor => {
      switch (oldColor){
        case Color.Transparent:
          return Color.Red
        case Color.Red:
          return Color.Green
        case Color.Green:
          return Color.Blue
        case Color.Blue:
          return Color.Transparent
        default:
          throw new Error(`Unknown Color: ${oldColor}`)
      }
    })
  }
  protected readonly Color = Color;
}


export enum Color {
  Transparent,
  Red,
  Green,
  Blue,
}
