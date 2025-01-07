import { Component } from '@angular/core';
import { SerialsService } from '../services/serials.service';

@Component({
  selector: 'app-liked-serials',
  templateUrl: './liked-serials.component.html',
  styleUrls: ['./liked-serials.component.css']
})
export class LikedSerialsComponent {

  // testProposedSerial:any = {}
  // image:HTMLImageElement = new Image()
  // imageDataUri:any = null
  // imagePath:any = "assets/img/placeholder.png"

  constructor (private serServ:SerialsService){}

  // saveImage(imageInput:any){
  //   if(this.imageDataUri != null){
  //     let formData = new FormData()
  //     formData.append("title", "WG")
  //     formData.append("authorName", "WB")
  //     formData.append("home", "https://en.wikipedia.org/wiki/Adam_Weishaupt")
  //     formData.append("firstCh", "https://www.youtube.com/watch?v=gz1FZpWHMgE")
  //     formData.append("status", "1")
  //     this.resizeImg(this.imageDataUri).then(
  //       (resizedImg:any) => {
  //         formData.append("bannerUpload", resizedImg)
  //         this.serServ.saveImage(formData).subscribe()
  //       }
  //     )
  //   }
  // }

  // fileSelect(event:any, displayedImage:HTMLImageElement){
  //   if (event.target != null){
  //     const reader = new FileReader()
  //     reader.readAsDataURL(event.target.files[0])
  //     reader.onloadend = (ev:any) => {
  //       this.imagePath = ev.target["result"]
  //       this.imageDataUri = ev.target["result"]
  //       this.image = displayedImage
  //     }
  //   }
  // }

  // resizeImg(imageDataUri:any) : Promise<File | null>{
  //   return new Promise((resolve) => {
  //     let img = new Image()
  //     img.src = imageDataUri
      
  //     let canvas = document.createElement("canvas")
  //     let context = canvas.getContext("2d")
  
  //     img.onload = () => {
  //       let newW
  //       let newH
  //       if(img.width >= img.height){
  //         let resizeVal = 700 / img.width
  //         newW = 700
  //         newH = img.height * resizeVal
  //       }
  //       else{
  //         let resizeVal = 700 / img.height
  //         newH = 700
  //         newW = img.width * resizeVal
  //       }
  //       canvas.width = newW
  //       canvas.height = newH
  
  //       context?.drawImage(img, 0, 0, newW, newH)
  //       canvas.toBlob((blob: Blob | null) => {
  //         if (blob) {
  //           const returnFile = new File([blob], "uploadImg", { type: "image/png" });
  //           console.log(returnFile);
  //           resolve(returnFile)
  //         }
  //         else{
  //           console.log("fucky")
  //           resolve(null)
  //         }
  //       }, "image/png")}
  //   })
  // }
  
  

}
