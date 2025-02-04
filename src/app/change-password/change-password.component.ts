import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UsersService } from '../services/users.service';
import { AuthService } from '../services/auth-service.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent {

  oldPass:any
  newPass:any

  passMatch:boolean = true

  isLoading:boolean = false
  error:boolean = false
  success:boolean = false

  constructor(private userServ:UsersService, private authServ:AuthService, private router:Router){}

  passwordCheck(pass1:string, pass2:string){
    if(pass1 === pass2){
      this.passMatch = true 
    }
    else{ this.passMatch = false }
  }

  async changePassword(){
    this.isLoading = true
    let user = this.authServ.getCurrentUser()
    if(user){
      let userModel = { 
        Id : user.id,
        CurrentPassword : this.oldPass,
        NewPassword : this.newPass
      }
      this.userServ.changePassword(userModel).subscribe({
        error: () => {
          this.error = true
          this.isLoading = false
        },
        complete: async () => {
          this.isLoading = false
          this.success = true
          await new Promise(f => setTimeout(f, 3000));
          this.router.navigate(['/home'])
        }
      })
    }
  }

}
