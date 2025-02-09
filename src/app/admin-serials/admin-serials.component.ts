import { AfterViewInit, ChangeDetectorRef, Component } from '@angular/core';
import { SerialsService } from '../services/serials.service';

@Component({
  selector: 'app-admin-serials',
  templateUrl: './admin-serials.component.html',
  styleUrls: ['./admin-serials.component.css']
})
export class AdminSerialsComponent implements AfterViewInit {

  unReviewedSerials:any = []
  reviewedSerials:any = []

  approving:boolean = false
  modifying:boolean = false
  addingChs:boolean = false
  deleting:boolean = false
  loadingSerials:boolean = false

  imageDataUri:any

  ngAfterViewInit() {
    this.changeDT.detectChanges();
  }

  constructor(private serServ:SerialsService, private changeDT: ChangeDetectorRef){
    this.getSerials()
  }

  getSerials(){
    this.loadingSerials = true
    this.reviewedSerials = []
    this.unReviewedSerials = []
    this.serServ.getSerials().subscribe({
      next: (serials:any) => {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            this.reviewedSerials.push(serial)
          }
          if(!serial.reviewStatus){
            this.unReviewedSerials.push(serial)
          }
        });
        this.changeDT.detectChanges();
      },
      error: (response:any) => {
        console.log("getting serials failed: " + response)
        this.loadingSerials = false
      },
      complete: () => {
        this.loadingSerials = false
      }
    })
  }

  modifySerial(serial:any){
    this.modifying = true
    let needsScraping = false
    if(serial.reviewStatus == false){
      needsScraping = true
    }
    let formData = new FormData()
    formData.append("title", serial.title)
    formData.append("status", serial.status)
    formData.append("firstCh", serial.firstCh)
    formData.append("home", serial.home)
    formData.append("nextChLinkXPath", serial.nextChLinkXPath)
    formData.append("secondaryNextChLinkXPath", serial.secondaryNextChLinkXPath)
    formData.append("otherNextChLinkXPaths", serial.otherNextChLinkXPaths)
    formData.append("titleXPath", serial.titleXPath)
    formData.append("description", serial.description)
    formData.append("authorName", serial.author.name)
    serial.reviewStatus = true
    formData.append("reviewStatus", serial.reviewStatus)
    formData.append("bannerUpload", this.imageDataUri)
    return this.serServ.putSerial(serial.id, formData).subscribe({
      next: () => {
        this.getSerials()
        if(needsScraping){
          this.addingChs = true
          this.serServ.updateSerial(serial.id)
        }
        this.addingChs = false
        this.modifying = false
      },
      error: (res:any) => {
        console.log(res)
        this.modifying = false
      },
      complete: () => this.modifying = false
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
      complete: () => this.approving = false
    })
  }

  //image handling

  fileSelect(event:any, serialId:any, reviewed:boolean){
    if (event.target != null){
      const reader = new FileReader()
      reader.readAsDataURL(event.target.files[0])
      reader.onloadend = (ev:any) => {
        let imgDisplay = document.getElementById("imageDisplay") as HTMLImageElement
        this.resizeImg(ev.target["result"]).then(
          (resizedImg:any) => {
            reader.readAsDataURL(resizedImg)
            reader.onloadend = (evi:any) => {
              let result = evi.target["result"]
              if(reviewed){
                this.reviewedSerials.find((ser:any) => ser.id == serialId).bannerUpload = result
              }
              else{
                this.unReviewedSerials.find((ser:any) => ser.id == serialId).bannerUpload = result
              }
              imgDisplay.src = result
              this.imageDataUri = result
            }
          }
        )
      }
    }
  }

  resizeImg(imageDataUri:any) : Promise<File | null>{
    return new Promise((resolve) => {
      let img = new Image()
      img.src = imageDataUri
      
      let canvas = document.createElement("canvas")
      let context = canvas.getContext("2d")
  
      img.onload = () => {
        let newW
        let newH
        if(img.width >= img.height){
          let resizeVal = 700 / img.width
          newW = 700
          newH = img.height * resizeVal
        }
        else{
          let resizeVal = 700 / img.height
          newH = 700
          newW = img.width * resizeVal
        }
        canvas.width = newW
        canvas.height = newH
  
        context?.drawImage(img, 0, 0, newW, newH)
        canvas.toBlob((blob: Blob | null) => {
          if (blob) {
            const returnFile = new File([blob], "uploadImg", { type: "image/png" });
            // console.log(returnFile);
            resolve(returnFile)
          }
          else{
            console.log("fucky")
            resolve(null)
          }
        }, "image/png")}
    })
  }

  //TODO del
  edbug(sm:any){
    console.log(sm)
  }
}
