import { Component } from '@angular/core';
import { UsersService } from '../services/users.service';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.css']
})
export class AdminUsersComponent {

  users:any = []

  constructor(private usersServ:UsersService){

    this.getUsers()
  }

  getUsers(){
    this.usersServ.getUsers().subscribe(
      (us:any) => this.users = us
    )
  }

  updateUser(user:any){
    this.usersServ.putUser(user).subscribe(
      () => this.getUsers()
    )
  }

  deleteUser(id:any){
    this.usersServ.deleteUser(id).subscribe(
      () => this.getUsers()
    )
  }


}
