import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Cows } from './cows';

describe('Cows', () => {
  let component: Cows;
  let fixture: ComponentFixture<Cows>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Cows],
    }).compileComponents();

    fixture = TestBed.createComponent(Cows);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
