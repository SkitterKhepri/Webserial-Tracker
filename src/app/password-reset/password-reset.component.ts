import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from '../services/users.service';
import { NavBarComponent } from "../nav-bar/nav-bar.component";

@Component({
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html',
  styleUrls: ['./password-reset.component.css']
})
export class PasswordResetComponent implements OnInit {

  private token:string = ""
  private id:string = ""
  newPass:string = ""

  passMatch:boolean = true

  isLoading:boolean = false
  errorMessage:string = ""
  constructor(private route:ActivatedRoute, private userServ:UsersService, private router:Router){ }

  ngOnInit(): void {
    this.route.queryParams.subscribe(
      (params:any) => {
        this.token = params["token"]
        this.id = params["id"]
      }
    )
  }

  resetPassword(pass1:string, pass2:string){
    this.errorMessage = ""
    this.isLoading = true
    this.passwordCheck(pass1, pass2)
    if(this.passMatch){
      let passResetDTO = {
        "token" : this.token,
        "password" : pass1
      }
      if (this.token != "" && this.id != "") {
        this.userServ.resetPassword(passResetDTO, this.id).subscribe({
          next: ()=>{
            this.userServ.justReg.next("reset")
            this.router.navigate(['/login'])
          },
          error: (error) => {
            this.errorMessage = error.message
            this.isLoading = false
          },
          complete: () => {
            this.isLoading = false
          }
        })
      }
    }
    else{ this.isLoading = false }
  }

  passwordCheck(pass1:string, pass2:string){
    if(pass1 === pass2){
      this.passMatch = true 
    }
    else{ this.passMatch = false }
  }
}
