import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent {

  passMatch:boolean = true

  isLoading:boolean = true

  constructor(){}

  passwordCheck(pass1:string, pass2:string){
    if(pass1 === pass2){
      this.passMatch = true 
    }
    else{ this.passMatch = false }
  }
}
