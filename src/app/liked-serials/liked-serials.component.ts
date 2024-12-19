import { Component } from '@angular/core';
import { SerialsService } from '../services/serials.service';

@Component({
  selector: 'app-liked-serials',
  templateUrl: './liked-serials.component.html',
  styleUrls: ['./liked-serials.component.css']
})
export class LikedSerialsComponent {

  testProposedSerial:any = {}
  image = "assets/img/placeholder.png"

  constructor (private serServ:SerialsService){}

  saveImage(image:any){
    let formData = new FormData()
    this.testProposedSerial.title = "WG"
    this.testProposedSerial.authorName = "WB"
    this.testProposedSerial.home = "https://en.wikipedia.org/wiki/Adam_Weishaupt"
    this.testProposedSerial.firstCh = "https://www.youtube.com/watch?v=gz1FZpWHMgE"
    this.testProposedSerial.status = 1
    this.testProposedSerial.bannerUpload = image
    formData.append("title", this.testProposedSerial.title)
    formData.append("authorName", this.testProposedSerial.authorName)
    formData.append("home", this.testProposedSerial.home)
    formData.append("firstCh", this.testProposedSerial.firstCh)
    formData.append("status", this.testProposedSerial.status)
    formData.append("bannerUpload", image.files[0])
    console.log(formData)
    this.serServ.saveImage(formData).subscribe()
  }

  fileSelect(event:any){
    if (event.target != null){
      const reader = new FileReader()
      reader.readAsDataURL(event.target.files[0])
      reader.onloadend = (ev:any) => {
        console.log(ev.target)
        this.image = ev.target['result']
      }
    }
    
  }
}
