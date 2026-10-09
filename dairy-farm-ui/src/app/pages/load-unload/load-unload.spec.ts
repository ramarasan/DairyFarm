import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoadUnload } from './load-unload';

describe('LoadUnload', () => {
  let component: LoadUnload;
  let fixture: ComponentFixture<LoadUnload>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoadUnload],
    }).compileComponents();

    fixture = TestBed.createComponent(LoadUnload);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
