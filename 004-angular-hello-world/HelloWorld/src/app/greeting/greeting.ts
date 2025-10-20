import { Component } from '@angular/core';

@Component({
  selector: 'app-greeting',
  imports: [],
  templateUrl: './greeting.html',
  styleUrl: './greeting.scss'
})
export class Greeting {
  protected readonly names: string[] = ['Kathi', 'Shara', 'Lena', 'Fabian', 'Flora'];

  protected currentNameIdx: number = 0;

  protected greet(): void {
    this.currentNameIdx++;
    if(this.currentNameIdx >= this.names.length)
    {
      this.currentNameIdx = 0;
    }
  }
}
