import { Component, OnInit } from '@angular/core';
import '/Users/kenan/Documents/GitHub/Wandelparcours/WebApp/src/assets/login-animation.js';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {

  email: string;
  password: string;
  constructor() {}

  ngOnInit() {}


  ngAfterViewInit() {
    (window as any).initialize();
  }

  login(){
    console.log(`email: ${this.email} password: ${this.password}`)
    alert(`Email: ${this.email} Password: ${this.password}`)
  }

}
