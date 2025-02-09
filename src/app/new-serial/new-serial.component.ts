import { Component } from '@angular/core';
import { AuthorsService } from '../services/authors.service';
import { NavBarComponent } from '../nav-bar/nav-bar.component';
import { SerialsService } from '../services/serials.service';
import { SerialStatuses } from '../enums/serial-statuses.enum';
import { UsersService } from '../services/users.service';
import { AuthService } from '../services/auth-service.service';

@Component({
  selector: 'app-new-serial',
  templateUrl: './new-serial.component.html',
  styleUrls: ['./new-serial.component.css']
})
export class NewSerialComponent {

  newSerial:any ={
    "id": 0,
    "title": "",
    "authorName": "",
    "home": "",
    "firstCh": "",
    "status": "",
    "reviewStatus": false
  }

  image:HTMLImageElement = new Image()
  imageDataUri:any = null
  imagePath:any = "assets/img/placeholder.png"

  isProposing = false
  errorProposing = false
  constructor(private serServ:SerialsService){}

  addSerial(){
    this.isProposing = true
    this.errorProposing = false
    let formData = new FormData()
    formData.append("title", this.newSerial.title)
    formData.append("authorName", this.newSerial.authorName)
    formData.append("home", this.newSerial.home)
    formData.append("firstCh", this.newSerial.firstCh)
    formData.append("status", this.newSerial.status)
    this.resizeImg(this.imageDataUri).then(
      (resImage:any) => {
        formData.append("bannerUpload", resImage)
      }
    )
    this.serServ.proposeSerial(formData).subscribe({
      next: () => {
        this.isProposing = false
      },
      error: (error) => {
        this.errorProposing = true
        this.isProposing = false
      },
      complete: () => {
        this.isProposing = false
      }
    })
    this.newSerial = {}
  }

  //Image handling

  fileSelect(event:any, displayedImage:HTMLImageElement){
    if (event.target != null){
      const reader = new FileReader()
      reader.readAsDataURL(event.target.files[0])
      reader.onloadend = (ev:any) => {
        this.resizeImg(ev.target["result"]).then(
          (resized:any) => {
            reader.readAsDataURL(resized)
            reader.onloadend = (ev:any) => {
              this.imagePath = ev.target["result"]
              this.imageDataUri = ev.target["result"]
              this.image = displayedImage
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
          let resizeVal = 600 / img.width
          newW = 600
          newH = img.height * resizeVal
        }
        else{
          let resizeVal = 600 / img.height
          newH = 600
          newW = img.width * resizeVal
        }
        canvas.width = newW
        canvas.height = newH
  
        context?.drawImage(img, 0, 0, newW, newH)
        canvas.toBlob((blob: Blob | null) => {
          if (blob) {
            const returnFile = new File([blob], "uploadImg", { type: "image/png" });
            console.log(returnFile);
            resolve(returnFile)
          }
          else{
            console.log("fucky")
            resolve(null)
          }
        }, "image/png")}
    })
  }
}
