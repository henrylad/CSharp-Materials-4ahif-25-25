import { Component, signal, WritableSignal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatButton } from '@angular/material/button'
import { Greeting } from "./greeting/greeting";
import {GreetingInstructions} from './greeting-instructions/greeting-instructions';

@Component({
  selector: 'app-root',
  imports: [
    MatButton,
    Greeting,
    GreetingInstructions
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly clickCounter: WritableSignal<number> = signal(0);

  protected handleClick(): void {
    this.clickCounter.update((n) => n + 1)
  }
}
