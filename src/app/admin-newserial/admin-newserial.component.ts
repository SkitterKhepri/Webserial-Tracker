import { Component } from '@angular/core';
import { AuthService } from '../services/auth-service.service';
import { SerialsService } from '../services/serials.service';
import { UsersService } from '../services/users.service';
import { Serial } from '../models';

@Component({
  selector: 'app-admin-newserial',
  templateUrl: './admin-newserial.component.html',
  styleUrls: ['./admin-newserial.component.css']
})
export class AdminNewserialComponent {

  newSerial:any = {}
  newSerialAuthor:any = ""
  isAdding:boolean = false
  missingData:boolean = true
  errorAdding:boolean = false

  image:HTMLImageElement = new Image()
  imageDataUri:any = null
  imagePath:any = "assets/img/placeholder.png"

  constructor(private serServ:SerialsService, private userServ : UsersService, private authServ : AuthService){}

  addSerial(){
    this.isAdding = true
    let formData = new FormData()
    formData.append("title", this.newSerial.title)
    formData.append("status", this.newSerial.status)
    formData.append("firstCh", this.newSerial.firstCh)
    formData.append("home", this.newSerial.home)
    formData.append("nextChLinkXPath", this.newSerial.nextChLinkXPath)
    formData.append("secondaryNextChLinkXPath", this.newSerial.secondaryNextChLinkXPath)
    formData.append("otherNextChLinkXPaths", this.newSerial.otherNextChLinkXPaths)
    formData.append("titleXPath", this.newSerial.titleXPath)
    formData.append("description", this.newSerial.description)
    formData.append("reviewStatus", "true")
    if(this.newSerial.authorName == ""){
      this.newSerial.authorName = "unknown"
    }
    formData.append("authorName", this.newSerial.authorName)
    // console.log(formData)
    this.serServ.addSerial(formData).subscribe({
      next: () => {
        this.isAdding = false
      },
      error: (error) => {
        this.errorAdding = true
        this.isAdding = false
      },
      complete: () => {
        this.isAdding = false
      }
    })
    this.newSerial = {}
    this.newSerialAuthor = ""
  }

  isDataMissing(){
    if(this.newSerial.title == "" || this.newSerial.status == "" || this.newSerial.firstCh == "" || this.newSerial.home == "" ||
    this.newSerial.nextChLinkXPath == "" || this.newSerial.titleXPath == ""){
      this.missingData = true
    }
    else{
      this.missingData = false
    }
  }

  //Image handling

  fileSelect(event:any, displayedImage:HTMLImageElement){
    if (event.target != null){
      const reader = new FileReader()
      reader.readAsDataURL(event.target.files[0])
      reader.onloadend = (ev:any) => {
        this.imagePath = ev.target["result"]
        this.imageDataUri = ev.target["result"]
        this.image = displayedImage
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

}
