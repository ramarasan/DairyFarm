import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DoctorVisits } from './doctor-visits';

describe('DoctorVisits', () => {
  let component: DoctorVisits;
  let fixture: ComponentFixture<DoctorVisits>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoctorVisits],
    }).compileComponents();

    fixture = TestBed.createComponent(DoctorVisits);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
