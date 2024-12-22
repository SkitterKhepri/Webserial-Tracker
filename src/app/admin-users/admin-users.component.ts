import { Component } from '@angular/core';
import { UsersService } from '../services/users.service';
import { Router } from '@angular/router';
import { StorageService } from '../services/storage.service';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.css']
})
export class AdminUsersComponent {

  users:any[] = []
  gettingUsers:boolean = false

  constructor(private usersServ:UsersService,private storage:StorageService, private router:Router){
    this.getCurrentUserClaims()
    // if(!this.currentUserClaims.includes("Admin") || !this.currentUserClaims.includes("SAdmin")){
    //   this.router.navigate(['/home'])
    // }
    this.getUsers()
    
  }

  getUsers(){
    this.gettingUsers = true
    this.usersServ.getUsers().subscribe({
      next: (uss:any) => {this.users = uss},
      error: (response:any) => {
        console.log("Error loading user data " + response)
        this.gettingUsers = false
      }
    })
  }

  getCurrentUserClaims(){
    return this.storage.getItem("userClaims")
  }
  
  updateUser(id:any, username:any, email:any){
    let user = {id, username, email}
    this.usersServ.updateUser(user).subscribe(
      (msg:any) => {
        this.getUsers()
        console.log(msg)
      }
    )
  }

  deleteUser(id:any){
    this.usersServ.deleteUser(id).subscribe(
      () => this.getUsers()
    )
  }

}
