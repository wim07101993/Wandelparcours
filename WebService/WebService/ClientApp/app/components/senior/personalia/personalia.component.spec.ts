import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonaliaComponent } from './personalia.component';

describe('PersonaliaComponent', () => {
  let component: PersonaliaComponent;
  let fixture: ComponentFixture<PersonaliaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PersonaliaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonaliaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
