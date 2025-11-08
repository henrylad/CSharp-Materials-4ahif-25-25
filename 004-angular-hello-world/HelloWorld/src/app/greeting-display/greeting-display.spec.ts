import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GreetingDisplay } from './greeting-display';

describe('GreetingDisplay', () => {
  let component: GreetingDisplay;
  let fixture: ComponentFixture<GreetingDisplay>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GreetingDisplay]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GreetingDisplay);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
