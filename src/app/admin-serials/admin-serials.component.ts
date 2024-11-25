import { Component } from '@angular/core';
import { SerialsService } from '../services/serials.service';
import { AuthorsService } from '../services/authors.service';
import { SerialStatuses } from '../enums/serial-statuses.enum';

@Component({
  selector: 'app-admin-serials',
  templateUrl: './admin-serials.component.html',
  styleUrls: ['./admin-serials.component.css']
})
export class AdminSerialsComponent {

  serials:any = []
  unReviewedSerials:any = []
  reviewedSerials:any = []

  approving:boolean = false
  modifying:boolean = false
  addingChs:boolean = false
  deleting:boolean = false


  constructor(private serServ:SerialsService, private authorServ:AuthorsService){
    this.getSerials()
  }

  getSerials(){
    this.reviewedSerials = []
    this.unReviewedSerials = []
    this.serServ.getSerials().subscribe(
      (serials:any)=> {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            this.reviewedSerials.push(serial)
          }
          if(!serial.reviewStatus){
            this.unReviewedSerials.push(serial)
          }
        });
      }
    )
  }

  modifySerial(serial:any){
    this.modifying = true
    let needsScraping = false
    if(serial.reviewStatus == false){
      needsScraping = true
    }
    serial.reviewStatus = true
    return this.serServ.putSerial(serial.id, serial).subscribe({
      next: () => {
        this.getSerials()
        if(needsScraping){
          this.addingChs = true
          this.serServ.updateSerial(serial.id)
        }
        this.addingChs = false
        this.modifying = false
      },
      error: (res:any) => console.log(res),
      complete: () => {
        this.modifying = false
      }
    })
  }

  deleteSerial(id:any){
    this.deleting = true
    this.serServ.deleteSerial(id).subscribe({
      next: ()=> {
        this.getSerials()
        this.deleting = false
      },
      error: (res:any)=> {
        console.log(res)
        this.deleting = false
      },
      complete: ()=> this.deleting = false
    })
  }

  approveSerial(id:any, reviewStatus:boolean){
    this.approving = true
    let reviewStatusObj = { "reviewStatus" : reviewStatus}
    this.serServ.approveSerial(id, reviewStatusObj).subscribe({
      next: () => {
        this.getSerials()
        this.approving = false
      },
      error: (res:any) => {
        console.log(res)
        this.approving = false
      },
      complete: () => {
        this.approving = false
      }
    })
  }
}
