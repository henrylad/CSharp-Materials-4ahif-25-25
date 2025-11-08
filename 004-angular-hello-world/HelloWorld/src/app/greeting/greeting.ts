import {Component, computed, Signal, signal, viewChild, WritableSignal} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {ClickEvent, GreetingDisplay} from '../greeting-display/greeting-display';
import {GreetingInstructions} from '../greeting-instructions/greeting-instructions';

@Component({
  selector: 'app-greeting',
  imports: [
    GreetingDisplay,
    GreetingInstructions,
    MatButton
  ],
  templateUrl: './greeting.html',
  styleUrl: './greeting.scss'
})
export class Greeting {
  private readonly names: string[] = ['Alice', 'Bob Brown Bob Brown', 'Charlie', 'Diana'];
  private currentNameIdx: WritableSignal<number> = signal(0);
  protected readonly currentName: Signal<string> = computed(() => {
    return this.names[this.currentNameIdx()];
  });
  protected readonly greetedPeople: WritableSignal<string[]> = signal([]);
  private readonly instructionsComponent: Signal<GreetingInstructions> = viewChild.required<GreetingInstructions>('instructions')

  protected greet(): void {
    this.currentNameIdx.update(n => n + 1);

    if (this.currentNameIdx() >= this.names.length) {
      this.currentNameIdx.set(0);
    }
    this.greetedPeople.update(old => [...old, this.currentName()]);
    this.instructionsComponent().changeColor();
  }

  public handelDisplayClicked(event: ClickEvent): void {
    const doubleName = (oldGreetings: string[]): string[] => {
      const newGreeting = [];
      for(const name of oldGreetings) {
        newGreeting.push(name);
        if(name === event[1]){
          newGreeting.push(name);
        }
      }
      return newGreeting;
    };

      const removeName = (oldGreetings: string[]):string[] => {
        const newGreetings = [];
        for(const name of oldGreetings) {
          if(name !== event[1]){
            newGreetings.push(name);
          }
        }
        return newGreetings;
      };

      if(event[0] === "title"){
        this.greetedPeople.update(doubleName);
      }
  }

}
