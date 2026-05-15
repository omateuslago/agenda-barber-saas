import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BarberShop } from './barber-shop';

describe('BarberShop', () => {
  let component: BarberShop;
  let fixture: ComponentFixture<BarberShop>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BarberShop],
    }).compileComponents();

    fixture = TestBed.createComponent(BarberShop);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
