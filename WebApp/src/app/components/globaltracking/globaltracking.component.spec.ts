import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {GlobaltrackingComponent} from './globaltracking.component';

describe('GlobaltrackingComponent', () => {
    let component: GlobaltrackingComponent;
    let fixture: ComponentFixture<GlobaltrackingComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [GlobaltrackingComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(GlobaltrackingComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});