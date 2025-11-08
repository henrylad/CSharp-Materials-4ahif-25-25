import {Component, input, InputSignal, output, OutputEmitterRef} from '@angular/core';
import {LocalDateTime} from '@js-joda/core';
import {Color} from '../greeting-instructions/greeting-instructions';
import {MatCard, MatCardContent, MatCardHeader, MatCardTitle} from '@angular/material/card';

@Component({
  selector: 'app-greeting-display',
  imports: [
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardContent
  ],
  templateUrl: './greeting-display.html',
  styleUrl: './greeting-display.scss'
})
export class GreetingDisplay {
  public readonly name:InputSignal<string> = input.required<string>();
  public readonly onClicked:OutputEmitterRef<ClickEvent> = output();
  protected readonly timestamp: LocalDateTime = LocalDateTime.now();

  protected handleClick(type: TitleOrContent): void {
    const clickedEvent: ClickEvent = [type,this.name()];
    this.onClicked.emit(clickedEvent);1
  }
}

export type ClickEvent = [TitleOrContent, string];
export type TitleOrContent = "title" | "content";
