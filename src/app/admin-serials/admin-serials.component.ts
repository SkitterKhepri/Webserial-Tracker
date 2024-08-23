import { Component } from '@angular/core';
import { SerialsService } from '../serials.service';

@Component({
  selector: 'app-admin-serials',
  templateUrl: './admin-serials.component.html',
  styleUrls: ['./admin-serials.component.css']
})
export class AdminSerialsComponent {

  serials:any = []

  constructor(private serServ:SerialsService){
    this.getSerials()
  }

  getSerials(){
    this.serServ.getSerials().subscribe(
      (serials:any)=> this.serials = serials
    )
  }
}
