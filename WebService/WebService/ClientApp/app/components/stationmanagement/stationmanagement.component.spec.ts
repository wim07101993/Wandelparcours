import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StationmanagementComponent } from './stationmanagement.component';

describe('StationmanagementComponent', () => {
  let component: StationmanagementComponent;
  let fixture: ComponentFixture<StationmanagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StationmanagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StationmanagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
